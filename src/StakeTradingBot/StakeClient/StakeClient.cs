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
    public class StakeClient : ITradingClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<StakeClient> _logger;
        private readonly Uri _uri = new Uri("https://prd-api.stake.com.au/api/");
        private DateTime? _lastConnectionDate;
        private readonly Configuration.StakeConfiguration _configuration;
        private AuthModel? _authModel;
        public StakeClient(IHttpClientFactory clientFactory, ILogger<StakeClient> logger, Configuration.StakeConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task LoginAsync()
        {
            if (_lastConnectionDate?.Date.AddDays(30) < DateTime.Now)
            {
                _logger.LogTrace("Using Stake cached session");
                return;
            }
            _logger.LogInformation("Connecting to Stake...");
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = _uri;
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                _configuration.Username,
                _configuration.Password,
                rememberMeDays = "30"
            }, options), Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync("sessions/createSession", content);
            if (result.IsSuccessStatusCode)
            {
                _logger.LogInformation("Connected to Stake!");
                _lastConnectionDate = DateTime.Now;
                _authModel = JsonSerializer.Deserialize<AuthModel>(await result.Content.ReadAsStringAsync());
                return;
            }

            _logger.LogError("Connection to Stake failed", result.ReasonPhrase);
            throw new ArgumentException();
        }

        public async Task<Status> GetMarketStatus()
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = _uri;
            var result = await httpClient.GetAsync("utils/marketStatus");
            if (result.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                var marketStatus = JsonSerializer.Deserialize<MarketStatusType>(await result.Content.ReadAsStringAsync(), options);
                _logger.LogInformation(marketStatus.Response.Message, result.ReasonPhrase);
                return marketStatus.Response.Status;
            }
            _logger.LogError("Retrieving market status failed", result.ReasonPhrase);
            throw new ArgumentException();
        }

        public async Task<Instrument?> GetInstrumentFromSymbol(string symbol)
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = _uri;
            var result = await httpClient.GetAsync($"products/getProductSuggestions/{symbol}");
            if (result.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var suggestion = JsonSerializer.Deserialize<ProductSuggestion>(await result.Content.ReadAsStringAsync(), options);
                var instrument = suggestion.Instruments.FirstOrDefault(s =>
                    s.Symbol.Equals(symbol, StringComparison.CurrentCultureIgnoreCase));
                if (instrument == null)
                {
                    _logger.LogInformation($"Instrument not found for symbol {symbol}", result.ReasonPhrase);
                }
                return instrument;
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
            await LoginAsync();
            if (_authModel == null)
            {
                return;
            }
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = _uri;
            httpClient.DefaultRequestHeaders.Add("Stake-Session-Token", _authModel.SessionKey);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var content = new StringContent(JsonSerializer.Serialize(new StakeOrder
            {
                Comments = string.Empty,
                ItemId = instrument.InstrumentId.ToString(),
                ItemType = "instrument",
                LimitPrice = string.Empty,
                OrderType = OrderType.Market,
                Quantity = order.Quantity.ToString(CultureInfo.InvariantCulture),
                StopPrice = string.Empty,
                UserId = _authModel.UserId.ToString()
            }, options), Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync($"{transactionType.ToString().ToLowerInvariant()}orders", content);
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
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = _uri;
            httpClient.DefaultRequestHeaders.Add("Stake-Session-Token", _authModel.SessionKey);
            var result = await httpClient.GetAsync($"users/accounts/cashAvailableForWithdrawal");
            if (result.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var cashAvailable = JsonSerializer.Deserialize<CashAvailable>(await result.Content.ReadAsStringAsync(), options);
                return cashAvailable.CashAvailableForTrade;
            }

            _logger.LogError($"Error while retrieving available money failed", result.ReasonPhrase);
            return 0;
        }
    }
}