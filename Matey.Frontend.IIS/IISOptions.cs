namespace Matey.Frontend.IIS
{
    public class IISOptions
    {
        public string WebsitesPath { get; init; } = IISOptionsDefaults.WebsitesPath;

        public string ServerFarmDelimiter { get; init; } = IISOptionsDefaults.ServerFarmDelimiter;

        public string ServerFarmSuffix { get; init; } = IISOptionsDefaults.ServerFarmSuffix;
    }
}
