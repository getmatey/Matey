using System.ComponentModel;

namespace Matey.Backend.Docker.Attributes
{
    internal class AttributeSection : IAttributeSection
    {
        private readonly string path;
        private readonly IDictionary<string, string> attributes;

        internal AttributeSection(string path, IDictionary<string, string> attributes)
        {
            this.path = path;
            this.attributes = new Dictionary<string, string>(attributes.Where(p => p.Key.StartsWith(path)));
        }

        private static string Combine(string path, string key) => $"{path}{Paths.Delimiter}{key}";

        public IAttributeSection GetSection(string key)
        {
            return new AttributeSection(Combine(path, key), attributes);
        }

        public bool TryGetValue<T>(string key, out T? value) where T : struct
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (attributes.TryGetValue(Combine(path, key), out string? source) &&
                converter.CanConvertTo(typeof(T)) &&
                converter.CanConvertFrom(typeof(string)))
            {
                value = (T?)converter.ConvertFromString(source);
                return true;
            }
            else
            {
                value = default(T?);
                return false;
            }
        }

        public T? GetValue<T>(string key) where T : struct
        {
            string source = GetString(key);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            return (T?)converter.ConvertFromString(source);
        }

        public string GetString(string key)
        {
            return attributes[Combine(path, key)];
        }

        public bool TryGetString(string key, out string? value)
        {
            return attributes.TryGetValue(Combine(path, key), out value);
        }
    }
}
