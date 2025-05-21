namespace RealState.Models;

public class DashboardDto
{
	public int tickets { get; set; }
	public int open_tickets { get; set; }
	public int replied_tickets { get; set; }

	public int properties { get; set; }
	public int pending_properties { get; set; }
	public int archived_properties { get; set; }

	public int users { get; set; }
	public int active_users { get; set; }
	public int de_active_users { get; set; }

	public int orders { get; set; }
	public int failed_orders { get; set; }
	public int success_orders { get; set; }
}
