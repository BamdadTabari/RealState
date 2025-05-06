using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class City : BaseEntity
{
	public string name { get; set; }

	public long province_id { get; set; }
	public Province province { get; set; }

	public ICollection<Agency> agency_list { get; set; }
}


public class CityEntityConfiguration : IEntityTypeConfiguration<City>
{
	public void Configure(EntityTypeBuilder<City> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();
		builder.Property(x => x.name).IsRequired();

		builder
		.HasMany(x => x.agency_list)
		.WithOne(x => x.city)
		.HasForeignKey(x => x.city_id)
		.OnDelete(DeleteBehavior.Restrict);
	}
}