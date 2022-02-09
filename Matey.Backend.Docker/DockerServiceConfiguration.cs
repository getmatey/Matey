using Matey.Frontend;
using System.Collections.Immutable;

namespace Matey.Backend.Docker
{
    internal record DockerServiceConfiguration(
        string Name,
        string Provider,
        bool IsEnabled,
        ImmutableArray<IBackendServiceConfiguration> Backends) : IServiceConfiguration;
}
