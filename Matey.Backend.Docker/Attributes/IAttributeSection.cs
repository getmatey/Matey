namespace Matey.Backend.Docker.Attributes
{
    public interface IAttributeSection
    {
        bool TryGetValue<T>(string key, out T? value) where T : struct;

        public T? GetValue<T>(string key) where T : struct;

        public IAttributeSection GetSection(string key);

        string GetString(string key);
    }
}
