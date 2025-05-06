namespace DataLayer;
public static class slugHelper
{
	public static string Generateslug(string title)
	{
		return title.ToLower()
					.Replace(" ", "-")  // Replace spaces with hyphens
					.Replace(".", "")   // Remove dots
					.Replace(",", "")   // Remove commas
					.Replace("?", "")   // Remove question marks
					.Replace("!", "");  // Remove exclamation marks
	}
}