namespace FinancialServices.IServices
{
    /// <summary>
    /// Interface for financial service to get price updates for financial instruments.
    /// </summary>
    public interface IFinancialService
    {
        /// <summary>
        /// Gets the current price of a specific financial instrument.
        /// </summary>
        /// <param name="instrument">The financial instrument to get the price for.</param>
        /// <returns>The current price of the financial instrument.</returns>
        Task<decimal?> GetPriceAsync(string instrument);
    }
}
