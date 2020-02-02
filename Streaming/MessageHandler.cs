using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TradingAutomation.Streaming
{
    public class MessageHandler : IMessageHandler
    {
        private readonly ISubscriptionManager _subscriptionManager;

        public MessageHandler(ISubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        }

        public void HandleMessageBundle(JArray jsonMessages)
        {
            foreach (var jsonMessage in jsonMessages)
                HandleMessage((JObject)jsonMessage);
        }

        private void HandleMessage(JObject jsonMessage)
        {
            var referenceId = (string)jsonMessage["ReferenceId"];

            if (referenceId.Equals("_heartbeat"))
            {
                HandleSubscriptionHeartbeatMessage(jsonMessage);
            }
            else if (referenceId.Equals("_resetsubscriptions"))
            {
                HandleResetSubscriptionsMessage(jsonMessage);
            }
            else if (referenceId.StartsWith("_"))
            {
                Console.WriteLine($"Unknown control message type received: {referenceId}");
            }
            else
            {
                HandleDataMessage(referenceId, jsonMessage);
            }
        }

        private void HandleSubscriptionHeartbeatMessage(JObject jsonMessage)
        {
            var jsonHeartbeats = (JArray)jsonMessage["Heartbeats"];
            foreach (var jsonHeartbeat in jsonHeartbeats)
            {
                var referenceId = (string)jsonHeartbeat["OriginatingReferenceId"];
                var subscription = _subscriptionManager.FindSubscription(referenceId);
                if (subscription != null)
                {
                    subscription.UpdateActivity();
                }
            }
        }

        private void HandleResetSubscriptionsMessage(JObject jsonMessage)
        {
            string[] targetReferenceIds;
            if (jsonMessage["TargetReferenceIds"] != null)
            {
                targetReferenceIds = ((JArray)jsonMessage["TargetReferenceIds"]).Select(r => (string)r).ToArray();

                if (targetReferenceIds.Length == 0)
                {
                    targetReferenceIds = _subscriptionManager.GetSubscriptions().Values.Select(s => s.ReferenceId).ToArray();
                }
            }
            else
            {
                targetReferenceIds = _subscriptionManager.GetSubscriptions().Values.Select(s => s.ReferenceId).ToArray();
            }

            Task.WhenAll(targetReferenceIds.Select(_subscriptionManager.ResetSubscription)).Wait();
        }

        private void HandleDataMessage(string referenceId, JObject jsonMessage)
        {
            var subscription = _subscriptionManager.FindSubscription(referenceId);
            
            if (subscription == null)
            {
                Console.WriteLine($"[{referenceId}]: Message for unknown subscription discarded");
            }
            else
            {
                subscription.UpdateActivity();
                subscription.HandleUpdate(jsonMessage);
                Console.WriteLine($"[{referenceId}]: Update received, hit SPACE to see current snapshot");
            }
        }
    }
}