
namespace Spike.Providers
{
    using Instrumentation.Monitoring;
    using Instrumentation.Monitoring.Monitors;

    public class AppTelemetry : AppTelemetryBase
    {
        private const string PaymentEventCounterName = "Payments";
 
        public AppTelemetry() : base("Spike.Counters", "This is the main console spike") { }

        private static AppTelemetry _instance;
        public static AppTelemetry Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = new AppTelemetry();
                _instance.Initalize();

                return _instance;
            }
        }

        private void Initalize()
        {
            HeartbeatMonitor = AddHeartBeatMonitor();
            PaymentMonitor = AddTwoStateMonitor(PaymentEventCounterName);
            
            RegisterCounters();
        }

        public TwoStateMonitor PaymentMonitor { get; set; }

        public HeartbeatMonitor HeartbeatMonitor { get; set; }
    }
}
