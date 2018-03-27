using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCheck.Models
{    
    /// <summary>
    /// Represents the Deposit object to be sent for neteller
    /// </summary>
    public class NetellerDepositRequest
    {
        public NetellerDepositRequest()
        {
            PaymentMethod = new NetellerPaymentMethod();
            Transaction = new NetellerTransaction();
        }
        /// <summary>
        /// The Neteller Payment Method to use
        /// </summary>
        [JsonProperty(PropertyName = "paymentMethod")]
        public NetellerPaymentMethod PaymentMethod { get; set; }
        /// <summary>
        /// The Neteller Transaction Details
        /// </summary>
        [JsonProperty(PropertyName = "transaction")]
        public NetellerTransaction Transaction { get; set; }
        /// <summary>
        /// Verfifcation code from the Client Secure ID
        /// </summary>
        [JsonProperty(PropertyName = "verificationCode")]
        public string VerficationCode { get; set; }
    }

    /// <summary>
    /// Represents the Neteller Deposit Reponse Object
    /// </summary>
    public class NetellerDepositResponse
    {
        /// <summary>
        /// The Customer to whom the request was made
        /// </summary>
        public NetellerCustomer Customer { get; set; }
        /// <summary>
        /// Reperesents the transaction details
        /// </summary>
        public NetellerTransaction Transaction { get; set; }
    }

}
