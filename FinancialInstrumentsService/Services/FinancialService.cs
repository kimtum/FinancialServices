using FinancialServices.IServices;
using Newtonsoft.Json;
using RestSharp;

namespace FinancialServices.Services
{
    /// <summary>
    /// Implementation of IFinancialService to fetch price data from Alpha Vantage.
    /// </summary>
    public class FinancialService : IFinancialService
    {
        private readonly RestClient _client;
        private readonly string _apiKey = "YOUR_ALPHA_VANTAGE_API_KEY";

        /// <summary>
        /// Initializes a new instance of the FinancialService class.
        /// </summary>
        public FinancialService()
        {
            _client = new RestClient("https://www.alphavantage.co");
        }

        /// <summary>
        /// Gets the current price of a specific financial instrument.
        /// </summary>
        /// <param name="instrument">The financial instrument to get the price for.</param>
        /// <returns>The current price of the financial instrument, or null if the price could not be fetched.</returns>
        public async Task<decimal?> GetPriceAsync(string instrument)
        {
            var request = new RestRequest("query", Method.Get);
            request.AddParameter("function", "CURRENCY_EXCHANGE_RATE");
            request.AddParameter("from_currency", instrument.Substring(0, 3));
            request.AddParameter("to_currency", instrument.Substring(3, 3));
            request.AddParameter("apikey", _apiKey);

            var response = await _client.ExecuteAsync(request);
            if (response.IsSuccessful && response.Content != null)
            {
                var exchangeRate = JsonConvert.DeserializeObject<AlphaVantageResponse>(response.Content);
                if (exchangeRate?.RealtimeCurrencyExchangeRate != null)
                {
                    return exchangeRate.RealtimeCurrencyExchangeRate.ExchangeRate;
                }
            }
            return null;
        }

        /// <summary>
        /// Represents the response structure from Alpha Vantage API.
        /// </summary>
        private class AlphaVantageResponse
        {
            [JsonProperty("Realtime Currency Exchange Rate")]
            public CurrencyExchangeRate RealtimeCurrencyExchangeRate { get; set; }

            /// <summary>
            /// Represents the exchange rate details from Alpha Vantage API.
            /// </summary>
            public class CurrencyExchangeRate
            {
                [JsonProperty("5. Exchange Rate")]
                public decimal ExchangeRate { get; set; }
            }
        }
    }
}

