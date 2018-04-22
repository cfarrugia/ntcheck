using Newtonsoft.Json;
using NTCheck;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace PaysafeCheck.Skrill
{
    public class SkrillImpl : IUserVerifier
    {
        public decimal MaximumChecksPerDay { get { return Convert.ToDecimal(ConfigurationManager.AppSettings["SkrillMaxChecksPerDay"]); } }
   
        private string IdVerifyUrl { get { return ConfigurationManager.AppSettings["SkrillUserIdUrl"]; } }

        private string BaseUrl { get { return ConfigurationManager.AppSettings["SkrillLiveBaseUrl"]; } }

        private string SkrillAPIClientId { get { return ConfigurationManager.AppSettings["SkrillAPIClientId"]; } }

        private string SkrillAPIClientPassword { get { return ConfigurationManager.AppSettings["SkrillAPIClientPassword"]; } }

        public async Task<UserVerificationResponse> VerifyUser(string email, string accountId)
        {
            RestClient client = new RestClient(this.BaseUrl);

            var request = new RestRequest(this.BaseUrl + IdVerifyUrl, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new SkrillUserIdRequest() { email = email, customerId = accountId, merchantId = this.SkrillAPIClientId, password = this.SkrillAPIClientPassword });
            //request.AddParameter("merchantId", this.SkrillAPIClientId);
            //request.AddParameter("password", this.SkrillAPIClientPassword);
            //request.AddParameter("customerId", accountId);
            //request.AddParameter("email", email);

            var response = new UserVerificationResponse()
            {
                Email = email,
                AccountId = accountId,
                VerificationLevel = VerificationLevel.UnknownError
            };

            try
            {
                var skrillResponse = await client.ExecuteAsync <SkrillUserIdResponse>(request);

                response.Payload = System.Text.Encoding.UTF8.GetString(skrillResponse.RawBytes);

                if (skrillResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response.Error = $"The user was not found with error code: {skrillResponse.StatusCode}";
                    response.VerificationLevel = VerificationLevel.UserNotFound;
                }
                else
                {
                    if (skrillResponse.Data.Email.ToLower() != "MATCH")
                        response.VerificationLevel = VerificationLevel.UserNotFound;
                    else
                        response.VerificationLevel = VerificationLevel.AccountVerified;
                }

                

            }
            catch (Exception ex)
            {
                response.VerificationLevel = VerificationLevel.UnknownError;
                response.Error = $"Error occurred: {ex.Message}";
            }



            // 
            //{
            //     "merchantId": "276261218",
            //     "password": "9f535b6ae672f627e4e5f79f2b7c63fe",
            //     "customerId": "276261219",
            //     "firstName" : "Sample",
            //     "lastName" : "Customer”,
            //     "postCode": "CR12BN"
            // }
            return response;
        }
    }
}
