
namespace Spike.Instrumentation.Monitoring.Monitors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class BasicMonitor : MonitorBase
    {
        public BasicMonitor(string categoryName, string monitorName) 
            : base(categoryName, monitorName)
        {
        }
        
        private CounterCreationData _basicCounterData;

        private CounterCreationData BasicCounterData
        {
            get
            {
                if (_basicCounterData != null)
                {
                    return _basicCounterData;
                }

                return _basicCounterData = new CounterCreationData
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
                this.Counters.Single(c => c.CounterName == BasicCounterData.CounterName).RawValue = value;
            }
            catch (Exception ex)
            {
                // this.LogError("Error Increment counter '{0}' by '{1}' - {2}", counterName, value, ex.Message);
            }
        }

        public void Increment(long value = 1)
        {
            try
            {
                this.Counters.Single(c => c.CounterName == BasicCounterData.CounterName).IncrementBy(value);
            }
            catch (Exception ex)
            {
                // this.LogError("Error Increment counter '{0}' by '{1}' - {2}", counterName, value, ex.Message);
            }
        }

        protected override List<CounterCreationData> CounterDataToRegister()
        {
            return new List<CounterCreationData>
            {
                BasicCounterData
            };
        }
    }
}
