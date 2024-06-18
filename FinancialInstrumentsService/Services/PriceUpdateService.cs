using FinancialServices.Hubs;
using FinancialServices.IServices;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace FinancialServices.Services
{
    /// <summary>
    /// Background service for fetching and broadcasting price updates.
    /// </summary>
    public class PriceUpdateService : BackgroundService
    {
        private readonly IHubContext<PriceHub> _hubContext;
        private readonly IFinancialService _financialService;
        private readonly ILogger<PriceUpdateService> _logger;

        private static readonly ConcurrentDictionary<string, decimal> LastPrices =
            new ConcurrentDictionary<string, decimal>();

        /// <summary>
        /// Initializes a new instance of the PriceUpdateService class.
        /// </summary>
        /// <param name="hubContext">The SignalR hub context.</param>
        /// <param name="financialService">The financial service to fetch price data.</param>
        /// <param name="logger">The logger for logging information and errors.</param>
        public PriceUpdateService(IHubContext<PriceHub> hubContext, IFinancialService financialService, ILogger<PriceUpdateService> logger)
        {
            _hubContext = hubContext;
            _financialService = financialService;
            _logger = logger;
        }

        /// <summary>
        /// Executes the background service to periodically fetch and broadcast price updates.
        /// </summary>
        /// <param name="stoppingToken">The token to monitor for stopping requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var instrument in PriceHub.InstrumentSubscribers.Keys)
                {
                    var price = await _financialService.GetPriceAsync(instrument);
                    if (price.HasValue && LastPrices.TryGetValue(instrument, out var lastPrice) && price != lastPrice)
                    {
                        LastPrices[instrument] = price.Value;
                        await _hubContext.Clients.Group(instrument).SendAsync("ReceivePriceUpdate", instrument, price.Value);
                        _logger.LogInformation($"Broadcasting {instrument} price update: {price}");
                    }
                }

                // Wait for a bit before fetching prices again
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
