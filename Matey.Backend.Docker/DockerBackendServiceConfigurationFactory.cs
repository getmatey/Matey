﻿using System.Net;

namespace Matey.Backend.Docker
{
    using Attributes;
    using global::Docker.DotNet.Models;
    using Matey.Backend.Abstractions;

    internal static class DockerBackendServiceConfigurationFactory
    {
        internal static DockerBackendServiceConfiguration Create(
            IAttributeSection attributes,
            IDictionary<string, EndpointSettings> networks,
            Func<IAttributeSection?, IFrontendServiceConfiguration> frontendConfigurationFactory,
            Func<IAttributeSection?, ILoadBalancerConfiguration> loadBalancerConfigurationFactory)
            => new DockerBackendServiceConfiguration(
                Name: attributes.Name,
                Protocol: attributes.GetString(Tokens.Protocol),
                Port: attributes.GetValue<int>(Tokens.Port),
                // TODO: Select network by name
                IPAddress: IPAddress.Parse(networks.Values.First().IPAddress),
                Weight: attributes.GetValue<int>(Tokens.Weight),
                Frontend: frontendConfigurationFactory(attributes.GetSection(Tokens.Frontend)),
                LoadBalancer: loadBalancerConfigurationFactory(attributes.GetSection(Tokens.LoadBalancer)));
    }
}
