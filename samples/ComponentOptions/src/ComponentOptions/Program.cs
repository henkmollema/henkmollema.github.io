using System;
using Microsoft.Extensions.DependencyInjection;

namespace ComponentOptions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Mimic the MVC startup class.
            var startup = new Startup();
            var services = new ServiceCollection();

            // Build a service provider.
            var sp = startup.ConfigureServices(services);

            // Use the service with its options.
            var writer = sp.GetService<IConsoleWriter>();
            writer.Write();

            Console.ReadLine();
        }
    }
}
