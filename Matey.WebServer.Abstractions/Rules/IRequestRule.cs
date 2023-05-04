namespace Matey.WebServer.Abstractions.Rules
{
    public interface IRequestRule
    {
        IEnumerable<IRequestRule> ToEnumerable() => this is IEnumerable<IRequestRule> enumerable ? enumerable : new IRequestRule[] { this };
    }

    public static class RequestRuleExtensions
    {
        public static IRequestRule And(this IRequestRule left, IRequestRule right)
        {
            IEnumerable<IRequestRule> lefts = left is AndRequestRule leftAnd ? leftAnd : left.ToEnumerable();
            IEnumerable<IRequestRule> rights = right is AndRequestRule rightAnd ? rightAnd : right.ToEnumerable();

            return new AndRequestRule(lefts.Concat(rights));
        }

        public static IRequestRule Not(this IRequestRule rule)
        {
            if (rule is NotRequestRule notRule)
            {
                return notRule.Rule;
            }
            else
            {
                return new NotRequestRule(rule);
            }
        }
    }
}
