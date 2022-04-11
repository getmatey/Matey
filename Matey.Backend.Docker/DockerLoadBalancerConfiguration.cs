using Matey.Backend.Abstractions;

namespace Matey.Backend.Docker
{
    internal record DockerLoadBalancerConfiguration(ILoadBalancerStickinessConfiguration Stickiness) : ILoadBalancerConfiguration;
}
