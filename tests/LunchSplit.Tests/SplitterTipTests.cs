using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core.Engine;
using LunchSplit.Core.Models;

namespace LunchSplit.Tests;

[TestClass]
public class SplitterTipTests
{
    private Splitter _splitter = null!;

    [TestInitialize]
    public void Setup()
    {
        _splitter = new Splitter();
    }

    // M1-1
    [TestMethod]
    public void ComputeTip_NoTipMode_ReturnsZero()
    {
        // Arrange
        decimal subtotal = 120.00m;

        // Act
        decimal tip = _splitter.ComputeTip(subtotal, TipMode.None, 15m);

        // Assert
        Assert.AreEqual(0.00m, tip);
    }

    // M1-2
    [TestMethod]
    public void ComputeTip_PercentTipMode_ReturnsCorrectPercent()
    {
        // Arrange
        decimal subtotal = 120.00m;
        decimal percent = 15m;

        // Act
        decimal tip = _splitter.ComputeTip(subtotal, TipMode.Percent, percent);

        // Assert
        Assert.AreEqual(18.00m, tip);
    }

    // M1-3
    [TestMethod]
    public void ComputeTip_FixedTipMode_ReturnsFixedAmount()
    {
        // Arrange
        decimal subtotal = 100.00m;
        decimal fixedTip = 15.00m;

        // Act
        decimal tip = _splitter.ComputeTip(subtotal, TipMode.Fixed, fixedTip);

        // Assert
        Assert.AreEqual(15.00m, tip);
    }

    // M1-4
    [TestMethod]
    public void ComputeTip_NegativeSubtotal_ThrowsException()
    {
        // Arrange
        decimal subtotal = -10.00m;

        // Act
        Action act = () => _splitter.ComputeTip(subtotal, TipMode.Percent, 15m);

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(act);
    }

    // M1-5
    [TestMethod]
    public void ComputeTip_NegativeFixedTip_ThrowsException()
    {
        // Arrange
        decimal fixedTip = -5.00m;

        // Act
        Action act = () => _splitter.ComputeTip(100.00m, TipMode.Fixed, fixedTip);

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(act);
    }
}
