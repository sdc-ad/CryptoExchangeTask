namespace CryptoExchangeTask.Core.Errors;

/// <summary>
/// Exception thrown if the user does not have a sufficient balance or there are not enough order book entries
/// to complete the requested order plan.
/// </summary>
public class InsufficientBalanceException : Exception
{
}
