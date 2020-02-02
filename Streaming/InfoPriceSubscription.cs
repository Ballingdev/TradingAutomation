using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TradingAutomation.Streaming
{
    public class InfoPriceSubscription
    {
        public readonly object DataLock = new object();
        private Queue<JArray> _updateQueue = new Queue<JArray>();
        private bool _updateQueueComplete;
        public string ReferenceId { get; }
        public IReadOnlyCollection<int> Uics { get; }
        public string AssetType { get; }
        public TimeSpan InactivityTimeout { get; set; }
        private DateTime _lastUpdated;
        public InfoPrice[] Data { get; set; }
        public bool Inactive => DateTime.Now - _lastUpdated > InactivityTimeout;

        public void UpdateActivity()
        {
            _lastUpdated = DateTime.Now;
        }

        public void SetSnapshot(JToken jsonSnapshot)
        {
            var infoPrices = new List<InfoPrice>();
            var jsonPrices = (JArray)jsonSnapshot;
            foreach (var jsonPrice in jsonPrices)
            {
                var jsonQuote = jsonPrice["Quote"];
                var quote = new Quote
                {
                    Amount = (int)jsonQuote["Amount"],
                    Ask = (decimal)jsonQuote["Ask"],
                    Bid = (decimal)jsonQuote["Bid"],
                    Mid = (decimal)jsonQuote["Mid"],
                    DelayedByMinutes = (int)jsonQuote["DelayedByMinutes"]
                };

                var infoPrice = new InfoPrice
                {
                    Uic = (int)jsonPrice["Uic"],
                    AssetType = (string)jsonPrice["AssetType"],
                    Quote = quote
                };

                infoPrices.Add(infoPrice);
            }

            lock (DataLock)
            {
                Data = infoPrices.ToArray();

                if (_updateQueueComplete)
                    FlushUpdateQueue();
            }
        }

        private void ApplyUpdate(JArray jsonPrices)
        {
            foreach (var jsonPrice in jsonPrices)
            {
                var uic = (int)jsonPrice["Uic"];
                var infoPrice = Data.FirstOrDefault(p => p.Uic == uic);
                if (infoPrice == null)
                {
                    Console.Error.WriteLine($"[{ReferenceId}]: Received an update for an unknown UIC: {uic}");
                    continue;
                }

                var jsonQuote = jsonPrice["Quote"];
                if (jsonQuote != null)
                {
                    if (jsonQuote["Amount"] != null)
                    {
                        infoPrice.Quote.Amount = (int)jsonQuote["Amount"];
                    }
                    if (jsonQuote["Ask"] != null)
                    {
                        infoPrice.Quote.Ask = (decimal)jsonQuote["Ask"];
                    }
                    if (jsonQuote["Bid"] != null)
                    {
                        infoPrice.Quote.Bid = (decimal)jsonQuote["Bid"];
                    }
                    if (jsonQuote["Mid"] != null)
                    {
                        infoPrice.Quote.Mid = (decimal)jsonQuote["Mid"];
                    }
                    if (jsonQuote["DelayedByMinutes"] != null)
                    {
                        infoPrice.Quote.DelayedByMinutes = (int)jsonQuote["DelayedByMinutes"];
                    }
                }
            }
        }

        private static bool IsComplete(JToken update)
        {
            var pn = update["__pn"];
            var pc = update["__pc"];
            if (pn == null || pc == null)
                return true;
            
            var partitionNumber = (int)pn;
            var partitionCount = (int)pc;
            return partitionNumber == partitionCount - 1;
        }

        private void EnqueueUpdate(JToken update)
        {
            var data = (JArray)update["Data"];
            _updateQueueComplete = IsComplete(update);
            _updateQueue.Enqueue(data);
        }

        private void FlushUpdateQueue()
        {
            while (_updateQueue.Count > 0)
            {
                var update = _updateQueue.Dequeue();
                ApplyUpdate(update);
            }
        }

        public void HandleUpdate(JToken update)
        {
            lock (DataLock)
            {
                EnqueueUpdate(update);

                if (Data == null)
                    return;

                if (_updateQueueComplete)
                    FlushUpdateQueue();
            }
        }

        public InfoPriceSubscription(string referenceId, IEnumerable<int> uics, string assetType)
        {
            ReferenceId = referenceId;
            Uics = uics.ToArray();
            AssetType = assetType;

            InactivityTimeout = TimeSpan.FromMinutes(2);
            UpdateActivity();
        }
    }
}