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
            args = new string[] { "-mass:neteller", @"-input:C:\Users\c.farrugia.BETAGY\Desktop\nt\list.csv", "-accountid:16787555" };
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


                    (List<UserVerificationResponse> success, List<UserVerificationResponse> failed) massResponse = (success:null, failed:null);
                    try
                    {

                        decimal maxIterationsPerDay = (provider == "neteller") ?
                            Convert.ToDecimal(ConfigurationManager.AppSettings["NetellerMaxChecksPerDay"]) :
                            Convert.ToDecimal(ConfigurationManager.AppSettings["SkrillMaxChecksPerDay"]);

                        massResponse = await ProcessMassDetails(verifier, maxIterationsPerDay, allDetails);
                    }
                    finally
                    {
                        if (massResponse.success != null && massResponse.success.Count > 0)
                            await SaveCSV(massResponse.success, true, path.Replace(".csv", "") + "_success.csv");

                        if (massResponse.failed != null && massResponse.failed.Count > 0)
                            await SaveCSV(massResponse.failed, true, path.Replace(".csv", "") + "_failed.csv");

                    }
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

        public static async Task SaveCSV(List<UserVerificationResponse> responses, bool includeVerificationLevel, string path)
        {
            var delimeter = ConfigurationManager.AppSettings["CsvDelimeter"][0];
            string[] lines = responses.Select(x => x.Email + delimeter + x.AccountId + (includeVerificationLevel ? delimeter + x.VerificationLevel.ToString() : "")).ToArray();

            await File.WriteAllLinesAsync(path, lines);
        }

        public static async Task<(List<UserVerificationResponse> success, List<UserVerificationResponse> failed)> 
            ProcessMassDetails(IUserVerifier verifier, decimal maxCheckPerDay, List<UserVerificationResponse> inputDetails)
        {
            var successList = new List<UserVerificationResponse>();
            
            
            Random rng = new Random();
            var averageIntervalInSeconds = 24.0m * 3600m / maxCheckPerDay;
            int minInterval = (int)(0.5m * averageIntervalInSeconds);
            int maxInterval = (int)(1.5m * averageIntervalInSeconds);



            bool firstTime = true;
            var readLineTask = Task.Run(async () =>
            {
                if (firstTime) Console.ReadLine();
                else await Task.Delay(TimeSpan.MaxValue);
            });



            for (int i = 0; i < inputDetails.Count; i++)
            {
                var waitInterval = Task.Run(async () =>
                {
                    var interval = rng.Next(minInterval, maxInterval);
                    Console.WriteLine($"Waiting for {interval} seconds");
                    await Task.Delay(interval * 1000);
                });

                // Wait either a readline or the delay. 
                if (await Task.WhenAny(readLineTask, waitInterval) == readLineTask)
                    break; // If signalled to break the loop, exit now. 

                // 
                var currentLine = inputDetails[0];

#if DEBUG
                var 
                    lineResponse = currentLine;
#else
                var lineResponse = await verifier.VerifyUser(currentLine.Email, currentLine.AccountId);
#endif
                successList.Add(lineResponse);
                inputDetails.RemoveAt(0);
            }

            // Return list.
            return (success: successList, failed: inputDetails);
        }

    }
}
