
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MI.Web.Controllers
{
    public class RabbitMQRegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MQRegister()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MQRegister(string RoutingKey,string InterfaceUrl)
        {
            return View();
        }
    }
}