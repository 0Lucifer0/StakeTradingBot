using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using StakeTradingBot.Configuration;
using StakeTradingBot.StakeClient;
using StakeTradingBot.Strategy;

namespace StakeTradingBot
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var conf = new ConfigurationBuilder().AddYamlFile("config.yml").Build();
            Validator.ValidateObject(conf, new ValidationContext(conf), true);
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "{Level:u4} {Timestamp:HH:mm:ss} -- {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .CreateLogger();
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureLogging(
                    loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddSerilog(dispose: true);
                    }
                )
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    switch (conf.GetValue<string>("Provider"))
                    {
                        case "Stake":
                            var stakeConfiguration = new StakeConfiguration();
                            conf.GetSection("StakeClient").Bind(stakeConfiguration);
                            services.AddSingleton(stakeConfiguration);
                            services.AddTransient<ITradingClient, StakeClient.StakeClient>();
                            break;

                        case "AlpacaPaperTrading":
                        default:
                            var alpacaPaperConfiguration = new AlpacaPaperTradingClientConfiguration();
                            conf.GetSection("AlpacaPaperTradingClient").Bind(alpacaPaperConfiguration);
                            services.AddSingleton(alpacaPaperConfiguration);
                            services.AddTransient<ITradingClient, AlpacaPaperTradingClient.AlpacaPaperTradingClient>();
                            break;
                    }
                    services.AddDbContext<StakeTradingBotContext>(options => options.UseSqlite("Data Source=./StakeTradingBot.db"));
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => typeof(IStrategy).IsAssignableFrom(p)).ToList().ForEach(t =>
                            services.RegisterAsImplementedInterfaces(t, ServiceLifetime.Transient));
                    
                    services.AddHostedService<Worker>();
                });
        }
    }
}
