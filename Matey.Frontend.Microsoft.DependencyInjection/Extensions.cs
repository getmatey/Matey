using Matey.Common.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Matey.Frontend.Microsoft.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddFrontend<T>(this IServiceCollection services) where T : class, IFrontend
        {
            // Add service as concrete type, making it accessible by interfaces.
            services.AddTransient<T>();

            // Add service by its interfaces.
            services.AddTransient<IFrontend>(sp => sp.GetRequiredService<T>());
            services.AddNotificationHandler<ServiceOnlineNotification>(sp => sp.GetRequiredService<T>());
            services.AddNotificationHandler<ServiceOfflineNotification>(sp => sp.GetRequiredService<T>());

            return services;
        }
    }
}