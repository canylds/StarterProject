using Core.Security.Entities;

namespace Domain.Entities;

public class RefreshToken : RefreshToken<int, int>
{
    public virtual User User { get; set; } = default!;
}
