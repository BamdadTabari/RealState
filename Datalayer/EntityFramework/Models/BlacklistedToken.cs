﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class BlacklistedToken : BaseEntity
{
	public string Token { get; set; } // The JWT token string
	public DateTime ExpiryDate { get; set; } // The expiration date of the token
	public DateTime BlacklistedOn { get; set; } = DateTime.UtcNow; // When the token was blacklisted
}

public class BlacklistedTokenConfiguration : IEntityTypeConfiguration<BlacklistedToken>
{
	public void Configure(EntityTypeBuilder<BlacklistedToken> builder)
	{
		builder.Property(x => x.Token).IsRequired();
		builder.Property(x => x.ExpiryDate).IsRequired();
		builder.Property(x => x.BlacklistedOn).IsRequired();
		builder.Property(x => x.Slug).IsRequired();
		builder.HasIndex(x => x.Slug).IsUnique();
	}
}
