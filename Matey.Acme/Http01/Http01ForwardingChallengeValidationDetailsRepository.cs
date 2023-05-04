using ACMESharp.Authorizations;
using Matey.WebServer.Abstractions;
using Matey.WebServer.Abstractions.Rules;
using System.Net;

namespace Matey.Acme.Http01
{
    public class Http01ForwardingChallengeValidationDetailsRepository : IChallengeValidationDetailsRepository
    {
        private readonly IWebServer forwarder;
        private readonly AcmeHttp01ChallengeResponderHostedService responder;
        private readonly Http01ChallengeValidationDetailsRepository repository;

        public Http01ForwardingChallengeValidationDetailsRepository(
            IWebServer forwarder,
            AcmeHttp01ChallengeResponderHostedService responder,
            Http01ChallengeValidationDetailsRepository repository)
        {
            this.forwarder = forwarder ?? throw new ArgumentNullException(nameof(forwarder));
            this.responder = responder ?? throw new ArgumentNullException(nameof(responder));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task AddAsync(IChallengeValidationDetails details)
        {
            if (details is null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            if (details is Http01ChallengeValidationDetails http01Details)
            {
                Uri uri = new Uri(http01Details.HttpResourceUrl);

                forwarder.AddRequestRoute(new RequestRoute(
                    "Acme",
                    new HostRequestRule(uri.Host).And(new PathRequestRule("/.well-known/acme-challenge/.*")),
                    new ApplicationRequestEndpoint(
                        Scheme: "http",
                        IPEndPoint: new IPEndPoint(forwarder.CallbackIPAddress, responder.Port),
                        Weight: 100)));

                await repository.AddAsync(http01Details);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<IChallengeValidationDetails> FindAsync(string challengeId)
        {
            if (challengeId is null)
            {
                throw new ArgumentNullException(nameof(challengeId));
            }

            return await repository.FindAsync(challengeId);
        }

        public async Task RemoveAsync(IChallengeValidationDetails details)
        {
            if (details is null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            await repository.RemoveAsync(details);
        }
    }
}
