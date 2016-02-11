---
layout: post
title: Web API in MVC 6
---

With MVC 6 in ASP.NET 5, the MVC and Web API framework have been merged into one framework called MVC. This is a good thing, since MVC and Web API share a lot of functionality, yet there always were subtle differences between MVC and Web API.

> Note: MVC 6 is soon going to be renamed to ASP.NET MVC Core 1.0 with RC2, when it drops I will update this post to reflect the changes.

However, merging these two into one also made it more diffucult to distinguish one from another. For example, the `Microsoft.AspNet.WebApi` represents the Web API 5.x.x framework, not the new one. But, when you include `Microsoft.AspNet.Mvc` (`v6.0.0-rc1-final1` for example), you get the complete package. This will contain _all_ the out-of-the-box features the MVC framework offers. Such as Razor, tag helpers and model binding.

When you just want to build an API (REST for example), we don't need all this features. So, how doe we build a minimalistic Web API? The answer is: [`Microsoft.AspNet.Mvc.Core`](https://www.nuget.org/packages/Microsoft.AspNet.Mvc.Core). MVC is split up into multiple packages and this package contains just the core components of the MVC framework, such as routing and authorization.

For this example, we're gonna create a minimal MVC API. Including a JSON formatter and CORS.

Add these packages to your project.json:

```json
"Microsoft.AspNet.Cors": "6.0.0-rc1-final",
"Microsoft.AspNet.Hosting": "6.0.0-rc1-final"
"Microsoft.AspNet.Mvc.Core": "6.0.0-rc1-final",
"Microsoft.AspNet.Mvc.Cors": "6.0.0-rc1-final",
"Microsoft.AspNet.Mvc.Formatters.Json": "6.0.0-rc1-final"
```

Now we can register MVC using `AddMvcCore()` in the startup class:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvcCore()
            .AddAuthorization()
            .AddCors()
            .AddJsonFormatters();
}
```

`AddMvcCore` returns an `IMvcCoreBuilder` instance which allows further building. Configuring the middleware is the same as usual:

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseCors(o => o.AllowAnyOrigin());
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

The `[ActionContext]` attribute specifies that the property should be set with the current `ActionContext` when MVC creates the controller. The `ActionContext` provides information about the current request. Full source [here](https://github.com/henkmollema/henkmollema.github.io/tree/master/samples/WebApi/ApiController.cs).

> Note: in RC2, there is going to be a [`ControllerBase`](https://github.com/aspnet/Mvc/tree/6.0.0-rc1/src/Microsoft.AspNetCore.Mvc.Core/ControllerBase.cs) class in the MVC Core package which is not tied to views. This also might suite your needs. However, it is still a lot bigger than ours.

## Conclusion
We can now build a minimal Web API using the MVC 6 framework. The new modular package structure allows us to just pull in the packages we need and create a lean and simple application.