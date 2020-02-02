using System.Runtime.Serialization;

namespace TradingAutomation.ApiAuth {

    [DataContract]
    public class OpenApiOAuth2TokenResponse
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }

        [DataMember(Name = "base_uri")]
        public string BaseUri { get; set; }
    }
}