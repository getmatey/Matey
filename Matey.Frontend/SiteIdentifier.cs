using System.Collections.Immutable;

namespace Matey.Frontend.Abstractions
{
    public record SiteIdentifier(ImmutableArray<string> Value)
    {
        public static SiteIdentifier Create(params string[] Parts)
        {
            return new SiteIdentifier(ImmutableArray.Create(Parts));
        }
    }
}
