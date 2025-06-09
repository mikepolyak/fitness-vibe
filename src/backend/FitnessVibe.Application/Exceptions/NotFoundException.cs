using System;

namespace FitnessVibe.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource is not found
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the exception
        /// </summary>
        public NotFoundException() : base() { }

        /// <summary>
        /// Initializes a new instance of the exception with a message
        /// </summary>
        public NotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the exception with a message and inner exception
        /// </summary>
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Creates a new instance for a specific resource type and ID
        /// </summary>
        public static NotFoundException For<T>(Guid id) => new($"{typeof(T).Name} with ID {id} was not found");

        /// <summary>
        /// Creates a new instance for a specific resource type and identifier
        /// </summary>
        public static NotFoundException For<T>(string identifier) => new($"{typeof(T).Name} with identifier {identifier} was not found");
    }
}
