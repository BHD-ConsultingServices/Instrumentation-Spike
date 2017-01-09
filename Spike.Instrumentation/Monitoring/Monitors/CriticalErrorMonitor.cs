
namespace Spike.Instrumentation.Monitoring.Monitors
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public class CriticalErrorMonitor : MonitorBase
    {
        public CriticalErrorMonitor(string categoryName, string monitorName) : base(categoryName)
        {
            SubCategoryName = monitorName.StartsWith("Critical") ? monitorName : $"Critical{categoryName}";
        }

        private CounterCreationData _criticalCounterData;

        private CounterCreationData CriticalCounterData
        {
            get
            {
                if (_criticalCounterData != null)
                {
                    return _criticalCounterData;
                }

                return _criticalCounterData = new CounterCreationData
                {
                    CounterName = this.SubCategoryName,
                    CounterType = PerformanceCounterType.NumberOfItems64
                };
            }
        }

        protected override List<CounterCreationData> CounterDataToRegister()
        {
            return new List<CounterCreationData>
            {
                CriticalCounterData
            };
        }
    }
}
