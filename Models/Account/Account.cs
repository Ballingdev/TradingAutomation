using System;
using System.Runtime.Serialization;

namespace TradingAutomation.Models.Account
{
    [DataContract]
    public class Account
    {
        [DataMember]
        public string AccountGroupKey { get; set; }

        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string AccountKey { get; set; }

        [DataMember]
        public string AccountType { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public bool CanUseCashPositionsAsMarginCollateral { get; set; }

        [DataMember]
        public bool CfdBorrowingCostsActive { get; set; }

        [DataMember]
        public string ClientId { get; set; }

        [DataMember]
        public string ClientKey { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public int CurrencyDecimals { get; set; }

        [DataMember]
        public bool DirectMarketAccess { get; set; }

        [DataMember]
        public bool IndividualMargining { get; set; }

        [DataMember]
        public bool IsCurrencyConversionAtSettlementTime { get; set; }

        [DataMember]
        public bool IsMarginTradingAllowed { get; set; }

        [DataMember]
        public bool IsShareable { get; set; }

        [DataMember]
        public bool IsTrialAccount { get; set; }

        [DataMember]
        public string[] LegalAssetTypes { get; set; }

        [DataMember]
        public string[] Sharing { get; set; }

        [DataMember]
        public bool SupportsAccountValueProtectionLimit { get; set; }

        [DataMember]
        public bool UseCashPositionsAsMarginCollateral { get; set; }
    }
}