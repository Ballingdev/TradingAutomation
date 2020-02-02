namespace TradingAutomation.Streaming
{
public class Quote
    {
        public int Amount { get; set; }

        public decimal Bid { get; set; }

        public decimal Ask { get; set; }

        public decimal Mid { get; set; }

        public int DelayedByMinutes { get; set; }
    }
}