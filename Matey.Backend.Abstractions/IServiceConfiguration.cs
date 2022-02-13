using System.Collections.Immutable;

namespace Matey.Backend.Abstractions
{
    public interface IServiceConfiguration
    {
        string Id { get; }
        string Name { get; }
        string Provider { get; }
        string? Target { get; }
        string Domain { get; }
        bool IsEnabled { get; }
        ImmutableArray<IBackendServiceConfiguration> Backends { get; }
    }
}
