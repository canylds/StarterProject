using Core.Security.Entities;

namespace Domain.Entities;

public class OperationClaim : OperationClaim<int>
{
    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = default!;
}
