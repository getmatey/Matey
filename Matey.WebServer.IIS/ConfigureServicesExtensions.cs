using Matey.WebServer.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Web.Administration;

namespace Matey.WebServer.IIS
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddIISWebServer(this IServiceCollection services, Action<IISOptions> configureOptions)
        {
            services.AddOptions<IISOptions>().Configure(configureOptions);
            services.AddTransient(sp =>
            {
                IOptions<IISOptions> options = sp.GetRequiredService<IOptions<IISOptions>>();

                if (options.Value.ServerName == null)
                {
                    return new ServerManager();
                }
                else
                {
                    return ServerManager.OpenRemote(options.Value.ServerName);
                }
            });
            services.AddFrontend<IISWebServer>();

            return services;
        }
    }
}
