using KSP.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PartInfoInPAW
{
	public delegate void PartPatchesHistoryCallback(List<MMPatchInfo> patchesList);

	public static class MMLogParser
	{
		public enum ParserStatus
		{
			NotInitialized,
			LogsNotFound,
			LoadingLogFile,
			ReadyToParse,
			ParsingLog,
			LogParsed
		}

		private static ParserStatus status = ParserStatus.NotInitialized;
		private static string[] LogFileLines;

		private static Dictionary<string, List<MMPatchInfo>> PatchesHistoryDict = new Dictionary<string, List<MMPatchInfo>>();

		public static ParserStatus GetStatus()
		{
			return status;
		}

		public static string GetStatusMsg(string partName)
		{
			switch (status)
			{
				case ParserStatus.LogParsed:
					if (PatchesHistoryDict.ContainsKey(partName))
					{
						return Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_PartPatchesMsg", partName, PatchesHistoryDict[partName].Count.ToString());
					}
					else
					{
						return Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_StatusLogParsed");
					}
				default:
					return "";
			}
		}

		public static async Task Initialize()
		{
			if (status != ParserStatus.NotInitialized) return;
			string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "ModuleManager", "MMPatch.log");
			if (!File.Exists(LogFilePath))
			{
				LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "ModuleManager", "ModuleManager.log");
				if (!File.Exists(LogFilePath))
				{
					status = ParserStatus.LogsNotFound;
					Utils.LogWarning("No Module Manager log file found");
					return;
				}
			}
			status = ParserStatus.LoadingLogFile;
			Utils.Log($"Reading Module Manager log file from {LogFilePath.Replace('\\', '/')}");
			await ReadLogFileLinesAsync(LogFilePath);
		}

		private static async Task ReadLogFileLinesAsync(string filePath)
		{
			await Task.Run(() => {
				try
				{
					// memory intensive, but fast
					LogFileLines = File.ReadAllLines(filePath, Encoding.UTF8);
					Utils.Log($"Finished reading Module Manager log file from {filePath.Replace('\\', '/')}");
					status = ParserStatus.ReadyToParse;
				}
				catch (Exception e)
				{
					Utils.LogError($"Could not read file {filePath.Replace('\\', '/')} : " + e.Message);
					Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CantReadFile_FailureMsg", filePath));
				}
			});
		}

		public static async Task<List<MMPatchInfo>> ParseLogFile(string partName)
		{
			await Initialize();
			List<MMPatchInfo> patches = new List<MMPatchInfo>();
			if ((status != ParserStatus.LogParsed) && (status != ParserStatus.ReadyToParse))
			{
				return patches;
			}
			if (PatchesHistoryDict.ContainsKey(partName))
			{
				Utils.LogDebugMsg($"Patches history for part {partName} found in cache");
				patches = PatchesHistoryDict[partName];
				status = ParserStatus.LogParsed;
			}
			else
			{
				status = ParserStatus.ParsingLog;
				Utils.LogDebugMsg($"Parsing MM log file for part {partName}");
				int lineNum = 0;
				int totalLinesCount = LogFileLines.Length;
				if (totalLinesCount > 0)
				{
					string searchStr = "/PART[" + @partName + "]";
					foreach (string line in LogFileLines)
					{
						if (line.IndexOf(searchStr) != -1)
						{
							Match m = Regex.Match(line.Trim(), RegexPattern(partName));
							if (m.Success)
							{
								patches.Add(new MMPatchInfo(MMPatchInfo.AddExtension(m.Groups[1].ToString()), m.Groups[2].ToString()));
								Utils.LogDebugMsg($"Found patch for part {partName} in MM log file");
							}
						}
						lineNum++;
					}
				}
				PatchesHistoryDict.Add(partName, patches);
				Utils.LogDebugMsg($"Finished parsing MM log file for part {partName}");
				status = ParserStatus.LogParsed;
			}
			return patches;
		}

		public static string GetPatchesHistoryAsStr(string partName)
		{
			if (PatchesHistoryDict.ContainsKey(partName))
			{
				List<MMPatchInfo> patches = PatchesHistoryDict[partName];
				string result = $"Patches for part {partName}: {patches.Count}" + Environment.NewLine + Environment.NewLine;
				foreach (MMPatchInfo m in patches)
				{
					result += m;
				}
				return result;
			}
			else
			{
				return $"Patches count for part {partName}: 0" + Environment.NewLine;
			}
		}

		private static string RegexPattern(string partName)
		{
			return @"^\[LOG \d{2}:\d{2}:\d{2}\.\d{3}\] Applying update (.+)/(@PART\[.+) to .+/PART\[" + Regex.Escape(partName) + @"\]$";
		}
	}

	public class MMPatchInfo
	{
		public string PatchFilePath { get; protected set; }
		public string Patch { get; protected set; }

		public MMPatchInfo(string patchFilePath, string patch)
		{
			PatchFilePath = patchFilePath;
			Patch = patch;
		}

		public static string AddExtension(string patch)
		{
			string result = patch;
			if (result[0] == '/')
			{
				result = result.Substring(1);
			}
			result += "." + UrlDir.configExtension;
			return result;
		}

		public override string ToString()
		{
			return PatchFilePath + Environment.NewLine + "\t" + Patch + Environment.NewLine;
		}
	}
}
