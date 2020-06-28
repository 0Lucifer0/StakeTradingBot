using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StakeTradingBot.StakeClient;
using StakeTradingBot.StakeClient.Model;
using StakeTradingBot.Strategy;

namespace StakeTradingBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITradingClient _tradingClient;
        private readonly IEnumerable<IStrategy> _strategies;

        public Worker(ILogger<Worker> logger, ITradingClient tradingClient, IEnumerable<IStrategy> strategies, StakeTradingBotContext dbContext)
        {
            _logger = logger;
            _tradingClient = tradingClient;
            _strategies = strategies;
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var marketStatus = await _tradingClient.GetMarketStatus();
                    var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                    var next = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

                    if (marketStatus.Current != MarketStatus.Open)
                    {
                        _logger.LogInformation("Waiting next opening", marketStatus.ChangeAt);
                        next = next.Add(TimeSpan.Parse(marketStatus.ChangeAt));
                        if (next < now)
                        {
                            next = next.DayOfWeek == DayOfWeek.Friday || next.DayOfWeek == DayOfWeek.Saturday
                                ? next.AddDays(8 - (byte) next.DayOfWeek)
                                : next.AddDays(1);
                        }
                        _logger.LogWarning("Market close - waiting {0} seconds", (int)(next - now).TotalSeconds);
                        await Task.Delay((int)(next - now).TotalMilliseconds, stoppingToken);
                    }

                    foreach (var strategy in _strategies)
                    {
                        await strategy.Execute(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }

                await Task.Delay((int)TimeSpan.FromMinutes(1).TotalMilliseconds, stoppingToken);
            }
        }
    }
}