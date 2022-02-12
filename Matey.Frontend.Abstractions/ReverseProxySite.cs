using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matey.Frontend.Abstractions
{
    public record ReverseProxySite(SiteIdentifier Identifier, string Domain, int Port, ProxyForwardDestination Destination);
}
