namespace PAS.Common.Domain;

public abstract class Entity {
    public int Id { get; protected set; }

    private readonly List<IDomainEvent> domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent eventItem) {
        domainEvents.Add(eventItem);
    }
}
