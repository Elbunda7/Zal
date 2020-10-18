using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Zal.Bridge.Tools
{
    public class JsonFormator
    {
        public string Endpoint { get; set; }

        internal JsonFormator(string endpoint)
        {
            Endpoint = endpoint;
        }

        internal string CreateApiRequestString(string operation, object content = null, string userToken = null)
        {
            JObject requestObject = new JObject {
                { "endpoint", Endpoint }
            };
            if (operation != API.METHOD.NON) requestObject.Add("operation", operation);
            if (userToken != null) requestObject.Add("token", userToken);
            if (content != null)
            {
                var bodyContent = JToken.FromObject(content);
                requestObject.Add("body", bodyContent);
                Analytics.TrackEvent("Request", new Dictionary<string, string>() { { Endpoint, operation + ": " + bodyContent } });
            }
            else
            {
                Analytics.TrackEvent("Request", new Dictionary<string, string>() { { Endpoint, operation } });
            }
            return requestObject.ToString();
        }
    }
}
