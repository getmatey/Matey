﻿
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Administration = Microsoft.Web.Administration;

namespace Matey.Frontend.IIS
{
    using Abstractions;
    using Abstractions.Rules;
    using Configuration;

    public class IISFrontend : IFrontend
    {
        private readonly IOptions<IISOptions> options;
        private readonly Administration.ServerManager serverManager;

        public string Name => "IIS";

        public IISFrontend(IOptions<IISOptions> options, Administration.ServerManager serverManager)
        {
            this.options = options;
            this.serverManager = serverManager;
        }

        private string CreateWebFarmName(HostRequestRule hostRequestRule)
        {
            return $"{hostRequestRule.Host}{options.Value.WebFarmDelimiter}{options.Value.WebFarmSuffix}";
        }

        private WebFarm AddRequestRouteWithoutCommit(
            RequestRoute route,
            WebFarmCollection webFarms,
            RulesCollection globalRules)
        {
            string webFarmName;

            if (route.Rule is HostRequestRule hostRule)
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

                webFarm.ApplicationRequestRouting.Affinity.UseCookie = route.StickinessSettings.IsSticky;
                webFarm.ApplicationRequestRouting.Affinity.CookieName = route.StickinessSettings.CookieName;

                WebFarmServer server = webFarm.CreateServer();
                server.Address = route.Endpoint.IPEndPoint.Address.ToString();
                server.ApplicationRequestRouting.HttpPort = route.Endpoint.IPEndPoint.Port;
                server.ApplicationRequestRouting.Weight = route.Endpoint.Weight ?? 100;
                webFarm.Add(server);

                return webFarm;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    
        public void AddRequestRoute(RequestRoute route)
        {
            Administration.Configuration config = serverManager.GetApplicationHostConfiguration();
            RulesCollection globalRules = new RulesCollection(
                config.GetSection("system.webServer/rewrite/globalRules").GetCollection());
            WebFarmCollection webFarms = new WebFarmCollection(
                config.GetSection("webFarms").GetCollection(), globalRules);

            AddRequestRouteWithoutCommit(route, webFarms, globalRules);
            serverManager.CommitChanges();
        }

        public void InitializeRequestRoutes(IEnumerable<RequestRoute> routes)
        {
            Administration.Configuration config = serverManager.GetApplicationHostConfiguration();
            RulesCollection globalRules = new RulesCollection(
                config.GetSection("system.webServer/rewrite/globalRules").GetCollection());
            WebFarmCollection webFarms = new WebFarmCollection(
                config.GetSection("webFarms").GetCollection(), globalRules);

            foreach (WebFarm webFarm in webFarms.ToList())
            {
                if (webFarm.Name is not null && webFarm.Name.EndsWith($"{options.Value.WebFarmDelimiter}{options.Value.WebFarmSuffix}"))
                {
                    webFarms.Remove(webFarm);
                }
            }

            foreach (RequestRoute route in routes)
            {
                AddRequestRouteWithoutCommit(route, webFarms, globalRules);
            }

            serverManager.CommitChanges();
        }

        public void RemoveRequestRoutes(ApplicationRequestEndpoint endpoint)
        {
            Administration.Configuration config = serverManager.GetApplicationHostConfiguration();
            RulesCollection globalRules = new RulesCollection(
                config.GetSection("system.webServer/rewrite/globalRules").GetCollection());
            WebFarmCollection webFarms = new WebFarmCollection(
                config.GetSection("webFarms").GetCollection(), globalRules);

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
                    }
                }
            }

            serverManager.CommitChanges();
        }
    }
}
