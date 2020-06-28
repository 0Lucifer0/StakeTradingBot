using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fynance;
using Microsoft.Extensions.Logging;
using StakeTradingBot.StakeClient.Model;

namespace StakeTradingBot.Strategy
{
    public class MeanReversionStrategy : IStrategy
    {
        private readonly ILogger<MeanReversionStrategy> _logger;
        private readonly ITradingClient _tradingClient;
        private StakeTradingBotContext _dbContext;

        public MeanReversionStrategy(ILogger<MeanReversionStrategy> logger, ITradingClient tradingClient, StakeTradingBotContext dbContext)
        {
            _logger = logger;
            _tradingClient = tradingClient;
            _dbContext = dbContext;
        }

        public async Task Execute(CancellationToken stoppingToken)
        {
            var availableCash = await _tradingClient.GetCashAvailable();
            _logger.LogInformation("Cash Available : {0} USD", availableCash);

            var marketStatus = await _tradingClient.GetMarketStatus();
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            var next = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            next = next.Add(TimeSpan.Parse(marketStatus.ChangeAt));
            var nextOrders = new List<Order>();

            var list = new List<string> { "MSFT" };

            foreach (var symbolName in list)
            {
                var symbol = await _tradingClient.GetInstrumentFromSymbol(symbolName);
                if (next.AddMinutes(-15) > now)
                {
                    var result = await Ticker.Build()
                        .SetSymbol(symbolName)
                        .SetPeriod(Period.OneMonth)
                        .SetInterval(Interval.OneDay)
                        .GetAsync();

                    var avg = result.Quotes.Average(item => item.Close);
                    var currentPrice = result.Quotes.OrderBy(s => s.Period).Last().Close;
                    var diff = avg - currentPrice;

                    if (diff <= 0)
                    {
                        nextOrders = GetOpenPositions(symbol.InstrumentId.ToString());
                    }
                    else if (diff / currentPrice > 0.05)
                    {
                        var amountToAdd = diff / currentPrice * 200;

                        if (amountToAdd > availableCash)
                        {
                            amountToAdd = availableCash;
                        }

                        var qtyToBuy = amountToAdd / currentPrice;

                        await _tradingClient.Buy(new Order
                        {
                            Quantity = (float)Math.Ceiling(qtyToBuy),
                            TransactionType = TransactionType.Buy,
                            Time = DateTime.Now,
                            Symbol = symbol.InstrumentId.ToString()
                        });
                    }
                }
                else
                {
                    nextOrders = GetOpenPositions(symbol.InstrumentId.ToString());
                }
                foreach (var nextOrder in nextOrders)
                {
                    if (nextOrder.TransactionType == TransactionType.Buy)
                    {
                        await _tradingClient.Buy(nextOrder);
                    }
                    else
                    {
                        await _tradingClient.Sell(nextOrder);
                    }

                    nextOrder.Time = DateTime.Now;
                }

                await _dbContext.Orders.AddRangeAsync(nextOrders, stoppingToken);
                await _dbContext.SaveChangesAsync(stoppingToken);
            }

        }

        private List<Order> GetOpenPositions(string symbol)
        {
            var nextOrders = new List<Order>();

            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(),
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            var groupedOrder = _dbContext.Orders.Where(s => s.Time > today.Date && s.TransactionType == TransactionType.Buy).ToList().GroupBy(s => s.Symbol);
            foreach (var ord in groupedOrder)
            {
                nextOrders.Add(new Order
                {
                    Quantity = ord.Where(s => s.TransactionType == TransactionType.Buy).Sum(s => s.Quantity) - ord.Where(s => s.TransactionType == TransactionType.Sell).Sum(s => s.Quantity),
                    Symbol = ord.Key,
                    TransactionType = TransactionType.Sell
                });
            }
            return nextOrders;
        }
    }
}
