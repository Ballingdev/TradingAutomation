using System;
using System.Configuration;
using System.Threading.Tasks;
using TradingAutomation.Models.Account;

namespace TradingAutomation.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly string BaseUrl = ConfigurationManager.AppSettings["OpenApiBaseUrl"];
        private readonly string MyAccountDataUrl = ConfigurationManager.AppSettings["MyAccountDataUrl"];
        private readonly IService _service;

        public AccountService(IService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<AccountsCollection> GetAccountData()
        {
            return await _service.GetData<AccountsCollection>(BaseUrl + MyAccountDataUrl);
        }
    }
}