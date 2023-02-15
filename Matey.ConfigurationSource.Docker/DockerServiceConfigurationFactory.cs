﻿using System.Collections.Immutable;

namespace Matey.ConfigurationSource.Docker
{
    using Abstractions;
    using Attributes;

    internal static class DockerServiceConfigurationFactory
    {
        internal static DockerServiceConfiguration Create(
            IAttributeRoot attributes,
            string containerId,
            string containerName,
            Func<IAttributeSection, DockerBackendServiceConfiguration> backendConfigurationFactory)
            => new DockerServiceConfiguration(
                Id: containerId,
                Name: containerName,
                Provider: "docker",
                Target: attributes.TryGetString(Tokens.Target, out string? target) ? target : null,
                IsEnabled: attributes.GetValue<bool>(Tokens.Enabled) ?? true,
                Backends: attributes.Sections
                    .Where(s => !Tokens.Reserved.Contains(s.Name))
                    .Select(s => backendConfigurationFactory(s))
                    .ToImmutableArray<IBackendServiceConfiguration>());
    }
}
