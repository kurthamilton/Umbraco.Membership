using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PayPal.Api;

namespace ODK.Payments
{
    public class PayPalPaymentService
    {
        private const string BaseUrl = "https://api.paypal.com/v1";
        private const string ExecuteUrl = PaymentUrl + "/{0}/execute";
        private const string PaymentUrl = BaseUrl + "/payments/payment";
        private const string TokenUrl = BaseUrl + "/oauth2/token";

        public JObject CreatePayment(PayPalCredentials credentials, Payment payment)
        {
            JObject response = Task.Run(() => CreatePaymentAsync(credentials, payment)).Result;
            return response;
        }

        public bool ExecutePayment(PayPalCredentials credentials, string paymentId, PaymentExecution payment)
        {
            bool complete = Task.Run(() => ExecutePaymentAsync(credentials, paymentId, payment)).Result;
            return complete;
        }

        private async Task<JObject> CreatePaymentAsync(PayPalCredentials credentials, Payment payment)
        {
            using (HttpClient httpClient = await CreateAuthenticatedClient(credentials))
            {
                HttpContent content = GetJsonContent(payment);

                HttpResponseMessage response = await httpClient.PostAsync(PaymentUrl, content);

                JObject responseObject = await GetJsonResponse(response);
                return responseObject;
            }
        }

        private async Task<bool> ExecutePaymentAsync(PayPalCredentials credentials, string paymentId, PaymentExecution payment)
        {
            using (HttpClient httpClient = await CreateAuthenticatedClient(credentials))
            {
                string url = string.Format(ExecuteUrl, paymentId);

                HttpContent content = GetJsonContent(payment);

                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                string responseText = await response.Content.ReadAsStringAsync();

                return response.IsSuccessStatusCode;
            }
        }

        private async Task<HttpClient> CreateAuthenticatedClient(PayPalCredentials credentials)
        {
            HttpClient httpClient = new HttpClient();

            string token = await GetTokenAsync(httpClient, credentials);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return httpClient;
        }

        private async Task<string> GetTokenAsync(HttpClient httpClient, PayPalCredentials credentials)
        {
            AddTokenAuthorization(httpClient, credentials.ClientId, credentials.ClientSecret);

            HttpContent content = GetTokenContent();
            HttpResponseMessage response = await httpClient.PostAsync(TokenUrl, content);

            JObject responseObject = await GetJsonResponse(response);

            return responseObject["access_token"].Value<string>();
        }

        private static void AddTokenAuthorization(HttpClient httpClient, string username, string password)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        private static HttpContent GetJsonContent(object content)
        {
            JObject token = JObject.FromObject(content);

            return GetJsonContent(token);
        }

        private static HttpContent GetJsonContent(JToken token)
        {
            return new StringContent(token.ToString(), Encoding.ASCII, "application/json");
        }

        private static async Task<JObject> GetJsonResponse(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            return JObject.Parse(responseBody);
        }

        private static HttpContent GetTokenContent()
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            });
        }
    }
}
