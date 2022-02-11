namespace Matey.Frontend.IIS
{
    public class IISOptions
    {
        public string WebsitesPath { get; init; } = IISConfigurationDefault.WebsitesPath;

        public string SiteIdentifierDelimiter { get; init; } = IISConfigurationDefault.SiteIdentifierDelimiter;
    }
}
