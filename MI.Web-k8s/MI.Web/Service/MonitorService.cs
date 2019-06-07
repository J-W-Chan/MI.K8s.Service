using MI.APIClientService;
using MI.Service.Monitor.Model.Request;
using MI.Service.Monitor.Model.Response;
using MI.Web.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.Service
{
    public class MonitorService: IMonitorService
    {
        public IConfiguration configuration;
        private readonly ILogger<PictureService> _logger;
        private readonly IApiHelperService apiHelperService;

        public MonitorService(IConfiguration configuration, ILogger<PictureService> _logger, IApiHelperService apiHelperService)
        {
            this.configuration = configuration;
            this._logger = _logger;
            this.apiHelperService = apiHelperService;
        }
        public async Task<QueryRabbitMqRoutingKeyInfoResponse> QueryRabbitMqRoutingKeyInfoAsync(QueryRabbitMqRoutingKeyInfoRequest request)
        {
            string url = $"{configuration["ServiceAddress:Service.Monitor"]}{configuration["MehtodName:Monitor.QueryRabbitMq.QueryRabbitMqRoutingKeyInfoAsync"]}";
            return await apiHelperService.PostAsync<QueryRabbitMqRoutingKeyInfoResponse>(url, request);
        }

        public async Task<RoutingKeyRegisterResponse> RoutingKeyRegisterAsync(RoutingKeyRegisterRequest request)
        {
            string url = $"{configuration["ServiceAddress:Service.Monitor"]}{configuration["MehtodName:Monitor.RabbitMQManager.RoutingKeyRegisterAsync"]}";
            return await apiHelperService.PostAsync<RoutingKeyRegisterResponse>(url, request);
        }
    }
}
