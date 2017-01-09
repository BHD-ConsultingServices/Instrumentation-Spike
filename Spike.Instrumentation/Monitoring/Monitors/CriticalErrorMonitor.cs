
using System;
using System.Linq;

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

        public void Set(long value)
        {
            try
            {
                this.Counters.Single(c => c.CounterName == CriticalCounterData.CounterName).RawValue = value;
            }
            catch (Exception ex)
            {
                // Log error
            }
        }

        public void Increment(long value = 1)
        {
            try
            {
                this.Counters.Single(c => c.CounterName == CriticalCounterData.CounterName).IncrementBy(value);
            }
            catch (Exception ex)
            {
                // Log error
            }
        }

        public void Decrement(long value = 1)
        {
            try
            {
                var decriment = -1 * value;
                this.Counters.Single(c => c.CounterName == CriticalCounterData.CounterName).IncrementBy(decriment);
            }
            catch (Exception ex)
            {
                // Log error
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
