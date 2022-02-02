using System.Runtime.Serialization;

namespace Matey.Common
{
    public class MateyException : Exception
    {
        public MateyException()
        {
        }

        public MateyException(string? message) : base(message)
        {
        }

        public MateyException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MateyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
