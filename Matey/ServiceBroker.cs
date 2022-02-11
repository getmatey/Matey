using System.Net;

namespace Matey
{
    using Common;
    using Backend.Abstractions;
    using Frontend.Abstractions;

    public class ServiceBroker : INotificationHandler<ServiceOnlineNotification>, INotificationHandler<ServiceOfflineNotification>
    {
        private readonly IDictionary<string, IFrontend> frontends;

        public ServiceBroker(IEnumerable<IFrontend> frontends)
        {
            this.frontends = frontends.ToDictionary(f => f.Name);
        }

        private IFrontend DefaultFrontend()
        {
            return frontends.First().Value;
        }

        public async Task HandleAsync(ServiceOnlineNotification notification, CancellationToken cancellationToken)
        {
            foreach(IBackendServiceConfiguration backend in notification.Configuration.Backends)
            {
                (IFrontendServiceConfiguration Frontend,
                IBackendServiceConfiguration Backend,
                IServiceConfiguration Service) Configuration = (backend.Frontend, backend, notification.Configuration);

                IFrontend frontend = Configuration.Frontend.Provider == null ? DefaultFrontend() : frontends[Configuration.Frontend.Provider];
                SiteIdentifier identifier = SiteIdentifier.Create(Configuration.Service.Provider, Configuration.Service.Name, Configuration.Backend.Name);
                string hostname = Configuration.Frontend.Rule.Substring("Host:".Length).Trim();

                await frontend.AddInboundProxyAsync(
                    new InboundProxySite(
                        identifier,
                        hostname, 80,
                        new ProxyForwardDestination(
                            "http",
                            new IPEndPoint(Configuration.Backend.IPAddress, Configuration.Backend.Port ?? 80))));
            }
        }

        public async Task HandleAsync(ServiceOfflineNotification notification, CancellationToken cancellationToken)
        {
            foreach(string backend in notification.Backends)
            {
                SiteIdentifier identifier = SiteIdentifier.Create(notification.Provider, notification.ServiceName, backend);
                // TODO: Read front-end
                IFrontend frontend = DefaultFrontend();
                await frontend.RemoveSiteAsync(identifier);
            }
        }
    }
}
