using System.Runtime.Serialization;

namespace Matey.Acme
{
    public class AcmeChallengeDetailsNotFoundException : MateyAcmeException
    {
        public AcmeChallengeDetailsNotFoundException()
        {
        }

        public AcmeChallengeDetailsNotFoundException(string? message) : base(message)
        {
        }

        public AcmeChallengeDetailsNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AcmeChallengeDetailsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public static AcmeChallengeDetailsNotFoundException Create(string challengeId)
        {
            return new AcmeChallengeDetailsNotFoundException($"Challenge '{challengeId}' was not found.");
        }
    }
}
