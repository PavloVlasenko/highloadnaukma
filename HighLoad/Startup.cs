using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using HighLoad.Entities;
using HighLoad.EventHandlers;
using HighLoad.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace HighLoad
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
            //var multiplexer = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("Redis"));
            //services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddSingleton<IMemoryService, MemoryService>();
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "HighLoad", Version = "v1"}); });
            services.AddHostedService<BookDbEventHandler>();
            services.AddHostedService<BookRedisEventHandler>();

            services.AddSingleton<DbSender>();
            services.AddSingleton<RedisSender>();

            services.AddDbContext<DbContext>(options => options.UseSqlServer(Configuration["Database"]));
            services.AddSingleton<ServiceBusClient>(_ => new (Configuration["ServiceBus"]));
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HighLoad v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        }
    }
}