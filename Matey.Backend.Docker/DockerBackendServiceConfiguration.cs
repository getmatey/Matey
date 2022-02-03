using Matey.Backend.Docker.Attributes;
using Matey.Frontend;

namespace Matey.Backend.Docker
{
    public class DockerBackendServiceConfiguration : IBackendServiceConfiguration
    {
        private IAttributeRoot attributes;

        public DockerBackendServiceConfiguration(IAttributeRoot attributes)
        {
            this.attributes = attributes;
        }

        public IFrontendServiceConfiguration Frontend => throw new NotImplementedException();

        public int? Port => attributes.GetValue<int>(Paths.Port);

        public bool IsEnabled => attributes.GetValue<bool>(Paths.Enabled) ?? true;
    }
}
