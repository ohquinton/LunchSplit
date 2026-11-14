namespace LunchSplit.Core.Payments;

/// <summary>Enforces payment preconditions and delegates remote operations via dependency injection.</summary>
public sealed class PaymentCoordinator
{
    private readonly IPaymentProcessor _processor;
    public PaymentCoordinator(IPaymentProcessor processor) =>
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));

    public async Task<PaymentResult> PayShareAsync(PaymentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        // Invalid amounts never generate a remote request (M3-05).
        if (request.Amount <= 0m)
            return new PaymentResult { IsSuccess = false, ErrorCode = "INVALID_AMOUNT", ErrorMessage = "Amount must be greater than zero." };
        if (decimal.Round(request.Amount, 2) != request.Amount)
            return new PaymentResult { IsSuccess = false, ErrorCode = "INVALID_AMOUNT", ErrorMessage = "Amount must have at most two decimal places." };
        try
        {
            return await _processor.ProcessPaymentAsync(request);
        }
        catch (HttpRequestException ex)
        {
            throw new PaymentGatewayTimeoutException("Payment gateway network failure.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new PaymentGatewayTimeoutException("Payment gateway request timed out.", ex);
        }
    }

    /// <summary>Attempts all attendees in order; failure of one does not undo prior successful payments.</summary>
    public async Task<BatchPaymentResult> PayBatchAsync(IEnumerable<PaymentRequest> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);
        var batch = new BatchPaymentResult();
        foreach (var request in requests)
        {
            try { batch.Payments.Add(await PayShareAsync(request)); }
            catch (PaymentGatewayTimeoutException ex)
            {
                batch.Payments.Add(new PaymentResult { IsSuccess = false, ErrorCode = "GATEWAY_UNAVAILABLE", ErrorMessage = ex.Message });
            }
        }
        return batch;
    }
}
