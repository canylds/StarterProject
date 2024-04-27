using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Persistence.EntityConfigurations;

public class UserOperationClaimConfiguration : IEntityTypeConfiguration<UserOperationClaim>
{
    public void Configure(EntityTypeBuilder<UserOperationClaim> builder)
    {
        builder.ToTable("UserOperationClaims").HasKey(uoc => uoc.Id);

        builder.Property(uoc => uoc.Id).HasColumnName("Id").IsRequired();
        builder.Property(uoc => uoc.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(uoc => uoc.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(uoc => uoc.DeletedDate).HasColumnName("DeletedDate");
        builder.Property(uoc => uoc.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(uoc => uoc.OperationClaimId).HasColumnName("OperationClaimId").IsRequired();

        builder.HasOne(uoc => uoc.User);
        builder.HasOne(uoc => uoc.OperationClaim);

        builder.HasIndex(indexExpression: u => new
        {
            u.UserId,
            u.OperationClaimId
        }, name: "UK_UserOperationClaims_UserId_OperationClaimId").IsUnique();

        builder.HasQueryFilter(uoc => !uoc.DeletedDate.HasValue);

        builder.HasData(GetSeeds());
    }

    private IEnumerable<UserOperationClaim> GetSeeds()
    {
        List<UserOperationClaim> userOperationClaims = new();

        UserOperationClaim adminUserOperationClaim = new()
        {
            Id = 1,
            UserId = 1,
            OperationClaimId = 1
        };

        userOperationClaims.Add(adminUserOperationClaim);

        return userOperationClaims;
    }
}