using Matey.Frontend;
using System.Collections.Immutable;

namespace Matey.Backend
{
    public record BackendToFrontendServiceConfiguration(ImmutableArray<string> Hostnames) : IFrontendServiceConfiguration;
}
