using LunchSplit.Core.Payments;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LunchSplit.Tests.Unit;

[TestClass]
public class PaymentProcessorMoqTests
{
    private static PaymentRequest Request(decimal amount = 25.50m) => new()
    {
        Amount = amount, Currency = "CAD", CardNonce = "mock-nonce", IdempotencyKey = Guid.NewGuid().ToString()
    };

    [TestMethod]
    public async Task M3_03_GatewayNetworkFailure_ThrowsDomainException()
    {
        // Arrange: remote socket failures must remain testable without a live network.
        var mock = new Mock<IPaymentProcessor>();
        mock.Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ThrowsAsync(new HttpRequestException("503 Service Unavailable"));
        var coordinator = new PaymentCoordinator(mock.Object);
        // Act / Assert
        await Assert.ThrowsExceptionAsync<PaymentGatewayTimeoutException>(() => coordinator.PayShareAsync(Request()));
    }

    [TestMethod]
    public async Task M3_04_Payment_PassesExactAmountAndInvokesOnce()
    {
        var mock = new Mock<IPaymentProcessor>();
        mock.Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(new PaymentResult { IsSuccess = true, TransactionId = "tx_123" });
        var coordinator = new PaymentCoordinator(mock.Object);
        var result = await coordinator.PayShareAsync(Request(25.50m));
        Assert.IsTrue(result.IsSuccess);
        mock.Verify(x => x.ProcessPaymentAsync(It.Is<PaymentRequest>(r => r.Amount == 25.50m)), Times.Once());
    }

    [TestMethod]
    public async Task M3_05_ZeroBill_DoesNotCallGateway()
    {
        var mock = new Mock<IPaymentProcessor>();
        var result = await new PaymentCoordinator(mock.Object).PayShareAsync(Request(0m));
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("INVALID_AMOUNT", result.ErrorCode);
        mock.Verify(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>()), Times.Never());
    }

    [TestMethod]
    public async Task M3_06_FourAttendees_FourCalls()
    {
        var mock = new Mock<IPaymentProcessor>();
        mock.Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(new PaymentResult { IsSuccess = true, TransactionId = "tx" });
        var requests = Enumerable.Range(0, 4).Select(_ => Request(12.50m)).ToList();
        var result = await new PaymentCoordinator(mock.Object).PayBatchAsync(requests);
        Assert.AreEqual(4, result.SuccessCount);
        mock.Verify(x => x.ProcessPaymentAsync(It.IsAny<PaymentRequest>()), Times.Exactly(4));
    }

}
