
namespace Spike.Providers
{
    using Instrumentation.Monitoring;
    using Instrumentation.Monitoring.Monitors;

    public class AppTelemetry : AppTelemetryBase
    {
        public const string PaymentEventMonitorName = "Payments";
 
        public AppTelemetry() : base("Spike.Counters", "This is the main console spike") { }

        private static AppTelemetry _instance;

        public static AppTelemetry Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = new AppTelemetry();

                return _instance;
            }
        }

        private TwoStateMonitor PaymentMonitor { get; set; }

        protected override void RegisterMonitors()
        {
            PaymentMonitor = AddTwoStateMonitor(PaymentEventMonitorName, IntervalType.FiveMinutes);
        }
    }
}
