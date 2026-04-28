using KSP.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PartInfoInPAW
{
	internal static class PartExtensions
	{
		public static float GetResourceCost(this Part part)
		{
			if (part.Resources == null)
			{
				return 0f;
			}
			return part.Resources.Sum(resource => (float)resource.amount * resource.info.unitCost);
		}

		public static float GetResourceCostMax(this Part part)
		{
			if (part.Resources == null)
			{
				return 0f;
			}
			return part.Resources.Sum(resource => (float)resource.maxAmount * resource.info.unitCost);
		}

		public static float GetResourceCostOffset(this Part part)
		{
			if (part.Resources == null)
			{
				return 0f;
			}
			return part.Resources.Sum(resource => (float)(resource.amount - resource.maxAmount) * resource.info.unitCost);
		}

		public static ProtoCrewMember[] GetCrew(this Part part)
		{
			// taken from Kerbalism code
			if (HighLogic.LoadedSceneIsFlight)
			{
				var crew = part.protoModuleCrew.ToArray();
				return crew;
			}
			else
			{
				long partID = 4294967296L + part.GetInstanceID();
				var manifest = KSP.UI.CrewAssignmentDialog.Instance.GetManifest();
				var partManifest = manifest.GetCrewableParts().Find(k => k.PartID == partID);
				if (partManifest != null)
				{
					return Array.FindAll(partManifest.GetPartCrew(), c => c != null);
				}
				return new ProtoCrewMember[0];
			}

		}

#if false
		public static int GetCrewHashCode(this Part part)
		{
			int CombineHashCodes(int h1, int h2)
			{
				return (((h1 << 5) + h1) ^ h2);
			}

			// taken from Kerbalism code
			long partID = 4294967296L + part.GetInstanceID();
			var manifest = KSP.UI.CrewAssignmentDialog.Instance.GetManifest();
			var partManifest = manifest.GetCrewableParts().Find(k => k.PartID == partID);
			if (partManifest != null)
			{
				return Array.FindAll(partManifest.GetPartCrew(), c => c != null).Aggregate(0, (hash, c) => CombineHashCodes(hash, c.GetHashCode()));
			}
			return 0;
		}
#else
		static int CombineHashCodes(int h1, int h2)
		{
			return (((h1 << 5) + h1) ^ h2);
		}
		public static int GetCrewHashCode(this Part part)
		{
			if (part == null)
				return 0;

			IEnumerable<ProtoCrewMember> crew = null;

			if (HighLogic.LoadedSceneIsEditor &&
				 KSP.UI.CrewAssignmentDialog.Instance != null)
			{
				long partID = 4294967296L + part.GetInstanceID();

				var manifest = KSP.UI.CrewAssignmentDialog.Instance.GetManifest();
				var partManifest = manifest.GetCrewableParts()
					 .Find(k => k.PartID == partID);

				if (partManifest != null)
					crew = partManifest.GetPartCrew();
			}
			else if (HighLogic.LoadedSceneIsFlight)
			{
				crew = part.protoModuleCrew;
			}

			if (crew == null)
				return 0;

			return crew
				 .Where(c => c != null)
				 .Aggregate(0, (hash, c) =>
					  CombineHashCodes(hash, c.name.GetHashCode()));
		}
#endif

		public static int GetCrewCount(this Part part)
		{
			return part.GetCrew().Length;
		}

		public static float GetCrewAndInventoryMass(this Part part)
		{
			if (Versioning.version_major == 1 && Versioning.version_minor >= 11)
			{
				ProtoCrewMember[] crew = part.GetCrew();
				float mass = crew.Length * PartInfoInPAW.GetKerbonautMass();
				foreach (ProtoCrewMember crewMember in crew)
				{
					mass += crew.Sum(invMass => crewMember.InventoryMass() + crewMember.ResourceMass());
				}
				return mass;
			}
			return 0f;
		}

		public static float GetCrewInventoryCost(this Part part)
		{
			if (Versioning.version_major == 1 && Versioning.version_minor >= 11)
			{
				ProtoCrewMember[] crew = part.GetCrew();
				return crew.Sum(crewMember => crewMember.InventoryCosts());
			}
			return 0f;
		}

		public static string GetPartName(this Part part)
		{
			string pName = "";
			try
			{
				pName = GameDatabase.Instance.GetConfigs("PART").
					Single(c => part.partInfo.name.Replace('_', '.') == c.name.Replace('_', '.')).name;
			}
			catch (Exception e)
			{
				Utils.LogError($"Couldn't get config value name for part {part.partInfo.name}: " + e.Message);
			}
			return pName;
		}

		public static string GetConfigNodeText(this Part part, string partName)
		{
			ConfigNode cfg;
			try
			{
				cfg = GameDatabase.Instance.GetConfigNode(part.partInfo.partUrl) ?? part.partInfo.partConfig;
			}
			catch (Exception e)
			{
				throw new Exception($"Couldn't get config node for part {part.partInfo.name}: " + e.Message);
			}
			string nodeText;
			if (cfg != null)
			{
				nodeText = cfg.ToString();
				if (cfg != null && !cfg.HasValue("name") && (partName != ""))
				{
					nodeText = Utils.ReplaceFirstOccurrence(nodeText, "{", "{" + $"{Environment.NewLine}\tname = {partName}");
				}
			}
			else
			{
				throw new Exception($"Couldn't get config node for part {part.partInfo.name}: CFG node is null for some reason");
			}
			return nodeText;
		}

		public static string GetPartGameDataFilePath(this Part part)
		{
			string path = part.partInfo.partUrl.Substring(0, part.partInfo.partUrl.LastIndexOf("/"));
			path += "." + UrlDir.configExtension;
			return path;
		}

		public static string GetPartFilePath(this Part part)
		{
			string gameDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData");
			return Path.Combine(gameDataPath, part.GetPartGameDataFilePath().Replace('/', Path.DirectorySeparatorChar));
		}

		public static string GetOrigPartConfigNodeText(this Part part)
		{
			string origFilePath = part.GetPartFilePath();
			string origFileContent;
			try
			{
				origFileContent = Utils.NormalizeNewLines(File.ReadAllText(origFilePath));
			}
			catch (Exception e)
			{
				Utils.LogError($"Could not read part CFG file {origFilePath}: " + e.Message);
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_OpenFile_PartOrigFile_FailureMsg", origFilePath));
				return "";
			}
			return origFileContent;
		}

		public static string GetGimbalInfo(this Part part, ModuleEngines engine)
		{
			ModuleGimbal[] gimbalModules;
			bool airBreathingEngine = engine.useVelCurve;
			gimbalModules = part.GetComponents<ModuleGimbal>();
			if (gimbalModules.Length > 0)
			{
				float maxGimbalRangeXP = 0f;
				float maxGimbalRangeXN = 0f;
				float maxGimbalRangeYP = 0f;
				float maxGimbalRangeYN = 0f;
				foreach (ModuleGimbal gimbalModule in gimbalModules)
				{
					float coeff = 0f;
					if (gimbalModule.engineMultsList == null)
					{
						gimbalModule.CreateEngineList();
					}
					for (int j = 0; j < gimbalModule.engineMultsList.Count; j++)
					{
						var engs = gimbalModule.engineMultsList[j];
						for (int k = 0; k < engs.Count; k++)
						{
							if (engs[k].Key == engine)
							{
								coeff += engs[k].Value;
							}
						}
					}
					if (coeff > 0f)
					{
						coeff = 1f;
					}
					maxGimbalRangeXP = Math.Max(maxGimbalRangeXP, gimbalModule.gimbalRangeXP * coeff);
					maxGimbalRangeXN = Math.Max(maxGimbalRangeXN, gimbalModule.gimbalRangeXN * coeff);
					maxGimbalRangeYP = Math.Max(maxGimbalRangeYP, gimbalModule.gimbalRangeYP * coeff);
					maxGimbalRangeYN = Math.Max(maxGimbalRangeYN, gimbalModule.gimbalRangeYN * coeff);
				}
				if (new[] { maxGimbalRangeXP, maxGimbalRangeXN, maxGimbalRangeYP, maxGimbalRangeYN }.Distinct().Count() == 1)
				{
					return Localizer.Format(
						(airBreathingEngine) ? "#LOC_PartInfoInPAW_EngineGimbal_Format1_AirBreathing" : "#LOC_PartInfoInPAW_EngineGimbal_Format1",
						maxGimbalRangeXP.ToString("N1")
					);
				}
				else if ((maxGimbalRangeXP == maxGimbalRangeXN) && (maxGimbalRangeYP == maxGimbalRangeYN))
				{
					return Localizer.Format(
						(airBreathingEngine) ? "#LOC_PartInfoInPAW_EngineGimbal_Format2_AirBreathing" : "#LOC_PartInfoInPAW_EngineGimbal_Format2",
						maxGimbalRangeXP.ToString("N1"),
						maxGimbalRangeYP.ToString("N1")
					);
				}
				else if (maxGimbalRangeXP == maxGimbalRangeXN)
				{
					return Localizer.Format(
						(airBreathingEngine) ? "#LOC_PartInfoInPAW_EngineGimbal_Format3_AirBreathing" : "#LOC_PartInfoInPAW_EngineGimbal_Format3",
						maxGimbalRangeXP.ToString("N1"),
						maxGimbalRangeYP.ToString("N1"),
						maxGimbalRangeYN.ToString("N1")
					);
				}
				else if (maxGimbalRangeYP == maxGimbalRangeYN)
				{
					return Localizer.Format(
						(airBreathingEngine) ? "#LOC_PartInfoInPAW_EngineGimbal_Format4_AirBreathing" : "#LOC_PartInfoInPAW_EngineGimbal_Format4",
						maxGimbalRangeXP.ToString("N1"),
						maxGimbalRangeYP.ToString("N1"),
						maxGimbalRangeYN.ToString("N1")
					);
				}
				else
				{
					return Localizer.Format(
						(airBreathingEngine) ? "#LOC_PartInfoInPAW_EngineGimbal_Format5_AirBreathing" : "#LOC_PartInfoInPAW_EngineGimbal_Format5",
						maxGimbalRangeXP.ToString("N1"),
						maxGimbalRangeXN.ToString("N1"),
						maxGimbalRangeYP.ToString("N1"),
						maxGimbalRangeYN.ToString("N1")
					);
				}
			}
			else
			{
				return Localizer.Format("#LOC_PartInfoInPAW_EngineGimbal_NoGimbal");
			}
		}
	}
}
