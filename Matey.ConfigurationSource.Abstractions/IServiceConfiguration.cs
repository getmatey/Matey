using System.Collections.Immutable;

namespace Matey.ConfigurationSource.Abstractions
{
    public interface IServiceConfiguration
    {
        string Id { get; }
        string Name { get; }
        string Provider { get; }
        string? Target { get; }
        bool IsEnabled { get; }
        ImmutableArray<IBackendServiceConfiguration> Backends { get; }
    }
}
