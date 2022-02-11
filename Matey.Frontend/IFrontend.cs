namespace Matey.Frontend.Abstractions
{
    using Common;

    public interface IFrontend
    {
        string Name { get; }

        Task AddInboundProxyAsync(InboundProxySite site);

        Task RemoveSiteAsync(SiteIdentifier identifier);
    }
}
