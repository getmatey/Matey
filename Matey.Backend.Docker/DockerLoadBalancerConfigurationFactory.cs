using Matey.Backend.Abstractions;
using Matey.Backend.Docker.Attributes;

namespace Matey.Backend.Docker
{
    internal static class DockerLoadBalancerConfigurationFactory
    {
        internal static DockerLoadBalancerConfiguration Create(
            IAttributeSection? attributes,
            Func<IAttributeSection?, ILoadBalancerStickinessConfiguration> stickinessConfigurationFactory)
        => new DockerLoadBalancerConfiguration(
            stickinessConfigurationFactory(attributes?.GetSection("stickiness")));
    }
}
