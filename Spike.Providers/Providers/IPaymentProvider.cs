
namespace Spike.Providers.Providers
{
    public interface IPaymentProvider
    {
        bool MakePayment(decimal amount);
    }
}
