using Matey.Frontend;

namespace Matey.Backend
{
    public record BackendOnlineNotification(IBackendServiceConfiguration Configuration) : IBackendNotification;
}
