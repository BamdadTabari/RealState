using DataLayer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using RealState.Models;

namespace RaelState.Models;

public class OrderDto : BaseDto
{
    public long amount { get; set; }
    public int status { get; set; }
    public string? response_message { get; set; }
    public string? authority { get; set; }
    public DateTime date_paid { get; set; }
    public string? card_number { get; set; }
    public long user_id { get; set; }
    //public UserDto? user { get; set; }
	public string mobile { get; set; }
	public string email { get; set; }
	public string username { get; set; }

	public long plan_id { get; set; }
	//[ValidateNever]
	//public Plan? plan { get; set; }
}
