using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ComponentOptions
{
    public class Startup
    {
        public Startup()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
        }

        private IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add the Options framework.
            services.AddOptions();

            // Add our ConsoleWriter service using manual configuration.
            //services.AddConsoleWriter(x => x.Message = "Hello world!");


            // Add our ConsoleWriter service using the configuration API.
            services.AddConsoleWriter(Configuration.GetSection("ConsoleWriter"));

            return services.BuildServiceProvider();
        }
    }
}
