using LunchSplit.Core.Models;

namespace LunchSplit.Core.Future;

public interface IPaymentGateway
{
    bool RequestPayment(string name, decimal amount);
}

// F6 - not built yet, TDD-27 should fail until this is done
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
