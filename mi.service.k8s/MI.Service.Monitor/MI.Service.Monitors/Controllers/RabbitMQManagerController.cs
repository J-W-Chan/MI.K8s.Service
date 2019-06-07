using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Abstractions;
using MI.Service.Monitor.Entity;
using MI.Service.Monitor.Model.Request;
using MI.Service.Monitor.Model.Response;
using MI.Untity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MI.Service.Monitor.Controllers
{
    //[Authorize]
    public class RabbitMQManagerController : Controller
    {
        private readonly MIContext _context;
        private readonly IEventBus _eventBus;
        private readonly IConfiguration Configuration;

        public RabbitMQManagerController(MIContext _context, IEventBus _eventBus, IConfiguration Configuration)
        {
            this._context = _context;
            this._eventBus = _eventBus;
            this.Configuration = Configuration;
        }

        public string Index()
        {
            return "Successful";
        }

        /// <summary>
        /// 新增RoutingKey
        /// </summary>
        [HttpPost]
        public async Task<RoutingKeyRegisterResponse> RoutingKeyRegisterAsync([FromBody]RoutingKeyRegisterRequest request)
        {
            RoutingKeyRegisterResponse response = new RoutingKeyRegisterResponse();
            try
            {
                if (string.IsNullOrEmpty(request.RoutingKey) || string.IsNullOrEmpty(request.ApiUrl))
                {
                    response.Successful = false;
                    response.Message = "RoutingKey与ApiUrl不能为空！";
                }
                if (string.IsNullOrEmpty(request.QueueName))
                {
                    request.QueueName = Configuration["SubscriptionClientName"];
                }
                var isExistsRoutingKey = _context.RabbitMqRegisterInfo.Any(a => a.RoutingKey == request.RoutingKey);
                if (isExistsRoutingKey)
                {
                    response.Successful = false;
                    response.Message = "对应的RoutingKey已存在！";
                    return response;
                }
                var rabbitMqRegister = new RabbitMQRegisterInfo
                {
                    RoutingKey = request.RoutingKey,
                    QueueName = request.QueueName,
                    ApiUrl = request.ApiUrl,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };

                _context.Add(rabbitMqRegister);
                _context.SaveChanges();

                //绑定RoutingKey和队列
                _eventBus.Subscribe(request.QueueName, request.RoutingKey);
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
            }

            
            return response;
        }

        /// <summary>
        /// RoutingKey信息修改
        /// </summary>
        [HttpPost]
        public async Task<RoutingKeyRegisterResponse> UpdateRoutingKeyInfoAsync([FromBody]RoutingKeyRegisterRequest request)
        {
            RoutingKeyRegisterResponse response = new RoutingKeyRegisterResponse();
            var routingKeyInfo = _context.RabbitMqRegisterInfo.SingleOrDefault(a => a.RoutingKey == request.RoutingKey);

            if (routingKeyInfo != null)
            {
                //修改Redis内的服务地址
                var redisRoutingApiUrls = await StackRedis.Current.GetAllList(request.RoutingKey);
                redisRoutingApiUrls.Remove(routingKeyInfo.ApiUrl);
                redisRoutingApiUrls.Add(request.ApiUrl);
                await StackRedis.Current.SetList(request.RoutingKey, redisRoutingApiUrls);

                routingKeyInfo.ApiUrl = request.ApiUrl;
                routingKeyInfo.UpdateTime = DateTime.Now;

                _context.Update(routingKeyInfo);
                _context.SaveChanges();
            }

            return response;
        }


        /// <summary>
        /// 删除RoutingKey
        /// </summary>
        [HttpPost]
        public async Task<RoutingKeyRegisterResponse> DeleteRoutingKeyAsync([FromBody]RoutingKeyRegisterRequest request)
        {
            RoutingKeyRegisterResponse response = new RoutingKeyRegisterResponse();
            var routingKeyInfo = _context.RabbitMqRegisterInfo.SingleOrDefault(a => a.RoutingKey == request.RoutingKey);
            if(routingKeyInfo!=null)
            {
                //删除Redis中的服务地址
                var routingApiUrlInRedis = StackRedis.Current.Remove(request.RoutingKey);
                //解绑RabbitMQ
                _eventBus.UnSubscribe(routingKeyInfo.QueueName, routingKeyInfo.RoutingKey);
                //删除数据库数据
                _context.RabbitMqRegisterInfo.Remove(routingKeyInfo);
            }
            return response;
        }

        /// <summary>
        /// MQ RoutingKey绑定队列
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MQSubscribeResponse> MQSubscribeAsync([FromBody]MQSubscribeRequest request)
        {
            MQSubscribeResponse response = new MQSubscribeResponse();
            var queueName = Configuration["SubscriptionClientName"];
            var rabbitMQRegisters = _context.RabbitMqRegisterInfo.ToList();
            foreach(var item in rabbitMQRegisters)
            {
                _eventBus.Subscribe(queueName, item.RoutingKey);
            }

            return response;
        }

    }
}