// <copyright file="CriticalErrorMonitor.cs" company="PayM8">
//     Copyright ©  2015
// </copyright>

namespace Paym8.Monitoring.Monitors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Logging;

    /// <summary>
    /// Monitor that checks for critical application errors
    /// </summary>
    /// <seealso cref="Paym8.Monitoring.MonitorBase" />
    public class CriticalErrorMonitor : MonitorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalErrorMonitor"/> class.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="monitorName">Name of the monitor.</param>
        public CriticalErrorMonitor(string categoryName, string monitorName) :
            base(categoryName, monitorName.ToLower().Contains("critical") ? monitorName : string.Format("Critical{0}", categoryName))
        {
        }

        /// <summary>
        /// The critical counter data
        /// </summary>
        private CounterCreationData _criticalCounterData;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly Logging _logger = LogFactory.Create("Paym8.Monitoring.Monitors.CriticalErrorMonitor");

        /// <summary>
        /// Gets the critical counter data.
        /// </summary>
        /// <value>
        /// The critical counter data.
        /// </value>
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

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Set(long value)
        {
            try
            {
                this.Counters.Single(c => c.CounterName == CriticalCounterData.CounterName).RawValue = value;
            }
            catch (Exception ex)
            {
                _logger.Warn(string.Format("Could not set counter [{0}]. Error [{1}]", CriticalCounterData.CounterName, ex.Message));
            }
        }

        /// <summary>
        /// Increments the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Increment(long value = 1)
        {
            try
            {
                this.Counters.Single(c => c.CounterName == CriticalCounterData.CounterName).IncrementBy(value);
            }
            catch (Exception ex)
            {
                _logger.Warn(string.Format("Could not increment counter [{0}]. Error [{1}]", CriticalCounterData.CounterName, ex.Message));
            }
        }

        /// <summary>
        /// Decrements the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Decrement(long value = 1)
        {
            try
            {
                var decriment = -1 * value;
                this.Counters.Single(c => c.CounterName == CriticalCounterData.CounterName).IncrementBy(decriment);
            }
            catch (Exception ex)
            {
                _logger.Warn(string.Format("Could not decrement counter [{0}]. Error [{1}]", CriticalCounterData.CounterName, ex.Message));
            }
        }

        /// <summary>
        /// Counters the data to register.
        /// </summary>
        /// <returns>
        /// List of Counts objects to register
        /// </returns>
        protected override List<CounterCreationData> CounterDataToRegister()
        {
            return new List<CounterCreationData>
            {
                CriticalCounterData
            };
        }
    }
}
