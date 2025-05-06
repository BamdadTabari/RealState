using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Order : BaseEntity
{
	public long Amount { get; set; }
	public int Status { get; set; }
	public string? Respmsg { get; set; }
	public string? Authority { get; set; }
	public DateTime DatePaid { get; set; }
	public string? CardNumber { get; set; }
	public long UserId { get; set; }
	public User? User { get; set; }
}
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Amount).IsRequired();
		builder.Property(x => x.Slug).IsRequired();
		builder.HasIndex(x => x.Slug).IsUnique();
	}
}