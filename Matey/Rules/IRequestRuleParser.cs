namespace Matey.Rules
{
    using WebServer.Abstractions.Rules;

    public interface IRequestRuleParser
    {
        IRequestRule Parse(string text);
    }
}
