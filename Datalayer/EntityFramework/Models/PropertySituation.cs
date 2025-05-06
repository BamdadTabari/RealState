using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class PropertySituation : BaseEntity
{
	public string name { get; set; }

	public ICollection<Property> properties { get; set; }
}

public class PropertySituationConfiguration : IEntityTypeConfiguration<PropertySituation>
{
	public void Configure(EntityTypeBuilder<PropertySituation> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder.Property(x => x.name).IsRequired();

		builder.HasMany(x => x.properties)
			.WithOne(x => x.situation)
			.HasForeignKey(X => X.situation_id)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
