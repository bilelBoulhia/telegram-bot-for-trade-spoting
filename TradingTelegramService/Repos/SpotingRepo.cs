
using System.Net.Http.Headers;
using System.Text.Json;
using TradingTelegramService.Interfaces;
using TradingTelegramService.models;
using TradingBot.Helpers;
using Binance.Spot.Models;
using System.Collections.Generic;
using Skender.Stock.Indicators;

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
                Console.WriteLine(ex);
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
                var tunicatedentryPrice = PriceUtility.turnicateNumber(priceData.Price);
                return new SpotModel()
                {
                    Symbol = symbol,
                    entryPrice = PriceUtility.turnicateNumber(priceData.Price),
                    target1 = PriceUtility.turnicateNumber(tunicatedentryPrice * 1.01m),
                    target2 = PriceUtility.turnicateNumber(tunicatedentryPrice * 1.02m),
                    stopLoss = PriceUtility.turnicateNumber(tunicatedentryPrice * 0.98m),
                    timeStamp = DateTime.Now
                };

              
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}", ex);
            }
        }

        
        
    }
}