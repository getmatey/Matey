using Microsoft.Web.Administration;

namespace Matey.Frontend.IIS.Configuration
{
    internal class WebFarmApplicationRequestRouting
    {
        public ApplicationRequestRoutingAffinity Affinity { get; }

        public WebFarmApplicationRequestRouting(ConfigurationElement element)
        {
            Affinity = new ApplicationRequestRoutingAffinity(element.GetChildElement("affinity"));
        }
    }
}
