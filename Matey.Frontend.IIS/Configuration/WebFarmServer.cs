using Microsoft.Web.Administration;

namespace Matey.Frontend.IIS.Configuration
{
    internal class WebFarmServer
    {
        internal readonly ConfigurationElement element;

        public string? Address
        {
            get => element["address"] as string;
            set => element["address"] = value;
        }

        public WebFarmServerApplicationRequestRouting ApplicationRequestRouting { get; }

        public WebFarmServer(ConfigurationElement element)
        {
            this.element = element;
            ApplicationRequestRouting = new WebFarmServerApplicationRequestRouting(
                element.GetChildElement("applicationRequestRouting"));
        }
    }
}
