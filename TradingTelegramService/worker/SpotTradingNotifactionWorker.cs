using TradingTelegramService.models;
using TradingTelegramService.services;

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

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var mainTask = Task.Run(() => RunFetchingTask(stoppingToken), stoppingToken);
                    var secondaryTask = Task.Run(() => RunSendingTask(stoppingToken), stoppingToken);

                    await Task.WhenAll(mainTask, secondaryTask);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception in worker. Restarting in 10 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
        }

        private async Task RunFetchingTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var coindata = await _spotTradingService.fetchCoinData();
                    var treatedCoins = _spotTradingService.performAnalays(coindata);

                    _logger.LogInformation($"the coins that are treated are of length : {treatedCoins.Count}");

                    if (treatedCoins.Count == 0)
                    {
                        _logger.LogInformation("No coins have enough liquidity.");
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                        continue;
                    }
                    _logger.LogInformation($"the MONITORED COINS ARE BEFORE BEING CLREAD : {moniteredCoins?.Count}");
                    moniteredCoins?.Clear();
                    moniteredCoins = await _spotTradingService.FetchSpotOfSelectedCoins(treatedCoins);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in fetching task");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task RunSendingTask(CancellationToken stoppingToken)
        {
            while (moniteredCoins == null && !stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (moniteredCoins.Count > 0)
                    {
                        var newCoinSpots = await _spotTradingService.FetchSpotOfSelectedCoins(moniteredCoins.Select(c => c.coinSP.Symbol).ToList());
                        foreach (var item in newCoinSpots)
                        {
                            _logger.LogInformation($"New COINS TO BE SPOTTED ARE . {item.coinSP.Symbol}");
                        }
                        foreach (var item in newCoinSpots)
                        {
                            _logger.LogInformation($"monitored COINS TO BE SPOTTED ARE . {item.coinSP.Symbol}");
                        }
                        await _spotTradingService.SendSelectedCoins(moniteredCoins, newCoinSpots);
                    }
                    else
                    {
                        _logger.LogInformation("No coins are being monitored.");
                    }

                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in sending task");
                }
            }
        }
    }
}
