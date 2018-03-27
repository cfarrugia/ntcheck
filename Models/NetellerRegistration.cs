using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace NTCheck.Models
{

    /// <summary>
    /// Represents the customer registration reponse
    /// </summary>
    public class NetellerCustomerRegistrationResponse
    {
        /// <summary>
        /// The email address that the user has registered wtih
        /// </summary>
        public long RegisteredNetellerAccountId { get; set; }

        /// <summary>
        /// The email address that the user has registered wtih
        /// </summary>
      public string RegisteredEmailAddress { get; set; }
    }

    
    /// <summary>
    /// The detials of the Customer Registration payload
    /// </summary> 
    public class NetellerRegistrationRequest
    {
        public NetellerRegistrationRequest()
        {
            //AccountProfile = new NetellerAccountProfile();
        }
        /// <summary>
        /// The user Profile detials
        /// </summary>
        [JsonProperty(PropertyName = "accountProfile")]
        public NetellerAccountProfile AccountProfile { get; set; }
        /// <summary>
        /// Where to receive a response once it has been sucesfully created
        /// </summary>
        [JsonProperty(PropertyName = "linkBackUrl")]
        public string LinkBackUrl { get; set; }
        /// <summary>
        /// Any btag code for Neteller referals
        /// </summary>
        [JsonProperty(PropertyName = "btag")]
        public string NetellerBTagCode { get; set; }
    }

    public class NetellerRegistrationResponse
    {
        public List<NetellerLink> Links { get; set; }
    }

    public class NetellerLink
    {
        public string Url { get; set; }

        public string Rel { get; set; }

        public string Method { get; set; }
    }

}
