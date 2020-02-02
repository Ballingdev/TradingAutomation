using Newtonsoft.Json.Linq;

namespace TradingAutomation.Streaming
{
    public interface IMessageHandler
    {
         void HandleMessageBundle(JArray jsonMessages);
    }
}