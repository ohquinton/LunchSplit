using LunchSplit.Core.Engine;

namespace LunchSplit.Core.Models;

public record Bill(decimal Subtotal, decimal Tax, TipMode TipMode, decimal TipInput)
{
    public decimal Tip => new Splitter().ComputeTip(Subtotal, TipMode, TipInput);

    public decimal GrandTotal => Subtotal + Tax + Tip;
}
