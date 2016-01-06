using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;

namespace ComponentOptions
{
    public static class ComponentServiceCollectionExtensions
    {
        public static void AddConsoleWriter(this IServiceCollection services, Action<ConsoleWriterOptions> setupAction)
        {
            // Add the service.
            services.AddSingleton<IConsoleWriter, ConsoleWriter>();

            // Configure the options manually.
            services.Configure(setupAction);

            // Configure application wide settings.
            services.ConfigureOptions<ConfigureAppSettings>();
        }

        public static void AddConsoleWriter(this IServiceCollection services, IConfiguration config)
        {
            // Add the service.
            services.AddSingleton<IConsoleWriter, ConsoleWriter>();

            // Configure the options using the given configuration.
            services.Configure<ConsoleWriterOptions>(config.OrSection("ConsoleWriter"));

            // Configure application wide settings.
            services.ConfigureOptions<ConfigureAppSettings>();
        }

        public static IConfigurationSection OrSection(this IConfiguration config, string key)
        {
            return config as IConfigurationSection ?? config.GetSection(key);
        }
    }

    public class ConfigureAppSettings : IConfigureOptions<AppSettings>
    {
        public void Configure(AppSettings options)
        {
            options.Option = $"Option from: {nameof(ConfigureAppSettings)}";
        }
    }
}
