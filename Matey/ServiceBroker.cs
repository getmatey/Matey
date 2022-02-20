namespace Matey
{
    using Backend.Abstractions;
    using Frontend.Abstractions;
    using System.Net;

    public class ServiceBroker : IServiceBroker
    {
        private readonly IDictionary<string, IFrontend> frontends;
        private readonly IEnumerable<IBackend> backends;
        private readonly IRequestRuleParser requestRuleParser;
        private readonly ILogger<ServiceBroker> logger;

        public ServiceBroker(
            IEnumerable<IFrontend> frontends,
            IEnumerable<IBackend> backends,
            IRequestRuleParser requestRuleParser,
            ILogger<ServiceBroker> logger)
        {
            this.frontends = frontends.ToDictionary(f => f.Name);
            this.backends = backends;
            this.requestRuleParser = requestRuleParser;
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
            IDictionary<IFrontend, IList<RequestRouteRule>> initializations = new Dictionary<IFrontend, IList<RequestRouteRule>>();
            foreach (IServiceConfiguration serviceConfiguration in backends.SelectMany(b => b.GetRunningServiceConfigurations()))
            {
                IFrontend frontend = SelectedFrontend(serviceConfiguration);
                foreach (RequestRouteRule rule in serviceConfiguration.CreateRequestRouteRules(requestRuleParser))
                {
                    IList<RequestRouteRule>? routeRules;
                    if(initializations.TryGetValue(frontend, out routeRules))
                    {
                        routeRules.Add(rule);
                    }
                    else
                    {
                        initializations[frontend] = new List<RequestRouteRule> { rule };
                    }
                }
            }

            foreach(KeyValuePair<IFrontend, IList<RequestRouteRule>> initialization in initializations)
            {
                initialization.Key.InitializeRequestRoutes(initialization.Value);
            }
        }

        public Task HandleAsync(ServiceOnlineNotification notification, CancellationToken cancellationToken)
        {
            IServiceConfiguration serviceConfiguration = notification.Configuration;
            IFrontend target = SelectedFrontend(serviceConfiguration);
            IEnumerable<RequestRouteRule> rules = serviceConfiguration.CreateRequestRouteRules(requestRuleParser);

            foreach (RequestRouteRule rule in rules)
            {
                target.AddRequestRoute(rule);
            }

            return Task.CompletedTask;
        }

        public Task HandleAsync(ServiceOfflineNotification notification, CancellationToken cancellationToken)
        {
            IServiceConfiguration configuration = notification.Configuration;

            IFrontend frontend = SelectedFrontend(configuration);
            foreach (IBackendServiceConfiguration backend in configuration.Backends)
            {
                frontend.RemoveRequestRoutes(new ApplicationRequestEndpoint("http", new IPEndPoint(backend.IPAddress, backend.Port ?? 80)));
            }

            return Task.CompletedTask;
        }
    }
}
