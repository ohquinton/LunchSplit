using System.Text;
using LunchSplit.Core.Models;

namespace LunchSplit.Core.Output;

public class ReceiptFormatter
{
    private readonly string _studentName;
    private readonly DateTime _createdAt;

    public ReceiptFormatter() : this("Quinton Jackson", DateTime.Now)
    {
    }

    public ReceiptFormatter(string studentName, DateTime createdAt)
    {
        _studentName = studentName;
        _createdAt = createdAt;
    }

    public string Format(Bill bill, List<Attendee> attendees, List<Share> roundedShares)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("===== LunchSplit Receipt =====");
        sb.AppendLine("Student: " + _studentName);
        sb.AppendLine("Date: " + _createdAt.ToString("yyyy-MM-dd"));
        sb.AppendLine("Created: " + _createdAt.ToString("yyyy-MM-dd HH:mm:ss"));
        sb.AppendLine("------------------------------");
        sb.AppendLine("Subtotal: $" + bill.Subtotal.ToString("0.00"));
        sb.AppendLine("Tax: $" + bill.Tax.ToString("0.00"));
        sb.AppendLine("Tip: $" + bill.Tip.ToString("0.00"));
        sb.AppendLine("Total: $" + bill.GrandTotal.ToString("0.00"));
        sb.AppendLine("------------------------------");

        for (int i = 0; i < roundedShares.Count; i++)
        {
            string note = "excluded";

            if (attendees[i].Included)
                note = "x" + attendees[i].Weight;

            sb.AppendLine(roundedShares[i].Name + " (" + note + "): $" + roundedShares[i].Amount.ToString("0.00"));
        }

        sb.AppendLine("==============================");
        return sb.ToString();
    }
}
