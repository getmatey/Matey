using Matey.Frontend;

namespace Matey.Backend.Docker
{
    internal record DockerFrontendServiceConfiguration(string Rule) : IFrontendServiceConfiguration;
}
