using Matey.Common;
using System.Runtime.Serialization;

namespace Matey.Acme
{
    public class MateyAcmeException : MateyException
    {
        public MateyAcmeException()
        {
        }

        public MateyAcmeException(string? message) : base(message)
        {
        }

        public MateyAcmeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MateyAcmeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
