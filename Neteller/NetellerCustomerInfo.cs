using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCheck.Models
{
    /// <summary>
    /// Represents the Neteller Customer Infomration Object
    /// </summary>
    public class NetellerCustomerInfo
    {
        public NetellerCustomerInfo()
        {
            //AccountProfile = new NetellerAccountProfile();
        }

        /// <summary>
        /// THe Neteller Customer ID
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// Customer Profile details
        /// </summary>
        public NetellerAccountProfile AccountProfile { get; set; }

        /// <summary>
        /// The User Veridicaton Level:
        /// 00 - Member has not been verified, no registered payment instruments
        /// 01 - Member has not been verified, has one ormore registered payment instruments
        /// 10 - Member is verified, no registered payment instruments
        /// 11 - Member is verified, has one or more registered payment instruments
        /// </summary>
        public string VerificationLevel { get; set; }
    }
}
