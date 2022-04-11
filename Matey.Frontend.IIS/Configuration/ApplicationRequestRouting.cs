using Microsoft.Web.Administration;

namespace Matey.Frontend.IIS.Configuration
{
    internal class ApplicationRequestRouting
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

        public ApplicationRequestRoutingAffinity Affinity { get; }

        public ApplicationRequestRouting(ConfigurationElement element)
        {
            this.element = element;
            Affinity = new ApplicationRequestRoutingAffinity(element.GetChildElement("affinity"));
        }
    }
}
