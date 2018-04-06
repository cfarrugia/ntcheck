using NTCheck.Neteller;
using PaysafeCheck;
using PaysafeCheck.Skrill;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTCheck
{
    class Program
    {

        public static async Task Main(string[] args)
        {
#if DEBUG
            args = new string[] { "-single:neteller", "-email:jaripekka.helminen@gmail.com" };
#endif


            string defaultHelpArgs = "- To verify a file with list of users: paysafecheck.exe -mass:<provider> -input:<input.csv> -output:<output.csv>\r\n" +
                                     "- To verify a file with list of users: paysafecheck.exe -single:<provider> -accountid:<accountid> -email:<email>";

            try
            {
                Dictionary<string, string> parameters = args.ToDictionary(x => x.Split(":")[0], y => y.Split(":")[1]);

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
                        if (parameters.ContainsKey("accountid")) accountId = parameters["accountid"];
                        else throw new ArgumentException("account id cannot be null for skrill");
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
                    throw new NotImplementedException("Mass import is not implemented yet");
                }
                else throw new ArgumentException("Unknown command (must be -single or -mass");

#if DEBUG
                Console.ReadLine();
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid inputs: {ex.ToString()}");
                Console.WriteLine(defaultHelpArgs);
            }
        }
    }
}
