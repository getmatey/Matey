using System.Net;

namespace Matey.Frontend.Abstractions
{
    public record ApplicationRequestEndpoint(string Scheme, IPEndPoint IPEndPoint);
}
