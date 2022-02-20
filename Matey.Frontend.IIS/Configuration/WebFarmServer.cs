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

        public ApplicationRequestRouting ApplicationRequestRouting { get; }

        public WebFarmServer(ConfigurationElement element)
        {
            this.element = element;
            ApplicationRequestRouting = new ApplicationRequestRouting(
                element.GetChildElement("applicationRequestRouting"));
        }
    }
}
