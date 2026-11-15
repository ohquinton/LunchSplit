using LunchSplit.Core.Models;

namespace LunchSplit.Core.Future;

public interface IPaymentGateway
{
    bool RequestPayment(string name, decimal amount);
}

public class PaymentRequester
{
    private readonly IPaymentGateway _gateway;

    public PaymentRequester(IPaymentGateway gateway)
    {
        _gateway = gateway;
    }

    public int RequestAll(List<Share> shares)
    {
        throw new NotImplementedException();
    }
}
