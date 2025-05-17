using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Contact : BaseEntity
{
	public string full_name { get; set; }
	public string email { get; set; }
	public string phone { get; set; }
	public string message { get; set; }
	public long user_id { get; set; }
	public User user { get; set; }
}
public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
	public void Configure(EntityTypeBuilder<Contact> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.full_name).IsRequired();
		builder.Property(x => x.email).IsRequired();
		builder.Property(x => x.phone).IsRequired();
		builder.Property(x => x.message).IsRequired();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();
	}
}
