using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;

public class UserRole : BaseEntity
{
	public long RoleId { get; set; }
	public long UserId { get; set; }

	public User User { get; set; }
	public Role Role { get; set; }

}
public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRole>
{
	public void Configure(EntityTypeBuilder<UserRole> builder)
	{

		builder.HasKey(x => new { x.UserId, x.RoleId, x.Id });
		builder.Property(x => x.Slug).IsRequired();
		builder.HasIndex(x => x.Slug).IsUnique();
		builder
			.HasOne(x => x.User)
			.WithMany(x => x.UserRoles)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(x => x.Role)
			.WithMany(x => x.UserRoles)
			.HasForeignKey(x => x.RoleId)
			.OnDelete(DeleteBehavior.Cascade);

	}
}