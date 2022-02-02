namespace Matey.Backend
{
    public interface IBackend
    {
        Task BeginMonitorAsync(CancellationToken cancellationToken);
    }
}
