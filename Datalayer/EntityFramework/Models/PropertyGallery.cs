using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class PropertyGallery : BaseEntity
{
	public string picture { get; set; }
	public string alt { get; set; }
	public long? property_id { get; set; }

	public long user_id { get; set; }
	public User user { get; set; }
	public Property property { get; set; }
}

public class PropertyGalleryConfiguration : IEntityTypeConfiguration<PropertyGallery>
{
	public void Configure(EntityTypeBuilder<PropertyGallery> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder.Property(x => x.picture).IsRequired();
		builder.Property(x => x.alt).IsRequired();

		builder
			.HasOne(x=>x.property)
			.WithMany(x=>x.gallery)
			.HasForeignKey(x=>x.property_id)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
