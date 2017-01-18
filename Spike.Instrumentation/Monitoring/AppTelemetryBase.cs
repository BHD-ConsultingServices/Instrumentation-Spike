
namespace Spike.Instrumentation.Monitoring
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Principal;
    using Monitors;

    public abstract class AppTelemetryBase
    {
        private readonly List<MonitorBase> _registeredMonitors = new List<MonitorBase>();

        private bool _isInitialized = false;

        public HeartbeatMonitor HeartbeatMonitor { get; set; }

        public string CategoryDescription { get; }

        public string CategoryName { get;}


        protected abstract void RegisterMonitors();

        protected AppTelemetryBase(string categoryName, string description, bool createHeartBeat = true)
        {
            CategoryName = categoryName;
            CategoryDescription = description;

            if (createHeartBeat)
            {
                HeartbeatMonitor = AddHeartBeatMonitor();
            }

            StartMonitoring();
        }

        private bool IsAdministrator 
        {
            get
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        protected TwoStateMonitor AddTwoStateMonitor(string monitorName, IntervalType averageInterval)
        {
            var monitor = new TwoStateMonitor(CategoryName, monitorName, averageInterval);
            _registeredMonitors.Add(monitor);

            return monitor;
        }
        
        protected HeartbeatMonitor AddHeartBeatMonitor()
        {
            var monitor = new HeartbeatMonitor(CategoryName);
            _registeredMonitors.Add(monitor);

            return monitor;
        }

        protected BasicMonitor AddBasicMonitor(string monitorName)
        {
            var monitor = new BasicMonitor(CategoryName, monitorName);
            _registeredMonitors.Add(monitor);

            return monitor;
        }

        protected PulseMonitor AddPulseMonitor(string monitorName)
        {
            var monitor = new PulseMonitor(CategoryName, monitorName);
            _registeredMonitors.Add(monitor);

            return monitor;
        }

        protected void RegisterCounters()
        {
            if (IsAdministrator)
            {
                if (PerformanceCounterCategory.Exists(CategoryName))
                {
                   PerformanceCounterCategory.Delete(CategoryName);
                }
            }

            if (PerformanceCounterCategory.Exists(CategoryName))
            {
                return;
            }
            
            var counterData = new List<CounterCreationData>();
            foreach (var monitor in _registeredMonitors)
            {
                counterData.AddRange(monitor.CounterData);
            }

            var dataCollection = new CounterCreationDataCollection(counterData.ToArray());

            PerformanceCounterCategory.Create(CategoryName, CategoryDescription, PerformanceCounterCategoryType.SingleInstance, dataCollection);
        }

        public void StartMonitoring()
        {
            if (_isInitialized) return;

            RegisterMonitors();
            RegisterCounters();

            foreach (var monitor in _registeredMonitors)
            {
                monitor.IntializeMonitor();
            }

            _isInitialized = true;
        }
    }
}
