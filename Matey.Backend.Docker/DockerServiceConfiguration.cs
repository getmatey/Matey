using System.Collections.Immutable;

namespace Matey.Backend.Docker
{
    using Abstractions;

    internal record DockerServiceConfiguration(
        string Name,
        string Provider,
        bool IsEnabled,
        ImmutableArray<IBackendServiceConfiguration> Backends) : IServiceConfiguration;
}
