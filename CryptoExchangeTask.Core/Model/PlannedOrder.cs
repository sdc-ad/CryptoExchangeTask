namespace CryptoExchangeTask.Core.Model;

/// <summary>
/// An order which is planned in order to fulfill a user's request.
/// </summary>
/// <param name="FulfilledByExchangeId">The exchange against which the order should be executed.</param>
/// <param name="FulfilledByOrderId">
/// The Id of the entry in the exchange's order book which would bu used to fulfill this order.
/// </param>
/// <param name="OrderType">Whether this is a buy or sell order.</param>
/// <param name="Amount">The crypto amount of the order.</param>
/// <param name="Price">The Euro unit price at which the order would be executed.</param>
public record PlannedOrder(
    string FulfilledByExchangeId,
    Guid FulfilledByOrderId,
    OrderType OrderType,
    decimal Amount,
    decimal Price);