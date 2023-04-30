using Matey.Acme.Http01;

namespace Matey.Acme
{
    public class AcmeOptions
    {
        public string? KeyExchangeAlgorithm { get; init; } = AcmeOptionsDefaults.KeyExchangeAlgorithm;
        public AcmeEnvironmentOptions[] Environments { get; init; } = AcmeOptionsDefaults.Environments;
        public string? ChallengeType { get; init; } = AcmeOptionsDefaults.ChallengeType;
        public AcmeHttp01Options Http01 { get; init; } = new AcmeHttp01Options();
        public string[]? AccountContactEmails { get; set; }
        public bool AcceptTermsOfService { get; init; } = AcmeOptionsDefaults.AcceptTermsOfService;
        public string Environment { get; set; } = AcmeOptionsDefaults.Environment;

        public AcmeEnvironmentOptions GetActiveEnvironment()
        {
            return Environments.First(e => e.Name == Environment);
        }
    }
}
