using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Option : BaseEntity
{
	public string optin_key { get; set; }
	public string option_value { get; set; }
}
public class OptionConfiguration : IEntityTypeConfiguration<Option>
{
	public void Configure(EntityTypeBuilder<Option> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.optin_key).IsRequired();
		builder.Property(x => x.option_value).IsRequired();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();
	}
}