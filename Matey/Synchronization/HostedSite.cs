using Matey.Frontend.Abstractions;

namespace Matey.Synchronization
{
    public record HostedSite(SiteIdentifier Identifier, IFrontend Host);
}
