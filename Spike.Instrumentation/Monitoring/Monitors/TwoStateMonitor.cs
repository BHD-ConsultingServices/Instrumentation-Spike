
namespace Spike.Instrumentation.Monitoring.Monitors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class TwoStateMonitor : MonitorBase
    {
        private const int AverageIntervalInMinutes = 1;

        private const string SuccessCounterName = "Success";
        private const string FailureCounterName = "Failure";
        private const string AverageSuccessCounterName = "SuccessAverage";
        private const string AverageFailureCounterName = "FailureAverage";
        private const string AverageAttemptCounterName = "AttemptsAverage";
        private const string LastSuccessCounterName = "LastSuccessPulse";
        private const string ConsecutiveFailuresCounterName = "ConsecutiveFailures";
        private const string AttemptCounterName = "Attempt";

        public TwoStateMonitor(string categoryName, string subCategoryName, IntervalType averagePeriod)
            : base(categoryName, subCategoryName)
        {
            _averagePeriod = averagePeriod;
        }

        private CounterCreationData _successCounterData;
        private CounterCreationData _failureCounterData;
        private CounterCreationData _averageSuccessesCounterData;
        private CounterCreationData _averageFailureCounterData;
        private CounterCreationData _averageAttemptsCounterData;
        private CounterCreationData _consecutivefailureCounterData;
        private CounterCreationData _lastSuccessCounterData;
        private CounterCreationData _attemptCounterData;

        private int _numberOfSuccesses;
        private int _numberOfFailures;
        private int _numberOfAttempts;

        private readonly IntervalType _averagePeriod;
        private LoopQueue _successQueue;
        private LoopQueue _failureQueue;
        private LoopQueue _attemptsQueue;

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

        private CounterCreationData AverageAttemptsCounterData
        {
            get
            {
                if (_averageAttemptsCounterData != null)
                {
                    return _averageAttemptsCounterData;
                }

                var counterName = string.Format("{0}.{1}", SubCategoryName, AverageAttemptCounterName);

                return _averageAttemptsCounterData = new CounterCreationData
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

        private CounterCreationData AttemptCounterData
        {
            get
            {
                if (_attemptCounterData != null)
                {
                    return _attemptCounterData;
                }

                var counterName = string.Format("{0}.{1}", SubCategoryName, AttemptCounterName);

                return _attemptCounterData = new CounterCreationData
                {
                    CounterName = counterName,
                    CounterType = PerformanceCounterType.NumberOfItems64
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

        private void OnAverageTic(object state)
        {
            var instance = (TwoStateMonitor) state;

            var averageSuccess = instance._successQueue.IntervalTic(ref _numberOfSuccesses);
            var averageFailure = instance._failureQueue.IntervalTic(ref _numberOfFailures);
            var averageAttempts = instance._attemptsQueue.IntervalTic(ref _numberOfAttempts);

            instance.SetCounter(AverageSuccessesCounterData.CounterName, averageSuccess);
            instance.SetCounter(AverageFailuresCounterData.CounterName, averageFailure);
            instance.SetCounter(AverageAttemptsCounterData.CounterName, averageAttempts);
        }

        public void StartTimer(int intervalInMinutes)
        {
            _timerHelper.Start(TimeSpan.FromMinutes(intervalInMinutes), true);
        }

        public override void IntializeMonitor()
        {
            _successQueue = new LoopQueue(_averagePeriod, AverageIntervalInMinutes);
            _failureQueue = new LoopQueue(_averagePeriod, AverageIntervalInMinutes);
            _attemptsQueue = new LoopQueue(_averagePeriod, AverageIntervalInMinutes);


            _timerHelper = new TimerHelper();
            _timerHelper.TimerEvent += (timer, state) => OnAverageTic(this);

            StartTimer(AverageIntervalInMinutes);
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

            _numberOfFailures++;
            this.IncrementCounter(FailureCounterData.CounterName, incrementBy);
            this.IncrementCounter(ConsecutiveFailureCounterData.CounterName, incrementBy);
        }

        public void Attempt(int incrementBy = 1)
        {
            Console.WriteLine("Attempt {0}", CategoryName);
            _numberOfAttempts += incrementBy;
            IncrementCounter(AttemptCounterData.CounterName, incrementBy);
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
                ConsecutiveFailureCounterData,

                AttemptCounterData,
                AverageAttemptsCounterData
            };
        }
    }
}
