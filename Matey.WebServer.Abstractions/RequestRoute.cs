using Matey.WebServer.Abstractions.Rules;

namespace Matey.WebServer.Abstractions
{
    public record RequestRoute(
        string ServiceName,
        IRequestRule Rule,
        ApplicationRequestEndpoint Endpoint,
        RequestRouteStickinessSettings? StickinessSettings = null);
}
