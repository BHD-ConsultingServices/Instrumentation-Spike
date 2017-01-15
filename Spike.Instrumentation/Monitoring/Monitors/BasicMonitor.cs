
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
                // Log error
            }
        }

        public void Reset()
        {
            try
            {
                this.Counters.Single(c => c.CounterName == BasicCounterData.CounterName).RawValue = 0;
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
                this.Counters.Single(c => c.CounterName == BasicCounterData.CounterName).IncrementBy(value);
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
                this.Counters.Single(c => c.CounterName == BasicCounterData.CounterName).IncrementBy(decriment);
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
                BasicCounterData
            };
        }
    }
}
