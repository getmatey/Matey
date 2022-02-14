namespace Matey.Frontend.Abstractions
{
    public record SiteIdentifier(string Provider, string Name, string Id)
    {
        public override int GetHashCode()
        {
            return HashCode.Combine(Provider, Name, Id);
        }

        public override string ToString()
        {
            return ToString(".");
        }

        public string ToString(string delimiter)
        {
            return $"{Provider}{delimiter}{Name}{delimiter}{Id}";
        }
    }
}
