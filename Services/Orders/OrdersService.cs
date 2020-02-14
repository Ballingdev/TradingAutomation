using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using TradingAutomation.Core;
using TradingAutomation.Models.Orders.Request;

namespace TradingAutomation.Services.Orders
{
    public class OrdersService : IOrdersService
    {
        private readonly string OrdersUrl = ConfigurationManager.AppSettings["OrdersUrl"];
        private readonly string MyOrdersUrl = ConfigurationManager.AppSettings["MyOrdersDataUrl"];
        private readonly IService _service;

        public OrdersService(IService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<string> PostOrder(PostOrderModel model)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            return await _service.PostData(OrdersUrl, model);
        }

        public async Task<string> DeleteOrder(params string[] urlParams)
        {
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(key: "AccountKey", value: AccountVariables.AccountKey)
            };

            return await _service.DeleteData(OrdersUrl, queryParams, urlParams);
        }
    }
}