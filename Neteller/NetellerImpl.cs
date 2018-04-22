using Newtonsoft.Json;
using NTCheck.Models;
using PaysafeCheck;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NTCheck.Neteller
{
    public class NetellerImpl : IUserVerifier
    {
        public decimal MaximumChecksPerDay { get { return Convert.ToDecimal(ConfigurationManager.AppSettings["NetellerMaxChecksPerDay"]); } }

        private string LiveBaseUrl { get { return ConfigurationManager.AppSettings["NetellerLiveBaseUrl"]; } }
        private string TestBaseUrl { get { return ConfigurationManager.AppSettings["NetellerTestBaseUrl"]; } }

        private string ClientUrl { get { return ConfigurationManager.AppSettings["NetellerClientUrl"]; } }

        private string AuthUrl { get { return ConfigurationManager.AppSettings["NetellerAuthUrl"]; } }
        
        private string BaseUrl { get { return this.IsTest ? TestBaseUrl : LiveBaseUrl; } }

        private bool IsTest { get { return ConfigurationManager.AppSettings["IsTest"].ToLower() == "true"; } }

        private string NetellerAPIClientId { get { return ConfigurationManager.AppSettings["NetellerAPIClientId"]; } }

        private string NetellerAPIClientPassword { get { return ConfigurationManager.AppSettings["NetellerAPIClientPassword"]; } }


        /*
                // Bet1128 credentials
                private bool IsTest { get; } = false;

                private string NetellerAPIClientId { get; } = "AAABTiESuSym96tw";

                private string NetellerAPIClientPassword { get; } = "0.LcGzGxEv4pSPk73Sv4JjcnBPMngVwWTeFfZmwktUqhw.EAAQ9EFnmPfZ5hiLMDteKsDg0v39fPU";
        */
        public NetellerImpl()
        {

        }

        public async Task<UserVerificationResponse> VerifyUser(string email, string accountId)
        {
            RestClient client = new RestClient(this.BaseUrl);

            var request = PrepareRequest(this.ClientUrl, Method.GET);

            // Only email is used. accountid is ignored.
            if (!string.IsNullOrEmpty(email))
                request.AddQueryParameter("email", email);

            request.AddHeader("Content-Type", "application/json");
            // Step 1) Request an access token
            string accessToken = await GetAccessToken();


            UserVerificationResponse response = new UserVerificationResponse()
            {
                Email = email,
                VerificationLevel = VerificationLevel.UnknownError
            };

            if (accessToken == "")
            {
                response.Error = "Authorization error to Neteller";
                return response;
            }

            request.AddHeader("Authorization", "Bearer " + accessToken);

            var ntResponse = await client.ExecuteAsync<NetellerCustomerInfo>(request);

            if (ntResponse.StatusCode == HttpStatusCode.OK)
            {
                // A typical response is like this:
                // { "CustomerId":"CUS_599A18D5-D37C-412D-B46D-8250E0D91E9A","AccountProfile":{ "accountId":"456723805165","firstName":null,"lastName":null,"email":"l.debattista@3be
                // tgaming.com","address1":null,"address2":null,"address3":null,"city":null,"country":null,"postCode":null,"gender":null,"dateOfBrith":"0001 - 01 - 01T00: 00:00","prefe
                // rences":null},"VerificationLevel":"00"}
                response.VerificationLevel = VerificationLevel.AccountVerified;
                response.AccountId = ntResponse.Data?.AccountProfile?.AccountId;
                response.Payload = JsonConvert.SerializeObject(ntResponse.Data);
                response.Error = "";
            }
            else
            {
                response.VerificationLevel = VerificationLevel.UserNotFound;
                response.Error = $"Error getting user details! Status: {ntResponse.StatusCode}, Err: {ntResponse.ErrorMessage}";
            }

            return response;
        }

        /// <summary>
        /// Gets an Access token from Neteller in order to be able to make a deposit or Withdrawal
        /// </summary>
        /// <returns> A string with the access token to be used for subsequent calls</returns>
        private async Task<string> GetAccessToken()
        {
            RestClient client = new RestClient(this.BaseUrl);

            var request = PrepareRequest(this.AuthUrl, Method.POST);
            request.AddHeader("Authorization", "Basic " + StringUtilities.EncodeToBase64(NetellerAPIClientId + ":" + NetellerAPIClientPassword));
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cache-Control", "no-cache");


            string token = "";
            var resp = await client.ExecuteAsync<NetellerAuthenticationResponse>(request);

            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                token = resp.Data.AccessToken;
            
            return token;
        }

        /// <summary>
        /// Prepare the request object to be sent over to Neteller
        /// </summary>
        /// <param name="url">The URL where to send the request</param>
        /// <param name="httpMethod">The HTTP Method to use</param>
        private RestRequest PrepareRequest(string url, Method httpMethod)
        {
            var request = new RestRequest(url, httpMethod);
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new RestSharpJsonNetSerializer(false);
            return request;
        }

    }


    
}
