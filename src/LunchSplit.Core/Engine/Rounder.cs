using LunchSplit.Core.Models;

namespace LunchSplit.Core.Engine;

public class Rounder
{
    public List<Share> RoundShares(List<Share> rawShares, RoundingMode mode)
    {
        List<Share> result = new List<Share>();

        if (rawShares.Count == 0 || mode == RoundingMode.None)
        {
            result.AddRange(rawShares);
            return result;
        }

        decimal rawTotal = 0;
        decimal roundedTotal = 0;
        decimal[] amounts = new decimal[rawShares.Count];

        for (int i = 0; i < rawShares.Count; i++)
        {
            amounts[i] = RoundToCents(rawShares[i].Amount, mode);
            rawTotal += rawShares[i].Amount;
            roundedTotal += amounts[i];
        }

        // Round the total the same way, then hand out or take back the leftover pennies
        decimal targetTotal = RoundToCents(Math.Round(rawTotal, 10), mode);
        int pennies = (int)((targetTotal - roundedTotal) * 100);

        for (int i = 0; i < amounts.Length && pennies != 0; i++)
        {
            if (rawShares[i].Amount == 0)
                continue;

            if (pennies > 0)
            {
                amounts[i] += 0.01m;
                pennies--;
            }
            else
            {
                amounts[i] -= 0.01m;
                pennies++;
            }
        }

        for (int i = 0; i < rawShares.Count; i++)
        {
            result.Add(new Share(rawShares[i].Name, amounts[i]));
        }

        return result;
    }

    private decimal RoundToCents(decimal amount, RoundingMode mode)
    {
        if (mode == RoundingMode.Bankers)
            return Math.Round(amount, 2, MidpointRounding.ToEven);

        if (mode == RoundingMode.Up)
            return Math.Ceiling(amount * 100) / 100;

        if (mode == RoundingMode.Down)
            return Math.Floor(amount * 100) / 100;

        return amount;
    }
}
