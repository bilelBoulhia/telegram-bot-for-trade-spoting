using CryptoExchange.Net.CommonObjects;
using System;
using TradingBot.Helpers;
using TradingTelegramService.models;
using TradingTelegramService.Services;

namespace TradingTelegramService.services
{
    public class SignalService
{
    private readonly BotRepo _botservices;
    private readonly SpotingRepo _spotingRepo;
    private List<string> coins = ClassUtil.GetDataFromClass();
    private Dictionary<string, List<List<object>>> _cachedCoinData = new(); 

    public SignalService(BotRepo botServices, SpotingRepo spotingRepo)
    {
        _botservices = botServices;
        _spotingRepo = spotingRepo;
    }

    public async Task<List<coinData>> fetchCoinData()
    {
        List<coinData> coindata = new();

        foreach (var c in coins)
        {
          
            if (_cachedCoinData.TryGetValue(c, out var oldData))
            {
                var newData = await _spotingRepo.FetchCandleStickData(c);

               
                if (newData.Count > 0 && oldData.Count > 0 )
                {
                 
                    continue; 
                }

                _cachedCoinData[c] = newData; 
                coindata.Add(new coinData { data = newData, symbol = c });
            }
            else
            {
                
                var newData = await _spotingRepo.FetchCandleStickData(c);
                _cachedCoinData[c] = newData;

                coindata.Add(new coinData { data = newData, symbol = c });

        
            }
        }

        return coindata;
    }
        public List<string> performAnalays(List<coinData> coindata)
        {
            List<string> coinsReadyTobeSignaled = new();
            foreach (var coin in coindata)
            {
                var quotas = StockUtil.convertToQuotas(coin.data);
                QuotaResults qr = new QuotaResults()
                {
                    rsiResult = StockUtil.PerFormRSI(quotas),
                    LatestQuote = quotas.LastOrDefault(),
                    atrResult = StockUtil.PerFormATR(quotas),
                    bandsResult = StockUtil.PerFormBollingBands(quotas),
                    macdResult = StockUtil.PerFormMACD(quotas),
                    obvResult = StockUtil.PerFormObv(quotas),
                    vwapResult = StockUtil.PerFormVWMP(quotas),
                };
                if (StockUtil.PerformTechnicalAnalasys(qr))
                {
                    coinsReadyTobeSignaled.Add(coin.symbol);
                }



            }
            return coinsReadyTobeSignaled;
        }

        public async Task<List<MoniteredCoinsModel>> FetchSpotOfSelectedCoins(List<string> SelectedCoins)
        {
            List<MoniteredCoinsModel> monitoredCoins = new();
            foreach (var coin in SelectedCoins)
            {
                monitoredCoins.Add(new MoniteredCoinsModel()
                {
                    coinSP = await _spotingRepo.FetchSpotings(coin),
                    createdAt = DateTime.UtcNow
                }); 
            }

            return monitoredCoins;

        }

        public async Task SendSelectedCoins(List<MoniteredCoinsModel> SelectedCoins)
        {
           
          foreach(var coin in SelectedCoins)
            {
                if(coin.attachedMessage == null)
                {
                    var spotMessage = MessageUtility.FormatSpotTradeSignal(coin.coinSP);
                    coin.attachedMessage = await _botservices.SendMessage(coin.coinSP, spotMessage);
                }
                else
                {
                    var duration = DateTime.UtcNow - coin.coinSP.timeStamp;
                    string reply = MessageUtility.FromatReplySpot(PriceUtility.CheckUpdatedPrice(coin.coinSP), coin.coinSP, duration);
                    await _botservices.ReplyToMessage(coin.attachedMessage, reply);

                }
               
            }

        }

    }


}
