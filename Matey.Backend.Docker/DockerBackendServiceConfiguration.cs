using Matey.Frontend;
using System.Net;

namespace Matey.Backend.Docker
{
    public record DockerBackendServiceConfiguration(
        string Name,
        IFrontendServiceConfiguration Frontend,
        IPAddress IPAddress,
        int? Port) : IBackendServiceConfiguration;
}
