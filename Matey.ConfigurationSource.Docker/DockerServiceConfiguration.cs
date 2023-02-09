using System.Collections.Immutable;

namespace Matey.ConfigurationSource.Docker
{
    using Abstractions;

    internal record DockerServiceConfiguration(
        string Id,
        string Name,
        string Provider,
        string? Target,
        string Domain,
        bool IsEnabled,
        ImmutableArray<IBackendServiceConfiguration> Backends) : IServiceConfiguration;
}
