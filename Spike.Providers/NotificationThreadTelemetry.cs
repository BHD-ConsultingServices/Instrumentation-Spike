
namespace Spike.Providers
{
    using Instrumentation.Monitoring;
    using Instrumentation.Monitoring.Monitors;

    public class NotificationThreadTelemetry : AppTelemetryBase
    {
        public const string NotificationQueueMonitorName = "NotificationQueueSize";
        
        public NotificationThreadTelemetry(bool createCounters) 
            : base("Spike.Counters.Notification", "This is the health monitor for the notification thread")
        {

            if (createCounters)
            {
                this.CreateCountersAllowed = true;
                this.RegisterCounters();
            }
        }

        private BasicMonitor NotificationQueueMonitor { get; set; }

        protected override void RegisterMonitors()
        {
            NotificationQueueMonitor = AddBasicMonitor(NotificationQueueMonitorName);
        }
    }
}
