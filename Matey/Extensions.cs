using System.Net;

namespace Matey
{
    using ConfigurationSource.Abstractions;
    using Rules;
    using WebServer.Abstractions;
    using WebServer.Abstractions.Rules;

    internal static class Extensions
    {
        internal static ApplicationRequestEndpoint ToApplicationRequestEndpoint(this IBackendServiceConfiguration backendServiceConfiguration)
        {
            return new ApplicationRequestEndpoint(
                Scheme: backendServiceConfiguration.Protocol ?? BackendServiceConfigurationDefaults.Protocol,
                IPEndPoint: new IPEndPoint(
                    backendServiceConfiguration.IPAddress,
                    backendServiceConfiguration.Port ?? BackendServiceConfigurationDefaults.Port),
                Weight: backendServiceConfiguration.Weight);
        }

        internal static IEnumerable<RequestRoute> ToRequestRoutes(
            this IServiceConfiguration serviceConfiguration,
            IRequestRuleParser ruleParser)
        {
            foreach(IBackendServiceConfiguration backend in serviceConfiguration.Backends)
            {
                ILoadBalancerStickinessConfiguration stickinessConfiguration = backend.LoadBalancer.Stickiness;
                IRequestRule rule;
                if (backend.Frontend.Rule is null)
                {
                    rule = new HostRequestRule($"{serviceConfiguration.Name}.{backend.Domain}");
                }
                else
                {
                    rule = ruleParser.Parse(backend.Frontend.Rule); 
                }

                yield return new RequestRoute(
                    rule,
                    backend.ToApplicationRequestEndpoint(),
                    new RequestRouteStickinessSettings(
                        stickinessConfiguration.IsEnabled ?? false,
                        stickinessConfiguration.CookieName));
            }
        }
    }
}
