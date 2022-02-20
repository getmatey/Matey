namespace Matey
{
    using Frontend.Abstractions.Rules;

    public interface IRequestRuleParser
    {
        IRequestRule Parse(string text);
    }
}
