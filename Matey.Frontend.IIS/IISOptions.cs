namespace Matey.Frontend.IIS
{
    public class IISOptions
    {
        public string WebsitesPath { get; init; } = IISOptionsDefaults.WebsitesPath;

        public string SiteNameDelimiter { get; init; } = IISOptionsDefaults.SiteNameDelimiter;

        public string SiteNamePrefix { get; init; } = IISOptionsDefaults.SiteNamePrefix;
    }
}
