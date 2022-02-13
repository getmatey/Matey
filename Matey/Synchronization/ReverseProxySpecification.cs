using Matey.Frontend.Abstractions;

namespace Matey.Synchronization
{
    public record ReverseProxySpecification(ReverseProxySite Configuration, IFrontend Target);
}
