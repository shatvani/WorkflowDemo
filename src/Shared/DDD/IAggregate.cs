
namespace Shared.DDD;

// Non-generic marker + minimal API for interceptors
public interface IAggregate
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IDomainEvent[] ClearDomainEvents();
}

// Generic aggregate extends marker and IEntity<T>
public interface IAggregate<T> : IAggregate, IEntity<T>
{
}
