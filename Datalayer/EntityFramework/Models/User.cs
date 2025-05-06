using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;

public class User : BaseEntity
{
	#region identity

	public string Username { get; set; }

	public string Mobile { get; set; }
	public bool IsMobileComfirmed { get; set; }

	public string Email { get; set; }

	#endregion

	#region Login

	public string PasswordHash { get; set; }

	public int FailedLoginCount { get; set; }
	public DateTime? LockoutEndTime { get; set; }

	public DateTime? LastLoginDate { get; set; }

	#endregion

	#region Management

	public string SecurityStamp { get; set; }
	public string ConcurrencyStamp { get; set; }
	public bool IsLockedOut { get; set; }
	public bool IsActive { get; set; }
	public string? RefreshToken { get; set; }
	public DateTime RefreshTokenExpiryTime { get; set; }
	#endregion

	#region Navigations
	public ICollection<UserRole> UserRoles { get; set; }
	public long? Agencyid { get; set; }
	public Agency Agency { get; set; }
	#endregion
}

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(x => x.id);

		builder.HasIndex(b => b.Username).IsUnique();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();
		#region Mappings

		builder.Property(b => b.Mobile)
			.IsRequired();

		#endregion

		#region Navigations

		builder
			.HasMany(x => x.UserRoles)
			.WithOne(x => x.User)
			.HasForeignKey(x => x.Userid)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(x => x.Agency)
			.WithOne(x => x.User)
			.HasForeignKey<Agency>(x => x.Userid)
			.OnDelete(DeleteBehavior.Cascade);

		#endregion
	}
}