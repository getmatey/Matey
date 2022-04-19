using Microsoft.Web.Administration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Matey.Frontend.IIS.Configuration
{
    internal class WebFarm : IEnumerable<WebFarmServer>
    {
        internal readonly ConfigurationElement element;
        private ConfigurationElementCollection servers => element.GetCollection();
        private IEnumerable<WebFarmServer> enumerable;

        public string? Name
        {
            get => element["name"] as string;
            set => element["name"] = value;
        }

        public int Count
        {
            get => servers.Count;
        }

        public WebFarmApplicationRequestRouting ApplicationRequestRouting { get; }

        // Following the pattern used in IIS, rules will be automatically removed when the web farm is removed.
        public string RewriteRuleName => $"ARR_{Name}_loadbalance";

        public WebFarm(ConfigurationElement element)
        {
            this.element = element;
            enumerable = servers.Select(e => new WebFarmServer(e));
            ApplicationRequestRouting = new WebFarmApplicationRequestRouting(
                element.GetChildElement("applicationRequestRouting"));
        }

        private void CreateEnumerable()
        {
            enumerable = servers.Select(e => new WebFarmServer(e));
        }

        public WebFarmServer CreateServer()
        {
            return new WebFarmServer(servers.CreateElement("server"));
        }

        public void Add(WebFarmServer server)
        {
            servers.Add(server.element);

            CreateEnumerable();
        }

        public void Remove(WebFarmServer server)
        {
            int index = 0;
            for(int i = 0; i < servers.Count; i++)
            {
                if(servers[i] == server.element)
                {
                    index = i;
                    break;
                }
            }
            servers.RemoveAt(index);

            CreateEnumerable();
        }

        public IEnumerator<WebFarmServer> GetEnumerator() => enumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => enumerable.GetEnumerator();
    }
}
