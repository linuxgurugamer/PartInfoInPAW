using KSP.Localization;
using System;
using System.Diagnostics;
using System.IO;

namespace PartInfoInPAW
{
	public static class Utils
	{
		public const bool LogDebugMsgs = true;

		public static void Log(string msg)
		{
			UnityEngine.Debug.Log("[PartInfoInPAW] " + msg);
		}

		public static void LogDebugMsg(string msg)
		{
			if (LogDebugMsgs)
			{
				UnityEngine.Debug.Log("[PartInfoInPAW] [DEBUG] " + msg);
			}
		}

		public static void LogWarning(string msg)
		{
			UnityEngine.Debug.LogWarning("[PartInfoInPAW] " + msg);
		}

		public static void LogError(string msg)
		{
			UnityEngine.Debug.LogError("[PartInfoInPAW] " + msg);
		}

		public static void OnScreenMsg(string msg, float delay = 3.0f)
		{
			ScreenMessages.PostScreenMessage(new ScreenMessage(
			  msg,
			  delay,
			  ScreenMessageStyle.UPPER_CENTER)
			);
		}

		public static string ReplaceFirstOccurrence(string Source, string Find, string Replace)
		{
			int Place = Source.IndexOf(Find);
			string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
			return result;
		}

		public static string GetPartFilePath(Part part)
		{
			string gameDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData");
			string origFilePath = part.partInfo.partUrl.Substring(0, part.partInfo.partUrl.LastIndexOf("/"));
			if (Path.DirectorySeparatorChar != '/')
			{
				origFilePath = origFilePath.Replace('/', Path.DirectorySeparatorChar);
			}
			origFilePath += "." + UrlDir.configExtension;
			return Path.Combine(gameDataPath, origFilePath);
		}
		
		public static void ShellOpenFile(string filePath)
		{
			ProcessStartInfo procInfo = new ProcessStartInfo
			{
				FileName = @filePath,
				UseShellExecute = true
			};
			try
			{
				Process.Start(procInfo);
				Log($"Opening file {filePath.Replace('\\', '/')} in default editor");
			}
			catch (Exception e)
			{
				LogError($"Could not open default editor for file {filePath}: " + e.Message);
				OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CantOpenFile_FailureMsg", filePath));
				return;
			}
		}

		public static string FormatMass(float mass)
		{
			string result;
			if (mass < 1.0f)
			{
				result = (mass * 1000.0f).ToString("F0") + " " + Localizer.Format("#LOC_PartInfoInPAW_Kg_Unit");
			}
			else
			{
				result = mass.ToString("F3") + " " + Localizer.Format("#LOC_PartInfoInPAW_T_Unit");
			}
			return result;
		}
	}
}
