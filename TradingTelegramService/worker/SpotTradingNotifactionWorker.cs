

using Telegram.Bot.Types;
using TradingBot.Helpers;
using TradingTelegramService.services;
using TradingTelegramService.Services;

namespace TradingTelegramService.Worker
{
    public class SpotTradingNotificationWorker : BackgroundService
    {
        private readonly ILogger<SpotTradingNotificationWorker> _logger;
        private readonly SpotTradingService _spotTradingService;

        public SpotTradingNotificationWorker(ILogger<SpotTradingNotificationWorker> logger, SpotTradingService spotTradingService)
        {
            _logger = logger;
            _spotTradingService = spotTradingService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Spot Trading Notification Worker running...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _spotTradingService.CheckAndSendNotificationsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in processing spot trading notifications");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}