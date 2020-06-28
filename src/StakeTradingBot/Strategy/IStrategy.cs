using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StakeTradingBot.Strategy
{
    public interface IStrategy
    {
        public Task Execute(CancellationToken stoppingToken);
    }
}
