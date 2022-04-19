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
            IDictionary<IFrontend, IList<RequestRoute>> initializations = new Dictionary<IFrontend, IList<RequestRoute>>();
            foreach (IServiceConfiguration serviceConfiguration in backends.SelectMany(b => b.GetRunningServiceConfigurations()))
            {
                IFrontend frontend = SelectedFrontend(serviceConfiguration);
                foreach (RequestRoute rule in serviceConfiguration.ToRequestRoutes(requestRuleParser))
                {
                    IList<RequestRoute>? routeRules;
                    if(initializations.TryGetValue(frontend, out routeRules))
                    {
                        routeRules.Add(rule);
                    }
                    else
                    {
                        initializations[frontend] = new List<RequestRoute> { rule };
                    }
                }
            }

            foreach(KeyValuePair<IFrontend, IList<RequestRoute>> initialization in initializations)
            {
                try
                {
                    initialization.Key.InitializeRequestRoutes(initialization.Value);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to initialize front-end '{0}'.", initialization.Key.Name);
                }
            }
        }

        public Task HandleAsync(ServiceOnlineNotification notification, CancellationToken cancellationToken)
        {
            IServiceConfiguration serviceConfiguration = notification.Configuration;
            IFrontend target = SelectedFrontend(serviceConfiguration);
            IEnumerable<RequestRoute> routes = serviceConfiguration.ToRequestRoutes(requestRuleParser);

            foreach (RequestRoute route in routes)
            {
                try
                {
                    target.AddRequestRoute(route);

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to add route {0} -> {1}.", route.Rule, route.Endpoint);
                }
            }

            return Task.CompletedTask;
        }

        public Task HandleAsync(ServiceOfflineNotification notification, CancellationToken cancellationToken)
        {
            IServiceConfiguration configuration = notification.Configuration;

            IFrontend frontend = SelectedFrontend(configuration);
            foreach (IBackendServiceConfiguration backend in configuration.Backends)
            {
                IPEndPoint ipEndPoint = new IPEndPoint(backend.IPAddress, backend.Port ?? 80);

                try
                {
                    frontend.RemoveRequestRoutes(new ApplicationRequestEndpoint("http", ipEndPoint, backend.Weight));

                    logger.LogInformation("Removed route {0} -> {1}.", configuration.Domain, ipEndPoint);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to remove route to {1}.", ipEndPoint);
                }
            }

            return Task.CompletedTask;
        }
    }
}
