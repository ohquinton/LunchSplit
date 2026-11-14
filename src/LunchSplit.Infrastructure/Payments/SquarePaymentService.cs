using LunchSplit.Core.Payments;
using Square;
using Square.Payments;

namespace LunchSplit.Infrastructure.Payments;

/// <summary>
/// Sandbox-only Square adapter. The SDK is intentionally confined to Infrastructure;
/// unit tests replace IPaymentProcessor with a Moq object.
/// </summary>
public sealed class SquarePaymentService : IPaymentProcessor
{
    private static readonly RequestOptions CallOptions = new()
    {
        Timeout = TimeSpan.FromSeconds(5),
        MaxRetries = 0
    };

    private readonly SquareClient _client;
    private readonly string _locationId;

    /// <param name="accessToken">Sandbox token; supply from environment or secret store.</param>
    /// <param name="locationId">ID of a Square Sandbox location, normally configured for CAD.</param>
    public SquarePaymentService(string accessToken, string locationId)
    {
        if (string.IsNullOrWhiteSpace(accessToken)) throw new ArgumentException("Sandbox token required.", nameof(accessToken));
        if (string.IsNullOrWhiteSpace(locationId)) throw new ArgumentException("Sandbox location required.", nameof(locationId));
        _locationId = locationId;
        // Explicit Sandbox environment prevents accidentally charging live cards.
        _client = new SquareClient(accessToken, new ClientOptions { BaseUrl = SquareEnvironment.Sandbox });
    }

    public static SquarePaymentService FromEnvironment() => new(
        Environment.GetEnvironmentVariable("SQUARE_SANDBOX_ACCESS_TOKEN") ?? "",
        Environment.GetEnvironmentVariable("SQUARE_SANDBOX_LOCATION_ID") ?? "");

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Amount <= 0m || decimal.Round(request.Amount, 2) != request.Amount)
            return new PaymentResult { ErrorCode = "INVALID_AMOUNT", ErrorMessage = "Amount must be positive with at most two decimal places." };
        if (string.IsNullOrWhiteSpace(request.CardNonce))
            return new PaymentResult { ErrorCode = "INVALID_SOURCE", ErrorMessage = "A card nonce is required." };
        if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
            return new PaymentResult { ErrorCode = "INVALID_IDEMPOTENCY_KEY", ErrorMessage = "A stable idempotency key is required." };
        // Square money uses integer minor units. Convert exactly, never binary floating point.
        long amountMinor = checked((long)(request.Amount * 100m));
        var currency = Currency.FromCustom(request.Currency.Trim().ToUpperInvariant());
        try
        {
            var response = await _client.Payments.CreateAsync(new CreatePaymentRequest
            {
                SourceId = request.CardNonce,
                IdempotencyKey = request.IdempotencyKey,
                AmountMoney = new Money { Amount = amountMinor, Currency = currency },
                LocationId = _locationId,
                Autocomplete = true
            }, CallOptions);
            var payment = response.Payment;
            return new PaymentResult
            {
                IsSuccess = payment is not null && payment.Status?.ToString() == "COMPLETED",
                TransactionId = payment?.Id,
                ErrorCode = payment is null ? "MISSING_PAYMENT" : null,
                ErrorMessage = payment is null ? "Square returned no payment." : null,
                HttpStatusCode = 200
            };
        }
        catch (SquareApiException ex)
        {
            // Square responds with structured errors for declines and invalid currencies.
            var error = ex.Errors?.FirstOrDefault();
            return new PaymentResult
            {
                IsSuccess = false,
                ErrorCode = error?.Code.ToString() ?? "SQUARE_ERROR",
                ErrorMessage = error?.Detail ?? ex.Message,
                HttpStatusCode = ex.StatusCode
            };
        }
    }

    public async Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId)) throw new ArgumentException("Transaction ID required.", nameof(transactionId));
        var response = await _client.Payments.GetAsync(new GetPaymentsRequest { PaymentId = transactionId }, CallOptions);
        return response.Payment?.Status?.ToString() switch
        {
            "PENDING" => PaymentStatus.Pending,
            "APPROVED" => PaymentStatus.Approved,
            "COMPLETED" => PaymentStatus.Completed,
            "CANCELED" => PaymentStatus.Canceled,
            "FAILED" => PaymentStatus.Failed,
            _ => PaymentStatus.Unknown
        };
    }
}
