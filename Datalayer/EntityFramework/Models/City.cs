using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class City : BaseEntity
{
    public string Name { get; set; }

    public long ProvinceId { get; set; }
    public Province Province { get; set; }

    public ICollection<Collection> Collections { get; set; }
}


public class CityEntityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Slug).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.Name).IsRequired();

        builder
        .HasMany(x => x.Collections)
        .WithOne(x => x.City)
        .HasForeignKey(x => x.CityId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}