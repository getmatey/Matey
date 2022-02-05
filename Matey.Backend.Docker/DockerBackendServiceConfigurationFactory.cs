using Matey.Backend.Docker.Attributes;

namespace Matey.Backend.Docker
{
    internal static class DockerBackendServiceConfigurationFactory
    {
        internal static DockerBackendServiceConfiguration Create(IAttributeSection attributes)
            => new DockerBackendServiceConfiguration(
                Name: attributes.Name,
                Port: attributes.GetValue<int>(Tokens.Port),
                Frontend: DockerFrontendServiceConfigurationFactory.Create(attributes.GetSection(Tokens.Frontend)));
    }
}
