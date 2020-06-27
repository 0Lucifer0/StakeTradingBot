using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StakeTradingBot.StakeClient.Model;

namespace StakeTradingBot.StakeClient
{
    public class AlpacaPaperTradingClient : ITradingClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<StakeClient> _logger;
        private readonly Uri _uri = new Uri("https://paper-api.alpaca.markets");
        private readonly Configuration.AlpacaPaperTradingClientConfiguration _configuration;
        public AlpacaPaperTradingClient(IHttpClientFactory clientFactory, ILogger<StakeClient> logger, Configuration.AlpacaPaperTradingClientConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task LoginAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Status> GetMarketStatus()
        {
            throw new NotImplementedException();
        }

        public async Task<Instrument?> GetInstrumentFromSymbol(string symbol)
        {
            throw new NotImplementedException();
        }

        private async Task SendTransaction(TransactionType transactionType, Order order)
        {
            throw new NotImplementedException();
        }

        public Task Sell(Order order)
        {
            return SendTransaction(TransactionType.Sell, order);
        }

        public Task Buy(Order order)
        {
            return SendTransaction(TransactionType.Buy, order);
        }

        public async Task<float> GetCashAvailable()
        {
            throw new NotImplementedException();
        }
    }
}