using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using MI.APIClientService;
using MI.MQStationServer.Model.Request;
using MI.MQStationServer.Model.Response;
using MI.Service.Monitor.Model.Request;
using MI.Service.Monitor.Model.Response;
using MI.Untity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MI.MQStationServer.Controllers
{
    public class MQConsumerOperationController : Controller
    {
        private readonly ILifetimeScope _autofac;
        private readonly string AUTOFAC_SCOPE_NAME = "mi_event_bus";
        private readonly IApiHelperService _apiHelperService;
        private readonly ILogger _logger;
        private readonly IConfiguration configuration;
        public MQConsumerOperationController(ILifetimeScope autofac, ILogger<MQConsumerOperationController> logger, IApiHelperService apiHelperService, IConfiguration configuration)
        {
            _autofac = autofac;
            _logger = logger;
            _apiHelperService = apiHelperService;
            this.configuration = configuration;
        }

        public string Index()
        {
            return "Test";
        }

        /// <summary>
        /// MQ消费到指定的服务接口
        /// </summary>
        [HttpPost]
        public async Task<ConsumerProcessEventResponse> ConsumerProcessEventAsync([FromBody]ConsumerProcessEventRequest request)
        {
            ConsumerProcessEventResponse response = new ConsumerProcessEventResponse();
            try
            {
                _logger.LogInformation($"MQ准备执行ConsumerProcessEvent方法，RoutingKey:{request.RoutingKey} Message:{request.MQBodyMessage}");
                using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    //获取绑定该routingKey的服务地址集合
                    var subscriptions = await StackRedis.Current.GetAllList(request.RoutingKey);
                    if (!subscriptions.Any())
                    {
                        //如果Redis中不存在 则从数据库中查询 加入Redis中
                        string url = $"{configuration["ServiceAddress:Service.Monitor"]}{configuration["MehtodName:Monitor.QueryRabbitMq.QueryRoutingKeyApiUrlAsync"]}";
                        var queryRoutingKeyApiUrlResponse = _apiHelperService.PostAsync<QueryRoutingKeyApiUrlResponse>(url, new QueryRoutingKeyApiUrlRequest { RoutingKey = request.RoutingKey });
                        if (queryRoutingKeyApiUrlResponse.Result != null && queryRoutingKeyApiUrlResponse.Result.ApiUrlList.Any())
                        {
                            subscriptions = queryRoutingKeyApiUrlResponse.Result.ApiUrlList;
                            Task.Run(() =>
                            {
                                StackRedis.Current.SetLists(request.RoutingKey, queryRoutingKeyApiUrlResponse.Result.ApiUrlList);
                            });
                        }
                    }
                    if(subscriptions!=null && subscriptions.Any())
                    {
                        foreach (var apiUrl in subscriptions)
                        {
                            Task.Run(() =>
                            {
                                _logger.LogInformation(request.MQBodyMessage);
                            });

                            //这里需要做判断 假如MQ要发送到多个服务接口 其中一个消费失败 应该将其单独记录到数据库 而不影响这个消息的确认
                            await _apiHelperService.PostAsync(apiUrl, request.MQBodyMessage);
                        }
                        _logger.LogInformation($"MQ执行ProcessEvent方法完成，RoutingKey:{request.RoutingKey} Message:{request.MQBodyMessage}");
                    }
                }
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                _logger.LogError(ex, $"MQ消费失败 RoutingKey:{request.RoutingKey} Message:{request.MQBodyMessage}");
            }

            return response;
        }
    }

    public class Test
    {
        public int CarId { get; set; }
        public int ProductId { get; set; }
        public int VersionId { get; set; }
        public int Num { get; set; }
    }
}