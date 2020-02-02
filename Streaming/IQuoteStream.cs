using System.Threading.Tasks;

namespace TradingAutomation.Streaming
{
    public interface IQuoteStream
    {
         Task Run();
    }
}