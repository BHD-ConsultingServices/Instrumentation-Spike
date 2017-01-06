
namespace Spike.CounterSimulator
{
    using System.Threading;
    using Providers;

    public class NotificationThread
    {
        public bool  IsActive;
        private NotificationThreadTelemetry _telemetry;

        public void IncrementCounter()
        {
            _telemetry.NotificationQueueMonitor.Increment(5);
        }

        public void IntitializeThread()
        {
            IsActive = true;
            _telemetry = new NotificationThreadTelemetry();

            while (IsActive)
            {
                Thread.Sleep(100);
            }

            _telemetry = null;
        }

        public void Exit()
        {
            IsActive = false;
        }
    }
}
