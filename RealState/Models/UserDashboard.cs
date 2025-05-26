namespace RealState.Models;

public class UserDashboard
{
	public int ads { get; set; }
	public int sell_ads { get; set; }
	public int rent_ads { get; set; }
	public int archived_ads { get; set; }

	public string plan_name { get; set; }
	public string plan_description { get; set; }
	public int plan_months { get; set; }
	public int plan_property_count { get; set; }


	public double days_until_expire { get; set; }
	public int remain_properties { get; set; }
}
