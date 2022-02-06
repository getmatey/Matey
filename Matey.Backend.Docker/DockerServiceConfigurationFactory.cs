using Matey.Backend.Docker.Attributes;
using Matey.Frontend;
using System.Collections.Immutable;

namespace Matey.Backend.Docker
{
    internal static class DockerServiceConfigurationFactory
    {
        internal static DockerServiceConfiguration Create(
            IAttributeRoot attributes,
            Func<IAttributeSection, DockerBackendServiceConfiguration> backendConfigurationFactory)
            => new DockerServiceConfiguration(
                IsEnabled: attributes.GetValue<bool>(Tokens.Enabled) ?? true,
                Backends: attributes.Sections
                    .Where(s => !Tokens.Reserved.Contains(s.Name))
                    .Select(s => backendConfigurationFactory(s))
                    .ToImmutableArray<IBackendServiceConfiguration>());
    }
}
