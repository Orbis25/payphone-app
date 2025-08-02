namespace Payphone.Domain.Core.Models;

public abstract class BaseModel
{
    public virtual int Id { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime? UpdatedAt { get; set; }
    public virtual string? CreatedBy { get; set; }
    public virtual string? UpdatedBy { get; set; }
    public virtual bool IsDeleted { get; set; }
}