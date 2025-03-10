using Telegram.Bot.Types;
using TradingBot.Helpers;
using TradingTelegramService.Services;

namespace TradingTelegramService.services
{
    public class SpotTradingService
    {
        private readonly BotRepo _botservices;
        private readonly SpotingRepo _spotingRepo;
        private Message LatestBotMessage;
        private decimal _latestprice = 0;

        public SpotTradingService(BotRepo botServices, SpotingRepo spotingRepo)
        {
            _botservices = botServices;
            _spotingRepo = spotingRepo;
        }

        public async Task CheckAndSendNotificationsAsync(CancellationToken stoppingToken)
        {
            var spot = await _spotingRepo.GetSpotings();
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
