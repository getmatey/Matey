namespace Matey.Backend.Abstractions
{
    public record ServiceOfflineNotification(
        string Provider,
        string ServiceId,
        string ServiceName) : IServiceNotification;
}
