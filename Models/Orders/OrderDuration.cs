using System.Runtime.Serialization;

namespace TradingAutomation.Models.Orders
{
    [DataContract]
    public class OrderDuration
    {
        [DataMember]
        public string DurationType { get; set; }
    }
}