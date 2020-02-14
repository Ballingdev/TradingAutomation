using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TradingAutomation.ApiHelper;
using TradingAutomation.Models;

namespace TradingAutomation.Services
{
    public class Service : IService
    {
        private HttpClient _httpClient = Helper.InitializeClient();

        public async Task<T> GetData<T>(string url) where T : IGetModel
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if(response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<T>();
            else
                throw new Exception(response.ReasonPhrase);
        }

        public async Task<T> GetData<T>(string url, List<KeyValuePair<string, string>> queryParams) where T : IGetModel
        {
            var urlWithParams = CreateUrlWithQuery(queryParams);

            HttpResponseMessage response = await _httpClient.GetAsync(url + queryParams);

            if(response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<T>();
            else
                throw new Exception(response.ReasonPhrase);

        }

        public Task<string> PostData<T>(string url, T model) where T : IPostModel
        {
            var requestMessage = new HttpRequestMessage(new HttpMethod("POST"), url);

            return CreateAndSendRequest(_httpClient, requestMessage, model);
        }

        public Task<string> PatchData<T>(string url, T model, params string[] urlParams) where T : IPatchModel
        {
            var urlWithParams = CreateUrlWithParams(urlParams);
            var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), url + urlWithParams);

            return CreateAndSendRequest(_httpClient, requestMessage, model);
        }

        public Task<string> DeleteData(string url, List<KeyValuePair<string, string>> queryParams, params string[] urlParams)
        {
            var urlWithParamsAndQuery = CreateUrlWithParamsAndQuery(urlParams, queryParams);
            var requestMessage = new HttpRequestMessage(new HttpMethod("DELETE"), url + urlWithParamsAndQuery);

            return CreateAndSendRequest(_httpClient, requestMessage);
        }

        private async Task<string> CreateAndSendRequest(HttpClient httpClient, HttpRequestMessage requestMessage)
        {
            HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.ReasonPhrase);
        }

        private async Task<string> CreateAndSendRequest<T>(HttpClient httpClient, HttpRequestMessage requestMessage, T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;

            if(response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.ReasonPhrase);
        }

        private string CreateUrlWithParams(string[] urlParams)
        {
            var url = "";

            foreach(string param in urlParams) 
            {
                url += "/" + param;
            }

            return url;
        }

        private string CreateUrlWithQuery(List<KeyValuePair<string, string>> queryParams)
        {
            var url = "/?";

            foreach(KeyValuePair<string, string> pair in queryParams) 
            {
                url += pair.Key + "=" + pair.Value + "&";
            }

            return url.TrimEnd('&');
        }

        private string CreateUrlWithParamsAndQuery(string[] urlParams, List<KeyValuePair<string, string>> queryParams)
        {
            return CreateUrlWithParams(urlParams) + CreateUrlWithQuery(queryParams);
        }
    }
}