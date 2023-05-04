using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Pro.Common
{
    public interface IApiHelper
    {
        T Post<T>(string api, object requestObj, string baseURL = null);
        T Put<T>(string api, object requestObj, string baseURL = null);
        T Get<T>(string api, string baseURL = null);
    }

    public class ApiHelper : IApiHelper
    {
        public T Post<T>(string api, object requestObj, string baseURL = null)
        {
            var acc = new WaitForInternetAccess();
            acc.WaitInternetAccess("Post");
            try
            {
                if (string.IsNullOrEmpty(api))
                {
                    return default(T);
                }

                if (!api.StartsWith("/"))
                {
                    api = "/" + api;
                }

                using (var client = GetHttpClient())
                {
                    var result = "";
                    var byteContent = GetObjectContent(requestObj);

                    var post = client.PostAsync(baseURL + api, byteContent);

                    result = post.Result.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            catch
            {
            }
            return default(T);
        }

        public T Put<T>(string api, object requestObj, string baseURL = null)
        {
            var result = "";
            try
            {
                if (string.IsNullOrEmpty(api))
                {
                    return default(T);
                }

                if (!api.StartsWith("/"))
                {
                    api = "/" + api;
                }

                using (var client = GetHttpClient())
                {
                    var byteContent = GetObjectContent(requestObj);

                    var post = client.PutAsync(baseURL + api, byteContent);

                    result = post.Result.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            catch (Exception e)
            {
                //e.WriteLog(nameof(ApiHelper.Put) + $", api: {baseURL + api}, result: {result}");
                throw;
            }
        }

        public T Get<T>(string api, string baseURL = null)
        {
            var acc = new WaitForInternetAccess();
            acc.WaitInternetAccess("Get");
            var result = "";
            try
            {
                if (string.IsNullOrEmpty(api))
                {
                    return default(T);
                }

                if (!api.StartsWith("/"))
                {
                    api = "/" + api;
                }

                using (var client = GetHttpClient())
                {
                    var post = client.GetAsync(baseURL + api);

                    result = post.Result.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            catch (Exception e)
            {
                //e.WriteLog(nameof(ApiHelper.Get) + $", api: {baseURL + api}, result: {result}");
                throw;
            }
        }

        public bool GetAsyn(string api, string baseURL = null)
        {
            var acc = new WaitForInternetAccess();
            acc.WaitInternetAccess("Get");
            try
            {
                if (string.IsNullOrEmpty(api))
                {
                    return false;
                }

                if (!api.StartsWith("/"))
                {
                    api = "/" + api;
                }

                using (var client = GetHttpClient())
                {
                    var post = client.GetAsync(baseURL + api);

                    post.Result.Content.ReadAsStringAsync();

                    return true;
                }
            }
            catch (Exception e)
            {
                //e.WriteLog(nameof(ApiHelper.Get) + $", api: {baseURL + api}, result: {result}");
                return false;
            }
        }


        protected virtual HttpContent GetObjectContent(object requestObj)
        {
            string myObject = JsonConvert.SerializeObject(requestObj);
            var buffer = Encoding.UTF8.GetBytes(myObject);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }

        protected virtual void SetHeader(HttpHeaders header) { }

        protected virtual HttpClient GetHttpClient(string baseURL = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var client = new HttpClient(handler);
            if (baseURL != null)
            {
                client.BaseAddress = new Uri(baseURL);
            }
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            SetHeader(client.DefaultRequestHeaders);
            return client;
        }
    }
}
