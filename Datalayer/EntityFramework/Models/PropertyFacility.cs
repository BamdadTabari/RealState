using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class PropertyFacility : BaseEntity
{
	public string name { get; set; }
	public ICollection<PropertyFacilityProperty> property_facility_properties { get; set; }
}


public class PropertyFacilityConfiguration : IEntityTypeConfiguration<PropertyFacility>
{
	public void Configure(EntityTypeBuilder<PropertyFacility> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder.Property(x => x.name).IsRequired();
	}
}
