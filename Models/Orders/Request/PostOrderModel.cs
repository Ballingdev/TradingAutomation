using System.Runtime.Serialization;
using TradingAutomation.Models;

namespace TradingAutomation.Models.Orders.Request
{
    [DataContract]
    public class PostOrderModel : IPostModel
    {
        [DataMember]
        public int Uic { get; set; }

        [DataMember]
        public string BuySell { get; set; }

        [DataMember]
        public string AssetType { get; set; }

        [DataMember]
        public decimal OrderPrice { get; set; }

        [DataMember]
        public string OrderType { get; set; }

        [DataMember]
        public string OrderRelation { get; set; }

        [DataMember]
        public OrderDuration OrderDuration { get; set; }

        [DataMember]
        public string AccountKey { get; set; }
    }
}