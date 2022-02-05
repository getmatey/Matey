using Matey.Backend.Docker.Attributes;
using Matey.Frontend;

namespace Matey.Backend.Docker
{
    public record DockerBackendServiceConfiguration(
        string Name,
        IFrontendServiceConfiguration Frontend,
        int? Port) : IBackendServiceConfiguration;
}
