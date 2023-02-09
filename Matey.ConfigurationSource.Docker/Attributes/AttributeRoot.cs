namespace Matey.ConfigurationSource.Docker.Attributes
{
    internal class AttributeRoot : AttributeSection, IAttributeRoot
    {
        internal AttributeRoot(string path, IDictionary<string, string> attributes) : base(path, attributes)
        {
        }
    }
}
