using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matey.Frontend.Abstractions
{
    public record InboundProxySite(SiteIdentifier Identifier, string Hostname, int Port, ProxyForwardDestination Destination);
}
