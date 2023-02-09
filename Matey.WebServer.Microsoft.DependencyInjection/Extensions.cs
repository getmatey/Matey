using Microsoft.Extensions.DependencyInjection;

namespace Matey.WebServer.Microsoft.DependencyInjection
{
    using Abstractions;

    public static class Extensions
    {
        public static IServiceCollection AddFrontend<T>(this IServiceCollection services) where T : class, IWebServer
        {
            services.AddTransient<IWebServer, T>();

            return services;
        }
    }
}