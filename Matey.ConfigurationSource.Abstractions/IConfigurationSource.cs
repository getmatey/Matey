namespace Matey.ConfigurationSource.Abstractions
{
    public interface IConfigurationSource
    {
        IEnumerable<IServiceConfiguration> GetRunningServiceConfigurations();

        Task BeginMonitorAsync(CancellationToken cancellationToken);
    }
}
