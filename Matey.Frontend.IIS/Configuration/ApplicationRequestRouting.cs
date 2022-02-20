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

        public ApplicationRequestRouting(ConfigurationElement element)
        {
            this.element = element;
        }
    }
}
