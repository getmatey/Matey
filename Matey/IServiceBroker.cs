namespace Matey
{
    using Backend.Abstractions;
    using Common;

    public interface IServiceBroker : INotificationHandler<ServiceOnlineNotification>, INotificationHandler<ServiceOfflineNotification>
    {
        void Synchronize();
    }
}
