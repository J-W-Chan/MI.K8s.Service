using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MI.Web.IService;
using Microsoft.AspNetCore.Mvc;

namespace MI.Web.Controllers
{
    public class RabbitMqManagerController : Controller
    {
        private readonly IMonitorService monitorService;

        public RabbitMqManagerController(IMonitorService monitorService)
        {
            this.monitorService = monitorService;
        }
        public async Task<IActionResult> Index()
        {
            var response = await monitorService.QueryRabbitMqRoutingKeyInfoAsync(new MI.Service.Monitor.Model.Request.QueryRabbitMqRoutingKeyInfoRequest
            {
                PageIndex = 1,
                PageSize = 10
            });
            return View(response.RabbitMQRegisterInfos);
        }

        public IActionResult AddRoutingKey()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoutingKey(string routingKey,string apiUrl)
        {
            var response= await monitorService.RoutingKeyRegisterAsync(new MI.Service.Monitor.Model.Request.RoutingKeyRegisterRequest
            {
                RoutingKey=routingKey,
                ApiUrl=apiUrl
            });
            return Json("OK");
        }
    }
}