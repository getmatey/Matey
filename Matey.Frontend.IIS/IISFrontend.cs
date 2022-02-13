
using Microsoft.Extensions.Options;
using Administration = Microsoft.Web.Administration;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace Matey.Frontend.IIS
{
    using Abstractions;
    using Configuration;
    using System.Net;

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
            return $"{options.Value.SiteNamePrefix}{identifier.ToString(options.Value.SiteNameDelimiter)}";
        }

        private string CreateWebsitePath(string websiteName) => Path.Combine(options.Value.WebsitesPath, websiteName);

        private string CreateWebsiteConfigPath(string websitePath) => Path.Combine(websitePath, "web.config");

        public void AddReverseProxy(ReverseProxySite inboundProxy)
        {
            string websiteName = CreateWebsiteName(inboundProxy.Identifier);
            string websitePath = CreateWebsitePath(websiteName);
            Directory.CreateDirectory(websitePath);

            WebConfiguration webConfiguration = new WebConfiguration()
            {
                WebServer = new WebServer()
                {
                    Rewrite = new Rewrite()
                    {
                        Rules = inboundProxy.Destinations.Select(d => new RewriteRule()
                        {
                            Name = $"{d.Name}InboundReverseProxyRule",
                            Match = new RewriteMatchRule { Url = "(.*)" },
                            Action = new RewriteRuleAction { Type = "Rewrite", Url = $"http://{d.IPEndPoint}/{{R:1}}" }
                        }).ToList()
                    }
                }
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(WebConfiguration));
            using (XmlWriter writer = XmlWriter.Create(CreateWebsiteConfigPath(websitePath), new XmlWriterSettings { Async = true }))
            {
                xmlSerializer.Serialize(writer, webConfiguration);
                writer.Flush();
            }

            Administration.Site site = serverManager.Sites.Add(websiteName, "http", $"*:{inboundProxy.Port}:{inboundProxy.Domain}", websitePath);
            site.ServerAutoStart = true;
            serverManager.CommitChanges();

            logger.LogInformation("Added website '{0}'.", websiteName);
        }

        public IEnumerable<SiteIdentifier> GetSiteIdentifiers()
        {
            return serverManager.Sites
                .Where(s => s.Name.StartsWith(options.Value.SiteNamePrefix))
                .Select(s => s.Name.Substring(options.Value.SiteNamePrefix.Length))
                .Select(i => i.Split(options.Value.SiteNameDelimiter))
                .Where(p => p.Length > 2)
                .Select(p => new SiteIdentifier(Provider: p[0], Name: p[1], Id: p[2]));
        }

        public void RemoveSite(SiteIdentifier identifier)
        {
            string websiteName = CreateWebsiteName(identifier);
            string websitePath = CreateWebsitePath(websiteName);
            Administration.Site site = serverManager.Sites[websiteName];

            serverManager.Sites.Remove(site);
            serverManager.CommitChanges();
            Directory.Delete(websitePath, recursive: true);

            logger.LogInformation("Removed website '{0}'.", websiteName);
        }
    }
}
