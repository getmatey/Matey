using ACMESharp.Authorizations;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Matey.Acme.Http01
{
    public class Http01ChallengeValidationDetailsRepository : IChallengeValidationDetailsRepository
    {
        private readonly ConcurrentDictionary<string, IChallengeValidationDetails> store = new ConcurrentDictionary<string, IChallengeValidationDetails>();
        private readonly ILogger<Http01ChallengeValidationDetailsRepository> logger;

        public Http01ChallengeValidationDetailsRepository(ILogger<Http01ChallengeValidationDetailsRepository> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task AddAsync(IChallengeValidationDetails details)
        {
            if (details == null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            if (details is Http01ChallengeValidationDetails httpDetails)
            {
                Uri uri = new Uri(httpDetails.HttpResourceUrl);
                store[uri.PathAndQuery] = httpDetails;
                logger.LogDebug("Serving '{0}' for ACME challenge.", httpDetails.HttpResourceUrl);

                return Task.CompletedTask;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public Task<IChallengeValidationDetails> FindAsync(string challengeId)
        {
            if (store.TryGetValue(challengeId, out IChallengeValidationDetails details))
            {
                return Task.FromResult(details);
            }
            else
            {
                throw new AcmeChallengeDetailsNotFoundException($"Challenge '{challengeId}' was not found.");
            }
        }

        public Task RemoveAsync(IChallengeValidationDetails details)
        {
            if (details is Http01ChallengeValidationDetails httpDetails)
            {
                string challengeId = httpDetails.HttpResourceUrl;
                if (!store.TryRemove(challengeId, out _))
                {
                    throw AcmeChallengeDetailsNotFoundException.Create(challengeId);
                }

                return Task.CompletedTask;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
