using MI.APIClientService;
using MI.Web.ApiModel.PictureService.Request;
using MI.Web.ApiModel.PictureService.Response;
using MI.Web.Common;
using MI.Web.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MI.Web.Service
{
    public class PictureService: IPictureService
    {
        public IConfiguration configuration;
        private readonly ILogger<PictureService> _logger;
        private readonly IApiHelperService apiHelperService;
        public PictureService(IConfiguration configuration, ILogger<PictureService> _logger, IApiHelperService apiHelperService)
        {
            this.configuration = configuration;
            this._logger = _logger;
            this.apiHelperService = apiHelperService;
        }

        public async Task<QueryHadrWareResponse> QueryHadrWare()
        {
            string url = $"{configuration["ServiceAddress:Service.Picture"]}{configuration["MehtodName:Picture.QueryPicture.QueryHadrWare"]}";
            return await apiHelperService.GetAsync<QueryHadrWareResponse>(url);
        }

        public async Task<QuerySlideImgResponse> QuerySlideImg()
        {
            string url = $"{configuration["ServiceAddress:Service.Picture"]}{configuration["MehtodName:Picture.QueryPicture.QuerySlideImg"]}";
            return await apiHelperService.GetAsync<QuerySlideImgResponse>(url);
        }

        public async Task<QueryStartProductResponse> QueryStartProduct(QueryStartProductRequest request)
        {
            string url = $"{configuration["ServiceAddress:Service.Picture"]}{configuration["MehtodName:Picture.QueryPicture.QueryStartProduct"]}";
            return await apiHelperService.PostAsync<QueryStartProductResponse>(url, request);
        }
    }
}
