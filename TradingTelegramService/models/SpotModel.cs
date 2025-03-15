using TradingBot.Helpers;

namespace TradingTelegramService.models
{
    public class SpotModel
    {
        public string Symbol { get; set; }

        private decimal _entryPrice;
        private decimal _target1;
        private decimal _target2;
        private decimal _stopLoss;

        public decimal entryPrice
        {
            get => PriceUtility.turnicateNumber(_entryPrice);
            set => _entryPrice = value;
        }

        public decimal target1
        {
            get => PriceUtility.turnicateNumber(_target1);
            set => _target1 = value;
        }

        public decimal target2
        {
            get => PriceUtility.turnicateNumber(_target2);
            set => _target2 = value;
        }

        public decimal stopLoss
        {
            get => PriceUtility.turnicateNumber(_stopLoss);
            set => _stopLoss = value;
        }

        public DateTime timeStamp { get; set; }
    }
}
