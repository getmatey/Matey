namespace Matey.Backend.Docker
{
    using Abstractions;

    internal record DockerFrontendServiceConfiguration(string? Provider, string? Rule) : IFrontendServiceConfiguration;
}
