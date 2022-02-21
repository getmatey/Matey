namespace Matey.Frontend.IIS
{
    public class IISOptions
    {
        public string WebFarmDelimiter { get; init; } = IISOptionsDefaults.WebFarmDelimiter;

        public string WebFarmSuffix { get; init; } = IISOptionsDefaults.WebFarmSuffix;
    }
}
