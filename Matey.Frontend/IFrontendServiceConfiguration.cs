using System.Collections.Immutable;

namespace Matey.Frontend
{
    public interface IFrontendServiceConfiguration
    {
        ImmutableArray<string> Hostnames { get; }
    }
}
