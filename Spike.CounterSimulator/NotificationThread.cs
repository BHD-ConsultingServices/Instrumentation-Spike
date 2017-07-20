
namespace Spike.CounterSimulator
{
    using System.Threading;
    using Providers;

    public class NotificationThread
    {
        public bool  IsActive;
        private NotificationThreadTelemetry _telemetry;
        private bool CreateCounters { get; }

        public NotificationThread(bool createCounters)
        {
            CreateCounters = createCounters;
        }

        public void IncrementCounter()
        {
            _telemetry.BasicMonitorInc(NotificationThreadTelemetry.NotificationQueueMonitorName, 5);
        }

        public void IntitializeThread()
        {
            IsActive = true;
            _telemetry = new NotificationThreadTelemetry(CreateCounters);

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
