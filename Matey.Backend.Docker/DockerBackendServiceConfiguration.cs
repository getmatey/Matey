using Matey.Backend.Docker.Attributes;
using Matey.Frontend;

namespace Matey.Backend.Docker
{
    public class DockerBackendServiceConfiguration : IBackendServiceConfiguration
    {
        public string Name { get; }

        public IFrontendServiceConfiguration Frontend { get; }

        public int? Port { get; }

        public DockerBackendServiceConfiguration(IAttributeSection attributes)
        {
            Name = attributes.Name;
            Port = attributes.GetValue<int>(Tokens.Port);
            Frontend = new DockerFrontendServiceConfiguration(attributes.GetSection(Tokens.Frontend));
        }
    }
}
