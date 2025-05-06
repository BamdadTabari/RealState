using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Agency : BaseEntity
{
	public string full_name { get; set; }
	public string mobile { get; set; }
	public string phone { get; set; }
	public string? agency_name { get; set; }

	public string city_province_full_name { get; set; }
	public long city_id { get; set; }
	public City  city { get; set; }

	public long user_id { get; set; }
	public User user { get; set; }
}

public class AgencyConfiguration : IEntityTypeConfiguration<Agency>
{
	public void Configure(EntityTypeBuilder<Agency> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder.Property(x => x.full_name).IsRequired();
		builder.Property(x => x.mobile).IsRequired();
		builder.Property(x => x.phone).IsRequired();
		builder.Property(x => x.city_province_full_name).IsRequired();
		builder.Property(x => x.city_id).IsRequired();
	}
}