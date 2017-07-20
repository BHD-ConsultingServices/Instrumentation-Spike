
namespace Spike.Providers.Providers
{
    public class PaymentProvider : IPaymentProvider
    {
        public bool MakePayment(decimal amount)
        {
            AppTelemetry.Instance.TwoStateMonitorAttempt(AppTelemetry.PaymentEventMonitorName);

            if (amount < 0)
            {
                AppTelemetry.Instance.TwoStateMonitorFailure(AppTelemetry.PaymentEventMonitorName);
                return false;
            }

            AppTelemetry.Instance.TwoStateMonitorSuccess(AppTelemetry.PaymentEventMonitorName);
            return true;
        }
    }
}
