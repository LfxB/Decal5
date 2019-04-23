using System;

// Taken from https://stackoverflow.com/a/444818
// I take no credit.

namespace GraffitiArtist
{
	public static class StringExtensions
	{
		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source?.IndexOf(toCheck, comp) >= 0;
		}

		public static string GetUntilOrEmpty(this string text, string stopAt = "-")
		{
			if (!String.IsNullOrWhiteSpace(text))
			{
				int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

				if (charLocation > 0)
				{
					return text.Substring(0, charLocation);
				}
			}

			return String.Empty;
		}
	}
}