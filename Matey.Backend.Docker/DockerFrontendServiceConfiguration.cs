using Matey.Backend.Docker.Attributes;
using Matey.Frontend;

namespace Matey.Backend.Docker
{
    internal class DockerFrontendServiceConfiguration : IFrontendServiceConfiguration
    {
        public string Rule { get; }

        public DockerFrontendServiceConfiguration(IAttributeSection section)
        {
            Rule = section.GetString(Tokens.Rule);
        }
    }
}
