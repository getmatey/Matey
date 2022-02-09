using Matey.Backend.Docker.Attributes;
using Matey.Frontend;
using System.Collections.Immutable;

namespace Matey.Backend.Docker
{
    internal static class DockerServiceConfigurationFactory
    {
        internal static DockerServiceConfiguration Create(
            IAttributeRoot attributes,
            string containerName,
            Func<IAttributeSection, DockerBackendServiceConfiguration> backendConfigurationFactory)
            => new DockerServiceConfiguration(
                Name: containerName,
                Provider: "docker",
                IsEnabled: attributes.GetValue<bool>(Tokens.Enabled) ?? true,
                Backends: attributes.Sections
                    .Where(s => !Tokens.Reserved.Contains(s.Name))
                    .Select(s => backendConfigurationFactory(s))
                    .ToImmutableArray<IBackendServiceConfiguration>());
    }
}
