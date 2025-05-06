using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;

public class Role : BaseEntity
{
	public string title { get; set; }

	#region Navigations

	public ICollection<UserRole> user_roles { get; set; }

	#endregion
}
public class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();
		#region Mapping
		builder.Property(b => b.title)
			.IsRequired();
		#endregion
		#region Navigations

		builder
			.HasMany(x => x.user_roles)
			.WithOne(x => x.role)
			.HasForeignKey(x => x.role_id)
			// just for now, at this time I choose the "DeleteBehavior.Restrict" ,
			// because there is not any logic selected yet, so I just select the easy way
			.OnDelete(DeleteBehavior.Restrict);

		#endregion

	}
}
