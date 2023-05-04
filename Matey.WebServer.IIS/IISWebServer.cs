using Microsoft.Extensions.Options;
using Administration = Microsoft.Web.Administration;

namespace Matey.WebServer.IIS
{
    using Abstractions;
    using Abstractions.Rules;
    using Configuration;
    using Matey.Pki;
    using System.Net;

    public class IISWebServer : IWebServer
    {
        private readonly IOptions<IISOptions> options;
        private readonly Administration.ServerManager serverManager;

        public string Name => "IIS";

        public ICertificateStore CertificateStore { get; }

        public IPAddress CallbackIPAddress => IPAddress.Loopback;

        public IISWebServer(IOptions<IISOptions> options, Administration.ServerManager serverManager, ICertificateStore certificateStore = null)
        {
            this.options = options;
            this.serverManager = serverManager;
            CertificateStore = certificateStore;
        }

        private string CreateWebFarmName(RequestRoute route)
        {
            return $"{route.ServiceName}{options.Value.WebFarmDelimiter}{options.Value.WebFarmSuffix}";
        }

        private WebFarm AddRequestRouteWithoutCommit(
            RequestRoute route,
            WebFarmCollection webFarms,
            RulesCollection globalRules)
        {
            WebFarm webFarm;
            Rule rewriteRule = globalRules.CreateRule();
            string webFarmName = CreateWebFarmName(route);

            if (!webFarms.TryGet(webFarmName, out webFarm))
            {
                webFarm = webFarms.CreateWebFarm();
                webFarm.Name = webFarmName;
                webFarms.Add(webFarm);
            }

            rewriteRule.Name = webFarm.RewriteRuleName;
            rewriteRule.StopProcessing = true;
            rewriteRule.Match.Url = ".*";
            rewriteRule.Action.Type = "Rewrite";
            rewriteRule.Action.Url = $"http://{webFarmName}/{{R:0}}";

            foreach (IRequestRule iteratedRule in route.Rule.ToEnumerable())
            {
                IRequestRule rule = iteratedRule;
                RuleCondition condition = rewriteRule.Conditions.CreateRuleCondition();
                if (rule is NotRequestRule notRule)
                {
                    condition.Negate = true;
                    rule = notRule.Rule;
                }

                if (rule is HostRequestRule hostRule)
                {
                    condition.Input = "{HTTP_HOST}";
                    condition.Pattern = hostRule.Host;
                }
                else if (rule is PathRequestRule pathRule)
                {
                    condition.Input = "{URL}";
                    condition.Pattern = pathRule.PathPattern;
                }
                else
                {
                    continue;
                }

                rewriteRule.Conditions.Add(condition);
            }

            globalRules.Add(rewriteRule);

            if (route.StickinessSettings != null)
            {
                webFarm.ApplicationRequestRouting.Affinity.UseCookie = route.StickinessSettings.IsSticky;
                webFarm.ApplicationRequestRouting.Affinity.CookieName = route.StickinessSettings.CookieName;
            }

            WebFarmServer server = webFarm.CreateServer();
            server.Address = route.Endpoint.IPEndPoint.Address.ToString();
            server.ApplicationRequestRouting.HttpPort = route.Endpoint.IPEndPoint.Port;
            server.ApplicationRequestRouting.Weight = route.Endpoint.Weight ?? 100;
            webFarm.Add(server);

            return webFarm;
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
