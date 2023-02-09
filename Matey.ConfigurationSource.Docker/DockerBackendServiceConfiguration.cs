using System.Net;

namespace Matey.ConfigurationSource.Docker
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
