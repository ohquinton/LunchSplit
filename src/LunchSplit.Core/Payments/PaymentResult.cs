namespace LunchSplit.Core.Payments;

/// <summary>Provider-independent success or error response for one attempt.</summary>
public sealed class PaymentResult
{
    public bool IsSuccess { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    /// <summary>HTTP status, when one was returned by a remote gateway.</summary>
    public int? HttpStatusCode { get; set; }
}
