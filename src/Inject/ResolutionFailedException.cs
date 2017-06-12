using System;

namespace Inject
{
    /// <summary>
    /// The exception that is thrown when a type fails to resolve.
    /// </summary>
    public class ResolutionFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionFailedException"/> class.
        /// </summary>
        public ResolutionFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionFailedException"/> class with the specifed error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ResolutionFailedException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolutionFailedException"/> class with the specifed error message
        /// and a reference to the inner exception that is the cause of the exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception that is the cause of the exception.</param>
        public ResolutionFailedException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}