using System.Collections.Immutable;

namespace Matey.Frontend.Abstractions.Rules
{
    public record HostRequestRule(string Host) : IRequestRule;
}
