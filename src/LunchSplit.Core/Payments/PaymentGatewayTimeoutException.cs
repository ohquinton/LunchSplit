namespace LunchSplit.Core.Payments;

/// <summary>Thrown by the coordinator when a payment request fails at the transport boundary.</summary>
public sealed class PaymentGatewayTimeoutException : Exception
{
    public PaymentGatewayTimeoutException(string message, Exception innerException) : base(message, innerException) { }
}
