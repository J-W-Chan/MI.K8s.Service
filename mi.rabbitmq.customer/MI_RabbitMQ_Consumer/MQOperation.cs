using Autofac;
using log4net;
using log4net.Core;
using MI.APIClientService;
using MI.MQStationServer.Model.Request;
using MI.MQStationServer.Model.Response;
using MI.Service.Monitor.Model.Request;
using MI.Service.Monitor.Model.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI_RabbitMQ_Consumer
{
    public class MQConsumerService
    {
        private readonly IApiHelperService _apiHelperService;
        private ILog _logger;

        public MQConsumerService(IApiHelperService apiHelperService,ILog logger)
        {
            _apiHelperService = apiHelperService;
            _logger = logger;
        }

        /// <summary>
        /// 发送MQ到MQ消费服务端
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        public void ProcessEvent(string routingKey, string message)
        {
            try
            {
                _logger.Info($"MQ准备执行ProcessEvent方法，RoutingKey:{routingKey} Message:{message}");
                _apiHelperService.PostAsync<ConsumerProcessEventResponse>(ServiceUrls.ConsumerProcessEvent,new ConsumerProcessEventRequest { RoutingKey=routingKey,MQBodyMessage=message});
            }
            catch(Exception ex)
            {
                _logger.Error($"MQ发送消费服务端失败 RoutingKey:{routingKey} Message:{message}",ex);
            }
        }

        /// <summary>
        /// MQ初始化 调用队列交换器绑定接口
        /// </summary>
        /// <returns></returns>
        public async Task MQSubscribeAsync()
        {
            try
            {
                var response= await _apiHelperService.PostAsync<MQSubscribeResponse>(ServiceUrls.MQSubscribe, new MQSubscribeRequest());
                if(!response.Successful)
                {
                    _logger.Error($"MQ绑定RoutingKey队列失败: {response.Message}");
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"MQ绑定RoutingKey队列失败",ex);
            }            
        }
    }
}
