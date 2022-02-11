namespace Matey.Backend.Abstractions
{
    public interface IFrontendServiceConfiguration
    {
        string? Provider { get; }
        string Rule { get; }
    }
}
