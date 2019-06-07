using MI.Service.Account.Model;
using MI.Service.Account.Model.Request;
using MI.Service.Account.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.IService
{
    public interface IAccountService
    {
        Task<SSOLoginResponse> SSOLogin(SSOLoginRequest request);
        GetShopCarByUserNameResponse GetUserInfoByUserNameAsync(GetShopCarByUserNameRequest request);
    }
}
