using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Models.ApiModels;

namespace Zal.Bridge.Tools
{
    public class GraphApiClient
    {

        static readonly Uri LoginUri = new Uri("https://login.microsoftonline.com/" + GraphOAuth.Tenant + "/oauth2/v2.0/token");
        static readonly Uri DriveUri = new Uri("https://graph.microsoft.com/v1.0/users/022363f5-f771-4c0a-b0dc-ff4c60ca9ac9/drive/");

        static readonly string includeThumbnails = "$expand=thumbnails($select=c200x200_crop)";
        static readonly string selectParams = "$select=id,name,file,folder";
        //static readonly Uri GraphUri3 = new Uri(DriveUri + "items/014QTCO24TW7BMRZRM6NALRKFI3ETNKQL3/thumbnails/0/medium/content");
        //static readonly Uri GraphUri4 = new Uri(DriveUri + "items/014QTCO24TW7BMRZRM6NALRKFI3ETNKQL3/thumbnails?&select=c300x300_crop");

        private static GraphToken token;

        public static Task<GraphFilesModel> GetFiles(int year = -1, string galleryName = null)//"Galerie/2020/VIP České Švýcarsko"
        {
            string path = "root:/Galerie";
            if (year != -1)
            {
                path += $"/{year}";
                if (!string.IsNullOrEmpty(galleryName)) path += $"/{galleryName}";
            }
            path += ":/children?";
            Uri uri = new Uri(DriveUri + path + selectParams + "&" + includeThumbnails);
            return GetFiles(uri);
        }

        internal static Task<GraphFilesModel> GetFiles(string id)
        {
            Uri uri = new Uri($"{DriveUri}items/{id}/children?{selectParams}&{includeThumbnails}");
            return GetFiles(uri);
        }

        internal static async Task<GraphFilesModel> GetFiles(Uri uri)
        {
            await Connect();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);
                var response = await client.GetAsync(uri);
                string message = await response.Content.ReadAsStringAsync();
                var resModel = JsonConvert.DeserializeObject<GraphFilesModel>(message);
                return resModel;
            }
        }

        //public static async Task<byte[]> GetContent(GraphToken token)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

        //        var response = await client.GetAsync(GraphUri3);
        //        byte[] message = await response.Content.ReadAsByteArrayAsync();
        //        //GraphToken token = JsonConvert.DeserializeObject<GraphToken>(message);
        //        return message;
        //    }
        //}

        private static async Task<bool> Connect()
        {
            if (token == null)//todo expiration
            {
                using (HttpClient client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "client_id", GraphOAuth.AppId },
                        { "scope", "https://graph.microsoft.com/.default" },
                        { "client_secret", GraphOAuth.Secret },
                        { "grant_type", "client_credentials" }
                    };
                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync(LoginUri, content);
                    string message = await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<GraphToken>(message);
                }
            }
            return token.expires_in > 0;
        }

        //public static async Task<string> Post(GraphToken token)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        var values = new Dictionary<string, string>();

        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

        //        var content = new FormUrlEncodedContent(values);

        //        var response = await client.PostAsync(GraphUri3, content);
        //        string message = await response.Content.ReadAsStringAsync();
        //        //GraphToken token = JsonConvert.DeserializeObject<GraphToken>(message);
        //        return message;
        //    }
        //}

        //private static async Task<string> SendAsync(HttpClient client, HttpContent request)
        //{
        //    HttpResponseMessage response;
        //    try
        //    {
        //        response = await client.PostAsync(ResourceUri, request);
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.Message.Contains(TokenExpiredCode))
        //        {
        //            throw new HttpRequestException("Token expired (response code: 421)");
        //        }
        //        throw e;
        //    }
        //    return await response.Content.ReadAsStringAsync();
        //}
    }
}
