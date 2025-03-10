using Binance.Spot;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TradingBot;
using TradingTelegramService.Interfaces;
using TradingTelegramService.models;


namespace TradingTelegramService.Services
{
    public class BotRepo : ITelegramobot
    {


        private TelegramBotClient _bot;

        private readonly IConfiguration _configuration;

        public BotRepo( TelegramBotClient bot, Market market, IConfiguration configuration)
        {
            _bot = bot;
         
            _configuration = configuration;
      
        }
        public async Task<Message> SendMessage(SpotModel sp,string responseMssage)
        {
            return await _bot.SendMessage(new ChatId(_configuration.GetValue<long>("Settings:chatId")), responseMssage);
        }
        public async Task<Message> ReplyToMessage(Message msg, string responseMessage)
        {
            return await _bot.SendMessage(
                chatId: msg.Chat.Id,
                text: responseMessage,
                replyParameters: new ReplyParameters
                {
                    MessageId = msg.MessageId
                }
            );
        }


    }
}
