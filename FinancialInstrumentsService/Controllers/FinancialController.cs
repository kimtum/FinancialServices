using FinancialServices.IServices;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancialServices.Controllers
{
    /// <summary>
    /// Controller to handle requests related to financial instruments and their prices.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FinancialController : ControllerBase
    {
        private readonly IFinancialService _financialService;

        /// <summary>
        /// Initializes a new instance of the FinancialController class.
        /// </summary>
        /// <param name="financialService">The financial service to get price data.</param>
        public FinancialController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        /// <summary>
        /// Gets a list of available financial instruments.
        /// </summary>
        /// <returns>A list of available financial instruments.</returns>
        [HttpGet("instruments")]
        public IActionResult GetInstruments()
        {
            var instruments = new List<string> { "EURUSD", "USDJPY", "BTCUSD" };
            return Ok(instruments);
        }

        /// <summary>
        /// Gets the current price of a specific financial instrument.
        /// </summary>
        /// <param name="instrument">The financial instrument to get the price for.</param>
        /// <returns>The current price of the financial instrument.</returns>

        [HttpGet("price/{instrument}")]
        public async Task<IActionResult> GetPrice(string instrument)
        {
            var price = await _financialService.GetPriceAsync(instrument);
            if (price != null)
            {
                return Ok(price);
            }
            return NotFound();
        }
    }
}
