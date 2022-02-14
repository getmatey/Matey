
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Xml;
using System.Xml.Serialization;
using Administration = Microsoft.Web.Administration;

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
            return $"{options.Value.SiteNamePrefix}{identifier.ToString(options.Value.SiteNameDelimiter)}";
        }

        private string CreateWebsitePath(string websiteName) => Path.Combine(options.Value.WebsitesPath, websiteName);

        private string CreateWebsiteConfigPath(string websitePath) => Path.Combine(websitePath, "web.config");

        private void SetWebsiteConfiguration(string websitePath, WebConfiguration webConfiguration)
        {
            string configPath = CreateWebsiteConfigPath(websitePath);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(WebConfiguration));
            using (XmlWriter writer = XmlWriter.Create(configPath, new XmlWriterSettings { Async = true }))
            {
                xmlSerializer.Serialize(writer, webConfiguration);
                writer.Flush();
            }
        }

        private WebConfiguration CreateWebConfiguration(ReverseProxySite site)
        {
            return new WebConfiguration()
            {
                WebServer = new WebServer()
                {
                    Rewrite = new Rewrite()
                    {
                        Rules = site.Destinations.Select(d => new RewriteRule()
                        {
                            Name = $"{d.Name}InboundReverseProxyRule",
                            Match = new RewriteMatchRule { Url = "(.*)" },
                            Action = new RewriteRuleAction { Type = "Rewrite", Url = $"http://{d.IPEndPoint}/{{R:1}}" }
                        }).ToList()
                    }
                }
            };
        }

        public void AddSite(ReverseProxySite site)
        {
            string websiteName = CreateWebsiteName(site.Identifier);
            string websitePath = CreateWebsitePath(websiteName);

            Directory.CreateDirectory(websitePath);
            SetWebsiteConfiguration(websitePath, CreateWebConfiguration(site));

            Administration.Site administration = serverManager.Sites.Add(websiteName, "http", $"*:{site.Port}:{site.Domain}", websitePath);
            administration.ServerAutoStart = true;
            serverManager.CommitChanges();

            logger.LogInformation("Added website '{0}'.", websiteName);
        }

        public void UpdateSite(ReverseProxySite site)
        {
            string websiteName = CreateWebsiteName(site.Identifier);
            string websitePath = CreateWebsitePath(websiteName);

            SetWebsiteConfiguration(websitePath, CreateWebConfiguration(site));

            logger.LogInformation("Updated website '{0}'.", websiteName);
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
