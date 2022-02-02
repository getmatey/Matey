namespace Matey.Common
{
    public interface INotificationHandler<TNotification> : MediatR.INotificationHandler<TNotification>
        where TNotification : INotification
    {
        Task HandleAsync(TNotification notification, CancellationToken cancellationToken);

        Task MediatR.INotificationHandler<TNotification>.Handle(TNotification notification, CancellationToken cancellationToken)
            => HandleAsync(notification, cancellationToken);
}
}
