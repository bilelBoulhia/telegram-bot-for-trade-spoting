using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using TradingTelegramService.Interfaces;
using TradingTelegramService.models;
using TradingBot;

namespace TradingTelegramService.Services
{
    public class SpotingService : ISpoting
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly HttpClient _httpClient;
        private readonly Helpers _helper;
        public SpotingService(IConfiguration configuration, Helpers helper)
        {
            _configuration = configuration;
            _helper = helper;
            _apiKey = configuration.GetValue<string>("Apis:apikey");
            _apiSecret = configuration.GetValue<string>("Apis:apiSecret");
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
        }

        public async Task<SpotModel> GetSpotings()
        {
            try
            {
                string symbol = "ARBUSDT";
                string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

                string queryString = $"symbol={symbol}";

        
                string signature = _helper.GenerateSignature(queryString, _apiSecret);

           
                string url = $"https://api.binance.com/api/v3/ticker/price?{queryString}";

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                
                
                string json = await response.Content.ReadAsStringAsync();
                var priceData = JsonSerializer.Deserialize<BinanceResponse>(json);


                return new SpotModel()
                {
                    Symbol = symbol,
                    entryPrice = Convert.ToDecimal(priceData.Price),
                    target1 = Convert.ToDecimal(priceData.Price) * 1.01m,
                    target2 = Convert.ToDecimal(priceData.Price) * 1.02m,
                    stopLoss = Convert.ToDecimal(priceData.Price) * 0.98m
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}", ex);
            }
        }

        
    }
}