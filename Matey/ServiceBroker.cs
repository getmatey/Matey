using System.Net;

namespace Matey
{
    using Backend.Abstractions;
    using Frontend.Abstractions;
    using Synchronization;

    public class ServiceBroker : IServiceBroker
    {
        private readonly IDictionary<string, IFrontend> frontends;
        private readonly IEnumerable<IBackend> backends;
        private readonly ILogger<ServiceBroker> logger;

        public ServiceBroker(
            IEnumerable<IFrontend> frontends,
            IEnumerable<IBackend> backends,
            ILogger<ServiceBroker> logger)
        {
            this.frontends = frontends.ToDictionary(f => f.Name);
            this.backends = backends;
            this.logger = logger;
        }


        private IFrontend DefaultFrontend()
        {
            return frontends.First().Value;
        }

        private IFrontend SelectedFrontend(IServiceConfiguration serviceConfiguration)
        {
            return serviceConfiguration.Target == null ? DefaultFrontend() : frontends[serviceConfiguration.Target];
        }

        public void Synchronize()
        {
            SitesSynchronizer synchronizer = new SitesSynchronizer();
            foreach(IFrontend frontend in frontends.Values)
            {
                foreach(SiteIdentifier siteIdentifier in frontend.GetSiteIdentifiers())
                {
                    synchronizer.Add(new HostedSite(siteIdentifier, frontend));
                }
            }

            foreach(IServiceConfiguration serviceConfiguration in backends.SelectMany(b => b.GetRunningServiceConfigurations()))
            {
                synchronizer.Add(new ReverseProxySpecification(
                    serviceConfiguration.CreateReverseProxySite(),
                    SelectedFrontend(serviceConfiguration)));
            }

            synchronizer.Synchronize();
        }

        public Task HandleAsync(ServiceOnlineNotification notification, CancellationToken cancellationToken)
        {
            IServiceConfiguration serviceConfiguration = notification.Configuration;
            IFrontend target = SelectedFrontend(serviceConfiguration);

            target.AddReverseProxy(serviceConfiguration.CreateReverseProxySite());
            return Task.CompletedTask;
        }

        public Task HandleAsync(ServiceOfflineNotification notification, CancellationToken cancellationToken)
        {
            SiteIdentifier identifier = new SiteIdentifier(notification.Provider, notification.ServiceName, notification.ServiceId);

            // TODO: Read front-end
            IFrontend frontend = DefaultFrontend();
            frontend.RemoveSite(identifier);

            return Task.CompletedTask;
        }
    }
}
