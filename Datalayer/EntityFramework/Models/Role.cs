﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;

public class Role : BaseEntity
{
	public string Title { get; set; }

	#region Navigations

	public ICollection<UserRole> UserRoles { get; set; }

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
		builder.Property(b => b.Title)
			.IsRequired();
		#endregion
		#region Navigations

		builder
			.HasMany(x => x.UserRoles)
			.WithOne(x => x.Role)
			.HasForeignKey(x => x.Roleid)
			// just for now, at this time I choose the "DeleteBehavior.Restrict" ,
			// because there is not any logic selected yet, so I just select the easy way
			.OnDelete(DeleteBehavior.Restrict);

		#endregion

	}
}
