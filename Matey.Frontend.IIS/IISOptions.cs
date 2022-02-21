namespace Matey.Frontend.IIS
{
    public class IISOptions
    {
        public string ServerFarmDelimiter { get; init; } = IISOptionsDefaults.ServerFarmDelimiter;

        public string ServerFarmSuffix { get; init; } = IISOptionsDefaults.ServerFarmSuffix;
    }
}
