using System;
using System.Collections.Generic;

namespace FitnessVibe.Domain.Common
{
    /// <summary>
    /// Base entity class - think of this as the DNA that every domain object inherits.
    /// Like how every living thing has basic cellular structure, every domain entity has basic properties.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// The unique identifier for the entity
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// When the entity was created
        /// </summary>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// When the entity was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; protected set; }

        /// <summary>
        /// Whether the entity is marked as deleted
        /// </summary>
        public bool IsDeleted { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        /// Domain events raised by this entity
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Creates a new entity with a new GUID and current timestamp
        /// </summary>
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the entity as updated, setting the UpdatedAt timestamp
        /// </summary>
        protected void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the entity as deleted and updates the timestamp
        /// </summary>
        public void MarkAsDeleted()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                MarkAsUpdated();
            }
        }

        /// <summary>
        /// Adds a domain event to this entity's collection
        /// </summary>
        /// <param name="domainEvent">The event to add</param>
        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent ?? throw new ArgumentNullException(nameof(domainEvent)));
        }

        /// <summary>
        /// Removes a domain event from this entity's collection
        /// </summary>
        /// <param name="domainEvent">The event to remove</param>
        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent ?? throw new ArgumentNullException(nameof(domainEvent)));
        }
    }
}
