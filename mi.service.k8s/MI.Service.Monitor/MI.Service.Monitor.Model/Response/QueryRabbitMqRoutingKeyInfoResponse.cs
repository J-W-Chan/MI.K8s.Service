using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.Monitor.Model.Response
{
    public class QueryRabbitMqRoutingKeyInfoResponse: BaseResponse
    {
        public QueryRabbitMqRoutingKeyInfoResponse()
        {
            RabbitMQRegisterInfos = new List<RabbitMQRegisterInfoEntity>();
        }
        public List<RabbitMQRegisterInfoEntity> RabbitMQRegisterInfos { get; set; }
    }

    public class RabbitMQRegisterInfoEntity
    {
        public string RoutingKey { get; set; }

        public List<string> ApiUrlList { get; set; }
    }
}
