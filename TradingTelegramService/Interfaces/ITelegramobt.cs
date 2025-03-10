using Binance.Spot;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TradingTelegramService.models;

namespace TradingTelegramService.Interfaces
{
    public interface ITelegramobot
    {
        Task<Message> SendMessage(SpotModel sp, string responseMessage);
        Task<Message> ReplyToMessage(Message msg, string responseMessage);

    }
    
}
