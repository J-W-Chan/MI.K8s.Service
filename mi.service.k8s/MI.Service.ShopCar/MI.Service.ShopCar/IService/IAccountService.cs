using MI.Service.Account.Model;
using MI.Service.Account.Model.Request;
using MI.Service.Account.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Service.ShopCar.Service
{
    public interface IAccountService
    {
        GetShopCarByUserNameResponse GetUserInfoByUserNameAsync(GetShopCarByUserNameRequest request);
    }
}
