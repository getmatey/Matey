using Matey.Backend.Abstractions;

namespace Matey.Backend.Docker
{
    internal record DockerLoadBalancerStickinessConfiguration(
        bool? IsEnabled,
        string? CookieName) : ILoadBalancerStickinessConfiguration;
}
