namespace LunchSplit.Core.Models;

public class ValidationResult
{
    public List<string> Errors { get; } = new List<string>();

    public bool IsValid => Errors.Count == 0;
}
