namespace TradingTelegramService.models
{
    public class SpotModel
    {
        public string Symbol { get; set; }
        public decimal entryPrice { get; set; }
        public decimal target1 { get; set; }
        public decimal target2 { get; set; }
        public decimal stopLoss { get; set; }

        public DateTime timeStamp { get; set; }

    }
}
