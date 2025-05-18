using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace DataLayer;
public interface IBaseEntity { }

public class BaseEntity : IBaseEntity
{
	[SwaggerSchema("ایدی جدول", Format = "Id", Nullable = false)]
	public long id { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }

	[MaxLength(1000)]
	public string slug { get; set; }
}
