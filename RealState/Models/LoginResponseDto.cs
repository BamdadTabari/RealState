namespace RealState.Models;

public class LoginResponseDto
{
	public string access_token { get; set; }
	public string refresh_token { get; set; }
	public double expire_in { get; set; }
}
