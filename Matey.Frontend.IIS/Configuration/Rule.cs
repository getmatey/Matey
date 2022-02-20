using Microsoft.Web.Administration;

namespace Matey.Frontend.IIS.Configuration
{
    internal class Rule
    {
        internal readonly ConfigurationElement element;

        public string? Name {
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
            get => bool.TryParse(element["stopProcessing"] as string, out var stopProcessing) ? stopProcessing : false;
            set => element["stopProcessing"] = value.ToString().ToLower();
        }

        public RuleMatch Match { get; }

        public RuleAction Action { get; }

        internal Rule(ConfigurationElement element)
        {
            this.element = element;
            Match = new RuleMatch(element.GetChildElement("match"));
            Action  = new RuleAction(element.GetChildElement("action"));

            if (this.element["patternSyntax"] is null)
            {
                this.element["patternSyntax"] = "Wildcard";
            }
        }
    }
}
