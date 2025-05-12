using Microsoft.AspNetCore.Mvc;

namespace RealState.Requests;

public class EditRequest<T>
{
	[FromForm]
	public T data { get; set; }

	[FromForm]
	public Dictionary<string, string>? extra_data { get; set; }
}
