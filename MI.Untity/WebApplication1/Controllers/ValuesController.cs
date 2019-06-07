using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using MI.APIClientService;
using MI.Service.Monitor.Model.Request;
using MI.Service.Monitor.Model.Response;
using MI.Service.ShopCar.Model.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IEventBus _eventBus;
        private readonly IApiHelperService apiHelperService;
        private readonly IConfiguration configuration;
        public ValuesController(IEventBus _eventBus, IApiHelperService apiHelperService, IConfiguration configuration)
        {
            this._eventBus = _eventBus;
            this.apiHelperService = apiHelperService;
            this.configuration = configuration;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //var addShopCarRequest = new AddShopCarRequest { ProductID = 8, VersionID = 9 };
            ////string url = "http://localhost:7001/Account/MiUser/Register";
            ////var response = apiHelperService.PostAsync<RegisterResponse>(url, request).Result;
            ////string message = response.Message;

            //_eventBus.Publish("AddShopCar", addShopCarRequest);

            string url = $"{configuration["ServiceAddress:Service.Monitor"]}{configuration["MehtodName:Monitor.QueryRabbitMq.QueryRoutingKeyApiUrlAsync"]}";
            var queryRoutingKeyApiUrlResponse = apiHelperService.PostAsync<QueryRoutingKeyApiUrlResponse>(url, new QueryRoutingKeyApiUrlRequest { RoutingKey = "AddShopCar" });
            if (queryRoutingKeyApiUrlResponse.Result != null && queryRoutingKeyApiUrlResponse.Result.ApiUrlList.Any())
            {

            }

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
