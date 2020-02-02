using System.Configuration;
using TradingAutomation.ApiAuth;

namespace TradingAutomation.ApiConfig
{
    public class Config
    {
        public static OpenApiOAuth2TokenResponse AccessToken { get; private set; }

        public static string ApiKey { get; private set; }

        public static string ApiSecret { get; private set; }

        public static string AuthenticationUrl { get; private set; }

        public static string OpenApiBaseUrl { get; private set; }

        public static string StreamingBaseUrl { get; private set; }


        static Config()
        {
            ApiKey = ConfigurationManager.AppSettings["ApiKey"];
            ApiSecret = ConfigurationManager.AppSettings["ApiSecret"];
            AuthenticationUrl = ConfigurationManager.AppSettings["AuthenticationUrl"];
            OpenApiBaseUrl = ConfigurationManager.AppSettings["OpenApiBaseUrl"];
            StreamingBaseUrl = ConfigurationManager.AppSettings["StreamingBaseUrl"];
        }
    }
}