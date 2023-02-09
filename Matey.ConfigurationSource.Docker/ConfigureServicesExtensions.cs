using Docker.DotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Matey.ConfigurationSource.Docker
{
    using Abstractions;

    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection ConfigureDockerBackend(
            this IServiceCollection services,
            Action<DockerOptions> configureOptions)
        {
            services.AddOptions<DockerOptions>().Configure(configureOptions);
            services.AddTransient(sp =>
            {
                IOptions<DockerOptions> options = sp.GetRequiredService<IOptions<DockerOptions>>();

                return new DockerClientConfiguration(new Uri(options.Value.Endpoint)).CreateClient();
            });
            services.AddTransient<IConfigurationSource, DockerConfigurationSource>();

            return services;
        }
    }
}
