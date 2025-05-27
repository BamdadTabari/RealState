using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Plan : BaseEntity
{
	public string name { get; set; }
	public string description { get; set; }
	public decimal price { get; set; }
	public int property_count { get; set; }
	public int plan_months { get; set; }

	public ICollection<User> users { get; set; }
	public ICollection<Order> orders { get; set; }
}
public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
	public void Configure(EntityTypeBuilder<Plan> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder.Property(x => x.name).IsRequired();
		builder.Property(x => x.description).IsRequired();
		builder.Property(x => x.price).IsRequired();
		builder.Property(x => x.property_count).IsRequired();
		builder.Property(x => x.plan_months).IsRequired();
	}
}
