using Microsoft.OpenApi.Any;
using System.Security.Cryptography;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TradingTelegramService.models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TradingBot.Helpers
{

    public static class UserUtility
    {
        public static async Task<User> GetBotUserAsync(TelegramBotClient bot)
        {
            return await bot.GetMe();
        }
    }


    public static class MessageUtility
    {
        public static string FormatSpotTradeSignal(SpotModel sp)
        {
            return $"🟢 Trading Signal 🟢\n" +
                   $"Coin: {sp.Symbol}\n" +
                   $"price: {PriceUtility.turnicateNumber(sp.entryPrice)}\n" +
                   $"🎯 Target 1 (1%): {PriceUtility.turnicateNumber(sp.target1)}\n" +
                   $"🎯 Target 2 (2%): {PriceUtility.turnicateNumber(sp.target2)}\n" +
                   $"🔻 Stop Loss: {PriceUtility.turnicateNumber(sp.stopLoss)}";
        }
        public static string FromatReplySpot(int index,SpotModel spotModel)
        {
            switch(index)
            {
                case 1:
                    return $"{spotModel.Symbol} Tp1 ✅ :  {PriceUtility.turnicateNumber(spotModel.target1)} ";
                case 2:
                    return $"{spotModel.Symbol} Tp2 ✅ :  {PriceUtility.turnicateNumber(spotModel.target2)} ";
                case 3:
                    return $"{spotModel.Symbol} stopLoss ❌:  {PriceUtility.turnicateNumber(spotModel.stopLoss)} ";
                case 0:
                    return null;
                default:
                    return null;


            }
            
        }
    }

  
    public static class PriceUtility
    {
        public static bool HasPriceChanged(decimal currentPrice, decimal lastCoinPrice)
        {
            if (currentPrice != lastCoinPrice)
            {
                return true;
            }
            return false;
        }
        public static int CheckUpdatedPrice(SpotModel spot)
        {

            switch (spot.entryPrice)
            {
                case var p when p >= spot.target1 && p < spot.target2:
                    return 1;
                case var p when p >= spot.target2:
                    return 2;
                case var p when p < spot.stopLoss:
                    return 3;
                default:
                    return 0;
            }

           
        }

        public static decimal turnicateNumber(object i) 
        {

            return Math.Truncate(Convert.ToDecimal(i) * 10000) / 10000;
        }
        
    }


    //public static class SecurityUtility
    //{
    //    public  static string GenerateSignature(string queryString, string apiSecret)
    //    {
    //        byte[] keyBytes = Encoding.UTF8.GetBytes(apiSecret);
    //        byte[] messageBytes = Encoding.UTF8.GetBytes(queryString);

    //        using (var hmac = new HMACSHA256(keyBytes))
    //        {
    //            byte[] hashBytes = hmac.ComputeHash(messageBytes);
    //            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    //        }
    //    }
    //}
}
