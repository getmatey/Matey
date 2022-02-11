using System.Net;

namespace Matey.Backend.Docker
{
    using Abstractions;

    public record DockerBackendServiceConfiguration(
        string Name,
        IFrontendServiceConfiguration Frontend,
        IPAddress IPAddress,
        int? Port) : IBackendServiceConfiguration;
}
