namespace Matey.Frontend.Abstractions
{
    public interface IFrontend
    {
        string Name { get; }

        void AddReverseProxy(ReverseProxySite site);

        IEnumerable<SiteIdentifier> GetSiteIdentifiers();

        void RemoveSite(SiteIdentifier identifier);
    }
}
