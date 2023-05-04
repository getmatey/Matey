using Matey.Pki;
using System.Net;

namespace Matey.WebServer.Abstractions
{
    public interface IWebServer
    {
        string Name { get; }

        IPAddress CallbackIPAddress { get; }

        ICertificateStore CertificateStore { get; }

        void InitializeRequestRoutes(IEnumerable<RequestRoute> routes);

        void AddRequestRoute(RequestRoute route);

        void RemoveRequestRoutes(ApplicationRequestEndpoint endpoint);
    }
}
