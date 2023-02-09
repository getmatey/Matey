using Matey.WebServer.Abstractions.Rules;

namespace Matey.WebServer.Abstractions
{
    public record RequestRoute(
        IRequestRule Rule,
        ApplicationRequestEndpoint Endpoint,
        RequestRouteStickinessSettings StickinessSettings);
}
