using System.Collections.Immutable;

namespace Matey.Backend.Docker
{
    using Abstractions;

    internal record DockerServiceConfiguration(
        string Name,
        string Provider,
        string Domain,
        bool IsEnabled,
        ImmutableArray<IBackendServiceConfiguration> Backends) : IServiceConfiguration;
}
