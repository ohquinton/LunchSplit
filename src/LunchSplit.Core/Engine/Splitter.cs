using LunchSplit.Core.Models;

namespace LunchSplit.Core.Engine;

public class Splitter
{
    private readonly Rounder _rounder = new Rounder();

    public decimal ComputeTip(decimal subtotal, TipMode mode, decimal tipInput)
    {
        if (subtotal < 0)
            throw new ArgumentOutOfRangeException(nameof(subtotal), "Subtotal cannot be negative.");

        if (mode != TipMode.None && tipInput < 0)
            throw new ArgumentOutOfRangeException(nameof(tipInput), "Tip cannot be negative.");

        if (mode == TipMode.Percent)
            return subtotal * tipInput / 100;

        if (mode == TipMode.Fixed)
            return tipInput;

        return 0;
    }

    public List<Share> CalculateShares(Bill bill, List<Attendee> attendees, RoundingMode roundingMode)
    {
        int includedCount = 0;
        int totalWeight = 0;

        foreach (Attendee attendee in attendees)
        {
            if (attendee.Included)
            {
                includedCount++;
                totalWeight += attendee.Weight;
            }
        }

        if (includedCount == 0)
            throw new InvalidOperationException("At least one attendee must be included in the split.");

        if (totalWeight <= 0)
            throw new InvalidOperationException("Total weight must be greater than zero.");

        decimal grandTotal = bill.GrandTotal;
        List<Share> rawShares = new List<Share>();

        foreach (Attendee attendee in attendees)
        {
            decimal amount = 0;

            if (attendee.Included)
                amount = grandTotal * attendee.Weight / totalWeight;

            rawShares.Add(new Share(attendee.Name, amount));
        }

        return _rounder.RoundShares(rawShares, roundingMode);
    }
}
