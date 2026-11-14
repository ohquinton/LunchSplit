namespace LunchSplit.Core.Payments;

/// <summary>Currency values use ISO 4217 codes; amount uses major units (e.g., CAD 12.34).</summary>
public sealed class PaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "CAD";
    public string CardNonce { get; set; } = string.Empty;
    /// <summary>Keep this key unchanged on retries to avoid duplicate charges.</summary>
    public string IdempotencyKey { get; set; } = string.Empty;
}
