
namespace Spike.Providers.Providers
{
    public class PaymentProvider : IPaymentProvider
    {
        public bool MakePayment(decimal amount)
        {
            if (amount < 0)
            {
                AppTelemetry.Instance.PaymentMonitor.Failure();
                return false;
            }

            AppTelemetry.Instance.PaymentMonitor.Success();
            return true;
        }
    }
}
