---
layout: post
title: Async tips and tricks
---

The `async` and `await` keywords along with the `Task` and `Task<T>` classes provide a great way to do asynchronous work in C#. However, there might be some confusion when to mark a method as `async`, when to `await` a task and when to use the `Task` class. In this post I will try to clear some things up using some simple examples.

### Avoid `async void`
It's tempting to change a regular `void` method to `async void`. You should, however, use `async Task` for these methods and avoid `async void` since exceptions that occur inside an `async void` method are handled different than when you await a `Task` and it will crash the process. The only exception here is for events.

Bad:

```csharp
public async void DoWorkAsync()
{
    // ...
}
```

Good:

```csharp
public async Task DoWorkAsync()
{
    // ...
}
```

Reference: [Haacked.com](http://haacked.com/archive/2014/11/11/async-void-methods/), [MSDN](https://msdn.microsoft.com/en-us/magazine/jj991977.aspx)

### Return a `Task` when you can
When an async method calls another async method. You can either choose to await the task and return the result or return the task to be awaited by the caller. It's better to return the task to the caller. This avoids unnecessary wrapping of methods in a state machine.

In stead of:

```csharp
public async Task<Result> HandleAsync(SomeQuery query)
{
    return await _dispatcher.DispatchQuery(query);
}

public async Task HandleAsync(SomeCommand command)
{
    await _dispatcher.DispatchCommand(command);
}
```

Do this:

```csharp
public Task<Result> HandleAsync(SomeQuery query)
{
    return _dispatcher.DispatchQuery(query);
}

public Task HandleAsync(SomeCommand command)
{
    return _dispatcher.DispatchCommand(command);
}
```

### _Do_ `return await` when you have multiple awaits

The exception for the above is when you have some async intermediate method which you await, you should await when returning too:

```csharp
public Task<Result> HandleAsync(SomeQuery query)
{
    var intermediate = await _repository.GetAsync(query);
    return await _dispatcher.DispatchQuery(intermediate);
}
```

Leaving out the `await` keyword even causes your code not compile. If you use `await` anywhere, you have to `await` everything.

### Use `Task.WhenAll()` for multiple tasks
When you have multiple tasks which are independent of eachother, you could either await each task or use `Task.WhenAll()`. It's prefered to use `Task.WhenAll()` since that only creates one state machine.

In stead of:

```csharp
public async Task RunAsyncAwaitEach()
{
    var t1 = DoWorkAsync1();
    var t2 = DoWorkAsync2();
    var t3 = DoWorkAsync3();

    await t1;
    await t2;
    await t3;
}
```

Use this:

```csharp
public async Task RunAsyncWhenAll()
{
    await Task.WhenAll(DoWorkAsync1(), DoWorkAsync2(), DoWorkAsync3());
}
```

When one of the tasks returns a value, you can use the `Result` property of `Task<T>`:

```csharp
var someTask = DoWorkAsync();
var someTaskWithResult = GetResultAsync();

await Task.WhenAll(someTask, someTaskWithResult);

var result = someTaskWithResult.Result;
```

Reference: [StackOverflow](http://stackoverflow.com/q/18310996)
