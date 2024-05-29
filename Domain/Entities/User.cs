using Core.Security.Entities;

namespace Domain.Entities;

public class User : User<int>
{
    public bool Status { get; set; }

    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = default!;
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = default!;
    public virtual ICollection<OtpAuthenticator> OtpAuthenticators { get; set; } = default!;
    public virtual ICollection<EmailAuthenticator> EmailAuthenticators { get; set; } = default!;

    public User() : base()
    {
        Status = true;
    }

    public User(string firstName, string lastName, string email, byte[] passwordSalt, byte[] passwordHash, bool status)
        : base(firstName, lastName, email, passwordSalt, passwordHash)
    {
        Status = status;
    }

    public User(int id, string firstName, string lastName, string email, byte[] passwordSalt, byte[] passwordHash, bool status)
        : base(id, firstName, lastName, email, passwordSalt, passwordHash)
    {
        Status = status;
    }
}
