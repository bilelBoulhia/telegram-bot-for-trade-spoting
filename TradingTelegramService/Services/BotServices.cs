using Binance.Spot;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TradingBot;
using TradingTelegramService.Interfaces;
using TradingTelegramService.Worker;

namespace TradingTelegramService.Services
{
    public class BotServices : ITelegramobot
    {

        
       private TelegramBotClient _bot;
       private SpotingService _spotingService;
       private readonly IConfiguration _configuration;
        private readonly Helpers _helper;
        public BotServices(Helpers helper,TelegramBotClient bot,SpotingService spotingService, Market market, IConfiguration configuration, Helpers helpers)
        {
            _bot = bot;
            _helper = helper;
            _configuration = configuration;
            _spotingService = spotingService;
        }
        public async Task<Message> SendSpot()
        {
            
            var resp = await _spotingService.GetSpotings();
            var respMessage = _helper.FormatDataToMessage(resp);
            return await _bot.SendMessage(new ChatId(_configuration.GetValue<long>("Settings:chatId")),respMessage);
        }

      
    }
}
