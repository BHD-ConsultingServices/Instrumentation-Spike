
namespace Spike.Instrumentation.Monitoring.Monitors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class TwoStateMonitor : MonitorBase
    {
        private const string SuccessCounterName = "Success";
        private const string FailureCounterName = "Failure";
        private const string SuccessInPeriodCounterName = "SuccessInPeriod";
        private const string FailureInPeriodCounterName = "FailureInPeriod";
        private const string LastSuccessCounterName = "LastSuccessPulse";
        private const string ConsecutiveFailuresCounterName = "ConsecutiveFailures";
        
        public TwoStateMonitor(string categoryName, string subCategoryName)
           : base(categoryName, subCategoryName) { }

        private CounterCreationData _successCounterData;
        private CounterCreationData _failureCounterData;
        private CounterCreationData _consecutivefailureCounterData;
        private CounterCreationData _lastSuccessCounterData;

        private CounterCreationData SuccessCounterData
        {
            get
            {
                if (_successCounterData != null)
                {
                    return _successCounterData;
                }

                var counterName = $"{SubCategoryName}.{SuccessCounterName}";

                return _successCounterData = new CounterCreationData
                {
                    CounterName = counterName,
                    CounterType = PerformanceCounterType.NumberOfItems64
                };
            }
        }

        private CounterCreationData FailureCounterData
        {
            get
            {
                if (_failureCounterData != null)
                {
                    return _failureCounterData;
                }

                var counterName = $"{SubCategoryName}.{FailureCounterName}";

                return _failureCounterData = new CounterCreationData
                {
                    CounterName = counterName,
                    CounterType = PerformanceCounterType.NumberOfItems64
                };
            }
        }
        
        private CounterCreationData ConsecutiveFailureCounterData
        {
            get
            {
                if (_consecutivefailureCounterData != null)
                {
                    return _consecutivefailureCounterData;
                }

                var counterName = $"{SubCategoryName}.{ConsecutiveFailuresCounterName}";
                
                return _consecutivefailureCounterData = new CounterCreationData
                {
                    CounterName = counterName,
                    CounterType = PerformanceCounterType.NumberOfItems64
                };
            }
        }

        private CounterCreationData LastSuccessCounterData
        {
            get
            {
                if (_lastSuccessCounterData != null)
                {
                    return _lastSuccessCounterData;
                }

                var counterName = $"{SubCategoryName}.{LastSuccessCounterName}";

                return _lastSuccessCounterData = new CounterCreationData
                {
                    CounterName = counterName,
                    CounterType = PerformanceCounterType.NumberOfItems32
                };
            }
        }

        private void ResetCounter(string counterName)
        {
            try
            {
                this.Counters.Single(c => c.CounterName == counterName).RawValue = 0;
            }
            catch (Exception ex)
            {
                // Add Loging here
            }

        }

        private void IncrementCounter(string counterName, long value)
        {
            try
            {
                this.Counters.Single(c => c.CounterName == counterName).IncrementBy(value);
            }
            catch (Exception ex)
            {
                // Add Loging here
            }
        }

        private void SetPulseCounter(string counterName)
        {
            try
            {
                var counter = this.Counters.Single(c => c.CounterName == counterName);
                var toggel = PulseMonitor.PulseToggel(counter.RawValue);

                counter.RawValue = toggel;
            }
            catch (Exception ex)
            {
                // Add Loging here
            }
        }

        public void Success(int incrementBy = 1)
        {
            Console.WriteLine("Success {0}", this.CategoryName);

            this.IncrementCounter(SuccessCounterData.CounterName, incrementBy);
            this.ResetCounter(ConsecutiveFailureCounterData.CounterName);
            this.SetPulseCounter(LastSuccessCounterData.CounterName);
        }

        public void Failure(int incrementBy = 1)
        {
            Console.WriteLine("Failure {0}", this.CategoryName);

            this.IncrementCounter(FailureCounterData.CounterName, incrementBy);
            this.IncrementCounter(ConsecutiveFailureCounterData.CounterName, incrementBy);
        }

        protected override List<CounterCreationData> CounterDataToRegister()
        {
            return new List<CounterCreationData>
            {
                SuccessCounterData,
                FailureCounterData,
                ConsecutiveFailureCounterData,
                LastSuccessCounterData
            };
        }
    }
}
