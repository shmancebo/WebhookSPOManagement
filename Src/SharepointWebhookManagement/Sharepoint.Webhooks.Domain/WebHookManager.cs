using Newtonsoft.Json;
using SharePoint.WebHooks.Common.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint.WebHooks.Domain
{
    /// <summary>
    /// Class with methods to manage SharePoint web hooks
    /// </summary>
    public class WebHookManager
    {
        public async Task<SubscriptionModel> AddListWebHookAsync(string siteUrl, string listId, string webHookEndPoint, string accessToken, int validityInMonths = 3)
        {
            string responseString = null;
            using (var httpClient = new HttpClient())
            {
                string requestUrl = String.Format("{0}/_api/web/lists('{1}')/subscriptions", siteUrl, listId);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                request.Content = new StringContent(JsonConvert.SerializeObject(
                    new SubscriptionModel()
                    {
                        Resource = String.Format("{0}/_api/web/lists('{1}')", siteUrl, listId.ToString()),
                        NotificationUrl = webHookEndPoint,
                        ExpirationDateTime = DateTime.Now.AddMonths(validityInMonths).ToUniversalTime(),
                        ClientState = "A0A354EC-97D4-4D83-9DDB-144077ADB449"
                    }),
                    Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    responseString = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Something went wrong...
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }

            return await Task.Run(() => JsonConvert.DeserializeObject<SubscriptionModel>(responseString));
        }

        public async Task<bool> UpdateListWebHookAsync(string siteUrl, string listId, string subscriptionId, string webHookEndPoint, DateTime expirationDateTime, string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                string requestUrl = String.Format("{0}/_api/web/lists('{1}')/subscriptions('{2}')", siteUrl, listId, subscriptionId);
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                request.Content = new StringContent(JsonConvert.SerializeObject(
                    new SubscriptionModel()
                    {
                        NotificationUrl = webHookEndPoint,
                        ExpirationDateTime = expirationDateTime.ToUniversalTime(),
                    }),
                    Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    // oops...something went wrong, maybe the web hook does not exist?
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return await Task.Run(() => true);
                }
            }
        }
 
        public async Task<bool> DeleteListWebHookAsync(string siteUrl, string listId, string subscriptionId, string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                string requestUrl = String.Format("{0}/_api/web/lists('{1}')/subscriptions('{2}')", siteUrl, listId, subscriptionId);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, requestUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    // oops...something went wrong, maybe the web hook does not exist?
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return await Task.Run(() => true);
                }
            }
        }

        public async Task<ResponseModel<SubscriptionModel>> GetListWebHooksAsync(string siteUrl, string listId, string accessToken)
        {
            string responseString = null;
            using (var httpClient = new HttpClient())
            {
                string requestUrl = String.Format("{0}/_api/web/lists('{1}')/subscriptions", siteUrl, listId);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    responseString = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // oops...something went wrong
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }

            return await Task.Run(() => JsonConvert.DeserializeObject<ResponseModel<SubscriptionModel>>(responseString));
        }

        public async Task<SubscriptionModel> GetListWebHookAsync(string siteUrl, string listId, string subscriptionId, string accessToken)
        {
            string responseString = null;
            using (var httpClient = new HttpClient())
            {
                string requestUrl = String.Format("{0}/_api/web/lists('{1}')/subscriptions('{2}')", siteUrl, listId, subscriptionId);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    responseString = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // oops...something went wrong, maybe the web hook does not exist?
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }

            return await Task.Run(() => JsonConvert.DeserializeObject<SubscriptionModel>(responseString));
        }


    }
}
