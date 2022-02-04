using Matey.Backend.Docker.Attributes;
using Matey.Frontend;
using System.Collections.Immutable;

namespace Matey.Backend.Docker
{
    internal class DockerServiceConfiguration : IServiceConfiguration
    {
        private ImmutableArray<DockerBackendServiceConfiguration> configurations;

        public IEnumerable<IBackendServiceConfiguration> Backends => configurations;

        public bool IsEnabled { get; }
    
        internal DockerServiceConfiguration(IAttributeRoot attributes)
        {
            IsEnabled = attributes.GetValue<bool>(Tokens.Enabled) ?? true;
            IList<DockerBackendServiceConfiguration> backends = new List<DockerBackendServiceConfiguration>();
            foreach(var section in attributes.Sections.Where(s => !Tokens.Reserved.Contains(s.Name)))
            {
                backends.Add(new DockerBackendServiceConfiguration(section));
            }
            configurations = backends.ToImmutableArray();
        }
    }
}
