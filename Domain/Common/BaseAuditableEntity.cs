namespace Domain.Common;

public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>
    where TId : IComparable<TId>
{
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}
