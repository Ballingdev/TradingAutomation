using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TradingAutomation.ApiConfig;
using TradingAutomation.ApiHelper;
using static System.FormattableString;

namespace TradingAutomation.Streaming
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private string SubscriptionUrl = Config.OpenApiBaseUrl + ConfigurationManager.AppSettings["SubscriptionUrl"];
        private string DeleteSubscriptionUrl = Config.OpenApiBaseUrl + "/trade/v1/infoprices/subscriptions";
        private string _contextId;
        private int _nextSubscriptionReferenceId;
        private ConcurrentDictionary<string, InfoPriceSubscription> _subscriptions = new ConcurrentDictionary<string, InfoPriceSubscription>();

        public ConcurrentDictionary<string, InfoPriceSubscription> GetSubscriptions() 
        {
            return _subscriptions;
        }

        private string GetReferenceId(string prefix)
        {
            var sequenceNo = Interlocked.Increment(ref _nextSubscriptionReferenceId);
            return Invariant($"{prefix}{sequenceNo}");
        }

        public InfoPriceSubscription FindSubscription(string referenceId)
        {
            InfoPriceSubscription subscription;
            if (!_subscriptions.TryGetValue(referenceId, out subscription))
                return null;
            return subscription;
        }

        public Task CreateSubscriptions(string contextId)
        {
            _contextId = contextId;

            return Task.WhenAll(
                CreateSubscription(GetReferenceId("infoprice"), "FxSpot", new[] { 21 }),
                CreateSubscription(GetReferenceId("infoprice"), "FxSpot", new[] { 22 }));
        }

        private async Task CreateSubscription(string referenceId, string assetType, IEnumerable<int> uics)
        {
            var subscription = new InfoPriceSubscription(referenceId, uics, assetType);
            if (!_subscriptions.TryAdd(referenceId, subscription))
            {
                Console.Error.WriteLine($"Subscription already exists: {referenceId}");
                return;
            }

            var requestData = new
            {
                Arguments = new
                {
                    AssetType = assetType,
                    Uics = string.Join(",", uics)
                },

                ContextId = _contextId,
                ReferenceId = referenceId,
                RefreshRate = 1000
            };

            var requestBody = JsonConvert.SerializeObject(requestData);
            using (var content = new StringContent(requestBody, Encoding.UTF8, "application/json"))
            {
                using (var response = await Helper.HttpClient.PostAsync(SubscriptionUrl, content))
                {
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseString);

                    var inactivityTimeout = (int)json["InactivityTimeout"];
                    subscription.InactivityTimeout = TimeSpan.FromSeconds(inactivityTimeout);

                    var snapshot = json["Snapshot"]["Data"];

                    subscription.SetSnapshot(snapshot);
                    subscription.UpdateActivity();

                    Console.WriteLine($"[{referenceId}]: Snapshot received");
                }
            }
        }

        private async Task<InfoPriceSubscription> DeleteSubscription(string referenceId)
        {
            InfoPriceSubscription subscription;
            if (!_subscriptions.TryRemove(referenceId, out subscription))
                return null;

            var deleteUri = $"{DeleteSubscriptionUrl}/{_contextId}/{referenceId}";
            using (var response = await Helper.HttpClient.DeleteAsync(deleteUri))
            {
                response.EnsureSuccessStatusCode();
            }
            return subscription;
        }

        public Task<InfoPriceSubscription[]> DeleteSubscriptions()
        {
            return Task.WhenAll(_subscriptions.Values.Select(s => DeleteSubscription(s.ReferenceId)));
        }

        public async Task ResetSubscription(string referenceId)
        {
            var subscription = await DeleteSubscription(referenceId);
            if (subscription != null)
            {
                await CreateSubscription(GetReferenceId("infoprice"), subscription.AssetType, subscription.Uics);
            }
        }
    }
}