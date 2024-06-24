using System.Linq;

namespace PartInfoInPAW
{
	internal static class Settings
	{
		public static bool debugLogInfo = false;

		public static void Load()
		{
			ConfigNode settingsNode;
			Utils.Log("Trying to load settings from GameData/PartInfoInPAW/Settings.cfg");
			if (GameDatabase.Instance.ExistsConfigNode("PartInfoInPAW/Settings/PARTINFOINPAW"))
			{
				Utils.Log("Settings file found");
				settingsNode = GameDatabase.Instance.GetConfigNodes("PARTINFOINPAW").First();
				settingsNode.TryGetValue("DebugLogInfo", ref debugLogInfo);
			}
			else
			{
				Utils.LogWarning("Settings file not found, using default settings values");

			}
			Utils.Log("Finished loading settings");
		}
	}
}
