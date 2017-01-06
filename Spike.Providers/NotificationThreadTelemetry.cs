
namespace Spike.Providers
{
    using Instrumentation.Monitoring;
    using Instrumentation.Monitoring.Monitors;

    public class NotificationThreadTelemetry : AppTelemetryBase
    {
        public const string NotificationQueueName = "NotificationQueueSize";
        
        public NotificationThreadTelemetry() 
            : base("Spike.Counters.Notification", "This is the health monitor for the notification thread")
        {
            HeartbeatMonitor = AddHeartBeatMonitor();
            NotificationQueueMonitor = AddBasicMonitor(NotificationQueueName);

            RegisterCounters();
            StartMonitoring();
        }

        public HeartbeatMonitor HeartbeatMonitor { get; set; }

        public BasicMonitor NotificationQueueMonitor { get; set; }
    }
}
