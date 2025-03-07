using System.Security.Cryptography;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TradingTelegramService.models;

namespace TradingBot
{
    public class Helpers
    {
        public async Task<User> retrunUser(TelegramBotClient bot)
        {
            return await bot.GetMe();
        }

        public string FormatDataToMessage(SpotModel sp) { 
            
            var Msg = $"🟢 Trading Signal 🟢\n" +
                       $"Coin: {sp.Symbol}\n" +
                       $"Entry Price: {sp.entryPrice}\n" +
                       $"🎯 Target 1 (1%): {sp.target1}\n" +
                       $"🎯 Target 2 (2%): {sp.target2}\n" +
                       $"🔻 Stop Loss: {sp.stopLoss}";

            return Msg;

        }
        public string GenerateSignature(string queryString, string apiSecret)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(apiSecret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(queryString);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
