
namespace Spike.Instrumentation.Monitoring
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Principal;
    using Monitors;

    public class AppTelemetryBase
    {
        private readonly List<MonitorBase> _registeredMonitors = new List<MonitorBase>();

        public string CategoryDescription { get; }

        public string CategoryName { get;}
        
        private bool IsAdministrator 
        {
            get
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        protected AppTelemetryBase(string categoryName, string description)
        {
            CategoryName = categoryName;
            CategoryDescription = description;
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
            foreach (var monitor in _registeredMonitors)
            {
                monitor.IntializeMonitor();
            }
        }
    }
}
