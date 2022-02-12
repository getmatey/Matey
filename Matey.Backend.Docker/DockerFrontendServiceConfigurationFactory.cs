namespace Matey.Backend.Docker
{
    using Attributes;

    internal static class DockerFrontendServiceConfigurationFactory
    {
        internal static DockerFrontendServiceConfiguration Create(IAttributeSection attributes)
            => new DockerFrontendServiceConfiguration(
                Provider: attributes.TryGetString(Tokens.Provider, out string? provider) ? provider : null,
                Rule: attributes.TryGetString(Tokens.Rule, out string? rule) ? rule : null);
    }
}
