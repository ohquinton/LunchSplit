using LunchSplit.Core.Models;
using LunchSplit.Core.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class ReceiptFormatterTests
{
    private readonly DateTime _fixedTime = new DateTime(2026, 10, 3, 14, 30, 5);
    private ReceiptFormatter _formatter = null!;
    private Bill _bill = null!;
    private List<Attendee> _attendees = null!;
    private List<Share> _shares = null!;

    [TestInitialize]
    public void Setup()
    {
        _formatter = new ReceiptFormatter("Quinton Jackson", _fixedTime);
        _bill = new Bill(100.00m, 13.00m, TipMode.Percent, 15m);
        _attendees = new List<Attendee> { new("Ana", Weight: 2), new("Ben"), new("Cal", Included: false) };
        _shares = new List<Share> { new("Ana", 85.33m), new("Ben", 42.67m), new("Cal", 0.00m) };
    }

    // M5-24
    [TestMethod]
    public void Format_DefaultBillReceipt_ContainsLineItems()
    {
        // Arrange
        Bill bill = _bill;

        // Act
        string receipt = _formatter.Format(bill, _attendees, _shares);

        // Assert
        StringAssert.Contains(receipt, "Subtotal: $100.00");
        StringAssert.Contains(receipt, "Tax: $13.00");
        StringAssert.Contains(receipt, "Tip: $15.00");
        StringAssert.Contains(receipt, "Total: $128.00");
        StringAssert.Contains(receipt, "Ana (x2): $85.33");
        StringAssert.Contains(receipt, "Ben (x1): $42.67");
        StringAssert.Contains(receipt, "Cal (excluded): $0.00");
    }

    // M5-25
    [TestMethod]
    public void Format_ReceiptOutput_ContainsRequiredMetadata()
    {
        // Arrange
        Bill bill = _bill;

        // Act
        string receipt = _formatter.Format(bill, _attendees, _shares);

        // Assert
        StringAssert.Contains(receipt, "Student: Quinton Jackson");
        StringAssert.Contains(receipt, "Date: 2026-10-03");
        StringAssert.Contains(receipt, "Created: 2026-10-03 14:30:05");
    }
}
