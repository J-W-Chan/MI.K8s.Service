using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.Monitor.Model.Request
{
    public class RoutingKeyRegisterRequest
    {
        public string RoutingKey { get; set; }
        public string QueueName { get; set; }
        public string ApiUrl { get; set; }
    }
}
