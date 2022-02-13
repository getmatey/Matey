namespace Matey.Synchronization
{
    public class SiteSynchronizer
    {
        private HostedSite? hostedSite;
        private ReverseProxySpecification? reverseProxySpecification;

        public HostedSite? HostedSite {
            get => hostedSite;
            set {
                if(ReverseProxySpecification is not null)
                {
                    // TODO: Sanity checks
                }

                hostedSite = value;
            }
        }

        public ReverseProxySpecification? ReverseProxySpecification {
            get => reverseProxySpecification;
            set
            {
                if(HostedSite is not null)
                {
                    // TODO: Sanity checks
                }

                reverseProxySpecification = value;
            }
        }

        public void Synchronize()
        {
            if (HostedSite is null && ReverseProxySpecification is not null)
            {
                ReverseProxySpecification.Target.AddReverseProxy(ReverseProxySpecification.Configuration);
            }
            else if (HostedSite is not null && ReverseProxySpecification is null)
            {
                HostedSite.Host.RemoveSite(HostedSite.Identifier);
            }
        }
    }
}
