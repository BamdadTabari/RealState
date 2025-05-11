using Microsoft.AspNetCore.Mvc;

namespace RealState.RequestsAndQueries;

public class GetQuery<T>
{
	[FromRoute]
	public T Data { get; set; }

	[FromRoute]
	public Dictionary<string, string>? ExtraData { get; set; }
}
