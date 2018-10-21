using EventualityPOC.PersonProfileContext.PersonAggregate.Application;
using EventualityPOC.PersonProfileContext.PersonAggregate.Framework;
using EventualityPOCApi.Gateway.BridgeHttp.Channel;
using EventualityPOCApi.Gateway.BridgeHttp.TransportAdapter;
using EventualityPOCApi.Gateway.Component.PersonProfileContext.PersonAggregate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace EventualityPOCApi.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();

            services.AddSingleton<IDecisionChannel, DecisionChannelRx>();
            services.AddSingleton<IPerceptionChannel, PerceptionChannelRx>();

            services.AddSingleton<HubPublisherWebsocket>();

            // Individual handlers and their dependencies, should be seemlessly replaced by azure functions in the cloud
            services.AddSingleton<DocumentClient>(s => new DocumentClient(
                new Uri(Configuration["CosmosDB:AccountEndpoint"]?.ToString()),
                Configuration["CosmosDB:AccountKey"]?.ToString()));
            services.AddSingleton<PersonComponent>();
            services.AddSingleton<IPersonRepository, PersonRepositoryCosmosDb>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            loggerFactory.AddDebug();

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:4200").AllowAnyHeader().WithMethods("GET", "POST").AllowCredentials();
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<HubSubscriberWebsocket>("/eventHub");
            });

            app.UseMvc();

            // Bind outgoing signalR handler
            serviceProvider.GetService<HubPublisherWebsocket>().RegisterOutgoingHandler();

            // Bind individual services
            serviceProvider.GetService<PersonComponent>().Configure();

            // Initialize repositories - TODO think about a better place for this
            serviceProvider.GetService<IPersonRepository>().InitializeAsync();
        }
    }
}
