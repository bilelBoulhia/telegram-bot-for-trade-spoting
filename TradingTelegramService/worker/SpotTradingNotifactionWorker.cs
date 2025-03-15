using System.Text.Json;
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
        private List<MoniteredCoinsModel> moniteredCoins;

        public SpotTradingNotificationWorker(ILogger<SpotTradingNotificationWorker> logger, SignalService spotTradingService)
        {
            _logger = logger;
            _spotTradingService = spotTradingService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting background worker...");

            var mainTask = Task.Run(() => RunFetchingTask(stoppingToken), stoppingToken);
            var secondaryTask = Task.Run(() => RunSendingTask(stoppingToken), stoppingToken);

            await Task.WhenAll(mainTask, secondaryTask);
        }

        private async Task RunFetchingTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var coindata = await _spotTradingService.fetchCoinData();
                    var treatedCoins = _spotTradingService.performAnalays(coindata);

                    if (treatedCoins.Count == 0)
                    {
                        _logger.LogInformation("No coins have enough liquidity.");
                    }
                    else
                    {
                        if (moniteredCoins != null)
                        {
                            moniteredCoins.Clear();
                        }
             
                        moniteredCoins = await _spotTradingService.FetchSpotOfSelectedCoins(treatedCoins);
                        
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in main task");
                }

                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }

        private async Task RunSendingTask(CancellationToken stoppingToken)
        {

            while (moniteredCoins == null && !stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting for monitored coins to be initialized...");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {
                    if (moniteredCoins.Count > 0)
                    {
                       var newCoinSpots =   await _spotTradingService.FetchSpotOfSelectedCoins(moniteredCoins.Select(c=>c.coinSP.Symbol).ToList());

                       await _spotTradingService.SendSelectedCoins(moniteredCoins,newCoinSpots);
                    }
                    else
                    {
                        _logger.LogInformation("No coins are being monitored.");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in parallel task");
                }
            }
        }

    }
}
