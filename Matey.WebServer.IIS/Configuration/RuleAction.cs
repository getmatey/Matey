using Microsoft.Web.Administration;

namespace Matey.WebServer.IIS.Configuration
{
    internal class RuleAction
    {
        private readonly ConfigurationElement element;

        public string? Type
        {
            get => element["type"] as string;
            set => element["type"] = value;
        }

        public string? Url
        {
            get => element["url"] as string;
            set => element["url"] = value;
        }

        public RuleAction(ConfigurationElement element)
        {
            this.element = element;
        }
    }
}
