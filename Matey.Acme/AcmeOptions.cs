using Matey.Acme.Http;

namespace Matey.Acme
{
    public class AcmeOptions
    {
        public string? KeyExchangeAlgorithm { get; init; } = AcmeOptionsDefaults.KeyExchangeAlgorithm;
        public string? CertificateAuthorityUri { get; init; } = AcmeOptionsDefaults.CertificateAuthorityUri;
        public AcmeHttpOptions Http { get; init; } = new AcmeHttpOptions();
        public string[]? AccountContactEmails { get; set; }
        public bool AcceptTermsOfService { get; init; } = AcmeOptionsDefaults.AcceptTermsOfService;
    }
}
