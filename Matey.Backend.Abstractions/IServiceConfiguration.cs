using System.Collections.Immutable;

namespace Matey.Backend.Abstractions
{
    public interface IServiceConfiguration
    {
        public string Name { get; }
        public string Provider { get; }
        public string Domain { get; }
        public bool IsEnabled { get; }
        public ImmutableArray<IBackendServiceConfiguration> Backends { get; }
    }
}
