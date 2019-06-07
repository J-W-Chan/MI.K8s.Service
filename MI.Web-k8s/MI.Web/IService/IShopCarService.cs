using MI.Service.ShopCar.Model.Request;
using MI.Service.ShopCar.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.IService
{
    public interface IShopCarService
    {
        Task<GetShopListResponse> GetShopList(GetShopListRequest request);

        Task<AddCountResponse> AddCountAsync(AddCountRequest request);

        Task<GetColorResponse> GetColorAsync(GetColorRequest request);

        Task<AddShopCarResponse> AddShopCarAsync(AddShopCarRequest request);

        Task<DelProductResponse> DelProductAsync(DelProductRequest request);

        Task<ChangeCheckResponse> ChangeCheckAsync(ChangeCheckRequest request);
    }
}
