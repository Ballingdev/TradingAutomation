using System;
using System.Net;
using System.Net.Http;
using TradingAutomation.ApiConfig;

namespace TradingAutomation.ApiHelper
{
    public class Helper : IDisposable
    {
        public static HttpClient HttpClient { get; set; }
 
        public static CookieContainer CookieContainer { get; set; }

        public static void InitializeClient()
        {
            CookieContainer = new CookieContainer();
            var clientHandler = new HttpClientHandler
            {
                CookieContainer = CookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseDefaultCredentials = true
            };
            HttpClient = new HttpClient(clientHandler);
            HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Config.ApiKey}");
        }

        public void Dispose()
        {
            if (HttpClient != null)
                HttpClient.Dispose();
                HttpClient = null;
        }
    }
}