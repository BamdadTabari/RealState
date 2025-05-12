using Microsoft.AspNetCore.Mvc;

namespace RealState.RequestsAndQueries;

public class GetQuery<T>
{
	[FromRoute]
	public T data { get; set; }

	[FromRoute]
	public Dictionary<string, string>? extra_data { get; set; }
}
