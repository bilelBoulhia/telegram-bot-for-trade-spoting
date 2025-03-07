using TradingTelegramService.models;

namespace TradingTelegramService.Interfaces
{
    public interface ISpoting
    {
        Task<SpotModel> GetSpotings();
    }
}
