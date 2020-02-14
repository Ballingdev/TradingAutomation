using System;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;
using TradingAutomation.ApiConfig;
using TradingAutomation.ApiHelper;

namespace TradingAutomation.Streaming
{
    public class QuoteStream : IQuoteStream, IDisposable
    {
        private string _contextId;
        private static string StreamingStickinessUrl = Config.StreamingBaseUrl + "/streaming/isalive";
        private static string ServiceStickinessUri = Config.OpenApiBaseUrl + "/trade/isalive";
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly ISignalRConnectionManager _signalRConnectionManager;
        private readonly IActivityMonitor _activityMonitor;

        public QuoteStream(
            ISubscriptionManager subscriptionManager,
            ISignalRConnectionManager signalRConnectionManager,
            IActivityMonitor activityMonitor)
        {
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
            _signalRConnectionManager = signalRConnectionManager ?? throw new ArgumentNullException(nameof(signalRConnectionManager));
            _activityMonitor = activityMonitor ?? throw new ArgumentNullException(nameof(activityMonitor));
        }

        private async Task EnsureStickiness()
        {
            using (var response = await Helper.HttpClient.GetAsync(ServiceStickinessUri))
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.Error.WriteLine("Please insert a valid access token in App.config.");
                }
                response.EnsureSuccessStatusCode();
            }
            using (var response = await Helper.HttpClient.GetAsync(StreamingStickinessUrl))
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.Error.WriteLine("Please insert a valid access token in App.config.");
                }
                response.EnsureSuccessStatusCode();
            }
        }

        private void PrintSnapshots()
        {
            foreach (var subscription in _subscriptionManager.GetSubscriptions().Values)
            {
                string snapshot;
                lock (subscription.DataLock)
                {
                    snapshot = subscription.Data == null ? "Not received" : JsonConvert.SerializeObject(subscription.Data, Formatting.Indented);
                }
                Console.WriteLine($"[{subscription.ReferenceId}]: {snapshot}");
            }
        }

        public async Task Run()
        {
            _contextId = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            await EnsureStickiness();

            await Task.WhenAll(
                _signalRConnectionManager.CreateStreamingConnection(_contextId),
                _subscriptionManager.CreateSubscriptions(_contextId));

            _activityMonitor.StartActivityMonitor();

            Console.WriteLine("Press 'Q' to quit or SPACE to see the current data snapshots");
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    break;
                PrintSnapshots();
            }

            _activityMonitor.StopActivityMonitor();

            await _subscriptionManager.DeleteSubscriptions();
            _signalRConnectionManager.StopConnection();
        }

        public void Dispose()
        {
            if (Helper.HttpClient != null)
            {
                Helper.HttpClient.Dispose();
                Helper.HttpClient = null;
            }
        }
    }
}