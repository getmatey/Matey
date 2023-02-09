namespace Matey.ConfigurationSource.Abstractions
{
    public interface ILoadBalancerConfiguration
    {
        ILoadBalancerStickinessConfiguration Stickiness { get; }
    }
}
