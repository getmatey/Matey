namespace Matey.ConfigurationSource.Abstractions
{
    public record ServiceOfflineNotification(IServiceConfiguration Configuration) : IServiceNotification;
}
