using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;

public class UserRole : BaseEntity
{
	public long Roleid { get; set; }
	public long Userid { get; set; }

	public User User { get; set; }
	public Role Role { get; set; }

}
public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRole>
{
	public void Configure(EntityTypeBuilder<UserRole> builder)
	{

		builder.HasKey(x => new { x.Userid, x.Roleid, x.id });
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();
		builder
			.HasOne(x => x.User)
			.WithMany(x => x.UserRoles)
			.HasForeignKey(x => x.Userid)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(x => x.Role)
			.WithMany(x => x.UserRoles)
			.HasForeignKey(x => x.Roleid)
			.OnDelete(DeleteBehavior.Cascade);

	}
}