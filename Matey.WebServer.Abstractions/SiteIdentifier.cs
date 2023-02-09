namespace Matey.WebServer.Abstractions
{
    public record SiteIdentifier(string Name)
    {
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
