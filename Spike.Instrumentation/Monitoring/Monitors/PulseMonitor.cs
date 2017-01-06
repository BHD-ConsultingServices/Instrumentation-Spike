
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Spike.Instrumentation.Monitoring.Monitors
{
    public class PulseMonitor : MonitorBase
    {
        public const int PulseValue = 10;

        public PulseMonitor(string categoryName, string monitorName = null) : base(categoryName, monitorName)
        {
        }

        private CounterCreationData _pulseCounterData;

        private CounterCreationData PulseCounterData
        {
            get
            {
                if (_pulseCounterData != null)
                {
                    return _pulseCounterData;
                }

                return _pulseCounterData = new CounterCreationData
                {
                    CounterName = this.SubCategoryName,
                    CounterType = PerformanceCounterType.NumberOfItems32
                };
            }
        }

        public void Pulse()
        {
            try
            {
                var counter = this.Counters.Single(c => c.CounterName == PulseCounterData.CounterName);
                var toggel = PulseMonitor.PulseToggel(counter.RawValue);

                counter.RawValue = toggel;
            }
            catch (Exception ex)
            {
                 // Add Loging here
            }
        }

        public static long PulseToggel(long currentValue)
        {
            if (currentValue == PulseValue)
            {
                return 0;
            }

            return PulseValue;
        }


        protected override List<CounterCreationData> CounterDataToRegister()
        {
            return new List<CounterCreationData>
            {
                PulseCounterData
            };
        }
    }
}
