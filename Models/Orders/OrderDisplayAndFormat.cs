using System.Runtime.Serialization;

namespace TradingAutomation.Models.Orders
{
    [DataContract]
    public class OrderDisplayAndFormat
    {
        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public int Decimals { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Format { get; set; }

        [DataMember]
        public string Symbol { get; set; }
    }
}