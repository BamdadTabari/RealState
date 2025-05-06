using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;

public class Province : BaseEntity
{
    public string Name { get; set; }

    public ICollection<City> Cities { get; set; }
    public ICollection<ProvinceNewsletter> ProvinceNewsletters { get; set; }
    public ICollection<Newsletter> Newsletters { get; set; }
}

public class ProvinceEntityConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Slug).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();

        // Cascade فقط روی شهرها (Cities)
        builder
            .HasMany(x => x.Cities)
            .WithOne(x => x.Province)
            .HasForeignKey(x => x.ProvinceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict برای جلوگیری از multiple cascade paths
        builder
            .HasMany(x => x.ProvinceNewsletters)
            .WithOne(x => x.Province)
            .HasForeignKey(x => x.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(x => x.Newsletters)
            .WithOne(x => x.Province)
            .HasForeignKey(x => x.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}