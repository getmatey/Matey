using Matey.Frontend.Abstractions.Rules;
using System.Collections.Immutable;

namespace Matey.Frontend.Abstractions
{
    public record RequestRouteRule(IRequestRule Rule, ApplicationRequestEndpoint Endpoint);
}
