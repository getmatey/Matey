namespace Matey.WebServer.IIS
{
    public class IISOptions
    {
        public string WebFarmDelimiter { get; init; } = IISOptionsDefaults.WebFarmDelimiter;

        public string WebFarmSuffix { get; init; } = IISOptionsDefaults.WebFarmSuffix;

        public string ServerName { get; init; } = IISOptionsDefaults.ServerName;
    }
}
