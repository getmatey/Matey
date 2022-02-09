using Microsoft.Extensions.DependencyInjection;

namespace Matey.Common.Microsoft.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddNotificationHandler<TNotification, THandler>(this IServiceCollection services) 
            where THandler : class, INotificationHandler<TNotification>
            where TNotification : INotification
        {
            services.AddTransient<MediatR.INotificationHandler<TNotification>, THandler>();

            return services;
        }

        public static IServiceCollection AddNotificationHandler<TNotification>(
            this IServiceCollection services,
            Func<IServiceProvider, INotificationHandler<TNotification>> implementationFactory)
            where TNotification : INotification
        {
            services.AddTransient<MediatR.INotificationHandler<TNotification>>(implementationFactory);

            return services;
        }
    }
}