using Binance.Spot;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TradingTelegramService.Interfaces
{
    public interface ITelegramobot
    {
        Task<Message> SendSpot();
    }
    
}
