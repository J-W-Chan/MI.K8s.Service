using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.APIClientService
{
    public interface IApiHelperService
    {
        Task<T> PostAsync<T>(string url, object Model);
        Task<T> GetAsync<T>(string url);
        Task PostAsync(string url, string requestMessage);
    }
}
