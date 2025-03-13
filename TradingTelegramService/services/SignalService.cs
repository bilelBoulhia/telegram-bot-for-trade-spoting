using System.Security.Cryptography;
using Telegram.Bot.Types;
using TradingBot.Helpers;
using TradingTelegramService.models;
using TradingTelegramService.Services;

namespace TradingTelegramService.services
{
    public class SignalService
    {
        private readonly BotRepo _botservices;
        private readonly SpotingRepo _spotingRepo;
        private decimal _latestprice;
        private Message LatestBotMessage;


        public SignalService(BotRepo botServices, SpotingRepo spotingRepo)
        {
            _botservices = botServices;
            _spotingRepo = spotingRepo;
        }


        public async Task<string> assignCoin()
        {
            List<string> coins = ClassUtil.GetCoins();
            foreach(var coin in coins)
            {
             var data = await  _spotingRepo.FetchCandleStickData(coin);
             var quotas = StockUtil.convertToQuotas(data);
                QuotaResults qr = new QuotaResults()
                {
                    rsiResult = StockUtil.PerFormRSI(quotas),
                    atrResult = StockUtil.PerFormATR(quotas),
                    bandsResult = StockUtil.PerFormBollingBands(quotas),
                    LatestQuote = quotas.LastOrDefault(),
                    macdResult = StockUtil.PerFormMACD(quotas),
                    obvResult = StockUtil.PerFormObv(quotas),
                    vwapResult = StockUtil.PerFormVWMP(quotas),
                };
                if (StockUtil.PerformTechnicalAnalasys(qr))
                {
                    return coin;
                    break;
                }  
                
            }
            return null;
        }

        public async Task PerfomSignalActionsync(string symbol)
        {
            var spot = await _spotingRepo.FetchSpotings(symbol);
            if (_latestprice == 0)
            {
                _latestprice = spot.entryPrice;
                var spotMessage = MessageUtility.FormatSpotTradeSignal(spot);
                LatestBotMessage = await _botservices.SendMessage(spot, spotMessage);
            }
            if (PriceUtility.HasPriceChanged(_latestprice, spot.entryPrice))
            {
                int index = PriceUtility.CheckUpdatedPrice(spot);

                if (index != 0)
                {
                    string reply = MessageUtility.FromatReplySpot(index, spot);
                    await _botservices.ReplyToMessage(LatestBotMessage, reply);
                }
            }
        }


    }
}
