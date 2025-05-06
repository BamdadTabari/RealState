using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class PropertyFacilityProperty : BaseEntity
{
	public long property_id { get; set; }
	public Property property { get; set; }

	public long property_facility_id { get; set; }
	public PropertyFacility property_facility { get; set; }
}


public class PropertyFacilityPropertyConfiguration : IEntityTypeConfiguration<PropertyFacilityProperty>
{
	public void Configure(EntityTypeBuilder<PropertyFacilityProperty> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder.Property(x => x.property_id).IsRequired();
		builder.Property(x => x.property_facility_id).IsRequired();

		builder.HasOne(x => x.property)
			.WithMany(x => x.property_facility_properties)
			.HasForeignKey(X => X.property_id)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.property_facility)
			.WithMany(x => x.property_facility_properties)
			.HasForeignKey(X => X.property_facility_id)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
