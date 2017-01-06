
namespace Spike.Instrumentation.Monitoring.Monitors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class TwoStateMonitor : MonitorBase
    {
        private const int AverageIntervalInSeconds = 1 * 60;

        private const string SuccessCounterName = "Success";
        private const string FailureCounterName = "Failure";
        private const string AverageSuccessCounterName = "SuccessAverage";
        private const string AverageFailureCounterName = "FailureAverage";
        private const string LastSuccessCounterName = "LastSuccessPulse";
        private const string ConsecutiveFailuresCounterName = "ConsecutiveFailures";
        
        public TwoStateMonitor(string categoryName, string subCategoryName)
           : base(categoryName, subCategoryName) { }

        private CounterCreationData _successCounterData;
        private CounterCreationData _failureCounterData;
        private CounterCreationData _averageSuccessesCounterData;
        private CounterCreationData _averageFailureCounterData;
        private CounterCreationData _consecutivefailureCounterData;
        private CounterCreationData _lastSuccessCounterData;

        private int _numberOfSuccesses;
        private int _numberOfFailures;

        private TimerHelper _timerHelper;
        private DateTime _lastSuccessActivity;

        private bool IsLockedForChange
        {
            get
            {
                var lockedFromTimstamp = DateTime.Now.AddSeconds(-1 * PulseMonitor.ChangeLockInSeconds);

                return _lastSuccessActivity > lockedFromTimstamp;
            }
        }

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

        private CounterCreationData AverageSuccessesCounterData
        {
            get
            {
                if (_averageSuccessesCounterData != null)
                {
                    return _averageSuccessesCounterData;
                }

                var counterName = $"{SubCategoryName}.{AverageSuccessCounterName}";

                return _averageSuccessesCounterData = new CounterCreationData
                {
                    CounterName = counterName,
                    CounterType = PerformanceCounterType.NumberOfItems32
                };
            }
        }

        private CounterCreationData AverageFailuresCounterData
        {
            get
            {
                if (_averageFailureCounterData != null)
                {
                    return _averageFailureCounterData;
                }

                var counterName = $"{SubCategoryName}.{AverageFailureCounterName}";

                return _averageFailureCounterData = new CounterCreationData
                {
                    CounterName = counterName,
                    CounterType = PerformanceCounterType.NumberOfItems32
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

        private void SetCounter(string counterName, long value)
        {
            try
            {
                this.Counters.Single(c => c.CounterName == counterName).RawValue = value;
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

        private int GetAverageSuccessForPeriod()
        {
            // TODO: Average = Sum of items in queue

            return _numberOfSuccesses;
        }

        private int GetAverageFailureForPeriod()
        {
            // TODO: Average = Sum of items in queue

            return _numberOfFailures;
        }

        private void OnAverageTic(object state)
        {
            var instance = (TwoStateMonitor) state;

            /* TODO: Read average from sum of items in queue (Success & Failure)
             * 1. Push (number of success/failures since last tic) to int queue
             * 2. If int queue length >= (IntervalType / AverageIntervalInSeconds) then pop oldest item in queue
             */

            instance.SetCounter(AverageSuccessesCounterData.CounterName, GetAverageSuccessForPeriod());
            instance.SetCounter(AverageFailuresCounterData.CounterName, GetAverageFailureForPeriod());

            instance._numberOfSuccesses = 0;
            instance._numberOfFailures = 0;
        }

        public void StartTimer(int interval)
        {
            _timerHelper.Start(TimeSpan.FromSeconds(interval), true);
        }

        public override void IntializeMonitor()
        {
            _timerHelper = new TimerHelper();
            _timerHelper.TimerEvent += (timer, state) => OnAverageTic(this);

            StartTimer(AverageIntervalInSeconds);
        }

        public void Success(int incrementBy = 1)
        {
            Console.WriteLine("Success {0}", this.CategoryName);

            _numberOfSuccesses++;
            this.IncrementCounter(SuccessCounterData.CounterName, incrementBy);
            this.ResetCounter(ConsecutiveFailureCounterData.CounterName);

            if (!IsLockedForChange)
            {
                this.SetPulseCounter(LastSuccessCounterData.CounterName);
                _lastSuccessActivity = DateTime.Now;
            }
        }

        public void Failure(int incrementBy = 1)
        {
            Console.WriteLine("Failure {0}", this.CategoryName);

            this.IncrementCounter(FailureCounterData.CounterName, incrementBy);
            this.IncrementCounter(ConsecutiveFailureCounterData.CounterName, incrementBy);
            _numberOfFailures++;
        }

        protected override List<CounterCreationData> CounterDataToRegister()
        {
            return new List<CounterCreationData>
            {
                SuccessCounterData,
                AverageSuccessesCounterData,
                LastSuccessCounterData,

                FailureCounterData,
                AverageFailuresCounterData,
                ConsecutiveFailureCounterData
            };
        }
    }
}
