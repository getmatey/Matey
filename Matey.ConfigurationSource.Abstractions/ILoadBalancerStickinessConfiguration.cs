namespace Matey.ConfigurationSource.Abstractions
{
    public interface ILoadBalancerStickinessConfiguration
    {
        bool? IsEnabled { get; }
        string? CookieName { get; }
    }
}
