using System.Collections.Generic;
using System.Threading.Tasks;
using TradingAutomation.Models;

namespace TradingAutomation.Services
{
    public interface IService
    {
        Task<T> GetData<T>(string url) where T : IGetModel;
        Task<T> GetData<T>(string url, List<KeyValuePair<string, string>> queryParams) where T : IGetModel;
        Task<string> PostData<T>(string url, T model) where T : IPostModel;
        Task<string> PatchData<T>(string url, T model, params string[] urlParams) where T : IPatchModel;
        Task<string> DeleteData(string url, List<KeyValuePair<string, string>> queryParams, params string[] urlParams);

    }
}