using MI.APIClientService;
using MI.Service.Account.Model.Request;
using MI.Service.Account.Model.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MI.Service.ShopCar.Service
{
    public class AccountService:IAccountService
    {
        public readonly IConfiguration configuration;
        private readonly ILogger<AccountService> _logger;
        private readonly IApiHelperService apiHelperService;
        public AccountService(IConfiguration configuration, ILogger<AccountService> logger, IApiHelperService apiHelperService)
        {
            this.configuration = configuration;
            this._logger = logger;
            this.apiHelperService = apiHelperService;
        }

        public GetShopCarByUserNameResponse GetUserInfoByUserNameAsync(GetShopCarByUserNameRequest request)
        {
            string url = $"{configuration["ServiceAddress:Service.Account"]}{configuration["MehtodName:Account.MiUser.GetUserInfoByUserNameAsync"]}";

            return apiHelperService.PostAsync<GetShopCarByUserNameResponse>(url, request).Result;
        }
    }
}
