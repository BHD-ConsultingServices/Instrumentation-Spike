
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
            
            PaymentMonitor = AddTwoStateMonitor(PaymentEventCounterName, IntervalType.FiveMinutes);
            
            RegisterCounters();
        }

        public TwoStateMonitor PaymentMonitor { get; set; }
    }
}
