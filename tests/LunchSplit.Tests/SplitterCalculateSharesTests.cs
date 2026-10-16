using LunchSplit.Core.Engine;
using LunchSplit.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class SplitterCalculateSharesTests
{
    private Splitter _splitter = null!;

    [TestInitialize]
    public void Setup()
    {
        _splitter = new Splitter();
    }

    // M4-17
    [TestMethod]
    public void CalculateShares_EqualSplit_ApportionsEvenly()
    {
        // Arrange
        var bill = new Bill(90.00m, 0m, TipMode.None, 0m);
        var attendees = new List<Attendee> { new("Ana"), new("Ben"), new("Cal") };

        // Act
        List<Share> shares = _splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(30.00m, shares[0].Amount);
        Assert.AreEqual(30.00m, shares[1].Amount);
        Assert.AreEqual(30.00m, shares[2].Amount);
    }

    // M4-18
    [TestMethod]
    public void CalculateShares_ProportionalSplit_ApportionsWeighted()
    {
        // Arrange
        var bill = new Bill(90.00m, 0m, TipMode.None, 0m);
        var attendees = new List<Attendee> { new("Ana", Weight: 2), new("Ben", Weight: 1) };

        // Act
        List<Share> shares = _splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(60.00m, shares[0].Amount);
        Assert.AreEqual(30.00m, shares[1].Amount);
    }

    // M4-19
    [TestMethod]
    public void CalculateShares_ExcludedAttendee_ApportionsZero()
    {
        // Arrange
        var bill = new Bill(90.00m, 0m, TipMode.None, 0m);
        var attendees = new List<Attendee>
        {
            new("Ana"),
            new("Ben"),
            new("Cal", Included: false)
        };

        // Act
        List<Share> shares = _splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(45.00m, shares[0].Amount);
        Assert.AreEqual(45.00m, shares[1].Amount);
        Assert.AreEqual(0.00m, shares[2].Amount);
    }

    // M4-20
    [TestMethod]
    public void CalculateShares_TaxAndPercentTip_ApportionsTotal()
    {
        // Arrange
        var bill = new Bill(100.00m, 13.00m, TipMode.Percent, 15m);
        var attendees = new List<Attendee> { new("Ana"), new("Ben") };

        // Act
        List<Share> shares = _splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(128.00m, bill.GrandTotal);
        Assert.AreEqual(64.00m, shares[0].Amount);
        Assert.AreEqual(64.00m, shares[1].Amount);
        Assert.AreEqual(bill.GrandTotal, shares.Sum(s => s.Amount));
    }

    // M4-21
    [TestMethod]
    public void CalculateShares_TaxAndFixedTip_ApportionsTotal()
    {
        // Arrange
        var bill = new Bill(100.00m, 13.00m, TipMode.Fixed, 20.00m);
        var attendees = new List<Attendee> { new("Ana"), new("Ben") };

        // Act
        List<Share> shares = _splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(133.00m, bill.GrandTotal);
        Assert.AreEqual(66.50m, shares[0].Amount);
        Assert.AreEqual(66.50m, shares[1].Amount);
        Assert.AreEqual(bill.GrandTotal, shares.Sum(s => s.Amount));
    }

    // M4-22
    [TestMethod]
    public void CalculateShares_AllAttendeesExcluded_ThrowsException()
    {
        // Arrange
        var bill = new Bill(90.00m, 0m, TipMode.None, 0m);
        var attendees = new List<Attendee>
        {
            new("Ana", Included: false),
            new("Ben", Included: false)
        };

        // Act
        Action act = () => _splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        // Assert
        Assert.ThrowsException<InvalidOperationException>(act);
    }

    // M4-23
    [TestMethod]
    public void CalculateShares_TotalWeightsZero_ThrowsException()
    {
        // Arrange
        var bill = new Bill(90.00m, 0m, TipMode.None, 0m);
        var attendees = new List<Attendee> { new("Ana", Weight: 0), new("Ben", Weight: 0) };

        // Act
        Action act = () => _splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        // Assert
        Assert.ThrowsException<InvalidOperationException>(act);
    }
}
