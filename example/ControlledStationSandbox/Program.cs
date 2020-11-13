namespace Sandbox
{
    using Iec608705104;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Console = Colorful.Console;

    public static class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Press any key to start the Slave.\r\nTerminate by Ctrl + C.", Color.Gray);

            using var mre = new ManualResetEvent(false);
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
            {
                e.Cancel = true;
                mre.Set();
            };

            try
            {
                var options = new ControlledStationOptions();
                using var slave = new ControlledStation(options);
                slave.ReadError += (_, e) => Debug.WriteLine($"ERROR {e.Error}");
                await slave.StartAsync(default).ConfigureAwait(false);
                mre.WaitOne();
                await slave.StopAsync(default).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\r\nERROR {ex.GetType().Name}: {ex.Message}", Color.Red);
            }
        }
    }
}
