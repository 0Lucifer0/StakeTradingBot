using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StakeTradingBot.AlpacaPaperTradingClient.Model;
using StakeTradingBot.StakeClient.Model;
using YamlDotNet.Core.Tokens;

namespace StakeTradingBot.AlpacaPaperTradingClient
{
    public class AlpacaPaperTradingClient : ITradingClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<StakeClient.StakeClient> _logger;
        private readonly Uri _uri = new Uri("https://paper-api.alpaca.markets");
        private readonly Configuration.AlpacaPaperTradingClientConfiguration _configuration;
        private AccountModel? _authModel;
        public AlpacaPaperTradingClient(IHttpClientFactory clientFactory, ILogger<StakeClient.StakeClient> logger, Configuration.AlpacaPaperTradingClientConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task LoginAsync()
        {
            _logger.LogInformation("Connecting to Alpaca...");
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = _uri;
            httpClient.DefaultRequestHeaders.Add("APCA-API-KEY-ID", _configuration.ApiKey);
            httpClient.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", _configuration.ApiSecret);
            var result = await httpClient.GetAsync("v2/account");
            if (result.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                _authModel = JsonSerializer.Deserialize<AccountModel>(await result.Content.ReadAsStringAsync(), options);
                _logger.LogInformation("Connected to Alpaca!");
                return;
            }

            _authModel = null;
            _logger.LogError("Connection to Alpaca failed", result.ReasonPhrase);
            throw new ArgumentException();
        }

        public async Task<Status> GetMarketStatus()
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = _uri;
            httpClient.DefaultRequestHeaders.Add("APCA-API-KEY-ID", _configuration.ApiKey);
            httpClient.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", _configuration.ApiSecret);

            var result = await httpClient.GetAsync("v2/clock");
            if (result.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var status = JsonSerializer.Deserialize<Clock>(await result.Content.ReadAsStringAsync(), options);
                var marketStatus = new Status
                {
                    Current = status.IsOpen ? MarketStatus.Open : MarketStatus.Close,
                    Next = status.IsOpen ? MarketStatus.Close : MarketStatus.Open,
                    ChangeAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(status.Timestamp).ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")).ToString("HH:mm:ss")
                };

                return marketStatus;
            }
            _logger.LogError("Retrieving market status failed", result.ReasonPhrase);
            throw new ArgumentException();
        }

        public async Task<Instrument?> GetInstrumentFromSymbol(string symbol)
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = _uri;
            httpClient.DefaultRequestHeaders.Add("APCA-API-KEY-ID", _configuration.ApiKey);
            httpClient.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", _configuration.ApiSecret);

            var result = await httpClient.GetAsync($"/v2/assets/{symbol}");
            if (result.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var asset = JsonSerializer.Deserialize<Asset>(await result.Content.ReadAsStringAsync(), options);
                if (asset == null)
                {
                    _logger.LogInformation($"Instrument not found for symbol {symbol}", result.ReasonPhrase);
                    return null;
                }
                return new Instrument
                {
                    InstrumentId = asset.Id,
                    Symbol = asset.Symbol,
                    Name = asset.Symbol,
                };
            }
            _logger.LogError($"Retrieving suggestions failed for symbol {symbol}", result.ReasonPhrase);
            throw new ArgumentException();
        }

        private async Task SendTransaction(TransactionType transactionType, Order order)
        {
            var instrument = await GetInstrumentFromSymbol(order.Symbol);
            if (instrument == null)
            {
                return;
            }

            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = _uri;
            httpClient.DefaultRequestHeaders.Add("APCA-API-KEY-ID", _configuration.ApiKey);
            httpClient.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", _configuration.ApiSecret);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            };
            var content = new StringContent(JsonSerializer.Serialize(new AlpacaOrder
            {
                Quantity = order.Quantity.ToString(CultureInfo.InvariantCulture),
                Side = transactionType.ToString().ToLowerInvariant(),
                Symbol = instrument.InstrumentId.ToString(),
                TimeInForce = "day",
                Type = OrderType.Market.ToString().ToLowerInvariant()
            }, options), Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync($"v2/orders", content);
            if (result.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{transactionType} order submitted!");
                return;
            }

            _logger.LogError($"Error while {transactionType}ing order failed", result.ReasonPhrase);
            throw new ArgumentException();
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
            await LoginAsync();
            if (_authModel == null)
            {
                return 0;
            }

            return float.Parse(_authModel.Cash);
        }
    }
}