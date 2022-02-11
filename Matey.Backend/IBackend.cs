namespace Matey.Backend.Abstractions
{
    public interface IBackend
    {
        Task BeginMonitorAsync(CancellationToken cancellationToken);
    }
}
