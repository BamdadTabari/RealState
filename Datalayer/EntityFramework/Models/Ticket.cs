using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Ticket : BaseEntity
{
	public string ticket_code { get; set; }
	public string subject { get; set; } // عنوان تیکت
	public string message { get; set; } // متن اولیه تیکت
	public string? picture { get; set; }
	public TicketStatus status { get; set; } = TicketStatus.Open;

	public long user_id { get; set; } // شناسه کاربر 
	public bool is_admin { get; set; }
	public User user { get; set; } // نویگیشن به کاربر

	public ICollection<TicketReply> replies { get; set; } = new List<TicketReply>();
}
public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
	public void Configure(EntityTypeBuilder<Ticket> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.message).IsRequired();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder
			.HasOne(x => x.user)
			.WithMany(x => x.tickets)
			.HasForeignKey(x => x.user_id)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
