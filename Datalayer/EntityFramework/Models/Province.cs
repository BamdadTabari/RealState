using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;

public class Province : BaseEntity
{
	public string name { get; set; }

	public ICollection<City> cities { get; set; }
}

public class ProvinceEntityConfiguration : IEntityTypeConfiguration<Province>
{
	public void Configure(EntityTypeBuilder<Province> builder)
	{
		builder.HasKey(x => x.id);

		builder.Property(x => x.name).IsRequired();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		// Cascade فقط روی شهرها (Cities)
		builder
			.HasMany(x => x.cities)
			.WithOne(x => x.province)
			.HasForeignKey(x => x.province_id)
			.OnDelete(DeleteBehavior.Cascade);
	}
}