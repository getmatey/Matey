using Matey.Frontend;
using System.Collections.Immutable;

namespace Matey.Backend.Docker
{
    internal record DockerServiceConfiguration(
        ImmutableArray<IBackendServiceConfiguration> Backends,
        bool IsEnabled) : IServiceConfiguration;
}
