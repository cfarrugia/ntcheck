using Newtonsoft.Json;
using NTCheck.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;

namespace NTCheck
{
    public class NetellerImpl
    {
        private string LiveBaseUrl { get { return ConfigurationManager.AppSettings["LiveBaseUrl"]; } }
        private string TestBaseUrl { get { return ConfigurationManager.AppSettings["TestBaseUrl"]; } }

        private string ClientUrl { get { return ConfigurationManager.AppSettings["ClientUrl"]; } }

        private string AuthUrl { get { return ConfigurationManager.AppSettings["AuthUrl"]; } }
        
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

        public string CheckUserDetails(string accountId, string email)
        {
            RestClient client = new RestClient(this.BaseUrl);

            var request = PrepareRequest(this.ClientUrl, Method.GET);
            request.AddQueryParameter("accountId", accountId);
            request.AddQueryParameter("email", email);
            request.AddHeader("Content-Type", "application/json");
            // Step 1) Request an access token
            string accessToken = GetAccessToken();

            if (accessToken == "") return "";

            // Otherwise add the access token and proceed.
            request.AddHeader("Authorization", "Bearer " + accessToken);

            client.ExecuteAsync<NetellerCustomerInfo>(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Full Data Retrieved for a/c: {0} with email:{1}", accountId, email);
                    Console.WriteLine(JsonConvert.SerializeObject(response.Data));
                }
                else
                {
                    Console.WriteLine("Error getting user details!");
                }
                

            });

            return null;
        }

        /// <summary>
        /// Gets an Access token from Neteller in order to be able to make a deposit or Withdrawal
        /// </summary>
        /// <returns> A string with the access token to be used for subsequent calls</returns>
        private string GetAccessToken()
        {
            RestClient client = new RestClient(this.BaseUrl);

            var request = PrepareRequest(this.AuthUrl, Method.POST);
            request.AddHeader("Authorization", "Basic " + StringUtilities.EncodeToBase64(NetellerAPIClientId + ":" + NetellerAPIClientPassword));
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cache-Control", "no-cache");

            string token = "";
            client.ExecuteAsync<NetellerAuthenticationResponse>(request, (resp) =>
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                    token = resp.Data.AccessToken;
                else
                    Console.WriteLine("Unauthorized access to Neteller.");
            });

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
