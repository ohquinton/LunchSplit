namespace LunchSplit.Core.Payments;

/// <summary>Payment gateway contract. Core never depends on Square itself.</summary>
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
    Task<PaymentStatus> GetTransactionStatusAsync(string transactionId);
}
