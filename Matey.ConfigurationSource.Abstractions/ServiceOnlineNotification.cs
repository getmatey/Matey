namespace Matey.ConfigurationSource.Abstractions
{
    public record ServiceOnlineNotification(IServiceConfiguration Configuration) : IServiceNotification;
}
