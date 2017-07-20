// <copyright file="AppTelemetryBase.cs" company="PayM8">
//     Copyright ©  2015
// </copyright>

namespace Paym8.Monitoring
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Principal;
    using Monitors;
    using Logging;
    using System.Linq;

    /// <summary>
    /// The telemetry base class for applications
    /// </summary>
    public abstract partial class AppTelemetryBase
    {
        public bool CreateCountersAllowed { get; set; }
        public ILogging Logger = LogFactory.Create("Paym8.Monitoring.AppTelemetryBase");

        /// <summary>
        /// The registered monitors
        /// </summary>
        private readonly List<MonitorBase> _registeredMonitors = new List<MonitorBase>();

        /// <summary>
        /// The has been initialized.
        /// </summary>
        private bool _hasBeenInitialized;

        /// <summary>
        /// Gets the category description.
        /// </summary>
        /// <value>
        /// The category description.
        /// </value>
        public string CategoryDescription { get; }

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        /// <value>
        /// The name of the category.
        /// </value>
        public string CategoryName { get;}

        /// <summary>
        /// Gets a value indicating whether this instance is administrator.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is administrator; otherwise, <c>false</c>.
        /// </value>
        private bool IsAdministrator 
        {
            get
            {
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppTelemetryBase" /> class.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="description">The description.</param>
        /// <param name="registerHeartbeat">if set to <c>true</c> [register heartbeat].</param>
        protected AppTelemetryBase(string categoryName, string description, bool registerHeartbeat = true)
        {
            CategoryName = categoryName;
            CategoryDescription = description;

            if (registerHeartbeat)
            {
                AddHeartBeatMonitor();
            }
        }

        /// <summary>
        /// Registers the monitors.
        /// </summary>
        protected abstract void RegisterMonitors();


        /// <summary>
        /// Gets the type of the monitor by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <returns>A types specific monitor</returns>
        private T GetMonitorByType<T>(string monitorName)
            where T : MonitorBase
        {
            var monitor = GetMonitor(monitorName);

            if (monitor == null)
            {
                Logger.Warn($"Monitor [{monitorName}] is not a registered monitor in [{CategoryName}]");
                return null;
            }


            if (monitor.GetType() == typeof(T))
            {
                return monitor as T;
            }

            Logger.Warn($"Monitor [{monitorName}] is not a [{typeof(T)}] monitor type");

            return null;
        }

        /// <summary>
        /// Gets the monitor.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <returns>The monitor requested (Type indipendant)</returns>
        private MonitorBase GetMonitor(string monitorName)
        {
            return _registeredMonitors?.FirstOrDefault(mon => mon.MonitorName == monitorName);
        }
        
        /// <summary>
        /// Adds the two state monitor.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <param name="averageInterval">The average interval.</param>
        /// <returns>The Two State Instance</returns>
        protected TwoStateMonitor AddTwoStateMonitor(string monitorName, IntervalType averageInterval)
        {
            var monitor = new TwoStateMonitor(CategoryName, monitorName, averageInterval);
            _registeredMonitors.Add(monitor);

            return monitor;
        }

        /// <summary>
        /// Adds the heart beat monitor.
        /// </summary>
        /// <returns>The Heartbeat instance</returns>
        protected HeartbeatMonitor AddHeartBeatMonitor()
        {
            var monitor = new HeartbeatMonitor(CategoryName);
            _registeredMonitors.Add(monitor);

            return monitor;
        }

        /// <summary>
        /// Adds the basic monitor.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <returns>The basic Monitor instance</returns>
        protected BasicMonitor AddBasicMonitor(string monitorName)
        {
            var monitor = new BasicMonitor(CategoryName, monitorName);
            _registeredMonitors.Add(monitor);

            return monitor;
        }

        /// <summary>
        /// Adds the pulse monitor.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <returns>Pulse monitor instance</returns>
        protected PulseMonitor AddPulseMonitor(string monitorName)
        {
            var monitor = new PulseMonitor(CategoryName, monitorName);
            _registeredMonitors.Add(monitor);

            return monitor;
        }

        /// <summary>
        /// Adds the critical monitor.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <returns></returns>
        protected CriticalErrorMonitor AddCriticalErrorMonitor(string monitorName)
        {
            var monitor = new CriticalErrorMonitor(CategoryName, monitorName);
            _registeredMonitors.Add(monitor);

            return monitor;
        }
        
        /// <summary>
        /// Registers the counters.
        /// </summary>
        public void RegisterCounters()
        {
            Logger.Info($"Checking if counters category [{CategoryName}] should be created = [{CreateCountersAllowed && IsAdministrator}]. Required >> CreateCounters [{CreateCountersAllowed}] IsAdministrator [{IsAdministrator}] Exists [{PerformanceCounterCategory.Exists(CategoryName)}]");

            if (CreateCountersAllowed && IsAdministrator)
            {
                Logger.Info($"Counters >> Beginning to removing existing category [{CategoryName}]");
                if (PerformanceCounterCategory.Exists(CategoryName))
                {
                   PerformanceCounterCategory.Delete(CategoryName);
                }
            }

            if (!CreateCountersAllowed || PerformanceCounterCategory.Exists(CategoryName)) return;

            Logger.Info($"Counters >> Beginning creation of a new category [{CategoryName}]");

            var counterData = new List<CounterCreationData>();
            foreach (var monitor in _registeredMonitors)
            {
                counterData.AddRange(monitor.CounterData);
            }

            var dataCollection = new CounterCreationDataCollection(counterData.ToArray());

            PerformanceCounterCategory.Create(CategoryName, CategoryDescription, PerformanceCounterCategoryType.SingleInstance, dataCollection);

            Logger.Info($"Counters >> Completed creating category [{CategoryName}]");
        }

        /// <summary>
        /// Starts the monitoring.
        /// </summary>
        public void StartMonitoring()
        {
            if(_hasBeenInitialized)
            {
                return;
            }

            RegisterMonitors();
            RegisterCounters();

            foreach (var monitor in _registeredMonitors)
            {
                monitor.IntializeMonitor();
            }
            _hasBeenInitialized = true;
        }
    }
}
