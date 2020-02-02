using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace TradingAutomation.Streaming
{
    public class ActivityMonitor : IActivityMonitor, IDisposable
    {
        private readonly ISubscriptionManager _subscriptionManager;
        private Timer _activityMonitorTimer;

        public ActivityMonitor(ISubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        }

        public Timer GetActivityMonitorTimer() 
        {
            return _activityMonitorTimer;
        }

        public void StartActivityMonitor()
        {
            _activityMonitorTimer = new Timer { AutoReset = true, Interval = 10000 };
            _activityMonitorTimer.Elapsed += (sender, eventArgs) => MonitorActivity().Wait();
            _activityMonitorTimer.Start();
        }

        public void StopActivityMonitor()
        {
            _activityMonitorTimer.Stop();
        }

        public void Dispose()
        {
            if (_activityMonitorTimer != null)
                _activityMonitorTimer.Dispose();
                _activityMonitorTimer = null;
        }

        private async Task MonitorActivity()
        {
            var inactiveSubscriptionReferenceIds = _subscriptionManager.GetSubscriptions().Values
                .Where(s => s.Inactive)
                .Select(s => s.ReferenceId)
                .ToArray();

            if (inactiveSubscriptionReferenceIds.Length > 0)
            {
                Console.WriteLine($"[Activity monitor]: Inactive subscriptions found: {string.Join(", ", inactiveSubscriptionReferenceIds)}");

                await Task.WhenAll(inactiveSubscriptionReferenceIds.Select(_subscriptionManager.ResetSubscription));
            }
            else
            {
                Console.WriteLine("[Activity monitor]: No inactive subscriptions");
            }
        }
    }
}