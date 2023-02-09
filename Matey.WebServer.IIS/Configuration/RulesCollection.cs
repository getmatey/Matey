using Microsoft.Web.Administration;
using System.Collections;

namespace Matey.WebServer.IIS.Configuration
{
    internal class RulesCollection : IEnumerable<Rule>
    {
        private readonly ConfigurationElementCollection collection;
        private IEnumerable<Rule> enumerable;

        public RulesCollection(ConfigurationElementCollection collection)
        {
            this.collection = collection;
            enumerable = collection.Select(e => new Rule(e));
        }

        private void CreateEnumerable()
        {
            enumerable = collection.Select(e => new Rule(e));
        }

        public Rule CreateRule()
        {
            return new Rule(collection.CreateElement("rule"));
        }

        public void Add(Rule rule)
        {
            collection.Add(rule.element);

            CreateEnumerable();
        }

        public void Remove(Rule rule)
        {
            collection.Remove(rule.element);

            CreateEnumerable();
        }

        public IEnumerator<Rule> GetEnumerator() => enumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => enumerable.GetEnumerator();
    }
}
