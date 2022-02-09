using Matey.Frontend.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.Administration;

namespace Matey.Frontend.IIS.Microsoft.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection ConfigureIISFrontend(this IServiceCollection services, Action<IISOptions> configureOptions)
        {
            services.AddOptions<IISOptions>().Configure(configureOptions);
            services.AddTransient(_ => new ServerManager());
            services.AddFrontend<IISFrontend>();

            return services;
        }
    }
}
