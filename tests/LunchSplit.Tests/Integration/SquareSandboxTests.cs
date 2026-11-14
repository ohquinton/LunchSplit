using LunchSplit.Core.Payments;
using LunchSplit.Infrastructure.Payments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests.Integration;

/// <summary>
/// Live integration tests. Set SQUARE_SANDBOX_ACCESS_TOKEN and SQUARE_SANDBOX_LOCATION_ID
/// on your machine; missing secrets mark the tests inconclusive rather than making live requests.
/// These tests intentionally never substitute Square with mocks.
/// </summary>
[TestClass]
[TestCategory("SquareSandbox")]
public class SquareSandboxTests
{
    private static SquarePaymentService Gateway()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_ACCESS_TOKEN")) ||
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_LOCATION_ID")))
            Assert.Inconclusive("Set Square Sandbox environment variables to execute live integration tests.");
        return SquarePaymentService.FromEnvironment();
    }

    private static PaymentRequest Request(string nonce = "cnon:card-nonce-ok", string currency = "CAD", string? key = null) => new()
    {
        Amount = 10.00m, CardNonce = nonce, Currency = currency,
        IdempotencyKey = key ?? Guid.NewGuid().ToString("N")
    };

    [TestMethod]
    public async Task M3_01_SuccessfulAuthorization_ReturnsPaymentId()
    {
        var result = await Gateway().ProcessPaymentAsync(Request());
        Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.TransactionId));
    }

}
