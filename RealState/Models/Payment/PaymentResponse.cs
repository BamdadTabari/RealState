namespace RealState.Models.Payment;

public class PaymentResponse
{
	public string message { get; set; }
	public bool success { get; set; }
	public long status_code { get; set; }
	public string ref_id { get; set; }
}
