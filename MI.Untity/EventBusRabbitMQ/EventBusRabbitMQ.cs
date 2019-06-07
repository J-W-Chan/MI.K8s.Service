using Autofac;
using EventBus.Abstractions;
using EventBus.Events;
using MI.APIClientService;
using MI.Service.Monitor.Model.Request;
using MI.Service.Monitor.Model.Response;
using MI.Untity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "mi_event_bus";
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly ILifetimeScope _autofac;
        private readonly IApiHelperService _apiHelperService;
        private readonly string AUTOFAC_SCOPE_NAME = "mi_event_bus";
        private readonly int _retryCount;

        private IModel _consumerChannel;
        private string _queueName;


        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            ILifetimeScope autofac, IApiHelperService apiHelperService, string queueName = null, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
            _autofac = autofac;
            _retryCount = retryCount;
            _apiHelperService = apiHelperService;
        }


        /// <summary>
        /// 发布消息
        /// </summary>
        public void Publish(string routingKey, object Model)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.ToString());
                });

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");
                var message = JsonConvert.SerializeObject(Model);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; //持久化

                    channel.BasicPublish(exchange: BROKER_NAME, routingKey: routingKey, mandatory: true, basicProperties: properties, body: body);
                });
            }
        }

        public void Subscribe(string QueueName, string RoutingKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueBind(queue: QueueName, exchange: BROKER_NAME, routingKey: RoutingKey);
            }
        }

        public void UnSubscribe(string QueueName, string RoutingKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: RoutingKey);
            }
        }


        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(ea.RoutingKey, message);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        /// <summary>
        /// 发送MQ到指定服务接口
        /// </summary>
        private async Task ProcessEvent(string routingKey, string message)
        {
            using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
            {
                //获取绑定该routingKey的服务地址集合
                var subscriptions = await StackRedis.Current.GetAllList(routingKey);
                if (!subscriptions.Any())
                {
                    //如果Redis中不存在 则从数据库中查询 加入Redis中
                    var response = _apiHelperService.PostAsync<QueryRoutingKeyApiUrlResponse>(ServiceAddress.QueryRoutingKeyApiUrlAsync, new QueryRoutingKeyApiUrlRequest { RoutingKey = routingKey });
                    if (response.Result != null && response.Result.ApiUrlList.Any())
                    {
                        subscriptions = response.Result.ApiUrlList;
                        Task.Run(() =>
                        {
                            StackRedis.Current.SetLists(routingKey, response.Result.ApiUrlList);
                        });

                    }
                }
                foreach (var apiUrl in subscriptions)
                {
                    Task.Run(() =>
                    {
                        _logger.LogInformation(message);
                    });

                    await _apiHelperService.PostAsync(apiUrl, message);
                }
            }
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
        }
    }
}
