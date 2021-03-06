﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MI.Service.Picture.Entity;
using MI.Untity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MI.Service.Picture
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<MIContext>(options => options.UseSqlServer(Configuration["ConnectionStrings"]));

            /*ervices.AddMvcCore().AddAuthorization().AddJsonFormatters();*/
            services.AddAuthentication(Configuration["Identity:Scheme"])
            .AddIdentityServerAuthentication(options =>
            {
                options.RequireHttpsMetadata = false; // for dev env
                options.Authority = $"http://{Configuration["Identity:IP"]}:{Configuration["Identity:Port"]}";
                options.ApiName = Configuration["Service:Name"]; // match with configuration in IdentityServer
            });

            services.AddMvc();
        }

     
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, ILoggerFactory loggerFactory)
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
                    template: "Picture/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", actioon = "Index" });
            });
        }
    }
}
