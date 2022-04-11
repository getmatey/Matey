namespace Matey.Backend.Abstractions
{
    public interface ILoadBalancerStickinessConfiguration
    {
        bool? IsEnabled { get; }
        string? CookieName { get; }
    }
}
