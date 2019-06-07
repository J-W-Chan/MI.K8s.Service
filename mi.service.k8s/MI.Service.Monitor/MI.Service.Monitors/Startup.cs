using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using MI.APIClientService;
using MI.Service.Monitor.Entity;
using MI.Untity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Polly;
using Polly.Extensions.Http;

namespace MI.Service.Monitors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<MIContext>(options => options.UseSqlServer(Configuration["ConnectionStrings"]));

            ///*ervices.AddMvcCore().AddAuthorization().AddJsonFormatters();*/
            //services.AddAuthentication(Configuration["Identity:Scheme"])
            //.AddIdentityServerAuthentication(options =>
            //{
            //    options.RequireHttpsMetadata = false; // for dev env
            //    options.Authority = $"http://{Configuration["Identity:IP"]}:{Configuration["Identity:Port"]}";
            //    options.ApiName = Configuration["Service:Name"]; // match with configuration in IdentityServer
            //});

            
            services.AddCustomMvc(Configuration).AddHttpServices();
            RegisterEventBus(services);

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IApplicationLifetime lifetime,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseCors("CorsPolicy");
            app.UseAuthentication();

            //app.UserZipkinCore(lifetime, loggerFactory);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "Monitor/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", actioon = "Index" });
            });
        }

        /// <summary>
        /// 消息总线RabbitMQ
        /// </summary>
        private void RegisterEventBus(IServiceCollection services)
        {
            #region 加载RabbitMQ账户
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"],
                    Port=int.Parse(Configuration["EventBusConnectionPort"])
                };

                if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
            #endregion

            var subscriptionClientName = Configuration["SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ.EventBusRabbitMQ>>();
                var apiHelper = sp.GetRequiredService<IApiHelperService>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ.EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, apiHelper, subscriptionClientName, retryCount);
            });
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IApiHelperService, ApiHelperService>();
            services.AddMvc();
            return services;
        }

        public static IServiceCollection AddHttpServices(this IServiceCollection services)
        {
            services.AddHttpClient("MI.Web")
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuiBreakerPolicy());

            return services;
        }

        /// <summary>
        /// 重试策略
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        /// <summary>
        /// 熔断策略
        /// </summary>
        private static IAsyncPolicy<HttpResponseMessage> GetCircuiBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }


}
