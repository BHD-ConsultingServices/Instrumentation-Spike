
namespace Spike.Instrumentation.Monitoring
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public abstract class MonitorBase
    {
        public string CategoryName { get; }

        public string MonitorName { get; set; }

        private IEnumerable<CounterCreationData> _counterData;

        public IEnumerable<CounterCreationData> CounterData
        {
            get
            {
                if (_counterData != null)
                {
                    return _counterData;
                }

                return _counterData = CounterDataToRegister();
            }
        }

        private List<PerformanceCounter> _counter;
        
        public List<PerformanceCounter> Counters
        {
            get
            {
                if (_counter != null)
                {
                    return _counter;
                }

                return _counter = CreateCounterInstances();
            }
        }
        
        protected abstract List<CounterCreationData> CounterDataToRegister();

        public virtual void IntializeMonitor() {}

        private List<PerformanceCounter> CreateCounterInstances()
        {
            var counterList = new List<PerformanceCounter>();

            foreach (var definition in CounterData)
            {
                var counter = new PerformanceCounter(CategoryName, definition.CounterName, false)
                {
                   RawValue = 0
                };

                counterList.Add(counter);
            }
            
            return counterList;
        }

        protected MonitorBase(string categoryName , string monitorName = null)
        {
            this.CategoryName = categoryName;
            this.MonitorName = monitorName;
        }
    }
}
