namespace Matey.Backend.Abstractions
{
    public record ServiceOnlineNotification(IServiceConfiguration Configuration) : IServiceNotification;
}
