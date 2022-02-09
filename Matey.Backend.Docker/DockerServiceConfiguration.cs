using Matey.Frontend;
using System.Collections.Immutable;

namespace Matey.Backend.Docker
{
    internal record DockerServiceConfiguration(
        string Name,
        bool IsEnabled,
        ImmutableArray<IBackendServiceConfiguration> Backends) : IServiceConfiguration;
}
