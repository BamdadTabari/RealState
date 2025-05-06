﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;

public class UserRole : BaseEntity
{
	public long role_id { get; set; }
	public long user_id { get; set; }

	public User user { get; set; }
	public Role role { get; set; }

}
public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRole>
{
	public void Configure(EntityTypeBuilder<UserRole> builder)
	{

		builder.HasKey(x => new { x.user_id, x.role_id, x.id });
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();
		builder
			.HasOne(x => x.user)
			.WithMany(x => x.user_roles)
			.HasForeignKey(x => x.user_id)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(x => x.role)
			.WithMany(x => x.user_roles)
			.HasForeignKey(x => x.role_id)
			.OnDelete(DeleteBehavior.Cascade);

	}
}