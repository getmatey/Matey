using Microsoft.Web.Administration;

namespace Matey.WebServer.IIS.Configuration
{
    internal class WebFarmServerApplicationRequestRouting
    {
        private readonly ConfigurationElement element;

        public int? HttpPort
        {
            get => element["httpPort"] as int?;
            set => element["httpPort"] = value;
        }

        public int? Weight
        {
            get => element["weight"] as int?;
            set => element["weight"] = value;
        }

        public WebFarmServerApplicationRequestRouting(ConfigurationElement element)
        {
            this.element = element;
        }
    }
}
