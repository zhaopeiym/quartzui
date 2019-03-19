using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace Host
{
    /// <summary>
    /// 请求帮助类
    /// </summary>
    public class HttpHelper
    {
        public static readonly HttpHelper Instance;
        static HttpHelper()
        {
            Instance = new HttpHelper();
        }
        /// <summary>
        /// 不同url分配不同HttpClient
        /// </summary>
        public static Dictionary<string, HttpClient> dictionary = new Dictionary<string, HttpClient>();

        private HttpClient GetHttpClient(string url)
        {
            var uri = new Uri(url);
            var key = uri.Scheme + uri.Host;
            if (!dictionary.Keys.Contains(key))
                dictionary.Add(key, new HttpClient());
            return dictionary[key];
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="jsonString">请求参数（Json字符串）</param>
        /// <param name="authorization">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, string jsonString, string authorization = null)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                jsonString = "{}";
            StringContent content = new StringContent(jsonString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            GetHttpClient(url).DefaultRequestHeaders.Remove("Authorization");
            GetHttpClient(url).DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);
            var httpResponseMessage = await GetHttpClient(url).PostAsync(new Uri(url), content);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url地址</param>
        /// <param name="content">请求参数</param>
        /// <param name="authorization">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<string> PostAsync<T>(string url, T content, string authorization = null) where T : class
        {
            return await PostAsync(url, JsonConvert.SerializeObject(content), authorization);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="authorization">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string url, string authorization = null)
        {
            GetHttpClient(url).DefaultRequestHeaders.Remove("Authorization");
            GetHttpClient(url).DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);
            var httpResponseMessage = await GetHttpClient(url).GetAsync(url);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="jsonString">请求参数（Json字符串）</param>
        /// <param name="authorization">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<string> PutAsync(string url, string jsonString, string authorization = null)
        {
            StringContent content = new StringContent(jsonString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            GetHttpClient(url).DefaultRequestHeaders.Remove("Authorization");
            GetHttpClient(url).DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);
            var httpResponseMessage = await GetHttpClient(url).PutAsync(url, content);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url地址</param>
        /// <param name="content">请求参数</param>
        /// <param name="authorization">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<string> PutAsync<T>(string url, T content, string authorization = null)
        {
            return await PutAsync(url, JsonConvert.SerializeObject(content), authorization);
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="authorization">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<string> DeleteAsync(string url, string authorization = null)
        {
            GetHttpClient(url).DefaultRequestHeaders.Remove("Authorization");
            GetHttpClient(url).DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);
            var httpResponseMessage = await GetHttpClient(url).DeleteAsync(url);
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }
    }
}
