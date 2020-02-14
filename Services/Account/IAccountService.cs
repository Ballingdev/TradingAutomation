using System.Threading.Tasks;
using TradingAutomation.Models.Account;

namespace TradingAutomation.Services.Account
{
    public interface IAccountService
    {
        Task<AccountsCollection> GetAccountData();
    }
}