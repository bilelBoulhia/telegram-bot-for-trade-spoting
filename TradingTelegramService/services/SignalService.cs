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
          var newData = await _spotingRepo.FetchCandleStickData(c);

          coindata.Add(new coinData { data = newData, symbol = c });
                    
        
            
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
                    createdAt = DateTime.UtcNow + TimeSpan.FromHours(1)
                }); 
            }

            return monitoredCoins;

        }

        public async Task SendSelectedCoins(List<MoniteredCoinsModel> MoniteredCoins, List<MoniteredCoinsModel> NewMoniteredCoinsSpots)
        {

            foreach (var coin in MoniteredCoins)
            {
                var newspotPrice = NewMoniteredCoinsSpots.Where(newc => coin.coinSP.Symbol == newc.coinSP.Symbol).Select(s => s.coinSP.entryPrice).FirstOrDefault();

                if (coin.attachedMessage == null || !PriceUtility.HasPriceChanged(coin.coinSP.entryPrice,newspotPrice))
                {
                    var spotMessage = MessageUtility.FormatSpotTradeSignal(coin.coinSP);
                    coin.attachedMessage = await _botservices.SendMessage(coin.coinSP, spotMessage);
                    Console.WriteLine($"this coin {coin}will sent by telegram with this message {spotMessage}");
                }
                else
                {
                    var duration = (DateTime.UtcNow + TimeSpan.FromHours(1)) - coin.coinSP.timeStamp;
                 
                    string reply = MessageUtility.FromatReplySpot(PriceUtility.CheckUpdatedPrice(coin.coinSP), coin.coinSP, duration);
                    if(reply != null)
                    {
                        await _botservices.ReplyToMessage(coin.attachedMessage, reply);
                    }
                  

                }
               
            }

        }

    }


}
