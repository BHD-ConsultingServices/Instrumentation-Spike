
using Spike.Instrumentation.Monitoring.Monitors;

namespace Spike.Instrumentation.Monitoring
{
    /// <summary>
    /// This class wraps counter actions using defensive code so that it does not cause application disruptions if counters are not present
    /// </summary>
    public abstract partial class AppTelemetryBase
    {
        /// <summary>
        /// Basics the monitor set.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <param name="value">The value.</param>
        public void BasicMonitorSet(string monitorName, int value)
        {
            var monitor = GetMonitorByType<BasicMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Set(value);
            }
        }

        /// <summary>
        /// Basics the monitor inc.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <param name="value">The value.</param>
        public void BasicMonitorInc(string monitorName, int value = 1)
        {
            var monitor = GetMonitorByType<BasicMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Increment(value);
            }
        }

        /// <summary>
        /// Basics the monitor decimal.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <param name="value">The value.</param>
        public void BasicMonitorDec(string monitorName, int value = 1)
        {
            var monitor = GetMonitorByType<BasicMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Decrement(value);
            }
        }

        /// <summary>
        /// Basics the monitor reset.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        public void BasicMonitorReset(string monitorName)
        {
            var monitor = GetMonitorByType<BasicMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Reset();
            }
        }

        /// <summary>
        /// Basics the monitor value.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <returns></returns>
        public long BasicMonitorValue(string monitorName)
        {
            var monitor = GetMonitorByType<BasicMonitor>(monitorName);
            return monitor.Value;
        }
        
        /// <summary>
        /// Pulses the monitor toggle.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        public void PulseMonitorToggle(string monitorName)
        {
            var monitor = GetMonitorByType<PulseMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Pulse();
            }
        }

        /// <summary>
        /// Criticals the monitor set.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <param name="value">The value.</param>
        public void CriticalMonitorSet(string monitorName, int value)
        {
            var monitor = GetMonitorByType<CriticalErrorMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Set(value);
            }
        }

        /// <summary>
        /// Criticals the monitor inc.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <param name="value">The value.</param>
        public void CriticalMonitorInc(string monitorName, int value = 1)
        {
            var monitor = GetMonitorByType<CriticalErrorMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Increment(value);
            }
        }

        /// <summary>
        /// Criticals the monitor decimal.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        /// <param name="value">The value.</param>
        public void CriticalMonitorDec(string monitorName, int value = 1)
        {
            var monitor = GetMonitorByType<CriticalErrorMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Decrement(value);
            }
        }

        /// <summary>
        /// Criticals the monitor reset.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        public void CriticalMonitorReset(string monitorName)
        {
            var monitor = GetMonitorByType<CriticalErrorMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Reset();
            }
        }

        /// <summary>
        /// Twoes the state monitor attempt.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        public void TwoStateMonitorAttempt(string monitorName)
        {
            var monitor = GetMonitorByType<TwoStateMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Attempt();
            }
        }

        /// <summary>
        /// Twoes the state monitor success.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        public void TwoStateMonitorSuccess(string monitorName)
        {
            var monitor = GetMonitorByType<TwoStateMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Success();
            }
        }

        /// <summary>
        /// Twoes the state monitor failure.
        /// </summary>
        /// <param name="monitorName">Name of the monitor.</param>
        public void TwoStateMonitorFailure(string monitorName)
        {
            var monitor = GetMonitorByType<TwoStateMonitor>(monitorName);

            lock (monitor)
            {
                monitor?.Failure();
            }
        }
    }
}
