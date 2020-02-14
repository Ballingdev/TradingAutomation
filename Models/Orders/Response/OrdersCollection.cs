using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TradingAutomation.Models.Orders.Response
{
    [DataContract]
    public class OrdersCollection : IGetModel
    {
        [DataMember]
        [JsonProperty(PropertyName = "Data")]
        public IEnumerable<Order> AllOrders { get; set; }
    }
}