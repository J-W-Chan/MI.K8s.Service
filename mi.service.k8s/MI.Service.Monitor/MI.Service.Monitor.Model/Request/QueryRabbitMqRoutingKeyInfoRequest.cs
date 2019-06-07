using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.Monitor.Model.Request
{
    public class QueryRabbitMqRoutingKeyInfoRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
