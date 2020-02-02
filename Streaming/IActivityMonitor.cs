using System.Timers;

namespace TradingAutomation.Streaming
{
    public interface IActivityMonitor
    {
        Timer GetActivityMonitorTimer();
        void StartActivityMonitor();
        void StopActivityMonitor();
    }
}