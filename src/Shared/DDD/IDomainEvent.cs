namespace Shared.DDD;
public interface IDomainEvent //: INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }

    //Guid EventId => Guid.NewGuid();
    //public DateTime OccurredOn => DateTime.Now;
    //public string EventType => GetType().AssemblyQualifiedName!;
}
