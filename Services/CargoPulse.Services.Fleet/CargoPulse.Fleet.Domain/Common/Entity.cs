using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Common
{

    public interface IDomainEvent { }
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }

    public abstract class AggregateRoot : Entity
    {
        public bool IsDeleted { get; private set; }

        public void Delete() => IsDeleted = true;
    }
}
