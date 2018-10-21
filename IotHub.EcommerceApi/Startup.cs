using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IotHub.Core.CqrsEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag.AspNetCore;
using System.Reflection;
using IotHub.OAuth;
using IotHub.Core.Redis;
using IotHub.Core;
using IotHub.Core.Authorize;
using IotHub.DbMigration;
using Microsoft.EntityFrameworkCore;

namespace IotHub.EcommerceApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigurationManagerExtensions.SetConfiguration(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc(option=> {
               // option.Filters.Add(new IotHubAuthorizeAttribute());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //RedisServices.Init("172.16.10.166", null, string.Empty);
            var redishost = ConfigurationManagerExtensions.GetConnectionString("RedisConnectionString");
            RedisServices.Init(redishost, null, string.Empty);
            CommandsAndEventsRegisterEngine.AutoRegister();
            
            EngineeCommandWorkerQueue.Start();
            EngineeEventWorkerQueue.Start();

            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSwaggerUi3WithApiExplorer(settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling =
                    PropertyNameHandling.CamelCase;
            });

            app.UseMvc();
        }
    }
}
