namespace Matey.Common
{
    public interface INotifier
    {
        public Task NotifyAsync<TNotification>(TNotification message) where TNotification : INotification;
    }
}
