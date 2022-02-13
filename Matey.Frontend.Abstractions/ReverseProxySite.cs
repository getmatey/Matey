namespace Matey.Frontend.Abstractions
{
    public record ReverseProxySite(
        SiteIdentifier Identifier,
        string Domain,
        int Port,
        IEnumerable<ProxyForwardDestination> Destinations);
}
