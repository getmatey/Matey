namespace Matey.Backend.Docker.Attributes
{
    internal class Path
    {
        internal const string Delimiter = ".";

        internal static string Combine(params string[] paths) => string.Join(Delimiter, paths);

        internal static string Leaf(string path)
        {
            if (path.Contains(Delimiter))
            {
                return path.Substring(path.LastIndexOf(Delimiter) + 1);
            }
            else
            {
                return path;
            }
        }

        internal static string Relative(string basePath, string path) => path.Substring(basePath.Length + 1);

        internal static string Child(string basePath, string path)
        {
            string relative = Relative(basePath, path);
            if (relative.Contains(Delimiter))
            {
                return Root(relative);
            }
            else
            {
                return relative;
            }
        }

        internal static string Root(string path)
        {
            if (path.Contains(Delimiter))
            {
                return path.Substring(0, path.IndexOf(Delimiter));
            }
            else
            {
                return path;
            }
        }
    }
}
