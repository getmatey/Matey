﻿using System.Net;

namespace Matey.Backend.Abstractions
{
    public interface IBackendServiceConfiguration
    {
        string Name { get; }

        string? Protocol { get; }

        IPAddress IPAddress { get; }

        int? Port { get; }

        int? Weight { get; }

        IFrontendServiceConfiguration Frontend { get; }

        ILoadBalancerConfiguration LoadBalancer { get; }
    }
}
