using Skender.Stock.Indicators;

namespace TradingTelegramService.models
{
    public class QuotaResults
    {
        public Quote LatestQuote { get; set; }
       public RsiResult rsiResult { get; set; }
        public VwapResult vwapResult { get; set; }

        public MacdResult macdResult { get; set; }


        public BollingerBandsResult bandsResult { get; set; }

        public AtrResult atrResult { get; set; }

        public ObvResult obvResult { get; set; }
    }
}
