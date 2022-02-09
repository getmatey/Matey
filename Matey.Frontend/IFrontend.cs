using Matey.Common;

namespace Matey.Frontend
{
    public interface IFrontend : INotificationHandler<ServiceOnlineNotification>, INotificationHandler<ServiceOfflineNotification>
    {
    }
}
