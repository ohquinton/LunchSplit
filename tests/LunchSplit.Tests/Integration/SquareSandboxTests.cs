using LunchSplit.Core.Payments;
using LunchSplit.Infrastructure.Payments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests.Integration;

/// <summary>
/// Live integration tests. Set SQUARE_SANDBOX_ACCESS_TOKEN and SQUARE_SANDBOX_LOCATION_ID
/// on your machine.
/// </summary>
[TestClass]
[TestCategory("SquareSandbox")]
public class SquareSandboxTests
{
    private static SquarePaymentService Gateway()
    {
        LoadLocalEnvironmentFile();
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_ACCESS_TOKEN")) ||
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_LOCATION_ID")))
            Assert.Inconclusive("Set Square Sandbox environment variables to execute live integration tests.");
        return SquarePaymentService.FromEnvironment();
    }

    private static void LoadLocalEnvironmentFile()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "LunchSplit.sln")))
            {
                string path = Path.Combine(directory.FullName, ".env");
                if (!File.Exists(path)) return;

                foreach (string line in File.ReadLines(path))
                {
                    string entry = line.Trim();
                    if (entry.Length == 0 || entry.StartsWith('#')) continue;

                    int separator = entry.IndexOf('=');
                    if (separator <= 0) continue;

                    string name = entry[..separator].Trim();
                    if (name != "SQUARE_SANDBOX_ACCESS_TOKEN" &&
                        name != "SQUARE_SANDBOX_LOCATION_ID") continue;

                    string value = entry[(separator + 1)..].Trim();
                    if (value.Length >= 2 &&
                        ((value.StartsWith('"') && value.EndsWith('"')) ||
                         (value.StartsWith('\'') && value.EndsWith('\''))))
                        value = value[1..^1];

                    if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name)) &&
                        !string.IsNullOrWhiteSpace(value))
                        Environment.SetEnvironmentVariable(name, value);
                }
                return;
            }
            directory = directory.Parent;
        }
    }

    private static PaymentRequest Request(string nonce = "cnon:card-nonce-ok", string currency = "CAD", string? key = null) => new()
    {
        Amount = 10.00m,
        CardNonce = nonce,
        Currency = currency,
        IdempotencyKey = key ?? Guid.NewGuid().ToString("N")
    };

    [TestMethod]
    public async Task M3_01_SuccessfulAuthorization_ReturnsPaymentId()
    {
        var result = await Gateway().ProcessPaymentAsync(Request());
        Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.TransactionId));
    }

    [TestMethod]
    public async Task M3_02_DeclinedNonce_ReturnsCardDeclined()
    {
        var result = await Gateway().ProcessPaymentAsync(Request(nonce: "cnon:card-nonce-declined"));
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("CARD_DECLINED", result.ErrorCode);
    }

    [TestMethod]
    public async Task M3_08_EuroAtCadLocation_ReturnsBadRequest()
    {

        var result = await Gateway().ProcessPaymentAsync(Request(currency: "EUR"));
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(400, result.HttpStatusCode);
    }

    [TestMethod]
    public async Task M3_09_IdenticalIdempotencyKey_ReturnsSamePaymentId()
    {
        var gateway = Gateway();
        var request = Request(key: Guid.NewGuid().ToString("N"));
        var first = await gateway.ProcessPaymentAsync(request);
        Assert.IsTrue(first.IsSuccess, first.ErrorMessage);
        var second = await gateway.ProcessPaymentAsync(request);
        Assert.IsTrue(second.IsSuccess, second.ErrorMessage);
        Assert.AreEqual(first.TransactionId, second.TransactionId);
    }

    [TestMethod]
    public async Task M3_10_GetPaymentStatus_ReturnsCompleted()
    {
        var gateway = Gateway();
        var created = await gateway.ProcessPaymentAsync(Request());
        Assert.IsTrue(created.IsSuccess, created.ErrorMessage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(created.TransactionId));
        var status = await gateway.GetTransactionStatusAsync(created.TransactionId!);
        Assert.AreEqual(PaymentStatus.Completed, status);
    }
}
