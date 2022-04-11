using Matey.Backend.Abstractions;
using Matey.Backend.Docker.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matey.Backend.Docker
{
    internal static class DockerLoadBalancerConfigurationFactory
    {
        internal static DockerLoadBalancerConfiguration Create(
            IAttributeSection? attributes,
            Func<IAttributeSection?, ILoadBalancerStickinessConfiguration> stickinessConfigurationFactory)
        => new DockerLoadBalancerConfiguration(
            stickinessConfigurationFactory(attributes));
    }
}
