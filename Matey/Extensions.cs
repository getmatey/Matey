using Matey.Backend.Abstractions;
using Matey.Frontend.Abstractions;
using System.Net;

namespace Matey
{
    internal static class Extensions
    {
        internal static SiteIdentifier CreateSiteIdentifier(this IServiceConfiguration serviceConfiguration)
        {
            return new SiteIdentifier(serviceConfiguration.Provider, serviceConfiguration.Name, serviceConfiguration.Id);
        }

        internal static ReverseProxySite CreateReverseProxySite(this IServiceConfiguration serviceConfiguration)
        {
            SiteIdentifier siteIdentifier = serviceConfiguration.CreateSiteIdentifier();
            return new ReverseProxySite(
                siteIdentifier,
                serviceConfiguration.Domain, 80,
                serviceConfiguration.Backends.Select(b => new ProxyForwardDestination(
                    b.Name,
                    "http",
                    new IPEndPoint(b.IPAddress, b.Port ?? 80))));
        }
    }
}
