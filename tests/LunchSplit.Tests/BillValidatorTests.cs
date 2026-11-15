using LunchSplit.Core.Helpers;
using LunchSplit.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class BillValidatorTests
{
    private BillValidator _validator = null!;
    private Bill _validBill = null!;
    private List<Attendee> _validAttendees = null!;

    [TestInitialize]
    public void Setup()
    {
        _validator = new BillValidator();
        _validBill = new Bill(100.00m, 13.00m, TipMode.Percent, 15m);
        _validAttendees = new List<Attendee> { new("Ana"), new("Ben") };
    }

    // M3-12
    [TestMethod]
    public void Validate_CompleteBillDetails_ReturnsOk()
    {
        // Arrange
        Bill bill = _validBill;

        // Act
        ValidationResult result = _validator.Validate(bill, _validAttendees);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    // M3-13
    [TestMethod]
    public void Validate_EmptyAttendeeCollection_ReturnsFail()
    {
        // Arrange
        var attendees = new List<Attendee>();

        // Act
        ValidationResult result = _validator.Validate(_validBill, attendees);

        // Assert
        Assert.IsFalse(result.IsValid);
        CollectionAssert.Contains(result.Errors, "Add at least one attendee to split the bill.");
    }

    // M3-14
    [TestMethod]
    public void Validate_NegativeSubtotal_ReturnsFail()
    {
        // Arrange
        Bill bill = _validBill with { Subtotal = -5.00m };

        // Act
        ValidationResult result = _validator.Validate(bill, _validAttendees);

        // Assert
        Assert.IsFalse(result.IsValid);
        CollectionAssert.Contains(result.Errors, "Subtotal cannot be negative.");
    }

    // M3-15
    [TestMethod]
    public void Validate_NegativeTax_ReturnsFail()
    {
        // Arrange
        Bill bill = _validBill with { Tax = -1.00m };

        // Act
        ValidationResult result = _validator.Validate(bill, _validAttendees);

        // Assert
        Assert.IsFalse(result.IsValid);
        CollectionAssert.Contains(result.Errors, "Tax cannot be negative.");
    }

    // M3-16: the list has people in it, but 0 of them are actually in the split.
    // (M3-13 already covers an empty list, so this tests a different case.)
    [TestMethod]
    public void Validate_ZeroAttendees_ReturnsFail()
    {
        // Arrange
        var attendees = new List<Attendee>
        {
            new("Ana", Included: false),
            new("Ben", Included: false)
        };

        // Act
        ValidationResult result = _validator.Validate(_validBill, attendees);

        // Assert
        Assert.IsFalse(result.IsValid);
        CollectionAssert.Contains(result.Errors, "At least one attendee must be included in the split.");
    }

    
    [TestMethod]
    public void Validate_NegativeWeight_ReturnsFail()
    {
        // Arrange
        var attendees = new List<Attendee> { new("Ana", Weight: -1), new("Ben") };

        // Act
        ValidationResult result = _validator.Validate(_validBill, attendees);

        // Assert
        Assert.IsFalse(result.IsValid);
        CollectionAssert.Contains(result.Errors, "Attendee weights cannot be negative.");
    }
}
