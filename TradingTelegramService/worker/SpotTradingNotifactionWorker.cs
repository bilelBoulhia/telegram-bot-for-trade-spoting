using Telegram.Bot.Types;
using TradingBot.Helpers;
using TradingTelegramService.models;
using TradingTelegramService.services;
using TradingTelegramService.Services;

namespace TradingTelegramService.Worker
{
    public class SpotTradingNotificationWorker : BackgroundService
    {
        private readonly ILogger<SpotTradingNotificationWorker> _logger;
        private readonly SignalService _spotTradingService;
        private List<coinData> coindata;
        private List<string> Treatedcoins;
        private List<MoniteredCoinsModel> moniteredCoinsModels;
      
        public SpotTradingNotificationWorker(ILogger<SpotTradingNotificationWorker> logger, SignalService spotTradingService)
        {
            _logger = logger;
            _spotTradingService = spotTradingService;
            Task.Run(async () => await InitializeAsync());
          
        }
        private async Task InitializeAsync()
        {
            coindata = await _spotTradingService.fetchCoinData();
            Treatedcoins = _spotTradingService.performAnalays(coindata);
            moniteredCoinsModels = await _spotTradingService.FetchSpotOfSelectedCoins(Treatedcoins);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("running...");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                  await  _spotTradingService.SendSelectedCoins(moniteredCoinsModels);


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

     

    
    }
}
