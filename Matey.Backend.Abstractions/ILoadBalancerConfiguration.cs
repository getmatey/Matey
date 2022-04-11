namespace Matey.Backend.Abstractions
{
    public interface ILoadBalancerConfiguration
    {
        ILoadBalancerStickinessConfiguration Stickiness { get; }
    }
}
