using System.Net;

namespace Matey.Frontend.Abstractions
{
    public record ProxyForwardDestination(string Name, string Scheme, IPEndPoint IPEndPoint);
}
