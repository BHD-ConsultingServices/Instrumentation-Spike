
namespace Spike.CounterSimulator
{
    using Providers.Providers;

    public class ProviderFactory
    {
        public static IPaymentProvider CreatePaymentProvider() => new PaymentProvider();
    }
}
