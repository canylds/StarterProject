using Core.Security.Entities;

namespace Domain.Entities;

public class OtpAuthenticator : OtpAuthenticator<int, int>
{
    public virtual User User { get; set; } = default!;
}
