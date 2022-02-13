namespace Matey.Backend.Docker
{
    using Attributes;

    internal static class DockerFrontendServiceConfigurationFactory
    {
        internal static DockerFrontendServiceConfiguration Create(IAttributeSection? attributes)
        {
            string? rule = null;

            if(attributes is not null)
            {
                attributes.TryGetString(Tokens.Rule, out rule);
            }

            return new DockerFrontendServiceConfiguration(Rule: rule);
        }
    }
}
