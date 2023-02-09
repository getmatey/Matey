using System.Net;

namespace Matey.WebServer.Abstractions
{
    public record ApplicationRequestEndpoint(string Scheme, IPEndPoint IPEndPoint, int? Weight);
}
