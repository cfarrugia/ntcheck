using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCheck.Models
{
    /// <summary>
    /// This represents the withdrawal object from Neteller
    /// </summary>
    [Serializable]
    [JsonObject(Title = "")]
    public class NetellerWithdrawalRequest
    {
        /// <summary>
        /// The Profile of the Payee
        /// </summary>
        [JsonProperty(PropertyName = "payeeProfile")]
        public NetellerAccountProfile PayeeProfile { get; set; }
        /// <summary>
        /// THe transaction details
        /// </summary>
        [JsonProperty(PropertyName = "transaction")]
        public NetellerTransaction Transaction { get; set; }
        
        /// <summary>
        /// Anuy message to send to the users
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        public NetellerWithdrawalRequest()
        {
            //PayeeProfile = new NetellerAccountProfile();
            //Transaction = new NetellerTransaction();
        }
    }

    [Serializable]
    public class NetellerWithdrawalResponse
    {
        public NetellerCustomer Customer { get; set; }
        public NetellerTransaction Transaction { get; set; }

    }
}
