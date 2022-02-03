namespace Matey.Backend.Docker.Attributes
{
    internal class Paths
    {
        internal const string Delimiter = ".";
        internal const string Backend = "backend";
        internal const string Frontend = "frontend";
        internal const string Port = $"{Backend}{Delimiter}port";
        internal const string Enabled = $"{Backend}{Delimiter}enabled";
        internal const string Hostname = $"{Frontend}{Delimiter}hostname";
    }
}
