namespace Matey
{
    using ConfigurationSource.Abstractions;
    using Matey.Acme;
    using Matey.WebServer.Abstractions.Rules;
    using Rules;
    using System.Security.Cryptography.X509Certificates;
    using WebServer.Abstractions;

    public class ServiceBroker : IServiceBroker
    {
        private readonly IDictionary<string, IWebServer> frontends;
        private readonly IEnumerable<IConfigurationSource> backends;
        private readonly IRequestRuleParser requestRuleParser;
        private readonly IAcmeCertificateIssuer certificateIssuer;
        private readonly ILogger<ServiceBroker> logger;

        public ServiceBroker(
            IEnumerable<IWebServer> frontends,
            IEnumerable<IConfigurationSource> backends,
            IRequestRuleParser requestRuleParser,
            IAcmeCertificateIssuer certificateIssuer,
            ILogger<ServiceBroker> logger)
        {
            this.frontends = frontends.ToDictionary(f => f.Name);
            this.backends = backends;
            this.requestRuleParser = requestRuleParser;
            this.certificateIssuer = certificateIssuer;
            this.logger = logger;
        }


        private IWebServer DefaultFrontend()
        {
            return frontends.First().Value;
        }

        private IWebServer SelectedFrontend(IServiceConfiguration serviceConfiguration)
        {
            return serviceConfiguration.Target == null ? DefaultFrontend() : frontends[serviceConfiguration.Target];
        }

        public void Synchronize()
        {
            IDictionary<IWebServer, IList<RequestRoute>> initializations = new Dictionary<IWebServer, IList<RequestRoute>>();
            foreach (IServiceConfiguration serviceConfiguration in backends.SelectMany(b => b.GetRunningServiceConfigurations()))
            {
                IWebServer frontend = SelectedFrontend(serviceConfiguration);
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

            foreach(KeyValuePair<IWebServer, IList<RequestRoute>> initialization in initializations)
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

        public async Task HandleAsync(ServiceOnlineNotification notification, CancellationToken cancellationToken)
        {
            IServiceConfiguration serviceConfiguration = notification.Configuration;
            IWebServer target = SelectedFrontend(serviceConfiguration);
            IEnumerable<RequestRoute> routes = serviceConfiguration.ToRequestRoutes(requestRuleParser);

            foreach (RequestRoute route in routes)
            {
                try
                {
                    target.AddRequestRoute(route);

                    HostRequestRule hostRequestRule = route.Rule.ToEnumerable().OfType<HostRequestRule>().First();
                    if (target.CertificateStore != null)
                    {
                        X509Certificate2 certificate = await certificateIssuer.OrderCertificateAsync(hostRequestRule.Host);
                        
                        await target.CertificateStore.InstallCertificateAsync(certificate);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to add route {0} -> {1}.", route.Rule, route.Endpoint);
                }
            }

        }

        public Task HandleAsync(ServiceOfflineNotification notification, CancellationToken cancellationToken)
        {
            IServiceConfiguration configuration = notification.Configuration;

            IWebServer frontend = SelectedFrontend(configuration);
            foreach (IBackendServiceConfiguration backend in configuration.Backends)
            {
                ApplicationRequestEndpoint endpoint = backend.ToApplicationRequestEndpoint();

                try
                {
                    frontend.RemoveRequestRoutes(endpoint);

                    logger.LogInformation("Removed route {0} -> {1}.", backend.Domain, endpoint.IPEndPoint);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to remove route {0} -> {1}.", backend.Domain, endpoint.IPEndPoint);
                }
            }

            return Task.CompletedTask;
        }
    }
}
