using Microsoft.AspNetCore.Mvc;

namespace RealState.Requests;

public class DeleteRequest<T>
{
	[FromForm]
	public T Id { get; set; }

	[FromForm]
	public Dictionary<string, string>? ExtraData { get; set; }
}
