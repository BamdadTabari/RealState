using Microsoft.AspNetCore.Mvc;

namespace RealState.Requests;

public class DeleteRequest<T>
{
	[FromForm]
	public T data { get; set; }

	[FromForm]
	public Dictionary<string, string>? extra_data { get; set; }
}
