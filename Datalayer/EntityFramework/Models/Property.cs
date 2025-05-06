using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer;
public class Property : BaseEntity
{
	public string name { get; set; }
	public string address { get; set; }
	public TypeEnum type_enum { get; set; }
	public string city_province_full_name { get; set; }
	
	public long city_id { get; set; }
	public City city { get; set; }

	public decimal meterage { get; set; }

	public long category_id { get; set; }
	public PropertyCategory property_category { get; set; }
	
	public bool is_for_sale { get; set; }
	public decimal sell_price { get; set; }
	/// <summary>
	/// rahn
	/// </summary>
	public decimal? mortgage_price { get; set; }
	/// <summary>
	/// ejare
	/// </summary>
	public decimal? rent_price { get; set; }

	public int bed_room_count { get; set; }

	public int property_age { get; set; }
	public int property_floor { get; set; }

	public DateTime expire_date { get; set; }
	public ICollection<PropertyFacilityProperty> property_facility_properties{ get; set; }

	public long situation_id { get; set; }
	public PropertySituation situation { get; set; }

	public Ad ad { get; set; }
}

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
	public void Configure(EntityTypeBuilder<Property> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();

		builder.Property(x => x.name).IsRequired();
		builder.Property(x => x.type_enum).IsRequired();
		builder.Property(x => x.city_province_full_name).IsRequired();
		builder.Property(x => x.city_id).IsRequired();
		builder.Property(x => x.meterage).IsRequired();
		builder.Property(x => x.category_id).IsRequired();
		builder.Property(x => x.sell_price).IsRequired();
		builder.Property(x => x.bed_room_count).IsRequired();
		builder.Property(x => x.property_age).IsRequired();
		builder.Property(x => x.property_floor).IsRequired();
		builder.Property(x => x.expire_date).IsRequired();
		builder.Property(x => x.situation_id).IsRequired();
	}
}