namespace LunchSplit.Core.Models;

public record Attendee(string Name, int Weight = 1, bool Included = true);
