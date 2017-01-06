
namespace Spike.CounterSimulator
{
    using Providers;
    using System.Threading;

    public class Program
    {
        public static void Main(string[] args)
        {
            AppTelemetry.Instance.StartMonitoring();

            var threadManager = new NotificationThread();
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
                else if(response == '5' && threadManager.IsActive)
                {
                    notificationThread.Abort();
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
    }
}
