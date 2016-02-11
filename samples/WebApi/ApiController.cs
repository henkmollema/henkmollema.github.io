/// <summary>
/// Base class for an API controller.
/// </summary>
public abstract class ApiController
{
    /// <summary>
    /// Gets or sets the <see cref="Microsoft.AspNet.Mvc.ActionContext"/> object.
    /// </summary>
    /// <remarks>
    /// Must be public and have a public setter.
    /// </remarks>
    [ActionContext]
    public ActionContext ActionContext { get; set; }

    /// <summary>
    /// Gets the <see cref="Microsoft.AspNet.Http.HttpContext"/> for the executing action.
    /// </summary>
    public HttpContext HttpContext => ActionContext?.HttpContext;

    /// <summary>
    /// Gets the <see cref="HttpRequest"/> for the executing action.
    /// </summary>
    public HttpRequest Request => ActionContext?.HttpContext?.Request;

    /// <summary>
    /// Gets the <see cref="HttpResponse"/> for the executing action.
    /// </summary>
    public HttpResponse Response => ActionContext?.HttpContext?.Response;

    /// <summary>
    /// Gets the request-specific <see cref="IServiceProvider"/>.
    /// </summary>
    public IServiceProvider Resolver => ActionContext?.HttpContext?.RequestServices;
}