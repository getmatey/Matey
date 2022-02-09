using Microsoft.Extensions.Options;
using Administration = Microsoft.Web.Administration;
using System.Net;
using Matey.Frontend.IIS.Configuration;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace Matey.Frontend.IIS
{
    public class IISFrontend : IFrontend
    {
        private readonly IOptions<IISOptions> options;
        private readonly Administration.ServerManager serverManager;
        private readonly ILogger<IISFrontend> logger;

        public IISFrontend(IOptions<IISOptions> options, Administration.ServerManager serverManager, ILogger<IISFrontend> logger)
        {
            this.options = options;
            this.serverManager = serverManager;
            this.logger = logger;
        }

        private static string CreateUniqueFrontendName(string serviceProvider, string serviceName, string backendName)
        {
            return $"{serviceProvider}.{serviceName}.{backendName}";
        }

        public async Task HandleAsync(ServiceOnlineNotification notification, CancellationToken cancellationToken)
        {
            IServiceConfiguration service = notification.Configuration;
            foreach(IBackendServiceConfiguration backend in service.Backends)
            {
                IPAddress ipAddress = backend.IPAddress;
                int? port = backend.Port ?? 80;
                string rule = backend.Frontend.Rule;
                string hostname = rule.Substring("Host:".Length).Trim();
                string websiteName = CreateUniqueFrontendName(service.Provider, service.Name, backend.Name);
                string websitePath = Path.Combine(options.Value.WebsitesPath, websiteName);
                Directory.CreateDirectory(websitePath);

                WebConfiguration webConfiguration = new WebConfiguration()
                {
                    WebServer = new WebServer()
                    {
                        Rewrite = new Rewrite()
                        {
                            Rules = new List<RewriteRule>
                            {
                                new RewriteRule()
                                {
                                    Name = "InboundReverseProxyRule",
                                    Match = new RewriteMatchRule { Url = "(.*)" },
                                    Action = new RewriteActionRule { Type = "Rewrite", Url = $"http://{ipAddress}:{port}/{{R:1}}" }
                                }
                            }
                        }
                    }
                };

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(WebConfiguration));
                await using (XmlWriter writer = XmlWriter.Create(Path.Combine(websitePath, "web.config"), new XmlWriterSettings { Async = true }))
                {
                    xmlSerializer.Serialize(writer, webConfiguration);
                    await writer.FlushAsync();
                }

                Administration.Site site = serverManager.Sites.Add(websiteName, "http", $"*:80:{hostname}", websitePath);
                Administration.Configuration configuration = site.GetWebConfiguration();
                site.ServerAutoStart = true;
                serverManager.CommitChanges();

                logger.LogInformation("Added website '{0}'.", websiteName);
            }
        }

        public Task HandleAsync(ServiceOfflineNotification notification, CancellationToken cancellationToken)
        {
            foreach(string backend in notification.Backends)
            {
                string websiteName = CreateUniqueFrontendName(notification.Provider, notification.ServiceName, backend);
                string websitePath = Path.Combine(options.Value.WebsitesPath, websiteName);
                Administration.Site site = serverManager.Sites[websiteName];
                serverManager.Sites.Remove(site);
                serverManager.CommitChanges();
                Directory.Delete(websitePath, true);

                logger.LogInformation("Removed website '{0}'.", websiteName);
            }

            return Task.CompletedTask;
        }
    }
}
