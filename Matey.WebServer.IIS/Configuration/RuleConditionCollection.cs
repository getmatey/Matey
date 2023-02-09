using Microsoft.Web.Administration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matey.WebServer.IIS.Configuration
{
    internal class RuleConditionCollection : IEnumerable<RuleCondition>
    {
        private readonly ConfigurationElementCollection collection;
        private IEnumerable<RuleCondition> enumerable;

        public RuleConditionCollection(ConfigurationElementCollection collection)
        {
            this.collection = collection;
            enumerable = collection.Select(e => new RuleCondition(e));
        }

        private void CreateEnumerable()
        {
            enumerable = collection.Select(e => new RuleCondition(e));
        }

        public RuleCondition CreateRuleCondition()
        {
            return new RuleCondition(collection.CreateElement("add"));
        }

        public void Add(RuleCondition ruleCondition)
        {
            collection.Add(ruleCondition.element);

            CreateEnumerable();
        }

        public void Remove(RuleCondition ruleCondition)
        {
            collection.Remove(ruleCondition.element);

            CreateEnumerable();
        }

        public IEnumerator<RuleCondition> GetEnumerator() => enumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => enumerable.GetEnumerator();
    }
}
