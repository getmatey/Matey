
using Microsoft.Extensions.Options;
using Administration = Microsoft.Web.Administration;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace Matey.Frontend.IIS
{
    using Abstractions;
    using Configuration;

    public class IISFrontend : IFrontend
    {
        private readonly IOptions<IISOptions> options;
        private readonly Administration.ServerManager serverManager;
        private readonly ILogger<IISFrontend> logger;

        public string Name => "IIS";

        public IISFrontend(IOptions<IISOptions> options, Administration.ServerManager serverManager, ILogger<IISFrontend> logger)
        {
            this.options = options;
            this.serverManager = serverManager;
            this.logger = logger;
        }

        private string CreateWebsiteName(SiteIdentifier identifier)
        {
            return string.Join(options.Value.SiteIdentifierDelimiter, identifier.Value);
        }

        public async Task AddInboundProxyAsync(InboundProxySite specification)
        {
            string websiteName = CreateWebsiteName(specification.Identifier);
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
                                    Action = new RewriteActionRule { Type = "Rewrite", Url = $"http://{specification.Destination.IPEndPoint}/{{R:1}}" }
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

            Administration.Site site = serverManager.Sites.Add(websiteName, "http", $"*:80:{specification.Hostname}", websitePath);
            Administration.Configuration configuration = site.GetWebConfiguration();
            site.ServerAutoStart = true;
            serverManager.CommitChanges();

            logger.LogInformation("Added website '{0}'.", websiteName);
        }

        public Task RemoveSiteAsync(SiteIdentifier identifier)
        {
            string websiteName = CreateWebsiteName(identifier);
            string websitePath = Path.Combine(options.Value.WebsitesPath, websiteName);
            Administration.Site site = serverManager.Sites[websiteName];
            serverManager.Sites.Remove(site);
            serverManager.CommitChanges();
            Directory.Delete(websitePath, true);

            logger.LogInformation("Removed website '{0}'.", websiteName);

            return Task.CompletedTask;
        }
    }
}
