namespace TradingAutomation.Streaming
{
public class InfoPrice
    {
        public int Uic { get; set; }

        public string AssetType { get; set; }

        public Quote Quote { get; set; }
    }
}