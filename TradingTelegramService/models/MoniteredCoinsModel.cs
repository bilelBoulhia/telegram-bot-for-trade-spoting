using Telegram.Bot.Types;

namespace TradingTelegramService.models
{
    public class MoniteredCoinsModel
    {
        public SpotModel coinSP { get; set; }
        public DateTime createdAt { get; set; }
        public Message? attachedMessage { get; set; }
    }
}
