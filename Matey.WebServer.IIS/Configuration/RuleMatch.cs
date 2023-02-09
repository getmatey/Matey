using Microsoft.Web.Administration;

namespace Matey.WebServer.IIS.Configuration
{
    internal class RuleMatch
    {
        private readonly ConfigurationElement element;

        public string? Url
        {
            get => element["url"] as string;
            set => element["url"] = value;
        }

        public RuleMatch(ConfigurationElement element)
        {
            this.element = element;
        }
    }
}
