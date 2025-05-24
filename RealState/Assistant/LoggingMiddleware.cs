namespace RealState.Assistant;

// LoggingMiddleware.cs
public class LoggingMiddleware
{
	private readonly RequestDelegate _next;
	private static readonly string logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "requests.txt");

	public LoggingMiddleware(RequestDelegate next)
	{
		_next = next;
		Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
	}

	public async Task Invoke(HttpContext context)
	{
		var logLine = $"{DateTime.Now}: {context.Request.Method} {context.Request.Path}{Environment.NewLine}";
		var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "requests.txt");
		Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

		byte[] bytes = System.Text.Encoding.UTF8.GetBytes(logLine);

		using (var stream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, true))
		{
			await stream.WriteAsync(bytes, 0, bytes.Length);
		}

		await _next(context);
	}
}