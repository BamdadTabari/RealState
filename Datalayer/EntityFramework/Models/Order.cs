using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Order : BaseEntity
{
	public long amount { get; set; }
	public int status {get; set; }
	public string ref_id { get; set; }
	public string? response_message { get; set; }
	public string? authority { get; set; }
	public DateTime date_paid { get; set; }
	public string? card_number { get; set; }
	public long user_id { get; set; }
	public User? user { get; set; }
	public string mobile { get; set; }
	public string email { get; set; }
	public string username { get; set; }

	public long plan_id { get; set; }
	public Plan plan { get; set; }
}
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.amount).IsRequired();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder
			.HasOne(x=>x.plan)
			.WithOne(x=>x.order)
			.HasForeignKey<Order>(x=>x.plan_id)
			.OnDelete(DeleteBehavior.Restrict);
	}
}