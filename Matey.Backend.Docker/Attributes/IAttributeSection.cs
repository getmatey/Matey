namespace Matey.Backend.Docker.Attributes
{
    public interface IAttributeSection
    {
        string Name { get; }

        IEnumerable<IAttributeSection> Sections { get; }

        bool TryGetValue<T>(string key, out T? value) where T : struct;

        public T? GetValue<T>(string key) where T : struct;

        public IAttributeSection GetSection(string key);

        string GetString(string key);

        bool TryGetString(string key, out string? value);
    }
}
