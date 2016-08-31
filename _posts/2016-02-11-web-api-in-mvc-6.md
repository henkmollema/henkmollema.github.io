---
layout: post
title: Web API in MVC 6
---

With ASP.NET Core MVC 1.0, the MVC and Web API framework have been merged into one framework called MVC. This is a good thing, since MVC and Web API share a lot of functionality, yet there always were subtle differences and code duplication.

However, merging these two into framework one also made it more diffucult to distinguish one from another. For example, the `Microsoft.AspNet.WebApi` represents the Web API 5.x.x framework, not the new one. But, when you include `Microsoft.AspNetCore.Mvc` (version `1.0.0`), you get the full blown package. This will contain _all_ the out-of-the-box features the MVC framework offers. Such as Razor, tag helpers and model binding.

When you just want to build an API, we don't need all this features. So, how do we build a minimalistic Web API? The answer is: [`Microsoft.AspNetCore.Mvc.Core`](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Core). In the new world MVC is split up into multiple packages and this package contains just the core components of the MVC framework, such as routing and authorization.

For this example, we're gonna create a minimal MVC API. Including a JSON formatter and CORS. Create an empty ASP.NET 5 Web Application and add these packages to your project.json:

```json
"Microsoft.AspNetCore.Mvc.Core": "1.0.0",
"Microsoft.AspNetCore.Mvc.Cors": "1.0.0",
"Microsoft.AspNetCore.Mvc.Formatters.Json": "1.0.0"
```

Now we can register MVC using `AddMvcCore()` in the startup class:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvcCore()
            .AddCors()
            .AddJsonFormatters();
}
```

`AddMvcCore` returns an `IMvcCoreBuilder` instance which allows further building. Configuring the middleware is the same as usual:

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseCors(policy =>
    {
        policy.AllowAnyOrigin();
    });
    app.UseMvc();
}
```

## Controllers
The 'old' Web API comes with its own controller base class: `ApiController`. In the new world there is no such thing, only the default `Controller` class. Unfortunately, this is a rather large base class and it's tied to model binding, views and JSON.NET.

Fortunately, in the new framework controller classes don't have to derive from `Controller` to be picked up by the routing mechanism. Just appending the name with `Controller` is enough. This allows us to build our own controller base class. Let's call it `ApiController`, just for old times sake:

```csharp
/// <summary>
/// Base class for an API controller.
/// </summary>
[Controller]
public abstract class ApiController
{
    [ActionContext]
    public ActionContext ActionContext { get; set; }

    public HttpContext HttpContext => ActionContext?.HttpContext;

    public HttpRequest Request => ActionContext?.HttpContext?.Request;

    public HttpResponse Response => ActionContext?.HttpContext?.Response;

    public IServiceProvider Resolver => ActionContext?.HttpContext?.RequestServices;
}
```

The `[Controller]` attribute indicates that the type or any derived type is considered as a controller by the default controller discovery mechanism. The `[ActionContext]` attribute specifies that the property should be set with the current `ActionContext` when MVC creates the controller. The `ActionContext` provides information about the current request. Full source [here](https://github.com/henkmollema/henkmollema.github.io/tree/master/samples/WebApi).

> ASP.NET Core MVC also offers a [`ControllerBase`](https://github.com/aspnet/Mvc/blob/1.0.0/src/Microsoft.AspNetCore.Mvc.Core/ControllerBase.cs) class which provides a controller base class just without views support. It's still much larger than ours though. Use it if you find it convenient.

## Conclusion
We can now build a minimal Web API using the MVC 6 framework. The new modular package structure allows us to just pull in the packages we need and create a lean and simple application.
