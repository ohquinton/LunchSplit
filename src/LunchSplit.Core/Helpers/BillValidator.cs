using LunchSplit.Core.Models;

namespace LunchSplit.Core.Helpers;

public class BillValidator
{
    public ValidationResult Validate(Bill bill, List<Attendee> attendees)
    {
        ValidationResult result = new ValidationResult();

        if (bill.Subtotal < 0)
            result.Errors.Add("Subtotal cannot be negative.");

        if (bill.Tax < 0)
            result.Errors.Add("Tax cannot be negative.");

        if (bill.TipMode != TipMode.None && bill.TipInput < 0)
            result.Errors.Add("Tip cannot be negative.");

        if (attendees.Count == 0)
        {
            result.Errors.Add("Add at least one attendee to split the bill.");
            return result;
        }

        bool anyIncluded = false;
        bool anyNegativeWeight = false;

        foreach (Attendee attendee in attendees)
        {
            if (attendee.Included)
                anyIncluded = true;

            if (attendee.Weight < 0)
                anyNegativeWeight = true;
        }

        if (!anyIncluded)
            result.Errors.Add("At least one attendee must be included in the split.");

        if (anyNegativeWeight)
            result.Errors.Add("Attendee weights cannot be negative.");

        return result;
    }
}
