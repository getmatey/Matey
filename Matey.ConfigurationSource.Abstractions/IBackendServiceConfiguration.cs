using System.Net;

namespace Matey.ConfigurationSource.Abstractions
{
    public interface IBackendServiceConfiguration
    {
        string Name { get; }

        string? Protocol { get; }

        IPAddress IPAddress { get; }

        int? Port { get; }

        int? Weight { get; }

        string Domain { get; }

        IFrontendServiceConfiguration Frontend { get; }

        ILoadBalancerConfiguration LoadBalancer { get; }
    }
}
