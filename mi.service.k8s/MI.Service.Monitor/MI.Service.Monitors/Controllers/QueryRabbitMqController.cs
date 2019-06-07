using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MI.Service.Monitor.Entity;
using MI.Service.Monitor.Model.Request;
using MI.Service.Monitor.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MI.Service.Monitor.Controllers
{
    //[Authorize]
    public class QueryRabbitMqController : Controller
    {
        public MIContext _context;
        public QueryRabbitMqController(MIContext _context)
        {
            this._context = _context;
        }


        /// <summary>
        /// 根据RoutingKey查询对应的服务地址
        /// </summary>
        public async Task<QueryRoutingKeyApiUrlResponse> QueryRoutingKeyApiUrlAsync([FromBody]QueryRoutingKeyApiUrlRequest request)
        {
            QueryRoutingKeyApiUrlResponse response = new QueryRoutingKeyApiUrlResponse();

            var routingKeyApiUrlList = _context.RabbitMqRegisterInfo.Where(a => a.RoutingKey == request.RoutingKey);
                
            if(!string.IsNullOrEmpty(request.QueueName))
            {
                routingKeyApiUrlList.Where(a => a.QueueName == request.QueueName);
            }

            var routingKeyApiUrls = routingKeyApiUrlList.Select(a => a.ApiUrl).ToList();
            if (routingKeyApiUrls.Any())
            {
                response.ApiUrlList = routingKeyApiUrls;
            }
            return response;
        }

        /// <summary>
        /// 查询RabbitMQ配置信息列表
        /// </summary>
        public async Task<QueryRabbitMqRoutingKeyInfoResponse> QueryRabbitMqRoutingKeyInfoAsync([FromBody]QueryRabbitMqRoutingKeyInfoRequest request)
        {
            QueryRabbitMqRoutingKeyInfoResponse response = new QueryRabbitMqRoutingKeyInfoResponse();

            var rabbitMQRegisterInfos = _context.RabbitMqRegisterInfo.GroupBy(a=>a.RoutingKey).OrderByDescending(a=>1).Skip((request.PageIndex-1)*request.PageSize);
            foreach(var item in rabbitMQRegisterInfos)
            {
                response.RabbitMQRegisterInfos.Add(new RabbitMQRegisterInfoEntity
                {
                    RoutingKey=item.Key,
                    ApiUrlList=item.Select(a=>a.ApiUrl).ToList()
                });
            }

            return response;
        }
    }
}