using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyHealth.Common;
using MyHealth.DBSink.Activity;
using MyHealth.DBSink.Activity.Functions;
using MyHealth.DBSink.Activity.Repository;
using MyHealth.DBSink.Activity.Repository.Interfaces;
using MyHealth.DBSink.Activity.Services;
using MyHealth.DBSink.Activity.Services.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MyHealth.DBSink.Activity
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri(Environment.GetEnvironmentVariable("myhealthappconfigendpoint")), new DefaultAzureCredential())
                    .Select("*")
                    .ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(new DefaultAzureCredential());
                    });
                })
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddAzureAppConfiguration();
            builder.Services.AddLogging();

            builder.Services.AddSingleton(sp =>
            {
                CosmosClientOptions cosmosClientOptions = new CosmosClientOptions
                {
                    MaxRetryAttemptsOnRateLimitedRequests = 3,
                    MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(60)
                };
                IConfiguration config = sp.GetRequiredService<IConfiguration>();
                return new CosmosClient(config["MyHealth:CosmosEndpoint"], new DefaultAzureCredential(), cosmosClientOptions);
            });

            builder.Services.AddSingleton<IServiceBusHelpers>(sp =>
            {
                IConfiguration config = sp.GetRequiredService<IConfiguration>();
                return new ServiceBusHelpers(config["VelidaEngine:ServiceBusConnectionString"]);
            });

            builder.Services.AddSingleton<IActivityRepository, ActivityRepository>();
            builder.Services.AddSingleton<IActivityService, ActivityService>();
        }       
    }
}
