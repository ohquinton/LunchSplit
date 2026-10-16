using LunchSplit.Core.Engine;
using LunchSplit.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class RounderTests
{
    private Rounder _rounder = null!;

    [TestInitialize]
    public void Setup()
    {
        _rounder = new Rounder();
    }

    // M2-6
    [TestMethod]
    public void RoundShares_NoRounding_PreservesRawDecimals()
    {
        // Arrange
        var raw = new List<Share> { new("Ana", 3.33333m), new("Ben", 6.66667m) };

        // Act
        List<Share> result = _rounder.RoundShares(raw, RoundingMode.None);

        // Assert
        Assert.AreEqual(3.33333m, result[0].Amount);
        Assert.AreEqual(6.66667m, result[1].Amount);
    }

    // M2-7
    [TestMethod]
    public void RoundShares_BankersRounding_RoundsToNearestEven()
    {
        // Arrange
        var raw = new List<Share> { new("Ana", 10.325m), new("Ben", 10.335m) };

        // Act
        List<Share> result = _rounder.RoundShares(raw, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(10.32m, result[0].Amount);
        Assert.AreEqual(10.34m, result[1].Amount);
    }

    // M2-8
    [TestMethod]
    public void RoundShares_RoundUp_AppliesCeilingToCents()
    {
        // Arrange
        var raw = new List<Share> { new("Ana", 10.331m) };

        // Act
        List<Share> result = _rounder.RoundShares(raw, RoundingMode.Up);

        // Assert
        Assert.AreEqual(10.34m, result[0].Amount);
    }

    // M2-9
    [TestMethod]
    public void RoundShares_RoundDown_AppliesFloorToCents()
    {
        // Arrange
        var raw = new List<Share> { new("Ana", 10.339m) };

        // Act
        List<Share> result = _rounder.RoundShares(raw, RoundingMode.Down);

        // Assert
        Assert.AreEqual(10.33m, result[0].Amount);
    }

    // M2-10
    [TestMethod]
    public void RoundShares_UnevenSplit_ReconcilesRemainder()
    {
        // Arrange
        decimal third = 10.00m / 3;
        var raw = new List<Share> { new("Ana", third), new("Ben", third), new("Cal", third) };

        // Act
        List<Share> result = _rounder.RoundShares(raw, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(3.34m, result[0].Amount);
        Assert.AreEqual(3.33m, result[1].Amount);
        Assert.AreEqual(3.33m, result[2].Amount);
        Assert.AreEqual(10.00m, result.Sum(s => s.Amount));
    }

    // M2-11
    [TestMethod]
    public void RoundShares_EmptyShareCollection_ReturnsEmpty()
    {
        // Arrange
        var raw = new List<Share>();

        // Act
        List<Share> result = _rounder.RoundShares(raw, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(0, result.Count);
    }
}
