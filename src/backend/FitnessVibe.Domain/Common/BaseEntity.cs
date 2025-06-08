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
        public int Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public bool IsDeleted { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        protected void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            MarkAsUpdated();
        }
    }
}
