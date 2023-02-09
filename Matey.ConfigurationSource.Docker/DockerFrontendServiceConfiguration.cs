namespace Matey.ConfigurationSource.Docker
{
    using Abstractions;

    internal record DockerFrontendServiceConfiguration(string? Rule) : IFrontendServiceConfiguration;
}
