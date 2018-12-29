using Newtonsoft.Json.Linq;

namespace Zal.Bridge.Tools
{
    public class JsonFormator
    {
        private string endpoint;

        internal JsonFormator(string endpoint)
        {
            this.endpoint = endpoint;
        }

        internal string CreateApiRequestString(string operation, object content = null, string userToken = null)
        {
            JObject requestObject = new JObject {
                { "endpoint", endpoint },
                { "operation", operation }
            };
            if (userToken != null) requestObject.Add("token", userToken);
            if (content != null) requestObject.Add("body", JToken.FromObject(content));
            return requestObject.ToString();
        }
    }
}
