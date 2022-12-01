using System.Net;

namespace Matey.Backend.Docker
{
    using Abstractions;

    public record DockerBackendServiceConfiguration(
        string Name,
        string? Protocol,
        IPAddress IPAddress,
        int? Port,
        int? Weight,
        IFrontendServiceConfiguration Frontend,
        ILoadBalancerConfiguration LoadBalancer) : IBackendServiceConfiguration;
}
