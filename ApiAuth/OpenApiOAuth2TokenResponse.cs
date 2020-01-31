using System.Runtime.Serialization;

namespace TradingAutomation.ApiAuth {

    [DataContract]
    public class OpenApiOAuth2TokenResponse
    {
        /// <summary>
        /// The access token; used for accessing Open API services
        /// </summary>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// The token type
        /// </summary>
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// The expiry of the access token in seconds
        /// </summary>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// The Refresh token; used for getting a new access token
        /// </summary>
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// The expiry of the refresh token in seconds
        /// </summary>
        [DataMember(Name = "refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }

        /// <summary>
        /// The base URI of the Open API service groups
        /// </summary>
        [DataMember(Name = "base_uri")]
        public string BaseUri { get; set; }
    }
}