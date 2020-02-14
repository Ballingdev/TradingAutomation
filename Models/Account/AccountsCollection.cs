using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TradingAutomation.Models.Account
{
    [DataContract]
    public class AccountsCollection : IGetModel
    {
        [DataMember]
        [JsonProperty(PropertyName = "Data")]
        public IEnumerable<Account> Accounts { get; set; }
    }
}