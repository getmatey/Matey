namespace Matey.Backend.Docker
{
    using Abstractions;

    internal record DockerFrontendServiceConfiguration(string? Rule) : IFrontendServiceConfiguration;
}
