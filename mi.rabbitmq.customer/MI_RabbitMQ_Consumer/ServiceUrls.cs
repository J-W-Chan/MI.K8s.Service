using System;
using System.Collections.Generic;
using System.Text;

namespace MI_RabbitMQ_Consumer
{
    public class ServiceUrls
    {
        /// <summary>
        /// 消费MQ
        /// </summary>
        public const string ConsumerProcessEvent = "http://mi.mqstationserver/MQStationServer/MQConsumerOperation/ConsumerProcessEventAsync";

        /// <summary>
        /// 绑定RoutingKey队列
        /// </summary>
        public const string MQSubscribe = "http://mi.service.monitor/Monitor/RabbitMQManager/MQSubscribeAsync";
    }
}
