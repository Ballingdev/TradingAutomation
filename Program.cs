
using System;
using StructureMap;
using TradingAutomation.Streaming;

namespace TradingAutomation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var container = new Container();

            container.Configure(c => c.Scan(_ => 
            {
                _.AssemblyContainingType(typeof(Program));
                _.WithDefaultConventions();
            }));

            var subscriptionManager = container.GetInstance<ISubscriptionManager>();
            var signalRConnectionManager = container.GetInstance<ISignalRConnectionManager>();
            var activityMonitor = container.GetInstance<IActivityMonitor>();

            try
            {
                using (var stream = new QuoteStream(subscriptionManager, signalRConnectionManager, activityMonitor))
                    stream.Run().Wait();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Stream terminated with exception:");
                Console.Error.WriteLine($"  {ex.GetType()}: {ex.Message}");
                if (ex.InnerException != null)
                    Console.Error.WriteLine($"  {ex.InnerException.GetType()}: {ex.InnerException.Message}");
            }
        }
    }
}
