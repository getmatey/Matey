using Microsoft.Web.Administration;

namespace Matey.WebServer.IIS.Configuration
{
    internal class RuleCondition
    {
        internal readonly ConfigurationElement element;

        public string? Input
        {
            get => element["input"] as string;
            set => element["input"] = value;
        }

        public string? Pattern
        {
            get => element["pattern"] as string;
            set => element["pattern"] = value;
        }

        public bool Negate
        {
            get => element["negate"] as bool? ?? false;
            set => element["negate"] = value;
        }

        public RuleCondition(ConfigurationElement element)
        {
            this.element = element;
        }
    }
}
