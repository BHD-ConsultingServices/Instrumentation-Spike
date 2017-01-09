
namespace Spike.Instrumentation
{
    using System;
    using System.Threading;
    public class TimerHelper
    {
        public Timer Timer;
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1);
        private readonly object _threadLock = new object();
        public event Action<Timer, object> TimerEvent;

        public Thread OrigiatingThread { get; private set; }

        public TimerHelper()
        {
            OrigiatingThread = Thread.CurrentThread;
        }

        public void Start(TimeSpan timerInterval, bool triggerAtStart = false,
            object state = null)
        {
            Stop();
            Timer = new Timer(Timer_Elapsed, state,
                System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            if (triggerAtStart)
            {
                Timer.Change(TimeSpan.FromTicks(0), timerInterval);
            }
            else
            {
                Timer.Change(timerInterval, timerInterval);
            }
        }

        public void Stop(TimeSpan? timeout = null)
        {
            timeout = timeout ?? Timeout;

            lock (_threadLock)
            {
                if (Timer != null)
                {
                    using (ManualResetEvent waitHandle = new ManualResetEvent(false))
                    {
                        if (Timer.Dispose(waitHandle))
                        {
                            // Timer has not been disposed by someone else
                            if (!waitHandle.WaitOne(timeout.Value))
                                throw new TimeoutException("Timeout waiting for timer to stop");
                        }
                        Timer = null;
                    }
                }
            }
        }

        public void Timer_Elapsed(object state)
        {
            if (Monitor.TryEnter(_threadLock))
            {
                var timerEvent = TimerEvent;

                try
                {
                    if (Timer == null)
                        return;
                    
                    var isRunning = (OrigiatingThread.ThreadState == ThreadState.Background 
                        || OrigiatingThread.ThreadState == ThreadState.Running 
                        || OrigiatingThread.ThreadState == ThreadState.WaitSleepJoin);

                    if (!isRunning || timerEvent == null)
                    {
                        Timer.Dispose();
                        Timer = null;
                        return;
                    }                

                    timerEvent(Timer, state);                
                }
                finally
                {
                    Monitor.Exit(_threadLock);
                }
            }
        }
    }
}
