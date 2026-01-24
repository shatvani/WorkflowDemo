namespace Shared.DDD;
public abstract class Aggregate<Tid> : Entity<Tid>, IAggregate<Tid>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IDomainEvent[] ClearDomainEvents()
    {
        IDomainEvent[] dequeuedEents = _domainEvents.ToArray();
        _domainEvents.Clear();
        return dequeuedEents;
    }
}


public abstract class AuditableAggregate<T> : Aggregate<T>, IAuditableEntity
{
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    public void AuditChanges(string user)
    {
        LastModified = DateTime.UtcNow;
        LastModifiedBy = user;
    }
}
