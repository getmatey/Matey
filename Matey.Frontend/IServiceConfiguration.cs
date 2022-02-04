using System.Collections.Immutable;

namespace Matey.Frontend
{
    public interface IServiceConfiguration
    {
        public IEnumerable<IBackendServiceConfiguration> Backends { get; }

        public bool IsEnabled { get; }
    }
}
