using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication1
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddCors()
                    .AddJsonFormatters();
        }

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseCors(policy =>
    {
        policy.AllowAnyOrigin();
    });
    app.UseMvc();
}
    }
}
