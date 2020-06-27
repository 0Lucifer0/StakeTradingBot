using System.Threading.Tasks;
using StakeTradingBot.StakeClient.Model;

namespace StakeTradingBot
{
    public interface ITradingClient
    {
        public Task LoginAsync();

        public Task<MarketStatusType> GetMarketStatus();

        public Task<Instrument?> GetInstrumentFromSymbol(string symbol);

        public Task Sell(Order order);

        public Task Buy(Order order);

        public Task<float> GetCashAvailable();
    }
}