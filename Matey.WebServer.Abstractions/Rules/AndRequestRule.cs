namespace Matey.WebServer.Abstractions.Rules
{
    public class AndRequestRule : List<IRequestRule>, IRequestRule
    {
        public AndRequestRule()
        {
        }

        public AndRequestRule(IEnumerable<IRequestRule> collection) : base(collection)
        {
        }

        public AndRequestRule(int capacity) : base(capacity)
        {
        }
    }
}
