using Matey.ConfigurationSource.Abstractions;

namespace Matey.ConfigurationSource.Docker
{
    internal record DockerLoadBalancerConfiguration(ILoadBalancerStickinessConfiguration Stickiness) : ILoadBalancerConfiguration;
}
