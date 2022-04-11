using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matey.Frontend.IIS.Configuration
{
    internal class ApplicationRequestRoutingAffinity
    {
        private readonly ConfigurationElement element;

        public bool? UseCookie { 
            get => element["useCookie"] as bool?;
            set => element["useCookie"] = value; 
        }

        public string? CookieName { 
            get => element["cookieName"] as string;
            set => element["cookieName"] = value;
        }

        public ApplicationRequestRoutingAffinity(ConfigurationElement element)
        {
            this.element = element ?? throw new ArgumentNullException(nameof(element));
        }
    }
}
