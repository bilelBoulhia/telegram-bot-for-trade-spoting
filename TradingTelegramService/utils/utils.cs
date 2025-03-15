using System.Reflection;
using TradingTelegramService.Constants;
using TradingTelegramService.models;
using Skender.Stock.Indicators;
using System.Text.Json;


namespace TradingBot.Helpers
{



    public static  class StockUtil{
        public static RsiResult PerFormRSI(List<Quote> quote)
        {
          return quote.GetRsi(14).LastOrDefault();
        }
        public static VwapResult PerFormVWMP(List<Quote> quote)
        {
            return quote.GetVwap().LastOrDefault();
        }
        public static MacdResult PerFormMACD(List<Quote> quote)
        {
            return quote.GetMacd().LastOrDefault();
        }
        public static BollingerBandsResult PerFormBollingBands(List<Quote> quote)
        {
            return quote.GetBollingerBands(20, 2).LastOrDefault();
        }

        public static AtrResult PerFormATR(List<Quote> quote)
        {
            return quote.GetAtr(14).LastOrDefault();
        }
        public static ObvResult PerFormObv(List<Quote> quote)
        {
            return quote.GetObv().LastOrDefault();
        }

        public static List<Quote> convertToQuotas(List<List<object>> obj)
        {
            List<Quote> quotes = new();

            foreach (var quota in obj)
            {
                quotes.Add(new Quote
                {
                    Date = DateTimeOffset.FromUnixTimeMilliseconds(((JsonElement)quota[0]).GetInt64()).UtcDateTime,
                    Open = Convert.ToDecimal(((JsonElement)quota[1]).GetString()),
                    High = Convert.ToDecimal(((JsonElement)quota[2]).GetString()),
                    Low = Convert.ToDecimal(((JsonElement)quota[3]).GetString()),
                    Close = Convert.ToDecimal(((JsonElement)quota[4]).GetString()),
                    Volume = Convert.ToDecimal(((JsonElement)quota[5]).GetString())
                });
            }

            return quotes;

        }

        public static bool PerformTechnicalAnalasys(QuotaResults quotaResults)
        {
            decimal atrThreshold = 0.5m;
            decimal rsiOverbought = 70m;
            decimal bandProximityPct = 0.01m;

            if ((decimal)quotaResults.rsiResult.Rsi < rsiOverbought &&
                 quotaResults.LatestQuote.Close < (decimal)quotaResults.vwapResult.Vwap &&
                (decimal)quotaResults.macdResult.Macd < (decimal)quotaResults.macdResult.Signal &&
                 quotaResults.LatestQuote.Close >= (decimal)quotaResults.bandsResult.UpperBand * (1 - bandProximityPct) &&
                (decimal)quotaResults.obvResult.Obv < 0)
            {
                return true;
            }

            return false;
        }




    }




    public static class ClassUtil
    {
     public static List<string> GetDataFromClass()
        {
            return typeof(CryptoConstants).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Select(F => F.GetValue(null)?.ToString())
                .ToList();
        }

        public static bool AreListsDifferent(List<List<object>> oldCandle, List<List<object>> newCandle)
        {



            try
            {
               
                decimal oldOpen = Convert.ToDecimal(((JsonElement)oldCandle[oldCandle.Count-1][1]).GetString());
                decimal oldHigh = Convert.ToDecimal(((JsonElement)oldCandle[oldCandle.Count - 1][2]).GetString());
                decimal oldLow = Convert.ToDecimal(((JsonElement)oldCandle[oldCandle.Count - 1][3]).GetString());
                decimal oldClose = Convert.ToDecimal(((JsonElement)oldCandle[oldCandle.Count - 1][4]).GetString());
                decimal oldVolume = Convert.ToDecimal(((JsonElement)oldCandle[oldCandle.Count - 1][5]).GetString());

                decimal newOpen = Convert.ToDecimal(((JsonElement)oldCandle[oldCandle.Count - 1][1]).GetString());
                decimal newHigh = Convert.ToDecimal(((JsonElement)oldCandle[oldCandle.Count - 1][2]).GetString());
                decimal newLow = Convert.ToDecimal(((JsonElement)oldCandle[oldCandle.Count - 1][3]).GetString());
                decimal newClose = Convert.ToDecimal(((JsonElement)oldCandle[oldCandle.Count - 1][4]).GetString());
                decimal newVolume = Convert.ToDecimal(((JsonElement)oldCandle[oldCandle.Count - 1][5]).GetString());

                return oldOpen != newOpen ||
                       oldHigh != newHigh ||
                       oldLow != newLow ||
                       oldClose != newClose ||
                       oldVolume != newVolume;
            }
            catch
            {
                return true; 
            }
        
        }

        public static void clearList<T>(List<T> l) 
        {
            l.Clear();
        }


    }



    public static class MessageUtility
    {
        public static string FormatSpotTradeSignal(SpotModel sp)
        {
            return $"🟢 Signal 🟢\n" +
                   $"Coin: {sp.Symbol}\n" +
                   $"prix: {sp.entryPrice}\n" +
                   $"🎯 Tp1 (1%): {sp.target1}\n" +
                   $"🎯 Tp2 (2%): {sp.target2}\n" +
                   $"🔻 SP : {sp.stopLoss}" +
                   $"🔻 temp : {sp.timeStamp}";

        }
        public static string FromatReplySpot(int index,SpotModel spotModel,TimeSpan duration)
        {
            switch(index)
            {
                case 1:
                    return $"{spotModel.Symbol} Tp1 ✅ :  {spotModel.target1}, duration:{duration.TotalMinutes} ";
                case 2:
                    return $"{spotModel.Symbol} Tp2 ✅ :  {spotModel.target2}, duration:{duration.TotalMinutes} ";
                case 3:
                    return $"{spotModel.Symbol} stopLoss ❌:  {spotModel.stopLoss} , duration:{duration.TotalMinutes}";
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



}
