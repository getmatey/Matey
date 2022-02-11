using System.Net;

namespace Matey.Frontend.Abstractions
{
    public record ProxyForwardDestination(string Scheme, IPEndPoint IPEndPoint);
}
