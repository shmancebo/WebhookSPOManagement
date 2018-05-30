using Newtonsoft.Json;
using SharePoint.WebHooks.Common.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace SharepointWebhook.Client.Controllers
{
    public class SPWebhookController : ApiController
    {
        private readonly string urlApi = "http://sharepointwebhookmanagementapi.azurewebsites.net/";
        [HttpPost]
        public HttpResponseMessage HandleRequest()
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string validationToken = string.Empty;
            IEnumerable<string> clientStateHeader = new List<string>();
            string webhookClientState = ConfigurationManager.AppSettings["webhookclientstate"].ToString();

            if (Request.Headers.TryGetValues("ClientState", out clientStateHeader))
            {
                string clientStateHeaderValue = clientStateHeader.FirstOrDefault() ?? string.Empty;

                if (!string.IsNullOrEmpty(clientStateHeaderValue) && clientStateHeaderValue.Equals(webhookClientState))
                {
                    var queryStringParams = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    if (queryStringParams.AllKeys.Contains("validationtoken"))
                    {
                        httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                        validationToken = queryStringParams.GetValues("validationtoken")[0].ToString();
                        httpResponse.Content = new StringContent(validationToken);
                        return httpResponse;
                    }
                    else
                    {
                        var requestContent = Request.Content.ReadAsStringAsync().Result;

                        if (!string.IsNullOrEmpty(requestContent))
                        {
                            NotificationModel notification = null;
                            try
                            {
                                var objNotification = JsonConvert.DeserializeObject<ResponseModel<NotificationModel>>(requestContent);
                                notification = objNotification.Value[0];

                            }
                            catch(Exception e)
                            {
                                return httpResponse;
                            }

                            if (notification != null)
                            {

                                if (AddTrace(notification))
                                {
                                    httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                                }
                                else
                                    httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                            }
                        }
                    }
                }
                else
                {
                    httpResponse = new HttpResponseMessage(HttpStatusCode.Forbidden);
                }
            }
            return httpResponse;
        }

        private bool AddTrace(NotificationModel notification)
        {
            notification.Origin = "Cliente api rest";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Format("{0}/api/SpOperation/AddTrace", urlApi));
            request.Content = new StringContent(JsonConvert.SerializeObject(notification), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                String requestResult = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<bool>(requestResult);
            }

            return false;
        }
    }
}
