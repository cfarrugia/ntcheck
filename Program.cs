using RestSharp;
using System;

namespace NTCheck
{
    class Program
    {


        private static void CheckSingleUser(string accountId, string email, bool printDetails)
        {

            NetellerImpl impl = new NetellerImpl();
            impl.CheckUserDetails(accountId, email);
        }

        static void Main(string[] args)
        {
#if DEBUG
            args = new string[] { "-single", "-accountid", "451724588290", "-email", "jaripekka.helminen@gmail.com" };
#endif

            string defaultHelpArgs = "- To verify a file with list of uers: ntcheck.exe -mass -input <input.csv> -output <output.csv>\r\n" +
                                     "- To verify a file with list of uers: ntcheck.exe -single -accountid <accountid> -email <email>";
            if (args.Length == 0)
            {
                Console.WriteLine("no arguments input. Please use application as follows:");
                Console.WriteLine(defaultHelpArgs);
            }
            else if (args[0] != "-mass" && args[0] != "-single")
            {
                Console.WriteLine("Invalid arguments {0}. Please use application as follows:", args[0]);
                Console.WriteLine(defaultHelpArgs);
            }
            else if ((args[0] == "-mass") && (args[1] != "-input" || args[3] != "-output"))
            {
                Console.WriteLine("Invalid arguments for -mass import. Please use application as follows:", args[0]);
                Console.WriteLine(defaultHelpArgs);
            }
            else if ((args[0] == "-single") && (args[1] != "-accountid" || args[3] != "-email"))
            {
                Console.WriteLine("Invalid arguments for -single account check. Please use application as follows:", args[0]);
                Console.WriteLine(defaultHelpArgs);
            }
            else if (args[0] == "-single")
            {
                CheckSingleUser(args[2], args[4], true); // Show details for single user
            }
            else
            {
                throw new NotImplementedException("Mass is not implemented yet.");
            }


            // Wait for input
            Console.ReadLine();
        }
    }
}
