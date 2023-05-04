namespace Matey.WebServer.Abstractions.Rules
{
    public record HostRequestRule(string Host) : IRequestRule;
}
