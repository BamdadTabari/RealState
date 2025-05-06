using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Otp : BaseEntity
{
	public int otp_code { get; set; }
	public string phone { get; set; }
	public DateTime expiry_date { get; set; } = DateTime.Now.AddMinutes(2);
}
public class OtpConfiguration : IEntityTypeConfiguration<Otp>
{
	public void Configure(EntityTypeBuilder<Otp> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.phone).IsRequired();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();
	}
}
