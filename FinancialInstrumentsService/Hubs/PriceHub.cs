using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FinancialServices.Hubs
{
    /// <summary>
    /// SignalR hub for managing subscriptions to financial instrument price updates.
    /// </summary>
    public class PriceHub : Hub
    {
        // ConcurrentDictionary to maintain a list of subscribers for each financial instrument
        // This ensures thread-safe operations when adding or removing subscribers
        internal static readonly ConcurrentDictionary<string, ConcurrentBag<string>> InstrumentSubscribers =
            new ConcurrentDictionary<string, ConcurrentBag<string>>();

        /// <summary>
        /// Subscribes the client to receive updates for a specific financial instrument.
        /// </summary>
        /// <param name="instrument">The financial instrument to subscribe to.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Subscribe(string instrument)
        {
            // Add the connection to the group associated with the instrument
            // SignalR groups allow broadcasting messages to multiple connections efficiently
            await Groups.AddToGroupAsync(Context.ConnectionId, instrument);

            // Add the connection to the instrument subscribers list
            InstrumentSubscribers.AddOrUpdate(instrument,
                new ConcurrentBag<string> { Context.ConnectionId },
                (key, bag) => { bag.Add(Context.ConnectionId); return bag; });

            // Log subscription
            Serilog.Log.Information($"Connection {Context.ConnectionId} subscribed to {instrument}");
        }

        /// <summary>
        /// Unsubscribes the client from receiving updates for a specific financial instrument.
        /// </summary>
        /// <param name="instrument">The financial instrument to unsubscribe from.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Unsubscribe(string instrument)
        {
            // Remove the connection from the group associated with the instrument
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, instrument);

            // Remove the connection from the instrument subscribers list
            if (InstrumentSubscribers.TryGetValue(instrument, out var subscribers))
            {
                subscribers.TryTake(out var connectionId);
            }

            // Log unsubscription
            Serilog.Log.Information($"Connection {Context.ConnectionId} unsubscribed from {instrument}");
        }

        /// <summary>
        /// Handles disconnection of a client by removing the client from all subscribed groups.
        /// </summary>
        /// <param name="exception">The exception that caused the disconnect, if any.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Remove the connection from all instrument subscribers lists
            foreach (var instrument in InstrumentSubscribers.Keys)
            {
                if (InstrumentSubscribers.TryGetValue(instrument, out var subscribers))
                {
                    subscribers.TryTake(out var connectionId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Broadcasts a price update to all subscribers of a specific financial instrument.This method will be called by a background service,
        /// </summary>
        /// <param name="instrument">The financial instrument being updated.</param>
        /// <param name="price">The new price of the financial instrument.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task BroadcastPriceUpdate(string instrument, decimal price)
        {
            // Broadcast to all connections in the SignalR group associated with the instrument
            // This ensures efficient message delivery to multiple subscribers
            await Clients.Group(instrument).SendAsync("ReceivePriceUpdate", instrument, price);
        }
    }
}
