using Matey.Backend.Abstractions;
using Matey.Frontend.Abstractions;
using Matey.Frontend.Abstractions.Rules;
using System.Net;

namespace Matey
{
    internal static class Extensions
    {
        internal static ApplicationRequestEndpoint CreateApplicationRequestEndpoint(this IBackendServiceConfiguration backendServiceConfiguration)
        {
            return new ApplicationRequestEndpoint(
                "http",
                new IPEndPoint(backendServiceConfiguration.IPAddress, backendServiceConfiguration.Port ?? 80));
        }

        internal static IEnumerable<RequestRouteRule> CreateRequestRouteRules(
            this IServiceConfiguration serviceConfiguration,
            IRequestRuleParser ruleParser)
        {
            foreach(IBackendServiceConfiguration backend in serviceConfiguration.Backends)
            {
                IRequestRule rule;
                if (backend.Frontend.Rule is null)
                {
                    rule = new HostRequestRule($"{serviceConfiguration.Name}.{serviceConfiguration.Domain}");
                }
                else
                {
                    rule = ruleParser.Parse(backend.Frontend.Rule); 
                }
                
                yield return new RequestRouteRule(rule, backend.CreateApplicationRequestEndpoint());
            }
        }
    }
}
