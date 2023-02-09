using Matey.ConfigurationSource.Abstractions;
using Matey.ConfigurationSource.Docker.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matey.ConfigurationSource.Docker
{
    internal static class DockerLoadBalancerStickinesssConfigurationFactory
    {
        internal static ILoadBalancerStickinessConfiguration Create(IAttributeSection? attributes)
            => new DockerLoadBalancerStickinessConfiguration(
                attributes?.TryGetValue("enabled", out bool? isEnabled) == true ? isEnabled : null,
                attributes?.TryGetString("cookieName", out string? cookieName) == true ? cookieName : null);
    }
}
