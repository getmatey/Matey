using ACMESharp.Authorizations;
using System.Collections.Concurrent;

namespace Matey.Acme
{
    internal class MemoryChallengeValidationDetailsRepository : IChallengeValidationDetailsRepository
    {
        private readonly ConcurrentDictionary<string, IChallengeValidationDetails> store = new ConcurrentDictionary<string, IChallengeValidationDetails>();

        public Task AddAsync(IChallengeValidationDetails details)
        {
            if (details == null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            if(details is Http01ChallengeValidationDetails httpDetails)
            {
                store[httpDetails.HttpResourceUrl] = httpDetails;

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
                if(!store.TryRemove(challengeId, out _))
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
