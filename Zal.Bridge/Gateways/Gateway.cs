using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Zal.Bridge.Tools;

namespace Zal.Bridge
{
    public class Gateway
    {
        protected JsonFormator JsonFormator { get; set; }

        protected Gateway(string ApiEndpoint) {
            JsonFormator = new JsonFormator(ApiEndpoint);
        }

        protected async Task<T> SendRequestFor<T>(string apiMethod, object content = null, string userToken = null) {
            string respond = await SendRequest(apiMethod, content, userToken);
            return JsonConvert.DeserializeObject<T>(respond);
        }

        protected async Task<T> SendRequestForNullable<T>(string apiMethod, object content = null, string userToken = null) {
            string respond = await SendRequest(apiMethod, content, userToken);
            return string.IsNullOrEmpty(respond) ? default(T) : JsonConvert.DeserializeObject<T>(respond);
        }

        protected Task<string> SendRequest(string apiMethod, object content, string userToken) {
            string str = JsonFormator.CreateApiRequestString(apiMethod, content, userToken);
            return ApiClient.PostRequest(str);
        }
    }
}
