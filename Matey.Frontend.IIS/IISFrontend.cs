
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
            return $"{identifier}{options.Value.ServerFarmDelimiter}{options.Value.ServerFarmSuffix}";
        }

        private static string CreateRewriteRuleName(string websiteName) => $"ARR_{websiteName}_loadbalance";

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
            Administration.Configuration config = serverManager.GetApplicationHostConfiguration();
            Administration.ConfigurationElementCollection webFarmsCollection = config
                .GetSection("webFarms")
                .GetCollection();
            Administration.ConfigurationElement webFarmElement = webFarmsCollection.CreateElement("webFarm");
            webFarmElement["name"] = websiteName;

            Administration.ConfigurationElementCollection webFarmCollection = webFarmElement.GetCollection();
            foreach (ProxyForwardDestination destination in site.Destinations)
            {
                Administration.ConfigurationElement serverElement = webFarmCollection.CreateElement("server");
                serverElement["address"] = destination.IPEndPoint.Address.ToString();
                webFarmCollection.Add(serverElement);
            }
            webFarmsCollection.Add(webFarmElement);

            Administration.SectionGroup webServer = config.RootSectionGroup.SectionGroups.First(g => g.Name == "system.webServer");
            Administration.ConfigurationElementCollection globalRulesCollection = config
                .GetSection("system.webServer/rewrite/globalRules")
                .GetCollection();
            Administration.ConfigurationElement ruleElement = globalRulesCollection.CreateElement("rule");
            ruleElement["name"] = CreateRewriteRuleName(websiteName);
            ruleElement["patternSyntax"] = "Wildcard";
            ruleElement["stopProcessing"] = "true";
            
            Administration.ConfigurationElement matchElement = ruleElement.GetChildElement("match");
            matchElement["url"] = "*";

            Administration.ConfigurationElement actionElement = ruleElement.GetChildElement("action");
            actionElement["type"] = "Rewrite";
            actionElement["url"] = $"http://{websiteName}/{{R:0}}";

            globalRulesCollection.Add(ruleElement);

            serverManager.CommitChanges();

            logger.LogInformation("Added load balancer '{0}'.", websiteName);
        }

        public void UpdateSite(ReverseProxySite site)
        {
            //string websiteName = CreateWebsiteName(site.Identifier);
            //string websitePath = CreateWebsitePath(websiteName);

            //SetWebsiteConfiguration(websitePath, CreateWebConfiguration(site));

            //logger.LogInformation("Updated website '{0}'.", websiteName);
        }

        public IEnumerable<SiteIdentifier> GetSiteIdentifiers()
        {
            string suffix = $"{options.Value.ServerFarmDelimiter}{options.Value.ServerFarmSuffix}";
            return serverManager.GetApplicationHostConfiguration()
                .GetSection("webFarms")
                .GetCollection()
                .Select(f => (string)f["name"])
                .Where(n => n.EndsWith(suffix))
                .Select(n => new SiteIdentifier(n.Substring(0, n.LastIndexOf(suffix))));
        }

        public void RemoveSite(SiteIdentifier identifier)
        {
            string websiteName = CreateWebsiteName(identifier);
            string rewriteRuleName = CreateRewriteRuleName(websiteName);

            Administration.ConfigurationElementCollection globalRulesCollection = serverManager.GetApplicationHostConfiguration()
                .GetSection("system.webServer/rewrite/globalRules")
                .GetCollection();
            globalRulesCollection.Remove(globalRulesCollection.First(r => r["name"] as string == rewriteRuleName));

            Administration.ConfigurationElementCollection webFarmsCollection = serverManager.GetApplicationHostConfiguration()
                .GetSection("webFarms")
                .GetCollection();
            webFarmsCollection.Remove(webFarmsCollection.First(f => f["name"] as string == websiteName));

            serverManager.CommitChanges();

            logger.LogInformation("Removed load balancer '{0}'.", websiteName);
        }
    }
}
