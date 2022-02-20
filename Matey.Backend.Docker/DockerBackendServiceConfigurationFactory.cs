using System.Net;

namespace Matey.Backend.Docker
{
    using Attributes;
    using global::Docker.DotNet.Models;
    using Matey.Backend.Abstractions;

    internal static class DockerBackendServiceConfigurationFactory
    {
        internal static DockerBackendServiceConfiguration Create(
            IAttributeSection attributes,
            IDictionary<string, EndpointSettings> networks,
            Func<IAttributeSection?, IFrontendServiceConfiguration> frontendConfigurationFactory)
            => new DockerBackendServiceConfiguration(
                Name: attributes.Name,
                Port: attributes.GetValue<int>(Tokens.Port),
                // TODO: Select network by name
                IPAddress: IPAddress.Parse(networks.Values.First().IPAddress),
                Frontend: frontendConfigurationFactory(attributes.GetSection(Tokens.Frontend)));
    }
}
