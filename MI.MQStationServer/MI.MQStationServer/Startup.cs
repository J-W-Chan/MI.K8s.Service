using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MI.APIClientService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace MI.MQStationServer
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
//            services.AddDbContextPool<MIContext>(options =>
//options.UseSqlServer(Configuration.GetConnectionString("SqlConnection")));

            /*ervices.AddMvcCore().AddAuthorization().AddJsonFormatters();*/
            ////IdentityServer4
            //services.AddAuthentication(Configuration["Identity:Scheme"])
            //.AddIdentityServerAuthentication(options =>
            //{
            //    options.RequireHttpsMetadata = false; // for dev env
            //    options.Authority = $"http://{Configuration["Identity:IP"]}:{Configuration["Identity:Port"]}";
            //    options.ApiName = Configuration["Service:Name"]; // match with configuration in IdentityServer
            //});

            services.AddCustomMvc(Configuration).AddHttpServices();

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "MQStationServer/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", actioon = "Index" });
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
