using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCheck.Models
{
    /// <summary>
    /// The Neteller Account profile that can be used to prepoulate certian elemnts of ther egistration on neteller
    /// </summary>
    public class NetellerAccountProfile
    {
        public NetellerAccountProfile()
        {
            //Preferences = new NetellerAccountPreferences();
        }

        /// <summary>
        /// User First Name
        /// </summary>
        [JsonProperty(PropertyName = "accountId")]
        public string AccountId { get; set; }

        /// <summary>
        /// User First Name
        /// </summary>
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }
        /// <summary>
        /// User Surname
        /// </summary>
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
        /// <summary>
        /// User Email
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        /// <summary>
        /// User Address Line 1
        /// </summary>
        [JsonProperty(PropertyName = "address1")]
        public string Address1 { get; set; }
        /// <summary>
        /// User Address Line 2
        /// </summary>
        [JsonProperty(PropertyName = "address2")]
        public string Address2 { get; set; }
        /// <summary>
        /// User Address line 3
        /// </summary>
        [JsonProperty(PropertyName = "address3")]
        public string Address3 { get; set; }
        /// <summary>
        /// User City
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
        /// <summary>
        /// A country two letter code
        /// </summary>
        [JsonProperty(PropertyName = "country")]
        public string CountryTwoLetterCode { get; set; }
        /// <summary>
        /// User Post Code
        /// </summary>
        [JsonProperty(PropertyName = "postCode")]
        public string PostCode { get; set; }
        /// <summary>
        /// The User Gender
        /// </summary>
        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }
        /// <summary>
        /// User date of birth
        /// </summary>
        [JsonProperty(PropertyName = "dateOfBrith")]
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// Account prefernces
        /// </summary>
        [JsonProperty(PropertyName = "preferences")]
        public NetellerAccountPreferences Preferences { get; set; }
    }

    public class NetellerTransaction
    {
        public NetellerTransaction()
        {
            
        }

        public string Id { get; set; }
        [JsonProperty(PropertyName = "merchantRefId")]
        public string MerchantRefId { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public int Amount { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public List<NetellerFee> Fees { get; set; }
        public string Status { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class NetellerFee
    {

        public string FeeName { get; set; }
        /// <summary>
        /// The type of fee
        /// </summary>
        public string FeeType { get; set; }
        /// <summary>
        /// The fee amount that was deducted.Amount fields reflect the smallest unit of currency with no decimals. Eg. $25.00 USD should be formatted as 2500.
        /// </summary>
        public int FeeAmount { get; set; }
        /// <summary>
        /// The currency the fee is in.
        /// </summary>
        public string FeeCurrency { get; set; }
    }


    public class NetellerPaymentMethod
    {
        /// <summary>
        /// Identifies the type of payment method.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        /// <summary>
        /// Further Identifies the payment type, and the source of funds for a payment.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }


    public class NetellerAccountPreferences
    {
        /// <summary>
        /// The User Language
        /// </summary>
        [JsonProperty(PropertyName = "lang")]
        public string LangCode { get; set; }
        /// <summary>
        /// The Currency code for the user.
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
    }


    public class NetellerCustomer
    {
        public NetellerCustomer()
        {
            AccountProfile = new NetellerAccountProfile();
            AvailabileBalance = new NetellerBalance();
        }

        public string CustomerId { get; set; }
        public NetellerAccountProfile AccountProfile { get; set; }
        public string VerficationLevel { get; set; }
        public NetellerBalance AvailabileBalance{ get; set; }
    }

    public class NetellerBalance
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
    }

    #region Neteller Errors

    /// <summary>
    /// The Class contianing the Neteller Errors
    /// </summary>
    public class NetellerGlobalError
    {
        /// <summary>
        /// THe Neteller Error Object
        /// </summary>
        public NetellerError Error { get; set; }
    }

    /// <summary>
    /// Neteller errors related to particular fields
    /// </summary>
    public class NetellerFieldError
    {
        /// <summary>
        /// The Field containing the error
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// The Error encountered
        /// </summary>
        public string Error { get; set; }
    }

    /// <summary>
    /// The building blocks for a neteller error
    /// </summary>
    public class NetellerError
    {
        /// <summary>
        /// THe Error Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The Message from Neteller
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Any extra details
        /// </summary>
        public string[] Details { get; set; }
        /// <summary>
        /// The Field errors
        /// </summary>
        public NetellerFieldError[] FieldErrors { get; set; }
    }

    #endregion
}
