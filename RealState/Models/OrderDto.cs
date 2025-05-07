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
    public UserDto? user { get; set; }
}
