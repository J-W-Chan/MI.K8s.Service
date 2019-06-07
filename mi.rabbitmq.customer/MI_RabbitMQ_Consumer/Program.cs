using log4net;
using log4net.Config;
using log4net.Repository;
using MI.APIClientService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MI_RabbitMQ_Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            //交换器（Exchange）
            const string BROKER_NAME = "mi_event_bus";
            //队列（Queue）
            var SubscriptionClientName = "RabbitMQ_Bus_MI";
            //log4net日志加载
            ILoggerRepository repository = LogManager.CreateRepository("MI.WinService.MQConsumer");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            ILog log = LogManager.GetLogger(repository.Name, "MI.WinService.MQConsumer");
            //依赖注入加载
            IServiceCollection serviceCollection = new ServiceCollection();
            //WebApi调用类
            serviceCollection.AddTransient<IApiHelperService, ApiHelperService>();
            var serviceProvider = serviceCollection.AddHttpClient().BuildServiceProvider();
            serviceProvider.GetService<ILogger>();
            var apiHelperService = serviceProvider.GetService<IApiHelperService>();
            //MQ消费类（发送MQ消息调用接口、绑定队列交换器）
            MQConsumerService consumerService = new MQConsumerService(apiHelperService,log);

            //MQ连接类
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "47.99.92.76",
                Port=8010
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            channel.QueueDeclare(queue: SubscriptionClientName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                //发送到MQ消费服务端
                var message = Encoding.UTF8.GetString(ea.Body);
                log.Info($"MQ准备消费消息 RoutingKey:{ea.RoutingKey} Message:{message}");

                //发送到MQ消费服务端MQStationServer
                Task result= Task.Run(() =>
                {
                    consumerService.ProcessEvent(ea.RoutingKey, message);
                });
                if(!result.IsFaulted)
                {
                    //确认ack
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            channel.BasicConsume(SubscriptionClientName, false, consumer);
            Console.WriteLine("消费者已启动！");

            //绑定队列RoutingKey
            Task taskResult= Task.Run(async() =>
            {
                await consumerService.MQSubscribeAsync();
            });

            taskResult.Wait();


            Console.WriteLine("队列RoutingKey绑定完成！");

            Console.ReadKey();
            channel.Dispose();
            connection.Close();
        }
    }
}
