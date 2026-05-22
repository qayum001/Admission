using Admission.Domain.Abstractions;
using Admission.Domain.ValueObjects;

namespace Admission.Domain.Entities;

public abstract class User : BaseEntity
{
    protected User()
    {
        
    }

    protected User(Guid id, string role, Guid externalId) : base(id)
    {
        Role = new(role);
        ExternalId = externalId;
    }
    
    public Text Role { get; private set; }
    public Guid ExternalId { get; private set; }
}