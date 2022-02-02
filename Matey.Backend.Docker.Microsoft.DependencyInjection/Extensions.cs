using Docker.DotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Matey.Backend.Docker.Microsoft.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection ConfigureDockerBackend(
            this IServiceCollection services,
            Action<DockerEngineOptions> configureOptions)
        {
            services.AddOptions<DockerEngineOptions>().Configure(configureOptions);
            services.AddTransient(sp =>
            {
                IOptions<DockerEngineOptions> options = sp.GetRequiredService<IOptions<DockerEngineOptions>>();

                return new DockerClientConfiguration(new Uri(options.Value.Endpoint)).CreateClient();
            });
            services.AddTransient<IBackend, DockerBackend>();

            return services;
        }
    }
}
