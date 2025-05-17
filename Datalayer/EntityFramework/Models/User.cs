using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;

public class User : BaseEntity
{
	#region identity

	public string user_name { get; set; }

	public string mobile { get; set; }
	public bool is_mobile_confirmed { get; set; }

	public string email { get; set; }

	#endregion

	#region Login

	public string password_hash { get; set; }

	public int failed_login_count { get; set; }
	public DateTime? lock_out_end_time { get; set; }

	public DateTime? last_login_date_time { get; set; }

	#endregion

	#region Management

	public string security_stamp { get; set; }
	public string concurrency_stamp { get; set; }
	public bool is_locked_out { get; set; }
	public bool is_active { get; set; }
	public string? refresh_token { get; set; }
	public DateTime refresh_token_expiry_time { get; set; }

	public bool is_delete_able { get; set; } = true;
	#endregion

	#region Navigations
	public ICollection<UserRole> user_roles { get; set; }

	public long? agency_id { get; set; }
	public Agency agency { get; set; }

	public long? plan_id { get; set; }
	public Plan plan { get; set; }

	public ICollection<Property> properties { get; set; }


	public DateTime expire_date { get; set; }
	public int property_count { get; set; }
	#endregion
}

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(x => x.id);

		builder.HasIndex(b => b.user_name).IsUnique();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();
		
		#region Mappings

		builder.Property(b => b.mobile)
			.IsRequired();

		#endregion

		#region Navigations

		builder
			.HasMany(x => x.user_roles)
			.WithOne(x => x.user)
			.HasForeignKey(x => x.user_id)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(x => x.agency)
			.WithOne(x => x.user)
			.HasForeignKey<User>(x => x.agency_id)
			.OnDelete(DeleteBehavior.Cascade);


		builder.HasMany(x=>x.properties)
			.WithOne(x=>x.user)
			.HasForeignKey(x=>x.owner_id)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(x=>x.plan)
			.WithMany(x=>x.users)
			.HasForeignKey(x=>x.plan_id)
			.OnDelete(DeleteBehavior.Cascade);
		#endregion
	}
}