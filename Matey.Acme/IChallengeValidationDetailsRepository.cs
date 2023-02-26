using ACMESharp.Authorizations;

namespace Matey.Acme
{
    public interface IChallengeValidationDetailsRepository
    {
        Task AddAsync(IChallengeValidationDetails details);

        Task<IChallengeValidationDetails> FindAsync(string challengeId);

        Task RemoveAsync(IChallengeValidationDetails details);
    }
}
