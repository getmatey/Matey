using System.Net;

namespace Matey.Backend.Docker
{
    using Attributes;

    internal static class DockerBackendServiceConfigurationFactory
    {
        internal static DockerBackendServiceConfiguration Create(IAttributeSection attributes, IPAddress ipAddress)
            => new DockerBackendServiceConfiguration(
                Name: attributes.Name,
                Port: attributes.GetValue<int>(Tokens.Port),
                IPAddress: ipAddress,
                Frontend: DockerFrontendServiceConfigurationFactory.Create(attributes.GetSection(Tokens.Frontend)));
    }
}
