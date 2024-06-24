using KSP.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PartInfoInPAW
{
	internal delegate void PartPatchesHistoryCallback(List<MMPatchInfo> patchesList);

	internal static class MMLogParser
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
		private static Dictionary<string, string[]> PatchesCFGFiles = new Dictionary<string, string[]>();

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
				Dictionary<string, int> patchesNums = new Dictionary<string, int>();
				if (LogFileLines.Length > 0)
				{
					string searchStr = "/PART[" + @partName + "]";
					int linesCount = LogFileLines.Length;
					for (int i = 0; i < linesCount; i++)
					{
						string line = LogFileLines[i];
						if (line.IndexOf(searchStr) != -1)
						{
							Match m = Regex.Match(line.Trim(), RegexPattern(partName));
							if (m.Success)
							{
								Utils.LogDebugMsg($"Found patch for part {partName} in MM log file, parsing patch CFG file");
								int patchNum = 0;
								string patchFile = m.Groups[1].ToString();
								string patchSelector = m.Groups[2].ToString();
								string key = patchFile + "/" + patchSelector + "/" + partName;
								if (patchesNums.ContainsKey(key))
								{
									patchNum = patchesNums[key] + 1;
									patchesNums[key] = patchNum;
								}
								else
								{
									patchesNums.Add(key, 0);
								}
								string[] patchCFGFileLines = new string[] { };
								string cfgFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData", MMPatchInfo.AddExtension(patchFile));
								if (PatchesCFGFiles.ContainsKey(patchFile))
								{
									Utils.LogDebugMsg($"CFG file {cfgFilePath.Replace('\\', '/')} found in cache");
									patchCFGFileLines = PatchesCFGFiles[patchFile];
								}
								else
								{
									try
									{
										patchCFGFileLines = File.ReadAllLines(cfgFilePath, Encoding.UTF8);
										Utils.LogDebugMsg($"Loaded CFG file {cfgFilePath.Replace('\\', '/')}");
									}
									catch (Exception e)
									{
										Utils.LogError($"Could not CFG file {cfgFilePath.Replace('\\', '/')} : " + e.Message);
									}
								}
								string fullPatch = null;
								int occurence = 0;
								int level = -1;
								bool found = false;
								int cfgLinesCount = patchCFGFileLines.Length;
								string[] commentStart = { "//" };
								for (int j = 0; j < cfgLinesCount; j++)
								{
									string[] code = patchCFGFileLines[j].Split(commentStart, 2, StringSplitOptions.None);
									if (code[0].IndexOf(patchSelector) != -1)
									{
										if (patchNum == occurence)
										{
											found = true;
											code[0] = code[0].Substring(code[0].IndexOf(patchSelector) + patchSelector.Length);
										}
										else
										{
											occurence++;
											continue;
										}
									}
									if (found)
									{
										if (!String.IsNullOrWhiteSpace(code[0]))
										{
											fullPatch += code[0] + ((code.Length > 1) ? "//" + code[1] : "") + Environment.NewLine;
											int openingBrCount = code[0].Count(c => c == '{');
											int closingBrCount = code[0].Count(c => c == '}');
											if (openingBrCount > 0 || closingBrCount > 0)
											{
												level += openingBrCount - closingBrCount;
												if (level <= -1)
												{
													break;
												}
											}
										}
									}
								}
								if (String.IsNullOrWhiteSpace(fullPatch))
								{
									fullPatch = null;
								}
								patches.Add(new MMPatchInfo(MMPatchInfo.AddExtension(patchFile), patchSelector, fullPatch));
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

		private static string RegexPattern(string partName)
		{
			return @"^\[LOG \d{2}:\d{2}:\d{2}\.\d{3}\] Applying update (.+)/(@PART.+) to .+/PART\[" + Regex.Escape(partName) + @"\]$";
		}
	}
}
