using System;

namespace FitnessVibe.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when a user attempts an unauthorized operation
    /// </summary>
    public class UnauthorizedOperationException : UnauthorizedAccessException
    {
        /// <summary>
        /// The ID of the user who attempted the operation
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// The type of resource that was accessed
        /// </summary>
        public string ResourceType { get; }

        /// <summary>
        /// The ID of the resource that was accessed
        /// </summary>
        public Guid ResourceId { get; }

        /// <summary>
        /// The type of operation that was attempted
        /// </summary>
        public string Operation { get; }

        /// <summary>
        /// Initializes a new instance of the exception
        /// </summary>
        public UnauthorizedOperationException(
            Guid userId,
            string resourceType,
            Guid resourceId,
            string operation,
            string message) : base(message)
        {
            UserId = userId;
            ResourceType = resourceType;
            ResourceId = resourceId;
            Operation = operation;
        }

        /// <summary>
        /// Creates a new instance for a specific resource and operation
        /// </summary>
        public static UnauthorizedOperationException For<T>(Guid userId, Guid resourceId, string operation)
        {
            return new UnauthorizedOperationException(
                userId,
                typeof(T).Name,
                resourceId,
                operation,
                $"User {userId} is not authorized to {operation} {typeof(T).Name} {resourceId}"
            );
        }
    }
}
