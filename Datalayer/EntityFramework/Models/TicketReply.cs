using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer;
public class TicketReply : BaseEntity
{
	public string message { get; set; }

	public string? picture { get; set; }

	public long ticket_id { get; set; }

	public Ticket ticket { get; set; }

	public long user_id { get; set; }
	public bool is_admin { get; set; }

	public User user { get; set; }
}
public class TicketReplyConfiguration : IEntityTypeConfiguration<TicketReply>
{
	public void Configure(EntityTypeBuilder<TicketReply> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.message).IsRequired();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder
			.HasOne(x=>x.ticket)
			.WithMany(x=>x.replies)
			.HasForeignKey(x=>x.ticket_id)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(x => x.user)
			.WithMany(x => x.ticket_replies)
			.HasForeignKey(x => x.user_id)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
