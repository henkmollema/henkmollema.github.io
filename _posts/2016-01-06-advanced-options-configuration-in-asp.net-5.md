---
layout: post
title: Advanced options configuration in ASP.NET 5
---

ASP.NET 5 comes with a completely new [Options framework](https://github.com/aspnet/Options) for accessing and configuration POCO settings. There are a few ways to configure the options and I'ld like to elaborate on some more advanced features.

## Component settings
When building a component-based system, these components probably have some settings. We define a component called `ConsoleWriter` implementing the `IConsoleWriter` interface:

```csharp
public interface IConsoleWriter
{
    void Write();
}

public class ConsoleWriter : IConsoleWriter
{
    private readonly IOptions<ConsoleWriterOptions> _options;

    public ConsoleWriter(IOptions<ConsoleWriterOptions> options)
    {
        _options = options;
    }

    public void Write()
    {
        Console.WriteLine(_options.Value.Message);
    }
}
```

`ConsoleWriterOptions` is a POCO options class which looks like this:

```csharp
public class ConsoleWriterOptions
{
    public string Message { get; set; }
}
```

The `ConsoleWriter` class can be added to the services using a convient extension method:
```csharp
public static void AddConsoleWriter(this IServiceCollection services)
{
    services.AddSingleton<IConsoleWriter, ConsoleWriter>();
}
```

Using that we can add the service:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add the Options framework.
    services.AddOptions();
    
    // Add our ConsoleWriter service using manual configuration.
    services.AddConsoleWriter();
}
```

Now we want to configure those `ConsoleWriterOptions` to change the message being printed. It can either be configured manually or via a configuration file.

### Manual configuration
To enable manual configuration of the options, we add an `Action<ConsoleWriterOptions>` parameter to the `AddConsoleWriter` method:
```
public static void AddConsoleWriter(this IServiceCollection services, Action<ConsoleWriterOptions> setupAction)
{
    // Add the service.
    services.AddSingleton<IConsoleWriter, ConsoleWriter>();

    // Configure the options manually.
    services.Configure(setupAction);
}
```

The `Configure` method is part of the Options API and accepts a `Action<TOptions>` parameter which in turn creates a `new ConfigureOptions<TOptions>` instance and calls `ConfigureOptions`. When the application is build, it will create an instance of the `ConsoleWriterOptions` class and apply the action we pass in the lambda expression. We can use it like this:
```csharp
services.AddConsoleWriter(x => x.Message = "Hello world!");
```

If we activate an instance of our `ConsoleWriter`, the runtime will inject an instance of `IOptions<ConsoleWriterOptions>` into our constructor.

### Configuration using the Configuration API
While this is nice, we usually want dynamic settings that can be changed at runtime. The [Configuration API](https://github.com/aspnet/Configuration/) allows us to populate our POCO options from a JSON file.

Let's add JSON file called `config.json`:
```json
{
  "ConsoleWriter": {
    "Message": "Hello World from config.json!"
  }
}
```

The `ConsoleWriter` element in the JSON file is the section we're going to use to populate our `ConsoleWriterOptions` object. To accomplish this, add another overload of `AddConsoleWriter`:
```csharp
public static void AddConsoleWriter(this IServiceCollection services, IConfigurationSection config)
{
    // Add the service.
    services.AddSingleton<IConsoleWriter, ConsoleWriter>();

    // Configure the options using the given configuration.
    services.Configure<ConsoleWriterOptions>(config);
}
```

To add the `config.json` file to our app, add a constructor in the startup class:
```csharp
public Startup()
{
    Configuration = new ConfigurationBuilder()
        .AddJsonFile("config.json")
        .Build();
}

private IConfiguration Configuration { get; }
```

And call our new overload:
```csharp
services.AddConsoleWriter(Configuration.GetSection("ConsoleWriter"));
```

We retrieve the section in the configuration properties by its name: `ConsoleWriter`. If we'd run the app, it will print:
```
Hello World from config.json!
```

We can make our live a bit easier by using a default section name in the component registration:
```csharp
public static void AddConsoleWriter(this IServiceCollection services, IConfiguration config)
{
    services.AddSingleton<IConsoleWriter, ConsoleWriter>();
    services.Configure<ConsoleWriterOptions>(config.OrSection("ConsoleWriter"));
}
```

The `OrSection` method is a nifty extension method to help us:
```csharp
public static IConfigurationSection OrSection(this IConfiguration config, string key)
{
    return config as IConfigurationSection ?? config.GetSection(key);
}
```

It tries to cast the given `IConfiguration` object to an `IConfigurationSection` object (`IConfigurationSection` derives from `IConfiguration`). That allows us to pass in either a section or just the complete configuration object. If the given object happens to be a section, we use that, otherwise we get the section using the given key as fallback. Now we can call `AddConsoleWriter` with just the `IConfiguration` object:

```csharp
services.AddConsoleWriter(Configuration);
```

And it will use thed default `ConsoleWriter` section. However, if the settings happens to be in another section, we can still pass that section:
```csharp
services.AddConsoleWriter(Configuration.GetSection("App:CustomConsoleWriter"));
```

## Global options configuration
Beside component settings, we could also have application-wide settings. These settings might be populated by components being added to the application.

For example, we have a class `AppSettings`:
```csharp
public class AppSettings
{
    public string Option { get; set; }
}
```

Now we want to configure some settings if the ConsoleWriter component is added to the application. To accomplish, we can implement the `IConfigureOptions<T>` interface:

```csharp
public class ConfigureAppSettings : IConfigureOptions<AppSettings>
{
    public void Configure(AppSettings options)
    {
        options.Option = $"Option from: {nameof(ConfigureAppSettings)}";
    }
}
```

The interface defines a method `Configure(T options)` which gets called by the Options framework when the options are being build. We have to configure this interface in the services:
```csharp
services.ConfigureOptions<ConfigureAppSettings>();
```

This adds the `IConfigureOptions<AppSettings>` implementation to the service collection causing the `OptionsManager` to call the `Configure` method.

## Conclusion
We're now able to write modular and reusable components and pass in configuration at application startup either manually or from a configuration source. We also learned how components can populate application wide settings using the `IConfigureOptions<T>` interface.
