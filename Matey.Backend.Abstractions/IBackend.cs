namespace Matey.Backend.Abstractions
{
    public interface IBackend
    {
        IEnumerable<IServiceConfiguration> GetRunningServiceConfigurations();

        Task BeginMonitorAsync(CancellationToken cancellationToken);
    }
}
