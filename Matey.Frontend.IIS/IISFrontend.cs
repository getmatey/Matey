
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
            return $"{options.Value.SiteNamePrefix}{string.Join(options.Value.SiteNameDelimiter, identifier.Value)}";
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
                        Rules = new List<RewriteRule>
                        {
                            new RewriteRule()
                            {
                                Name = "InboundReverseProxyRule",
                                Match = new RewriteMatchRule { Url = "(.*)" },
                                Action = new RewriteRuleAction { Type = "Rewrite", Url = $"http://{inboundProxy.Destination.IPEndPoint}/{{R:1}}" }
                            }
                        }
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

        public IEnumerable<ReverseProxySite> GetInboundProxies()
        {
            IList<ReverseProxySite> inboundProxies = new List<ReverseProxySite>();
            foreach(Administration.Site site in serverManager.Sites.Where(s => s.Name.StartsWith(options.Value.SiteNamePrefix)))
            {
                string identifierString = site.Name.Substring(options.Value.SiteNamePrefix.Length);
                string websitePath = CreateWebsitePath(site.Name);
                SiteIdentifier identifier = SiteIdentifier.Create(identifierString.Split(options.Value.SiteNameDelimiter));

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(WebConfiguration));
                using (XmlReader reader = XmlReader.Create(CreateWebsiteConfigPath(websitePath)))
                {
                    WebConfiguration? configuration = (WebConfiguration?)xmlSerializer.Deserialize(reader);
                    if(configuration is not null)
                    {
                        Administration.Binding binding = site.Bindings.First();
                        RewriteRule? rule = configuration?.WebServer?.Rewrite?.Rules.FirstOrDefault();
                        RewriteRuleAction? action = rule?.Action;

                        if(action?.Url is not null)
                        {
                            Uri? uri = new Uri(action.Url);

                            inboundProxies.Add(
                                new ReverseProxySite(
                                    identifier,
                                    binding.Host,
                                    binding.EndPoint.Port,
                                    new ProxyForwardDestination(uri.Scheme, new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port))));
                        }
                    }
                }
            }

            return inboundProxies;
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
