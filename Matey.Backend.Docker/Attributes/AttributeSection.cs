using System.ComponentModel;

namespace Matey.Backend.Docker.Attributes
{
    internal class AttributeSection : IAttributeSection
    {
        private readonly string path;
        private readonly IDictionary<string, string> attributes;
        private readonly IDictionary<string, IAttributeSection> subsections = new Dictionary<string, IAttributeSection>();
        private readonly IDictionary<string, string> values = new Dictionary<string, string>();

        public string Name { get; }

        public IEnumerable<IAttributeSection> Sections => subsections.Values;

        internal AttributeSection(string path, IDictionary<string, string> attributes)
        {
            Name = Path.Leaf(path);
            this.path = path;
            this.attributes = new Dictionary<string, string>(attributes.Where(p => p.Key.StartsWith(path)));

            foreach (var attribute in this.attributes)
            {
                // Child paths
                string relative = Path.Child(path, attribute.Key);
                string absolute = Path.Combine(path, relative);

                if (absolute == attribute.Key) // Value (leaf)
                {
                    values.Add(relative, attribute.Value);
                }
                else if (!subsections.ContainsKey(relative)) // Section
                {
                    subsections.Add(relative, new AttributeSection(absolute, this.attributes));
                }
            }
        }

        public IAttributeSection? GetSection(string key)
        {
            if (key.Contains(Path.Delimiter))
            {
                string root = Path.Root(key);
                return GetSection(root)?.GetSection(Path.Relative(root, key));
            }
            else
            {
                return subsections.TryGetValue(key, out IAttributeSection? section) ? section : null;
            }
        }

        public bool TryGetValue<T>(string key, out T? value) where T : struct
        {
            object? valueObj = null;
            if (TryGetValue(key, typeof(T), out valueObj))
            {
                value = (T?)valueObj;
                return true;
            }
            else
            {
                value = default(T?);
                return false;
            }
        }

        public bool TryGetValue(string key, Type type, out object? value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (attributes.TryGetValue(Path.Combine(path, key), out string? source) &&
                converter.CanConvertTo(type) &&
                converter.CanConvertFrom(typeof(string)))
            {
                value = converter.ConvertFromString(source);
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public T? GetValue<T>(string key) where T : struct
        {
            return (T?)GetValue(key, typeof(T));
        }

        public object? GetValue(string key, Type type)
        {
            string? source = GetString(key);
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return source is null ? null : converter.ConvertFromString(source);
        }

        public string? GetString(string key)
        {
            if (key.Contains(Path.Delimiter))
            {
                string root = Path.Root(key);
                return GetSection(root)?.GetString(Path.Relative(root, key));
            }
            else
            {
                return values.TryGetValue(key, out string? value) ? value : null;
            }
        }

        public bool TryGetString(string key, out string? value)
        {
            value = null;

            if (key.Contains(Path.Delimiter))
            {
                string root = Path.Root(key);
                return GetSection(root)?.TryGetString(Path.Relative(root, key), out value) ?? false;
            }
            else
            {
                return values.TryGetValue(key, out value);
            }
        }
    }
}
