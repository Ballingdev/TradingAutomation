using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json.Linq;
using TradingAutomation.ApiConfig;
using TradingAutomation.ApiHelper;

namespace TradingAutomation.Streaming
{
    public class SignalRConnectionManager : ISignalRConnectionManager, IDisposable
    {
        private readonly IMessageHandler _messageHandler;
        private string StreamingConnectionUrl = Config.StreamingBaseUrl + "/streaming/connection";
        private Connection _streamingConnection;

        public SignalRConnectionManager(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        public Task CreateStreamingConnection(string contextId)
        {
            var transport = new AutoTransport(new DefaultHttpClient());

            var queryStringData = new Dictionary<string, string>
            {
                { "authorization", HttpUtility.UrlEncode($"Bearer {Config.ApiKey}") },

                { "context", HttpUtility.UrlEncode(contextId) }
            };

            _streamingConnection = new Connection(StreamingConnectionUrl, queryStringData)
            {
                CookieContainer = Helper.CookieContainer
            };

            _streamingConnection.StateChanged += Connection_StateChanged;
            _streamingConnection.Received += Connection_Received;
            _streamingConnection.Error += Connection_Error;

            return _streamingConnection.Start(transport);
        }

        public void StopConnection() 
        {
            _streamingConnection.Stop();
        }

        public void Dispose()
        {
            if (_streamingConnection != null) 
            {
                _streamingConnection.Dispose();
                _streamingConnection = null;
            }
        }

        private void Connection_StateChanged(StateChange stateChange)
        {
            Console.WriteLine($"[Connection]: {stateChange.NewState}");
        }

        private void Connection_Received(string message)
        {
            Console.WriteLine("[Connection]: Message received");
            _messageHandler.HandleMessageBundle(JArray.Parse(message));
        }

        private void Connection_Error(Exception exception)
        {
            Console.Error.WriteLine($"[Connection]: Error: {exception.Message}");
        }
    }
}