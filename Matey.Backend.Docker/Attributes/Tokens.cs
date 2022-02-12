using System.Collections.Immutable;

namespace Matey.Backend.Docker.Attributes
{
    internal class Tokens
    {
        internal const string Frontend = "frontend";
        internal const string Port = "port";
        internal const string Enabled = "enabled";
        internal const string Rule = "rule";
        internal const string Docker = "docker";
        internal const string Provider = "provider";
        internal const string Domain = "domain";
        internal static ImmutableHashSet<string> Reserved = (new string[] {
            Docker,
            Enabled
        }).ToImmutableHashSet();
    }
}
