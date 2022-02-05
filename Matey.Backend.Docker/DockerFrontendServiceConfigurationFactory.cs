using Matey.Backend.Docker.Attributes;

namespace Matey.Backend.Docker
{
    internal static class DockerFrontendServiceConfigurationFactory
    {
        internal static DockerFrontendServiceConfiguration Create(IAttributeSection attributes)
            => new DockerFrontendServiceConfiguration(
                Rule: attributes.GetString(Tokens.Rule));
    }
}
