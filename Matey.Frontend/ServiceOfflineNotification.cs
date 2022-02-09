using System.Collections.Immutable;

namespace Matey.Frontend
{
    public record ServiceOfflineNotification(
        string Provider,
        string ServiceName,
        ImmutableArray<string> Backends) : IServiceNotification;
}
