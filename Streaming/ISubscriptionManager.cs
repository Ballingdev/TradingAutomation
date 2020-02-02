using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TradingAutomation.Streaming
{
    public interface ISubscriptionManager
    {
        ConcurrentDictionary<string, InfoPriceSubscription> GetSubscriptions();
        InfoPriceSubscription FindSubscription(string referenceId);
        Task CreateSubscriptions(string contextId);
        Task<InfoPriceSubscription[]> DeleteSubscriptions();
        Task ResetSubscription(string referenceId);
    }
}