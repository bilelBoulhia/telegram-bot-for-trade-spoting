
using System.Net.Http.Headers;
using System.Text.Json;
using TradingTelegramService.Interfaces;
using TradingTelegramService.models;
using TradingBot.Helpers;



namespace TradingTelegramService.Services
{
    public class SpotingRepo : ISpoting
    {

        private readonly string _apiKey;

        
        private readonly HttpClient _httpClient;


        public SpotingRepo(IConfiguration configuration)
        {
            _apiKey = configuration.GetValue<string>("Apis:apikey");
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<List<List<object>>> FetchCandleStickData(string symbol)
        {
            try
            {
                string url = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval=5m";

                HttpResponseMessage res = await _httpClient.GetAsync(url);
                string json = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<List<object>>>(json);
            }
            catch (Exception ex)
            {

                return null;
            }
         
            
        }
        
        public async Task<SpotModel> FetchSpotings(string symbol)
        {
            try
            {

                string url = $"https://api.binance.com/api/v3/ticker/price?symbol={symbol}";

                
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                
                
                string json = await response.Content.ReadAsStringAsync();
                var priceData = JsonSerializer.Deserialize<BinanceResponse>(json);
                var tunicatedentryPrice = Convert.ToDecimal(priceData.Price);
                return new SpotModel()
                {
                    Symbol = symbol,
                    entryPrice = tunicatedentryPrice,
                    target1 = tunicatedentryPrice * 1.01m,
                    target2 = tunicatedentryPrice * 1.02m,
                    stopLoss = tunicatedentryPrice * 0.98m,
                    timeStamp = DateTime.UtcNow
               
                };

              
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}", ex);
            }
        }

        
        
    }
}

