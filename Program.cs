using Newtonsoft.Json.Linq;
using NTCheck.Neteller;
using PaysafeCheck;
using PaysafeCheck.Skrill;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace NTCheck
{
    class Program
    {

        public static async Task Main(string[] args)
        {
#if DEBUG
            args = new string[] { "-single:skrill", "-email:clyfar@gmail.com", "-accountid:16787555" };
            args = new string[] { "-mass:neteller", @"-input:C:\Users\c.farrugia.BETAGY\Desktop\nt\list.txt" };
#endif
            string defaultHelpArgs = "- To verify a file with list of users: paysafecheck.exe -mass:<provider> -input:<input.csv> -output:<output.csv>\r\n" +
                                     "- To verify a file with list of users: paysafecheck.exe -single:<provider> -accountid:<accountid> -email:<email>";

            try
            {
                Dictionary<string, string> parameters = args.ToDictionary(x => x.Split(":")[0], y => y.Substring(y.Split(":")[0].Length + 1));

                var provider = parameters.ContainsKey("-single") ? parameters["-single"] : parameters["-mass"];
                if (provider != "skrill" && provider != "neteller") throw new ArgumentException($"Unknown provider: {provider}");
                IUserVerifier verifier = provider == "skrill" ? (IUserVerifier)new SkrillImpl() : (IUserVerifier)new NetellerImpl();

                UserVerificationResponse resp = null; // Stores the verification response object.

                if (parameters.ContainsKey("-single"))
                {
                    var email = parameters["-email"]; // email is compulsory for both skrill / neteller.

                    string accountId = null;

                    if (provider == "skrill")
                    {
                        if (!parameters.ContainsKey("-accountid"))
                            throw new ArgumentException("account id cannot be null for skrill");

                        accountId = parameters["-accountid"];
                    }

                    try
                    {
                        resp = await verifier.VerifyUser(email, accountId);
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine($"An unknown error has occurred: {ex.ToString()}");
                        return;
                    }

                    switch (resp.VerificationLevel)
                    {
                        case VerificationLevel.UnknownError: Console.WriteLine($"Unknown Error occurred: {resp.Error}"); break;
                        case VerificationLevel.UserNotFound: Console.WriteLine($"Input user was not found. Error: {resp.Error}"); break;
                        case VerificationLevel.AccountVerified: Console.WriteLine($"Account was verified for email: {resp.Email} / accountid: {resp.AccountId}"); break;
                        case VerificationLevel.AccountIsActive: Console.WriteLine($"Account was verified and active for email: {resp.Email} / accountid: {resp.AccountId}"); break;
                    }

                    Console.WriteLine("PayLoad:");
                    Console.WriteLine(resp.Payload);

                }
                else if (parameters.ContainsKey("-mass"))
                {

                    // Read the whole CSV
                    string path = parameters["-input"];
                    List<string[]> allLines = File.ReadAllLines(path).ToList().Select(x => x.Split(ConfigurationManager.AppSettings["CsvDelimeter"][0])).ToList();

                    List<UserVerificationResponse> allDetails = new List<UserVerificationResponse>();

                    // Add the data in the details.
                    if (provider == "neteller")
                        allLines.ForEach(x => allDetails.Add(new UserVerificationResponse() { Email = x[0] } ));
                    else
                        allLines.ForEach(x => allDetails.Add(new UserVerificationResponse() { Email = x[0], AccountId = x[1] }));

                    await ProcessMassDetails(verifier, allDetails, path);
                }
                else throw new ArgumentException("Unknown command (must be -single or -mass");

                Console.WriteLine("Processing complete!");
#if DEBUG
                Console.ReadLine();
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid inputs: {ex.ToString()}");
                Console.WriteLine(defaultHelpArgs);
                Console.ReadLine();
            }
        }

        public static async Task SaveCSV(bool includeVerificationLevel, string path, bool appendIfExists, params UserVerificationResponse[] responses)
        {
            var delimeter = ConfigurationManager.AppSettings["CsvDelimeter"][0];
            string[] lines = responses.Select(x => x.Email + delimeter + x.AccountId + (includeVerificationLevel ? delimeter + x.VerificationLevel.ToString() : "")).ToArray();

            if (File.Exists(path) && appendIfExists)
                await File.AppendAllLinesAsync(path, lines);
            else
                await File.WriteAllLinesAsync(path, lines);
        }

        public static async Task<VerificationLevel> VerifyEmail(string email)
        {
          
            try
            {
                string serviceUrl = ConfigurationManager.AppSettings["EmailVerificationServiceUrl"].Replace("{email}", email);
                Uri uri = new Uri(serviceUrl);

                // Override default RestSharp JSON deserializer
                var client = new RestClient(serviceUrl.Replace("/{email}", ""));
                client.AddHandler("application/json", new DynamicJsonDeserializer());

                // Get response from the verification service.
                var response = client.Execute<dynamic>(new RestRequest(serviceUrl, Method.GET));
                if (response.StatusCode != System.Net.HttpStatusCode.OK) return VerificationLevel.EmailVerificationServiceFailed;

                //
                var address = response?.Data?.address?.Value;
                var validFormat = response?.Data?.validFormat?.Value;
                var deliverable = response?.Data?.deliverable?.Value;
                var hostExists = response?.Data?.hostExists?.Value;

                if (validFormat == null || validFormat == false || address == null || address != email) return VerificationLevel.EmailInvalid;

                if (deliverable == null || deliverable == false || hostExists == null || hostExists == false) return VerificationLevel.EmailNotDeliverable;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifiying email address: {ex.ToString()}");
                return VerificationLevel.EmailVerificationServiceFailed;
            }

            // Add this point all checks are good!
            return VerificationLevel.EmailVerified;
        }

        public static async Task ProcessMassDetails(IUserVerifier verifier, List<UserVerificationResponse> inputDetails, string path)
        {
            var successList = new List<UserVerificationResponse>();
            
            
            Random rng = new Random();
            var averageAccountCheckIntervalInSeconds = 24.0m * 3600m / verifier.MaximumChecksPerDay;
            int minAccountCheckInterval = (int)(0.5m * averageAccountCheckIntervalInSeconds);
            int maxAccountCheckInterval = (int)(1.5m * averageAccountCheckIntervalInSeconds);

            // For the email verifier there is no need to make it look organic.
            var averageEmailVerifierIntervalInSeconds = 24.0m * 3600m / Convert.ToDecimal(ConfigurationManager.AppSettings["EmailMaxChecksPerDay"]);

            bool firstTime = true;
            var readLineTask = Task.Run(async () =>
            {
                if (firstTime) Console.ReadLine();
                else await Task.Delay(TimeSpan.MaxValue);
            });

         

            var waitEmailCheckInterval
                = Task.Run(async () =>
                {

                    Console.WriteLine($"Waiting for {averageEmailVerifierIntervalInSeconds} seconds to verify email address");
                    await Task.Delay((int)averageEmailVerifierIntervalInSeconds * 1000);
                });


            try
            {
                while (inputDetails.Count > 0)
                {

                    // Wait either a readline or the delay to check the email.. 
                    if (await Task.WhenAny(readLineTask, waitEmailCheckInterval) == readLineTask)
                        break; // If signalled to break the loop, exit now. 

                    // 
                    var currentLine = inputDetails[0];

                    //
                    Console.WriteLine($"Checking Email: {currentLine.Email}");

                    // Verify the email. If the verification level is good, proceed to the account verification
                    currentLine.VerificationLevel = await VerifyEmail(currentLine.Email);

                    if (currentLine.VerificationLevel == VerificationLevel.EmailVerified)
                    {
                        //
                        Console.WriteLine($"Email Verified.");

                        var waitAccountCheckInterval = Task.Run(async () =>
                        {
                            var interval = rng.Next(minAccountCheckInterval, maxAccountCheckInterval);
                            Console.WriteLine($"Waiting for {interval} seconds to verify account");
                            await Task.Delay(interval * 1000);
                        });

                        // Wait either a readline or the delay. 
                        if (await Task.WhenAny(readLineTask, waitAccountCheckInterval) == readLineTask)
                            break; // If signalled to break the loop, exit now. 

#if !DEBUG
                        currentLine = await verifier.VerifyUser(currentLine.Email, currentLine.AccountId);
#endif
                    }
                    else
                    {
                        Console.WriteLine($"Invalid Email: {currentLine.VerificationLevel}");
                    }

                    // This line is processed now! Save it.
                    successList.Add(currentLine);
                    await SaveCSV(true, path.Replace(".csv", "") + "_success.csv", true, currentLine);
                    inputDetails.RemoveAt(0);
                }

            }
            finally
            {
                await SaveCSV(true, path, false, inputDetails.ToArray());
            }
        }

    }
}
