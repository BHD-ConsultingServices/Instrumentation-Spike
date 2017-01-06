
namespace Spike.CounterSimulator
{
    using System;
    using System.Threading;

    public class CounterDashboard
    {
        public static char UserMenuSelection()
        {
            Console.Clear();
            Console.WriteLine("Main Console Application");
            Console.WriteLine("============================================");
            Console.WriteLine("1] Successfull Payment");
            Console.WriteLine("2] Failed Payment");

            Console.WriteLine("");
            Console.WriteLine("Notification Thread");
            Console.WriteLine("============================================");
            Console.WriteLine("3] Start notification thread");
            Console.WriteLine("4] Close notification thread (Controlled)");
            Console.WriteLine("5] Close notification thread (Force)");
            Console.WriteLine("6] Increment notification queue on Notification Thread");
          

            Console.WriteLine("");
            Console.WriteLine("Press [x] to exit.");

            var raw = Console.ReadKey().KeyChar;
            return  Convert.ToChar(raw);
        }

        public static void ProcessUserRequest(char selection)
        {
            Console.Clear();
            var provider = ProviderFactory.CreatePaymentProvider();

            switch (selection)
            {
                case '1':
                    provider.MakePayment(1200); break;
                case '2':
                    provider.MakePayment(-1); break;
            }

            Thread.Sleep(500);
        }
    }
}
