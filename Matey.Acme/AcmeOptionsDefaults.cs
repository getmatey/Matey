namespace Matey.Acme
{
    internal static class AcmeOptionsDefaults
    {
        public const string KeyExchangeAlgorithm = "RSA";
        public const string CertificateAuthorityUri = "https://acme-v02.api.letsencrypt.org/directory";
        public const bool AcceptTermsOfService = false;
    }
}
