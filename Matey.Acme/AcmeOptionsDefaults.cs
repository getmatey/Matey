namespace Matey.Acme
{
    internal static class AcmeOptionsDefaults
    {
        public const string KeyExchangeAlgorithm = "RSA";
        public static AcmeEnvironmentOptions[] Environments => new[]
        {
            new AcmeEnvironmentOptions()
            {
                Name = "Production",
                CertificateAuthorityUri = "https://acme-v02.api.letsencrypt.org/"
            },
            new AcmeEnvironmentOptions()
            {
                Name = "Staging",
                CertificateAuthorityUri = "https://acme-staging-v02.api.letsencrypt.org/"
            }
        };
        public const string ChallengeType = "http-01";
        public const bool AcceptTermsOfService = false;
        public const string Environment = "Production";
    }
}
