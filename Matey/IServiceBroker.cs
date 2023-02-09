namespace Matey
{
    using Common;
    using ConfigurationSource.Abstractions;

    public interface IServiceBroker : INotificationHandler<ServiceOnlineNotification>, INotificationHandler<ServiceOfflineNotification>
    {
        void Synchronize();
    }
}
