namespace Matey.WebServer.Abstractions.Rules
{
    public class NotRequestRule : IRequestRule
    {
        public IRequestRule Rule { get; }

        internal NotRequestRule(IRequestRule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            if (rule is NotRequestRule)
            {
                throw new InvalidOperationException($"Parameter '{nameof(rule)}' must not be of type '{nameof(NotRequestRule)}'.");
            }

            Rule = rule;
        }
    }
}
