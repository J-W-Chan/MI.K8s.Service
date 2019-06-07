using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MQStationServer.Model.Request
{
    public class ConsumerProcessEventRequest
    {
        /// <summary>
        /// MQ RoutingKey
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// 消息体
        /// </summary>
        public string MQBodyMessage { get; set; }

    }
}
