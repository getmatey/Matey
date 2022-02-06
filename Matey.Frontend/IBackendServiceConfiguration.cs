using System.Net;

namespace Matey.Frontend
{
    public interface IBackendServiceConfiguration
    {
        string Name { get; }

        IPAddress IPAddress { get; }

        int? Port { get; }

        IFrontendServiceConfiguration Frontend { get; }
    }
}
