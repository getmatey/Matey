namespace Matey.WebServer.Abstractions.Rules
{
    public record PathRequestRule(string PathPattern) : IRequestRule;
}
