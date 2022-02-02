using Matey.Frontend;

namespace Matey.Backend
{
    public interface IBackendServiceConfiguration
    {
        IFrontendServiceConfiguration Frontend { get; }

        int Port { get; }

        bool IsEnabled { get; }
    }
}
