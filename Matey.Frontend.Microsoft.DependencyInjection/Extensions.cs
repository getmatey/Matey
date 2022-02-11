using Microsoft.Extensions.DependencyInjection;

namespace Matey.Frontend.Microsoft.DependencyInjection
{
    using Abstractions;

    public static class Extensions
    {
        public static IServiceCollection AddFrontend<T>(this IServiceCollection services) where T : class, IFrontend
        {
            services.AddTransient<IFrontend, T>();

            return services;
        }
    }
}