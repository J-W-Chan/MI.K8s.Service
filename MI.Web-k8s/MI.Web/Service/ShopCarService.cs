using MI.APIClientService;
using MI.Service.ShopCar.Model.Request;
using MI.Service.ShopCar.Model.Response;
using MI.Web.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.Service
{
    public class ShopCarService:IShopCarService
    {
        public IConfiguration configuration;
        private readonly ILogger<PictureService> _logger;
        private readonly IApiHelperService apiHelperService;

        public ShopCarService(IConfiguration configuration, ILogger<PictureService> _logger, IApiHelperService apiHelperService)
        {
            this.configuration = configuration;
            this._logger = _logger;
            this.apiHelperService = apiHelperService;
        }

        public async Task<GetShopListResponse> GetShopList(GetShopListRequest request)
        {
            string url = $"{configuration["ServiceAddress:Service.ShopCar"]}{configuration["MehtodName:ShopCar.ShopCarOperation.GetShopListAsync"]}";
            return await apiHelperService.PostAsync<GetShopListResponse>(url, request);
        }

        public async Task<AddCountResponse> AddCountAsync(AddCountRequest request)
        {
            string url = $"{configuration["ServiceAddress:Service.ShopCar"]}{configuration["MehtodName:ShopCar.ShopCarOperation.AddCountAsync"]}";
            return await apiHelperService.PostAsync<AddCountResponse>(url, request);

        }

        public async Task<GetColorResponse> GetColorAsync(GetColorRequest request)
        {
            string url = $"{configuration["ServiceAddress:Service.ShopCar"]}{configuration["MehtodName:ShopCar.ShopCarOperation.GetColorAsync"]}";
            return await apiHelperService.PostAsync<GetColorResponse>(url, request);
        }

        public async Task<AddShopCarResponse> AddShopCarAsync(AddShopCarRequest request)
        {
            string url = $"{configuration["ServiceAddress:Service.ShopCar"]}{configuration["MehtodName:ShopCar.ShopCarOperation.AddCarAsync"]}";
            return await apiHelperService.PostAsync<AddShopCarResponse>(url, request);
        }

        public async Task<DelProductResponse> DelProductAsync(DelProductRequest request)
        {
            string url = $"{configuration["ServiceAddress:Service.ShopCar"]}{configuration["MehtodName:ShopCar.ShopCarOperation.DelProductAsync"]}";
            return await apiHelperService.PostAsync<DelProductResponse>(url, request);
        }

        public async Task<ChangeCheckResponse> ChangeCheckAsync(ChangeCheckRequest request)
        {
            string url = $"{configuration["ServiceAddress:Service.ShopCar"]}{configuration["MehtodName:ShopCar.ShopCarOperation.ChangeCheckAsync"]}";
            return await apiHelperService.PostAsync<ChangeCheckResponse>(url, request);
        }
    }
}
