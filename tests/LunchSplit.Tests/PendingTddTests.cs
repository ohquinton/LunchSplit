using LunchSplit.Core.Engine;
using LunchSplit.Core.Future;
using LunchSplit.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

/// <summary>
/// Run everything else with: dotnet test --filter "TestCategory!=Pending"
/// </summary>
[TestClass]
public class PendingTddTests
{
    // TDD-26 (feature F5)
    [TestMethod]
    [TestCategory("Pending")]
    public void Format_CsvReceiptExporter_MatchesSchema()
    {
        // Arrange
        var exporter = new CsvReceiptExporter();
        var shares = new List<Share> { new("Ana", 30.00m), new("Ben", 60.00m) };
        string expected = "Name,Amount\nAna,30.00\nBen,60.00\n";

        // Act
        string csv = exporter.Export(shares);

        // Assert
        Assert.AreEqual(expected, csv);
    }

    // TDD-27 (feature F6)
    [TestMethod]
    [TestCategory("Pending")]
    public void CalculateShares_PaymentRequest_ValidatesStripeMock()
    {
        // Arrange
        var gateway = new FakeStripeGateway();
        var requester = new PaymentRequester(gateway);
        var bill = new Bill(90.00m, 0m, TipMode.None, 0m);
        var attendees = new List<Attendee> { new("Ana"), new("Ben"), new("Cal") };
        List<Share> shares = new Splitter().CalculateShares(bill, attendees, RoundingMode.Bankers);

        // Act
        int sent = requester.RequestAll(shares);

        // Assert
        Assert.AreEqual(3, sent);
        Assert.AreEqual(3, gateway.Requests.Count);
        Assert.AreEqual(90.00m, gateway.Requests.Sum(r => r.Amount));
    }

    /// <summary>Fake Stripe so tests never touch a real payment API.</summary>
    private class FakeStripeGateway : IPaymentGateway
    {
        public List<(string Name, decimal Amount)> Requests { get; } = new();

        public bool RequestPayment(string name, decimal amount)
        {
            Requests.Add((name, amount));
            return true;
        }
    }
}
