using Core.Security.Entities;

namespace Domain.Entities;

public class UserOperationClaim : UserOperationClaim<int, int, int>
{
    public virtual User User { get; set; } = default!;
    public virtual OperationClaim OperationClaim { get; set; } = default!;
}
