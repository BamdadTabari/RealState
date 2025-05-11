using Microsoft.AspNetCore.Mvc;

namespace RealState.Requests;

public class EditRequest<T>
{
	[FromForm]
	public T Data { get; set; }

	[FromForm]
	public Dictionary<string, string>? ExtraData { get; set; }
}
