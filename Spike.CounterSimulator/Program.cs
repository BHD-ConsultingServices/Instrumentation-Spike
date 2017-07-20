
using System;

namespace Spike.CounterSimulator
{
    using Providers;
    using System.Threading;

    public class Program
    {
        private static bool CreateCounters { get; set; }

        public static void Main(string[] args)
        {
            // Only create counters in a controlled way because when deleted they will
            // break the connection with active monitoring software
            if (Environment.UserInteractive)
            {
                CreateCounters = true;

                AppTelemetry.Instance.CreateCountersAllowed = CreateCounters;
                AppTelemetry.Instance.StartMonitoring();
                RunAsConsole();
            }
            else
            {
                AppTelemetry.Instance.StartMonitoring();
                RunAsService();
            }
        }


        public static void RunAsConsole()
        {
            var threadManager = new NotificationThread(CreateCounters);
            Thread notificationThread = null;

            var response = '-';

            while (response != 'x')
            {
                response = char.ToLower(CounterDashboard.UserMenuSelection());

                if (response == '3' && !threadManager.IsActive)
                {
                    notificationThread = new Thread(threadManager.IntitializeThread);
                    notificationThread.Start();
                }
                else if (response == '4' && notificationThread != null)
                {
                    threadManager.Exit();
                    notificationThread.Join();
                }
                else if (response == '6' && threadManager.IsActive)
                {
                    threadManager.IncrementCounter();
                }
                else if (response == '5' && threadManager.IsActive)
                {
                    notificationThread?.Abort();
                }
                else
                {
                    CounterDashboard.ProcessUserRequest(response);
                }
            }

            if (notificationThread != null)
            {
                threadManager.Exit();
                notificationThread.Join();
            }
        }
 
        public static void RunAsService()
        {
            
        }
    }
}
