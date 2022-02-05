using System.Collections.Immutable;

namespace Matey.Frontend
{
    public interface IServiceConfiguration
    {
        public ImmutableArray<IBackendServiceConfiguration> Backends { get; }

        public bool IsEnabled { get; }
    }
}
