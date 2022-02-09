using System.Collections.Immutable;

namespace Matey.Frontend
{
    public interface IServiceConfiguration
    {
        public string Name { get; }
        public bool IsEnabled { get; }
        public ImmutableArray<IBackendServiceConfiguration> Backends { get; }
    }
}
