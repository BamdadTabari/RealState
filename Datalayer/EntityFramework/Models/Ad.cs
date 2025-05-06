using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Ad : BaseEntity
{
	public string name { get; set; }
	public string description { get; set; }

	public AdStateEnum state_enum { get; set; }

	public Property property { get; set; }
}

public class AdConfiguration : IEntityTypeConfiguration<Ad>
{
	public void Configure(EntityTypeBuilder<Ad> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder.Property(x => x.name).IsRequired();
		builder.Property(x => x.description).IsRequired();

		builder.
			HasOne(x => x.property)
			.WithOne(x => x.ad);

	}
}