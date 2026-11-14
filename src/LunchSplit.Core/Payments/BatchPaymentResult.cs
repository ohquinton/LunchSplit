namespace LunchSplit.Core.Payments;

/// <summary>Ordered results retain their attendee positions for partial-failure reporting.</summary>
public sealed class BatchPaymentResult
{
    public List<PaymentResult> Payments { get; } = new();
    public int SuccessCount => Payments.Count(r => r.IsSuccess);
    public int FailureCount => Payments.Count(r => !r.IsSuccess);
}
