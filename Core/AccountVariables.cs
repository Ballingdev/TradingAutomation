using System;
using System.Linq;
using TradingAutomation.Models.Account;
using TradingAutomation.Services.Account;

namespace TradingAutomation.Core
{
    public class AccountVariables
    {
        private readonly IAccountService _accountService;

        public AccountVariables(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        public void InitializeVariables()
        {
            AccountsCollection accounts = _accountService.GetAccountData().Result;

            foreach(Account acc in accounts.Accounts) {
                Console.WriteLine($"{acc.AccountKey}");
                Console.WriteLine($"{acc.Currency}");
            }

            ClientKey = accounts.Accounts.FirstOrDefault().ClientKey;
            AccountKey = accounts.Accounts.FirstOrDefault().AccountKey;
        }

        public static string ClientKey { get; private set; }
        public static string AccountKey { get; private set; }
    }
}