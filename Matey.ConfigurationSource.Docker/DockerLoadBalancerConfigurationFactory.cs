using Matey.ConfigurationSource.Abstractions;
using Matey.ConfigurationSource.Docker.Attributes;

namespace Matey.ConfigurationSource.Docker
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
