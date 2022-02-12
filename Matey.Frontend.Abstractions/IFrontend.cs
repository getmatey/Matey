namespace Matey.Frontend.Abstractions
{
    public interface IFrontend
    {
        string Name { get; }

        void AddReverseProxy(ReverseProxySite site);

        IEnumerable<ReverseProxySite> GetInboundProxies();

        void RemoveSite(SiteIdentifier identifier);
    }
}
