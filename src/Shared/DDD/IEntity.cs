namespace Shared.DDD;

public interface IEntity<T>
{   
    public T Id { get; set; }
}

public interface IAuditableEntity
{
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}
