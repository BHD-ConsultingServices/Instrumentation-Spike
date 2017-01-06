
namespace Spike.Instrumentation.Monitoring.Monitors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class PulseMonitor : MonitorBase
    {
        public const int PulseValue = 10;
        public const int ChangeLockInSeconds = 2;

        public PulseMonitor(string categoryName, string monitorName = null) : base(categoryName, monitorName)
        {
            _lastActivity = DateTime.Now.AddSeconds(-1 * (ChangeLockInSeconds + 1));
        }

        private CounterCreationData _pulseCounterData;
        private DateTime _lastActivity;

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

        private bool IsLockedForChange
        {
            get
            {
                var lockedFromTimstamp = DateTime.Now.AddSeconds(-1 * ChangeLockInSeconds);

                return _lastActivity > lockedFromTimstamp;
            }
        }

        public void Pulse()
        {
            try
            {
                if (IsLockedForChange)
                {
                    return;
                }

                var counter = this.Counters.Single(c => c.CounterName == PulseCounterData.CounterName);
                var toggel = PulseMonitor.PulseToggel(counter.RawValue);
                
                counter.RawValue = toggel;
                _lastActivity = DateTime.Now;

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
