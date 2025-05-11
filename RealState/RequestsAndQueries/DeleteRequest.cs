using Microsoft.AspNetCore.Mvc;

namespace RealState.Requests;

public class DeleteRequest
{
	[FromRoute]
	public long Id { get; set; }

	[FromRoute]
	public Dictionary<string, string>? ExtraData { get; set; }
}
