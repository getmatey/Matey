namespace Matey.Frontend.Abstractions
{
    public interface IFrontend
    {
        string Name { get; }

        void AddSite(ReverseProxySite site);

        void UpdateSite(ReverseProxySite site);

        IEnumerable<SiteIdentifier> GetSiteIdentifiers();

        void RemoveSite(SiteIdentifier identifier);
    }
}
