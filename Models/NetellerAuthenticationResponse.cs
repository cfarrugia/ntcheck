using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCheck.Models
{
    /// <summary>
    /// Represents the Authentication Reponse from Neteller
    /// </summary>
    internal class NetellerAuthenticationResponse
    {
        /// <summary>
        /// The Access token granted
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// The token type 
        /// </summary>
        public string TokenType { get; set; }
        /// <summary>
        /// How long in seconds before the token expires
        /// </summary>
        public int ExpiresIn { get; set; }

    }

    
}
