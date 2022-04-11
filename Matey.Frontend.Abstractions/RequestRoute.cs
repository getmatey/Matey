using Matey.Frontend.Abstractions.Rules;

namespace Matey.Frontend.Abstractions
{
    public record RequestRoute(
        IRequestRule Rule, 
        ApplicationRequestEndpoint Endpoint,
        RequestRouteStickinessSettings StickinessSettings);
}
