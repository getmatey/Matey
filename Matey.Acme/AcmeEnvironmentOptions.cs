namespace Matey.Acme
{
    public class AcmeEnvironmentOptions
    {
        public string? Name { get; init; }
        public string? CertificateAuthorityUri { get; init; }
        public bool UsePostAsGet { get; init; }

        public bool IsStaging()
        {
            return Name == "Staging";
        }

        public bool IsProduction()
        {
            return Name == "Production";
        }
    }
}
