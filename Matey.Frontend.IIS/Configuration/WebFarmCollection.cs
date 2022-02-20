using Microsoft.Web.Administration;
using System.Collections;

namespace Matey.Frontend.IIS.Configuration
{
    internal class WebFarmCollection : IEnumerable<WebFarm>
    {
        private readonly ConfigurationElementCollection collection;
        private readonly RulesCollection? globalRules;
        private IEnumerable<WebFarm> enumerable;

        public WebFarmCollection(ConfigurationElementCollection collection, RulesCollection? globalRules = null)
        {
            this.collection = collection;
            this.globalRules = globalRules;
            enumerable = collection.Select(e => new WebFarm(e));
        }

        private void CreateEnumerable()
        {
            enumerable = collection.Select(e => new WebFarm(e));
        }

        public WebFarm CreateWebFarm()
        {
            return new WebFarm(collection.CreateElement("webFarm"));
        }

        public void Add(WebFarm webFarm)
        {
            collection.Add(webFarm.element);
            CreateEnumerable();
        }

        public WebFarm Get(string name)
        {
            return new WebFarm(collection.First(f => f["name"] as string == name));
        }

        public bool TryGet(string name, out WebFarm webFarm)
        {
            ConfigurationElement? element = collection.FirstOrDefault(f => f["name"] as string == name);
            webFarm = null;
            if(element is not null)
            {
                webFarm = new WebFarm(element);
            }

            return webFarm is not null;
        }

        public void Remove(WebFarm webFarm)
        {
            collection.Remove(webFarm.element);
            CreateEnumerable();

            Rule? rule = globalRules?.FirstOrDefault(r => r.Name == webFarm.RewriteRuleName);
            if(rule is not null && globalRules is not null)
            {
                globalRules.Remove(rule);
            }
        }

        public IEnumerator<WebFarm> GetEnumerator() => enumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => enumerable.GetEnumerator();
    }
}
