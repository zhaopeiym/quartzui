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
        /// <param name="headers">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string url, string jsonString, Dictionary<string, string> headers = null)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                jsonString = "{}";
            StringContent content = new StringContent(jsonString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (headers != null && headers.Any())
            {
                //如果有headers认证等信息，则每个请求实例一个HttpClient
                using (HttpClient http = new HttpClient())
                {
                    foreach (var item in headers)
                    {
                        http.DefaultRequestHeaders.Remove(item.Key);
                        http.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                    }
                    return await http.PostAsync(new Uri(url), content); 
                }
            }
            else
            {
                return await GetHttpClient(url).PostAsync(new Uri(url), content); 
            }
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url地址</param>
        /// <param name="content">请求参数</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync<T>(string url, T content, Dictionary<string, string> headers = null) where T : class
        {
            return await PostAsync(url, JsonConvert.SerializeObject(content), headers);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            if (headers != null && headers.Any())
            {
                //如果有headers认证等信息，则每个请求实例一个HttpClient
                using (HttpClient http = new HttpClient())
                {
                    foreach (var item in headers)
                    {
                        http.DefaultRequestHeaders.Remove(item.Key);
                        http.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                    }
                    return await http.GetAsync(url); 
                }
            }
            else
            {
                return await GetHttpClient(url).GetAsync(url); 
            }
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="jsonString">请求参数（Json字符串）</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync(string url, string jsonString, Dictionary<string, string> headers = null)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
                jsonString = "{}";
            StringContent content = new StringContent(jsonString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (headers != null && headers.Any())
            {
                //如果有headers认证等信息，则每个请求实例一个HttpClient
                using (HttpClient http = new HttpClient())
                {
                    foreach (var item in headers)
                    {
                        http.DefaultRequestHeaders.Remove(item.Key);
                        http.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                    }
                    return await http.PutAsync(url, content); 
                }
            }
            else
            {
                return await GetHttpClient(url).PutAsync(url, content); 
            }
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url地址</param>
        /// <param name="content">请求参数</param>
        /// <param name="headers">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync<T>(string url, T content, Dictionary<string, string> headers = null)
        {
            return await PutAsync(url, JsonConvert.SerializeObject(content), headers);
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers">webapi做用户认证</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync(string url, Dictionary<string, string> headers = null)
        {
            if (headers != null && headers.Any())
            {
                //如果有headers认证等信息，则每个请求实例一个HttpClient
                using (HttpClient http = new HttpClient())
                {
                    foreach (var item in headers)
                    {
                        http.DefaultRequestHeaders.Remove(item.Key);
                        http.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                    }
                    return await http.DeleteAsync(url); 
                }
            }
            else
            {
                return await GetHttpClient(url).DeleteAsync(url); 
            }
        }
    }
}
