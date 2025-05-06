namespace DataLayer;
public class PropertyType : BaseEntity
{
	public string name { get; set; }

	public  ICollection<Property> properties { get; set; }
}
