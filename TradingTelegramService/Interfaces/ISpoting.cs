using TradingTelegramService.models;

namespace TradingTelegramService.Interfaces
{
    public interface ISpoting
    {
        Task<SpotModel> FetchSpotings(string symbol);
        Task<List<List<object>>> FetchCandleStickData(string symbol);

    }
}
