

using Binance.Spot;
using Telegram.Bot;
using TradingBot;
using TradingTelegramService.Services;

namespace TradingTelegramService.Worker
{
    public class SpotTradingNotificationWorker : BackgroundService
    {
        private readonly ILogger<SpotTradingNotificationWorker> _logger;
        private readonly BotServices _botservices;


        public SpotTradingNotificationWorker(ILogger<SpotTradingNotificationWorker> logger,BotServices botServices)
        {
            _logger = logger;
            _botservices = botServices;
     }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("running...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _botservices.SendSpot();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "error");
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken); 
            }
        }
    }
}