using System;
using System.Text;

namespace NTCheck
{
    public static class StringUtilities
    {
        /// <summary>
        /// Encodes a string to a Base64 string
        /// </summary>
        /// <param name="encoder">Default UTF 8, else pass your own encoder</param>
        /// <param name="toEncode">The string to encode</param>
        /// <returns>THe string in base 64</returns>
        public static string EncodeToBase64(string toEncode, Encoding encoder = null)
        {
            if (encoder == null)
                encoder = System.Text.UTF8Encoding.UTF8;

            byte[] toEncodeAsBytes = encoder.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        /// <summary>
        /// Calculates the Number of digits found after a decimal places
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int DigitsAfterDecimalPlaces(decimal value)
        {
            string[] parts = value.ToString().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            if (parts[1] == "0")
            {
                return 0;
            }

            return parts[1].Length;
        }


        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int ComputeLevenshteinDistance(string fromString, string toString)
        {
            int n = fromString.Length;
            int m = toString.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (toString[j - 1] == fromString[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }


        /// <summary>
        /// Converts a String to Byte Array
        /// </summary>
        /// <param name="stringToBeConverted"></param>
        /// <returns></returns>
        public static byte[] ConvertStringToByteArray(this string stringToBeConverted)
        {
            return Encoding.Unicode.GetBytes(stringToBeConverted);
        }
    }
}
