using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Zal.Bridge.Tools
{
    public class ApiClient
    {
        private const string TokenExpiredCode = "421";

        static readonly Uri BaseUri = new Uri("http://zalesak.hlucin.com");
        static readonly Uri ResourceUri = new Uri("http://zalesak.hlucin.com/api/index.php");

        public static async Task<string> PostRequest(string jsonContent, byte[] rawImage = null)
        {
            string str = jsonContent.Encrypt();
            using (HttpClient client = new HttpClient())
            {
                var content = new MultipartFormDataContent
                {
                    { new StringContent(str), "x" }
                };
                if (rawImage != null)
                {
                    content.Add(new ByteArrayContent(rawImage), "y");
                    client.Timeout = TimeSpan.FromSeconds(15);
                }
                str = await SendAsync(client, content);
            }
            return string.IsNullOrEmpty(str) ? str : str.Decrypt().Unzip();
        }

        public static async Task<string> Ping()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(ResourceUri);
                return await response.Content.ReadAsStringAsync();
            }
        }

        private static async Task<string> SendAsync(HttpClient client, HttpContent request)
        {
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(ResourceUri, request);
            }
            catch (Exception e)
            {
                if (e.Message.Contains(TokenExpiredCode))
                {
                    throw new HttpRequestException("Token expired (response code: 421)");
                }
                throw e;
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}
