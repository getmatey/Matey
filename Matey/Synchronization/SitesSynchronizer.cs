namespace Matey.Synchronization
{
    using Frontend.Abstractions;

    public class SitesSynchronizer
    {
        private readonly IDictionary<SiteIdentifier, SiteSynchronizer> synchronizers = new Dictionary<SiteIdentifier, SiteSynchronizer>();

        private SiteSynchronizer GetOrAdd(SiteIdentifier identifier)
        {
            if (!synchronizers.TryGetValue(identifier, out SiteSynchronizer? link))
            {
                link = new SiteSynchronizer();
                synchronizers.Add(identifier, link);
            }

            return link;
        }

        public void Add(HostedSite hostedSite)
        {
            SiteSynchronizer synchronizer = GetOrAdd(hostedSite.Identifier);
            synchronizer.HostedSite = hostedSite;
        }

        public void Add(ReverseProxySpecification specification)
        {
            SiteSynchronizer synchronizer = GetOrAdd(specification.Configuration.Identifier);
            synchronizer.ReverseProxySpecification = specification;
        }

        public void Synchronize()
        {
            foreach (SiteSynchronizer synchronizer in synchronizers.Values)
            {
                synchronizer.Synchronize();
            }
        }
    }
}
