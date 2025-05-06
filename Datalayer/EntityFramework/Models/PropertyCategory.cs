using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class PropertyCategory : BaseEntity
{
	public string name { get; set; }

	public ICollection<Property> properties { get; set; }
}

public class PropertyCategoryConfiguration : IEntityTypeConfiguration<PropertyCategory>
{
	public void Configure(EntityTypeBuilder<PropertyCategory> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder.Property(x => x.name).IsRequired();

		builder.HasMany(x => x.properties)
			.WithOne(x => x.property_category)
			.HasForeignKey(X => X.category_id)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
