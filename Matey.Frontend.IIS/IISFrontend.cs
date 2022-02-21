
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Xml;
using System.Xml.Serialization;
using Administration = Microsoft.Web.Administration;

namespace Matey.Frontend.IIS
{
    using Abstractions;
    using Configuration;
    using Matey.Frontend.Abstractions.Rules;

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

        private string CreateWebFarmName(HostRequestRule hostRequestRule)
        {
            return $"{hostRequestRule.Host}{options.Value.WebFarmDelimiter}{options.Value.WebFarmSuffix}";
        }

        private WebFarm AddRequestRouteWithoutCommit(
            RequestRouteRule rule,
            WebFarmCollection webFarms,
            RulesCollection globalRules)
        {
            string webFarmName;

            if (rule.Rule is HostRequestRule hostRule)
            {
                webFarmName = CreateWebFarmName(hostRule);
                WebFarm webFarm;
                if (!webFarms.TryGet(webFarmName, out webFarm))
                {
                    webFarm = webFarms.CreateWebFarm();
                    webFarm.Name = webFarmName;
                    webFarms.Add(webFarm);

                    Rule rewriteRule = globalRules.CreateRule();
                    rewriteRule.Name = webFarm.RewriteRuleName;
                    rewriteRule.StopProcessing = true;
                    rewriteRule.Match.Url = ".*";
                    rewriteRule.Action.Type = "Rewrite";
                    rewriteRule.Action.Url = $"http://{webFarmName}/{{R:0}}";

                    RuleCondition condition = rewriteRule.Conditions.CreateRuleCondition();
                    condition.Input = "{HTTP_HOST}";
                    condition.Pattern = hostRule.Host;
                    rewriteRule.Conditions.Add(condition);

                    globalRules.Add(rewriteRule);
                }

                WebFarmServer server = webFarm.CreateServer();
                server.Address = rule.Endpoint.IPEndPoint.Address.ToString();
                server.ApplicationRequestRouting.HttpPort = rule.Endpoint.IPEndPoint.Port;
                webFarm.Add(server);

                return webFarm;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    
        public void AddRequestRoute(RequestRouteRule rule)
        {
            Administration.Configuration config = serverManager.GetApplicationHostConfiguration();
            RulesCollection globalRules = new RulesCollection(config
                .GetSection("system.webServer/rewrite/globalRules")
                .GetCollection());
            WebFarmCollection webFarms = new WebFarmCollection(config
                .GetSection("webFarms")
                .GetCollection(), globalRules);

            WebFarm webFarm = AddRequestRouteWithoutCommit(rule, webFarms, globalRules);
            serverManager.CommitChanges();
            logger.LogInformation("Added load balancer '{0}'.", webFarm.Name);
        }

        public void InitializeRequestRoutes(IEnumerable<RequestRouteRule> rules)
        {
            Administration.Configuration config = serverManager.GetApplicationHostConfiguration();
            RulesCollection globalRules = new RulesCollection(config
                .GetSection("system.webServer/rewrite/globalRules")
                .GetCollection());
            WebFarmCollection webFarms = new WebFarmCollection(config
                .GetSection("webFarms")
                .GetCollection(), globalRules);

            foreach (WebFarm webFarm in webFarms)
            {
                if (webFarm.Name is not null && webFarm.Name.EndsWith($"{options.Value.WebFarmDelimiter}{options.Value.WebFarmSuffix}"))
                {
                    webFarms.Remove(webFarm);
                }
            }

            foreach (RequestRouteRule rule in rules)
            {
                AddRequestRouteWithoutCommit(rule, webFarms, globalRules);
            }

            serverManager.CommitChanges();
        }

        public void RemoveRequestRoutes(ApplicationRequestEndpoint endpoint)
        {
            Administration.Configuration config = serverManager.GetApplicationHostConfiguration();
            RulesCollection globalRules = new RulesCollection(config
                .GetSection("system.webServer/rewrite/globalRules")
                .GetCollection());
            WebFarmCollection webFarms = new WebFarmCollection(config
                .GetSection("webFarms")
                .GetCollection(), globalRules);

            foreach (WebFarm webFarm in webFarms.ToList())
            {
                WebFarmServer? server = webFarm.FirstOrDefault(
                    s => s.Address == endpoint.IPEndPoint.Address.ToString() && (s.ApplicationRequestRouting.HttpPort ?? 80) == endpoint.IPEndPoint.Port);
                
                if (server is not null)
                {
                    webFarm.Remove(server);

                    if (webFarm.Count == 0)
                    {
                        webFarms.Remove(webFarm);
                        logger.LogInformation("Removed load balancer '{0}'.", webFarm.Name);
                    }
                }
            }

            serverManager.CommitChanges();
        }
    }
}
