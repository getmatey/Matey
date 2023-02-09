using Microsoft.Web.Administration;

namespace Matey.WebServer.IIS.Configuration
{
    internal class Rule
    {
        internal readonly ConfigurationElement element;

        public string? Name
        {
            get => element["name"] as string;
            set => element["name"] = value;
        }

        public string? PatternSyntax
        {
            get => element["patternSyntax"] as string;
            set => element["patternSyntax"] = value;
        }

        public bool StopProcessing
        {
            get => element["stopProcessing"] as bool? ?? false;
            set => element["stopProcessing"] = value;
        }

        public RuleMatch Match { get; }

        public RuleAction Action { get; }

        public RuleConditionCollection Conditions { get; }

        internal Rule(ConfigurationElement element)
        {
            this.element = element;
            Match = new RuleMatch(element.GetChildElement("match"));
            Action = new RuleAction(element.GetChildElement("action"));
            Conditions = new RuleConditionCollection(element.GetChildElement("conditions").GetCollection());

            if (this.element["patternSyntax"] is null)
            {
                this.element["patternSyntax"] = "Wildcard";
            }
        }
    }
}
