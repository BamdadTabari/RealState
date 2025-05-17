using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Contact : BaseEntity
{
	public string message { get; set; }
	public long user_id { get; set; }
	public User user { get; set; }
	public bool is_admin { get; set; }
}
public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
	public void Configure(EntityTypeBuilder<Contact> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.message).IsRequired();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder
			.HasOne(x => x.user)
			.WithMany(x => x.contacts)
			.HasForeignKey(x => x.user_id)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
