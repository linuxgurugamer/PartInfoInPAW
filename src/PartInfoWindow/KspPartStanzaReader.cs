using System;
using System.Collections.Generic;
using System.IO;

public static class KspPartStanzaReader
{
	public static List<string>[] ReadStanzas(string filePath)
	{
		return ReadStanzasFromText(File.ReadAllText(filePath));
	}

	public static List<string>[] ReadStanzasFromText(string text)
	{
		var result = new List<List<string>>();
		var partLevelFields = new List<string>();
		result.Add(partLevelFields);

		var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

		var braceDepth = 0;
		var current = new List<string>();
		var pendingHeader = new List<string>();

		foreach (var rawLine in lines)
		{
			string line = rawLine;
			string trimmed = line.Trim();

			if (trimmed.Length == 0)
				continue;

			if (trimmed.StartsWith("//"))
				continue;

			int opens = CountCharOutsideComment(line, '{');
			int closes = CountCharOutsideComment(line, '}');

			if (braceDepth == 1 && opens == 0 && closes == 0 && IsFieldLine(trimmed))
			{
				partLevelFields.Add(line);
				continue;
			}

			if (braceDepth == 1 && opens == 0 && closes == 0)
			{
				pendingHeader.Clear();
				pendingHeader.Add(line);
				continue;
			}

			if (braceDepth >= 2 || opens > 0)
			{
				if (braceDepth == 1 && pendingHeader.Count > 0 && opens > 0)
				{
					current = new List<string>();
					current.AddRange(pendingHeader);
					pendingHeader.Clear();
				}

				current.Add(line);
			}

			braceDepth += opens;
			braceDepth -= closes;

			if (braceDepth == 1 && current.Count > 0)
			{
				result.Add(current);
				current = new List<string>();
			}
		}

		return result.ToArray();
	}

	private static bool IsFieldLine(string trimmed)
	{
		return trimmed.Contains("=") && !trimmed.StartsWith("@") && !trimmed.StartsWith("%");
	}

	private static int CountCharOutsideComment(string line, char ch)
	{
		int comment = line.IndexOf("//", StringComparison.Ordinal);
		string effective = comment >= 0 ? line.Substring(0, comment) : line;

		int count = 0;
		foreach (char c in effective)
		{
			if (c == ch)
				count++;
		}

		return count;
	}
}