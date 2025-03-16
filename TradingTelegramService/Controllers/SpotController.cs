using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradingTelegramService.models;
using TradingTelegramService.services;

namespace TradingTelegramService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotController : ControllerBase
    {
        private readonly SignalService _spotTradingService;
  

        public SpotController(SignalService spotTradingService)
        {
            _spotTradingService = spotTradingService;
        }

        [HttpGet("/CronTrigger")]
        public async Task<IActionResult> CronTreatCoinsTrigger()
        {
            try
            {
                var coindata = await _spotTradingService.fetchCoinData();
                var treatedCoins = _spotTradingService.performAnalays(coindata);



                if (treatedCoins.Count == 0)
                {
                    return Ok($"no liquidity");
                }

                var monitoredCoins = await _spotTradingService.FetchSpotOfSelectedCoins(treatedCoins);
                _spotTradingService.UpdateMonitoredCoins(monitoredCoins);

                return Ok($"coins of size {_spotTradingService.MonitoredCoins.Count} been treated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }



        [HttpGet("ReturnMonitoredCoins")]
        public IActionResult ReturnMonitoredCoins()
        {
            return Ok($"coins being treated right now: {_spotTradingService.MonitoredCoins.Count}");
        }


        [HttpGet("/SendMonitoredCoins")]
        public async Task<IActionResult> SendMonitoredCoins()
        {
            try
            {
                if (_spotTradingService.MonitoredCoins.Count > 0)
                {
                    var newCoinSpots = await _spotTradingService.FetchSpotOfSelectedCoins(_spotTradingService.MonitoredCoins.Select(c => c.coinSP.Symbol).ToList());
               
                    await _spotTradingService.SendSelectedCoins(_spotTradingService.MonitoredCoins, newCoinSpots);
                    return BadRequest("telegram message sent");
                }
                else
                {
                    return BadRequest("no coins being monitored");
                }

        
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
    
        }
    }
}
