using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StakeTradingBot.StakeClient;
using StakeTradingBot.StakeClient.Model;

namespace StakeTradingBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITradingClient _tradingClient;

        public Worker(ILogger<Worker> logger, ITradingClient tradingClient)
        {
            _logger = logger;
            _tradingClient = tradingClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var marketStatus = await _tradingClient.GetMarketStatus();
                    var availableCash = await _tradingClient.GetCashAvailable();
                    _logger.LogInformation("Cash Available : {0} USD", );

                    if (marketStatus.Response.Status.Current == MarketStatus.Close)
                    {
                        _logger.LogInformation("Waiting next opening", marketStatus.Response.Status.ChangeAt);
                        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                        var next = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                        next = next.Add(TimeSpan.Parse(marketStatus.Response.Status.ChangeAt));
                        if (next < now)
                        {
                            if (next.DayOfWeek == DayOfWeek.Friday || next.DayOfWeek == DayOfWeek.Saturday)
                            {
                                next = next.AddDays(8 - (byte)next.DayOfWeek);
                            }
                            else
                            {
                                next = next.AddDays(1);
                            }
                        }
                        _logger.LogWarning("Market close - waiting {0} seconds", (int)(next - now).TotalSeconds);
                        await Task.Delay((int)(next - now).TotalMilliseconds, stoppingToken);
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