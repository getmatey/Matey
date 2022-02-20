namespace Matey.Backend.Abstractions
{
    public record ServiceOfflineNotification(IServiceConfiguration Configuration) : IServiceNotification;
}
