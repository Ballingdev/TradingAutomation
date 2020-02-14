using System;
using System.Runtime.Serialization;

namespace TradingAutomation.Models.Orders.Response
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AccountKey { get; set; }

        [DataMember]
        public string Amount { get; set; }

        [DataMember]
        public string AssetType { get; set; }

        [DataMember]
        public string BuySell { get; set; }

        [DataMember]
        public string CalculationReliability { get; set; }

        [DataMember]
        public string ClientKey { get; set; }

        [DataMember]
        public string CurrentPrice { get; set; }

        [DataMember]
        public int CurrentPriceDelayMinutes { get; set; }

        [DataMember]
        public OrderDisplayAndFormat DisplayAndFormat { get; set; }

        [DataMember]
        public string DistanceToMarket { get; set; }

        [DataMember]
        public OrderDuration Duration { get; set; }

        [DataMember]
        public bool IsMarketOpen { get; set; }

        [DataMember]
        public decimal MarketPrice { get; set; }

        [DataMember]
        public string OpenOrderType { get; set; }

        [DataMember]
        public string OrderAmountType { get; set; }

        [DataMember]
        public string OrderId { get; set; }

        [DataMember]
        public string OrderRelation { get; set; }

        [DataMember]
        public DateTime OrderTime { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public int Uic { get; set; }
    }
}