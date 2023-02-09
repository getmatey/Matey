using Matey.ConfigurationSource.Abstractions;

namespace Matey.ConfigurationSource.Docker
{
    internal record DockerLoadBalancerStickinessConfiguration(
        bool? IsEnabled,
        string? CookieName) : ILoadBalancerStickinessConfiguration;
}
