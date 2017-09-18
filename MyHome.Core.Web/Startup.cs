using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Hangfire;
using Hangfire.MemoryStorage;

using MyHome.Shared;
using MyHome.Core.Plugin;
using MyHome.Core.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using MyHome.Core.Hangfire;
using Newtonsoft.Json.Serialization;

namespace MyHome.Core
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IContainer ApplicationContainer { get; private set; }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddCors(x =>
            {
                x.AddPolicy("CorsPolicy",
                    options => options
                        .WithOrigins("*")
                        .WithMethods("*")
                        .WithHeaders("*")
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .Build()
                    );
            });

            services.AddHangfire(x => x.UseMemoryStorage());

            services.AddApplicationInsightsTelemetry();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss";
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            });

            services.AddSignalR(cfg =>
            {
                cfg.JsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.Configure<IISOptions>(options => {
            });

            services.AddSingleton(x => Configuration);

            services.AddSingleton(_ => new JsonSerializer {
                ContractResolver = new SignalRContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });

            services
                .AddDbContext<DB>(options =>
                {
                    options.UseSqlite(Configuration.GetSection("Data").GetSection("DefaultConnection").GetValue<string>("ConnectionString"));
                });

            var builder = new ContainerBuilder();

            builder.RegisterType<PluginManager>();
            builder.RegisterType<SignalRProxy>().As<ISignalRProxy>();
            builder.Populate(services);
            ApplicationContainer = builder.Build();

            using (var client = ApplicationContainer.Resolve<DB>())
            {
                //client.Database.Migrate();
            }

            var pm = ApplicationContainer.Resolve<PluginManager>();
            var plugins = pm.Load();

            builder = new ContainerBuilder();

            builder.RegisterType<PluginManager>().InstancePerLifetimeScope();
            builder.RegisterType<SignalRProxy>().As<ISignalRProxy>();
            builder.RegisterType<ScheduleJob>();
            builder.Populate(services);
            builder.RegisterTypes(plugins.ToArray()).As<IPlugin>();

            foreach (var item in plugins)
            {
                builder.RegisterType(item);
            }

            ApplicationContainer = builder.Build();

            return ApplicationContainer.Resolve<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }

            app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ManageHub>("manageHub");
            });

            app.UseMvc();

            foreach (var plugin in ApplicationContainer.Resolve<IEnumerable<IPlugin>>())
            {
                plugin.Init();
                plugin.Process();
            }

            RecurringJob.AddOrUpdate(() => ApplicationContainer.Resolve<ScheduleJob>().Process(), Cron.Minutely());
        }
    }
}
