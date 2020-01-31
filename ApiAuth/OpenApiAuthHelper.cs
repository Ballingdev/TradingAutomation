using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TradingAutomation.ApiAuth
{
    public static class OpenApiAuthHelper {
        public static async Task<OpenApiOAuth2TokenResponse> GetAccessToken(
            string authenticationUrl,
            string apiKey,
            string apiSecret,
            string samlToken) 
            {
                var authorizationUrl = authenticationUrl + "/token";
                var authorizationCode = ParseAndGetAuthorizationCode(samlToken);
                var requestPayload = "grant_type=authorization_code&code=" + authorizationCode;

                return await SendAuthorizationRequest(authorizationUrl, apiKey, apiSecret, requestPayload).ConfigureAwait(false);
            }

        public static Task<OpenApiOAuth2TokenResponse> RefreshToken(
            string authenticationUrl,
            string apiKey,
            string apiSecret,
            string refreshToken)
        {
            var authorizationUrl = authenticationUrl + "/token";

            var requestPayload = "grant_type=authorization_code&code=" + refreshToken;

                return SendAuthorizationRequest(authorizationUrl, apiKey, apiSecret, requestPayload);
        }

        public static string BuildSamlRequest(string authenticationUrl, string applicationUrl, string issuerUrl)
        {
            var timestamp = $"{DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture)}Z";

            return $@"
                    <samlp:AuthnRequest ID=""_{Guid.NewGuid()}"" Version=""2.0"" ForceAuthn""false"" IsPassive""false""
                    ProtocolBinding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"" xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
                    IssueInstant=""{timestamp}"" Destination=""{authenticationUrl}"" AssertionConsumerServiceUrl=""{applicationUrl}"">
                    <samlp:Issuer xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion"">{issuerUrl}</saml:issuer>
                    </samlp:AuthnRequest>";
        }

        private static async Task<OpenApiOAuth2TokenResponse> SendAuthorizationRequest(
            string authenticationUrl,
            string apiKey,
            string apiSecret,
            string requestPayload)
            {
                var credentials = $"{apiKey}:{apiSecret}";
                var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

                var client = new HttpClient(new HttpClientHandler
                {
                    CookieContainer = new CookieContainer(),
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    UseDefaultCredentials = true
                });

                client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

                using (var content = new StringContent(requestPayload)) {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var response = await client.PostAsync(authenticationUrl, content).ConfigureAwait(false);
                    var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                    var serializer = new DataContractJsonSerializer(typeof(OpenApiOAuth2TokenResponse));
                    var tokenResponse = serializer.ReadObject(stream) as OpenApiOAuth2TokenResponse;

                    return tokenResponse;
                }
            }

        private static string ParseAndGetAuthorizationCode(string saml) 
        {
            var xml = new XmlDocument();
            xml.LoadXml(saml);

            var nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            nsmgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

            var attribute = xml.SelectSingleNode("/saml:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='AuthorizationCode']/saml:AttributeValue", nsmgr);

            return attribute?.InnerText;
        }
    }
}