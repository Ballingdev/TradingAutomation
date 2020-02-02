using System.Threading.Tasks;

namespace TradingAutomation.Streaming
{
    public interface ISignalRConnectionManager
    {
         Task CreateStreamingConnection(string contextId);
         void StopConnection();
    }
}