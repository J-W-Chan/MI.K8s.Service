using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(string routingKey, object Model);
        void Subscribe(string QueueName, string RoutingKey);
    }
}
