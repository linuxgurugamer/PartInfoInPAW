using KSP.Localization;
using System;
using System.Diagnostics;

namespace PartInfoInPAW
{
	internal static class Utils
	{
		public static void Log(string msg)
		{
			UnityEngine.Debug.Log("[PartInfoInPAW] " + msg);
		}

		public static void LogDebugMsg(string msg)
		{
			if (Settings.debugLogInfo)
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

		public static string ReplaceFirstOccurrence(string source, string find, string replace)
		{
			int n = source.IndexOf(find);
			string result = source.Remove(n, find.Length).Insert(n, replace);
			return result;
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

		public static string WrapStringWithBoldColorTags(string strToWrap, string color)
		{
			if (color == null)
			{
				return "<b>" + strToWrap + "</b>";
			}
			return "<color=" + color + "><b>" + strToWrap + "</b></color>";
		}

		public static string NormalizeNewLines(string text)
		{
			string usedNewLine = Environment.NewLine;
			if (text.Contains("\r\n"))
			{
				usedNewLine = "\r\n";
			}
			else if (text.Contains("\r"))
			{
				usedNewLine = "\r";
			}
			else if (text.Contains("\n"))
			{
				usedNewLine = "\n";
			}
			if (usedNewLine == Environment.NewLine)
			{
				return text;
			}
			return text.Replace(usedNewLine, Environment.NewLine);
		}
	}
}
