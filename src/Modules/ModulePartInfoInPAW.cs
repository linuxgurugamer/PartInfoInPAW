using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PartInfoInPAW
{
	public class ModulePartInfoInPAW : PartModule
	{
		#region GUI: Fields

		[KSPField(isPersistant = false, guiActiveEditor = true, guiActive = false, guiName = "#LOC_PartInfoInPAW_PartName_Title", groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle", groupStartCollapsed = true)]
		public string partName = "";

		[KSPField(isPersistant = false, guiActiveEditor = true, guiActive = false, guiName = "#LOC_PartInfoInPAW_PartMod_Title", groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle", groupStartCollapsed = true)]
		public string partModName = "";

		[KSPField(isPersistant = false, guiActiveEditor = true, guiActive = false, guiName = "#LOC_PartInfoInPAW_PartDryMass_Title", groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle", groupStartCollapsed = true)]
		public string partMass = "0 kg";

		[KSPField(isPersistant = false, guiActiveEditor = true, guiActive = false, guiName = "#LOC_PartInfoInPAW_PartCost_Title", groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle", groupStartCollapsed = true)]
		public string partCost = "0";

		[KSPField(isPersistant = false, guiActiveEditor = true, guiActive = false, guiName = "#LOC_PartInfoInPAW_PartEntryCost_Title", groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle", groupStartCollapsed = true)]
		public int partEntryCost = 0;

		[KSPField(isPersistant = false, guiActiveEditor = false, guiActive = false, guiName = "#LOC_PartInfoInPAW_EnginePropellants_Title", groupName = "engine1Info", groupDisplayName = "#LOC_PartInfoInPAW_Engine1Info_GroupTitle", groupStartCollapsed = true)]
		public string engine1Propellants = "";

		[KSPField(isPersistant = false, guiActiveEditor = false, guiActive = false, guiName = "#LOC_PartInfoInPAW_EngineThrust_Title", groupName = "engine1Info", groupDisplayName = "#LOC_PartInfoInPAW_Engine1Info_GroupTitle", groupStartCollapsed = true)]
		public string engine1Thrust = "";

		[KSPField(isPersistant = false, guiActiveEditor = false, guiActive = false, guiName = "#LOC_PartInfoInPAW_EngineMinThrust_Title", groupName = "engine1Info", groupDisplayName = "#LOC_PartInfoInPAW_Engine1Info_GroupTitle", groupStartCollapsed = true, guiUnits = "%")]
		public int engine1MinThrust = 0;

		[KSPField(isPersistant = false, guiActiveEditor = false, guiActive = false, guiName = "#LOC_PartInfoInPAW_EngineISP_Title", groupName = "engine1Info", groupDisplayName = "#LOC_PartInfoInPAW_Engine1Info_GroupTitle", groupStartCollapsed = true)]
		public string engine1ISP = "";

		[KSPField(isPersistant = false, guiActiveEditor = false, guiActive = false, guiName = "#LOC_PartInfoInPAW_EngineGimbal_Title", groupName = "engine1Info", groupDisplayName = "#LOC_PartInfoInPAW_Engine1Info_GroupTitle", groupStartCollapsed = true)]
		public string engine1Gimbal = "";

		[KSPField(isPersistant = false, guiActiveEditor = false, guiActive = false, guiName = "#LOC_PartInfoInPAW_EnginePropellants_Title", groupName = "engine2Info", groupDisplayName = "#LOC_PartInfoInPAW_Engine2Info_GroupTitle", groupStartCollapsed = true)]
		public string engine2Propellants = "";

		[KSPField(isPersistant = false, guiActiveEditor = false, guiActive = false, guiName = "#LOC_PartInfoInPAW_EngineThrust_Title", groupName = "engine2Info", groupDisplayName = "#LOC_PartInfoInPAW_Engine2Info_GroupTitle", groupStartCollapsed = true)]
		public string engine2Thrust = "";

		[KSPField(isPersistant = false, guiActiveEditor = false, guiActive = false, guiName = "#LOC_PartInfoInPAW_EngineMinThrust_Title", groupName = "engine2Info", groupDisplayName = "#LOC_PartInfoInPAW_Engine2Info_GroupTitle", groupStartCollapsed = true, guiUnits = "%")]
		public int engine2MinThrust = 0;

		[KSPField(isPersistant = false, guiActiveEditor = false, guiActive = false, guiName = "#LOC_PartInfoInPAW_EngineISP_Title", groupName = "engine2Info", groupDisplayName = "#LOC_PartInfoInPAW_Engine2Info_GroupTitle", groupStartCollapsed = true)]
		public string engine2ISP = "";

		[KSPField(isPersistant = false, guiActiveEditor = false, guiActive = false, guiName = "#LOC_PartInfoInPAW_EngineGimbal_Title", groupName = "engine2Info", groupDisplayName = "#LOC_PartInfoInPAW_Engine2Info_GroupTitle", groupStartCollapsed = true)]
		public string engine2Gimbal = "";

		#endregion GUI: Fields

		#region GUI: Buttons

		[KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "#LOC_PartInfoInPAW_CopyPartName_Action", active = true, groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle")]
		public void CopyPartName()
		{
			try
			{
				if (partName == "")
				{
					partName = part.GetPartName();
				}
				GUIUtility.systemCopyBuffer = partName;
				Utils.Log($"Part {part.partInfo.name} : ID copied to clipboard");
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CopyToClipboard_PartID_SuccessMsg", partName), 2.0f);
			}
			catch (Exception e)
			{
				Utils.LogError($"Part {part.partInfo.name} : failed to copy part ID to clipboard: " + e.Message);
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CopyToClipboard_PartID_FailureMsg", partName));
			}
		}

		[KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "#LOC_PartInfoInPAW_CopyPartNode_Action", active = true, groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle")]
		public void CopyPartConfigNode()
		{
			string partCFG;
			try
			{
				partCFG = GetConfigNodeText();
			}
			catch (Exception e)
			{
				Utils.LogError(e.Message);
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_GetNode_PartCFG_FailureMsg", partName));
				return;
			}
			try
			{
				GUIUtility.systemCopyBuffer = partCFG;
				Utils.Log($"Part {part.partInfo.name} : CFG node copied to clipboard");
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CopyToClipboard_PartCFG_SuccessMsg", partName), 2.0f);
			}
			catch (Exception e)
			{
				Utils.LogError($"Part {part.partInfo.name} : failed to copy CFG node to clipboard: " + e.Message);
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CopyToClipboard_PartCFG_FailureMsg", partName));
			}
		}

		[KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "#LOC_PartInfoInPAW_OpenPartCFGInEditor_Action", active = true, groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle")]
		public void OpenPartCFGInEditor()
		{
			string partCFG;

			ConfigNode cfg = GameDatabase.Instance.GetConfigNode(part.partInfo.partUrl) ?? part.partInfo.partConfig;
			try
			{
				partCFG = GetConfigNodeText();
			}
			catch (Exception e)
			{
				Utils.LogError(e.Message);
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_GetNode_PartCFG_FailureMsg", partName));
				return;
			}
			string fileName = partName;
			if (fileName == "")
			{
				fileName = part.GetPartName();
			}
			foreach (var c in Path.GetInvalidFileNameChars())
			{
				fileName = fileName.Replace(c, '-');
			}
			string filePath = Path.Combine(Path.GetTempPath(), Process.GetCurrentProcess().Id.ToString() + "_" + fileName + "." + UrlDir.configExtension);
			if (!File.Exists(filePath))
			{
				try
				{
					File.WriteAllText(filePath, partCFG, Encoding.UTF8);
				}
				catch (Exception e)
				{
					Utils.LogError($"Could not write part CFG to file {filePath}: " + e.Message);
					Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CantWriteToTempFile_PartCFG_FailureMsg", filePath));
					return;
				}
			}
			Utils.ShellOpenFile(filePath);
		}

		[KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "#LOC_PartInfoInPAW_CopyOrigPartNode_Action", active = true, groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle")]
		public void CopyOrigPartConfigNode()
		{
			string origFilePath = part.GetPartFilePath();
			string origFileContent = part.GetOrigPartConfigNodeText();
			try
			{
				GUIUtility.systemCopyBuffer = origFileContent;
				Utils.Log($"Part {part.partInfo.name} : CFG file {origFilePath.Replace('\\', '/')} copied to clipboard");
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CopyToClipboard_PartOrigFile_SuccessMsg", partName), 2.0f);
			}
			catch (Exception e)
			{
				Utils.LogError($"Part {part.partInfo.name} : failed to copy original CFG file to clipboard: " + e.Message);
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CopyToClipboard_PartOrigFile_FailureMsg", partName));
			}
		}

		[KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "#LOC_PartInfoInPAW_OpenOrigPartCFGInEditor_Action", active = true, groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle")]
		public void OpenOrigPartCFGInEditor()
		{
			Utils.ShellOpenFile(part.GetPartFilePath());
		}

		[KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "#LOC_PartInfoInPAW_ShowPartMMPatchesHistory_Action", active = true, groupName = "partInfo", groupDisplayName = "#LOC_PartInfoInPAW_PartInfo_GroupTitle")]
		public void ShowMMPatchesHistory()
		{
			MMPatchesHistoryDialog.ShowDialog(partName, part.partInfo.title, part);
		}

		#endregion GUI: Buttons

		protected ModuleEngines engine1;
		protected ModuleEngines engine2;

		protected bool InfoUpdated = false;

		#region ModWithComplexFoldersStruct

		public readonly struct ModWithComplexFoldersStruct
		{
			private readonly string ModFolder;
			private readonly int NestingLevel;

			public ModWithComplexFoldersStruct(string folder, int nestLevel = 2)
			{
				ModFolder = folder;
				NestingLevel = nestLevel;
			}

			public bool URLMatches(string url)
			{
				return (ModFolder == url.Split('/')[0]);
			}

			public string BuildModName(string url)
			{
				int nestLevel = NestingLevel;
				string[] folders = url.Split('/');
				string result = "";
				if (folders.Length < nestLevel)
				{
					nestLevel = folders.Length;
				}
				for (int i = 0; i < nestLevel; i++)
				{
					if (i > 0)
					{
						result += "/" + folders[i];
					}
					else
					{
						result += folders[i];
					}
				}
				return result;
			}
		}

		protected static List<ModWithComplexFoldersStruct> ModsWithComplexFoldersStruct = new List<ModWithComplexFoldersStruct>()
		{
			new ModWithComplexFoldersStruct("RealEnginesPack"),
			new ModWithComplexFoldersStruct("reDIRECT"),
			new ModWithComplexFoldersStruct("SquadExpansion"),
			new ModWithComplexFoldersStruct("UmbraSpaceIndustries"),
			new ModWithComplexFoldersStruct("WildBlueIndustries"),
			new ModWithComplexFoldersStruct("Bluedog_DB", 3),
		};

		#endregion ModWithComplexFoldersStruct

		private void Start()
		{
			GameEvents.onEditorShipModified.Add(EditorShipModified);

			PartInfoInPAWGameSettings_PartInfo settingsPartInfo = HighLogic.CurrentGame.Parameters.CustomParams<PartInfoInPAWGameSettings_PartInfo>();

			Fields["partName"].guiActiveEditor = settingsPartInfo.showPartInfo;
			Fields["partModName"].guiActiveEditor = settingsPartInfo.showPartInfo;
			Fields["partMass"].guiActiveEditor = settingsPartInfo.showPartInfo;
			Fields["partCost"].guiActiveEditor = settingsPartInfo.showPartInfo;
			Fields["partEntryCost"].guiActiveEditor = settingsPartInfo.showPartInfo;

			Events["CopyPartName"].guiActiveEditor = settingsPartInfo.showCopyPartNameBtn;
			Events["CopyPartConfigNode"].guiActiveEditor = settingsPartInfo.showCopyPartNodeBtn && Utils.ModuleManagerInstalled;
			Events["OpenPartCFGInEditor"].guiActiveEditor = settingsPartInfo.showOpenPartCFGInEditorBtn && Utils.ModuleManagerInstalled;
			Events["CopyOrigPartConfigNode"].guiActiveEditor = settingsPartInfo.showCopyOrigPartNodeBtn;
			Events["OpenOrigPartCFGInEditor"].guiActiveEditor = settingsPartInfo.showOpenOrigPartCFGInEditorBtn;
			Events["ShowMMPatchesHistory"].guiActiveEditor = settingsPartInfo.showPartMMPatchesHistoryBtn && Utils.ModuleManagerInstalled;
		}

		private void OnDestroy()
		{
			GameEvents.onEditorShipModified.Remove(EditorShipModified);
		}

		public override void OnUpdate()
		{
			// disabled in flight scene
			isEnabled = false;
		}

		private void EditorShipModified(ShipConstruct construct)
		{
			InfoUpdated = false;
		}

		public void Update()
		{
			if (!HighLogic.LoadedSceneIsEditor)
			{
				enabled = false;
				return;
			}
			// No UI update needed if PAW menu is not opened
			if (part.PartActionWindow == null || !(part.PartActionWindow.isActiveAndEnabled))
			{
				return;
			}
			if (!InfoUpdated)
			{
				UpdateInfo();
			}
		}

		private void UpdateInfo()
		{
			if (partName == "")
			{
				partName = part.GetPartName();
			}
			if (partModName == "")
			{
				partModName = GetPartModName();
			}

			ModuleEngines[] engines;
			MultiModeEngine[] multiModeEngines;

			float prefabMass = part.partInfo.partPrefab.mass;
			float dryMass = prefabMass + part.GetModuleMass(prefabMass);
			float resMass = part.GetResourceMass();
			float wetMass = dryMass + resMass;
			if (Math.Abs(resMass) <= float.Epsilon)
			{
				// Dry mass only
				Fields["partMass"].guiName = Localizer.Format("#LOC_PartInfoInPAW_PartDryMass_Title");
				partMass = Utils.FormatMass(dryMass);
			}
			else
			{
				// Dry mass / wet mass
				Fields["partMass"].guiName = Localizer.Format("#LOC_PartInfoInPAW_PartDryWetMass_Title");
				partMass = Utils.FormatMass(dryMass) + " / " + Utils.FormatMass(wetMass);
			}
			float partFullCost = part.partInfo.cost + part.GetModuleCosts(part.partInfo.cost);
			float fullCost = partFullCost + part.GetResourceCostOffset();
			float emptyCost = partFullCost - part.GetResourceCostMax();
			if (Math.Abs(fullCost - emptyCost) < float.Epsilon)
			{
				// Cost without resources
				Fields["partCost"].guiName = Localizer.Format("#LOC_PartInfoInPAW_PartCost_Title");
				partCost = fullCost.ToString("F0");
			}
			else
			{
				// Full cost (with resources) and cost without resources
				Fields["partCost"].guiName = Localizer.Format("#LOC_PartInfoInPAW_PartEmptyCostFullCost_Title");
				partCost = emptyCost.ToString("F0") + " / " + fullCost.ToString("F0");
			}

			partEntryCost = part.partInfo.entryCost;

			engines = part.GetComponents<ModuleEngines>();
			PartInfoInPAWGameSettings_EngineInfo settingsEngineInfo = HighLogic.CurrentGame.Parameters.CustomParams<PartInfoInPAWGameSettings_EngineInfo>();
			if (engines.Length == 0 || !settingsEngineInfo.showEngineInfo)
			{
				Fields["engine1Propellants"].guiActiveEditor = false;
				Fields["engine1Thrust"].guiActiveEditor = false;
				Fields["engine1MinThrust"].guiActiveEditor = false;
				Fields["engine1ISP"].guiActiveEditor = false;
				Fields["engine1Gimbal"].guiActiveEditor = false;
				Fields["engine2Propellants"].guiActiveEditor = false;
				Fields["engine2Thrust"].guiActiveEditor = false;
				Fields["engine2MinThrust"].guiActiveEditor = false;
				Fields["engine2ISP"].guiActiveEditor = false;
				Fields["engine2Gimbal"].guiActiveEditor = false;
			}
			if (engines.Length > 0 && settingsEngineInfo.showEngineInfo)
			{
				engine1 = engines[0];
				if (settingsEngineInfo.showEnginesPropellantsInfo)
				{
					engine1Propellants = engine1.GetPropellantsInfo();
				}
				if (settingsEngineInfo.showEnginesThrustInfo)
				{
					engine1Thrust = engine1.GetThrustInfo();
				}
				if (settingsEngineInfo.showEnginesMinThrustInfo && engine1.minThrust > float.Epsilon)
				{
					if (engine1.maxThrust > float.Epsilon)
					{
						engine1MinThrust = (int)Math.Round(engine1.minThrust * 100 / engine1.maxThrust);
					}
					else
					{
						engine1MinThrust = 0;
					}
				}
				if (settingsEngineInfo.showEnginesISPInfo)
				{
					engine1ISP = engine1.GetISPInfo();
				}
				if (settingsEngineInfo.showEnginesGimbalInfo)
				{
					engine1Gimbal = part.GetGimbalInfo(engine1);
				}
				Fields["engine1Propellants"].guiActiveEditor = settingsEngineInfo.showEnginesPropellantsInfo;
				Fields["engine1Thrust"].guiActiveEditor = settingsEngineInfo.showEnginesThrustInfo;
				Fields["engine1MinThrust"].guiActiveEditor = settingsEngineInfo.showEnginesMinThrustInfo && (engine1.minThrust > float.Epsilon);
				Fields["engine1ISP"].guiActiveEditor = settingsEngineInfo.showEnginesISPInfo;
				Fields["engine1Gimbal"].guiActiveEditor = settingsEngineInfo.showEnginesGimbalInfo;
				if (engines.Length > 1)
				{
					engine2 = engines[1];
					if (settingsEngineInfo.showEnginesPropellantsInfo)
					{
						engine2Propellants = engine2.GetPropellantsInfo();
					}
					if (settingsEngineInfo.showEnginesThrustInfo)
					{
						engine2Thrust = engine2.GetThrustInfo();
					}
					if (engine2.minThrust > float.Epsilon)
					{
						if (engine2.maxThrust > float.Epsilon)
						{
							engine2MinThrust = (int)Math.Round(engine2.minThrust * 100 / engine2.maxThrust);
						}
						else
						{
							engine2MinThrust = 0;
						}
					}
					if (settingsEngineInfo.showEnginesISPInfo)
					{
						engine2ISP = engine2.GetISPInfo();
					}
					if (settingsEngineInfo.showEnginesGimbalInfo)
					{
						engine2Gimbal = part.GetGimbalInfo(engine2);
					}
					Fields["engine2Propellants"].guiActiveEditor = settingsEngineInfo.showEnginesPropellantsInfo;
					Fields["engine2Thrust"].guiActiveEditor = settingsEngineInfo.showEnginesThrustInfo;
					Fields["engine2MinThrust"].guiActiveEditor = settingsEngineInfo.showEnginesMinThrustInfo && (engine2.minThrust > float.Epsilon);
					Fields["engine2ISP"].guiActiveEditor = settingsEngineInfo.showEnginesISPInfo;
					Fields["engine2Gimbal"].guiActiveEditor = settingsEngineInfo.showEnginesGimbalInfo;
					multiModeEngines = part.GetComponents<MultiModeEngine>();
					if (multiModeEngines.Length > 0)
					{
						if (engine1.engineID == multiModeEngines[0].primaryEngineID)
						{
							Fields["engine1Propellants"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].primaryEngineModeDisplayName);
							Fields["engine1Thrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].primaryEngineModeDisplayName);
							Fields["engine1MinThrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].primaryEngineModeDisplayName);
							Fields["engine1ISP"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].primaryEngineModeDisplayName);
							Fields["engine1Gimbal"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].primaryEngineModeDisplayName);
						}
						else if (engine1.engineID == multiModeEngines[0].secondaryEngineID)
						{
							Fields["engine1Propellants"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].secondaryEngineModeDisplayName);
							Fields["engine1Thrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].secondaryEngineModeDisplayName);
							Fields["engine1MinThrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].secondaryEngineModeDisplayName);
							Fields["engine1ISP"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].secondaryEngineModeDisplayName);
							Fields["engine1Gimbal"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].secondaryEngineModeDisplayName);
						}
						else
						{
							Fields["engine1Propellants"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_Engine1Info_GroupTitle");
							Fields["engine1Thrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_Engine1Info_GroupTitle");
							Fields["engine1MinThrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_Engine1Info_GroupTitle");
							Fields["engine1ISP"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_Engine1Info_GroupTitle");
							Fields["engine1Gimbal"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_Engine1Info_GroupTitle");
						}
						if (engine2.engineID == multiModeEngines[0].primaryEngineID)
						{
							Fields["engine2Propellants"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].primaryEngineModeDisplayName);
							Fields["engine2Thrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].primaryEngineModeDisplayName);
							Fields["engine2MinThrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].primaryEngineModeDisplayName);
							Fields["engine2ISP"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].primaryEngineModeDisplayName);
							Fields["engine2Gimbal"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].primaryEngineModeDisplayName);
						}
						else if (engine2.engineID == multiModeEngines[0].secondaryEngineID)
						{
							Fields["engine2Propellants"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].secondaryEngineModeDisplayName);
							Fields["engine2Thrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].secondaryEngineModeDisplayName);
							Fields["engine2MinThrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].secondaryEngineModeDisplayName);
							Fields["engine2ISP"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].secondaryEngineModeDisplayName);
							Fields["engine2Gimbal"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_MultimodeEngineInfo_GroupTitle", multiModeEngines[0].secondaryEngineModeDisplayName);
						}
						else
						{
							Fields["engine2Propellants"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_Engine2Info_GroupTitle");
							Fields["engine2Thrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_Engine2Info_GroupTitle");
							Fields["engine2MinThrust"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_Engine2Info_GroupTitle");
							Fields["engine2ISP"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_Engine2Info_GroupTitle");
							Fields["engine2Gimbal"].group.displayName = Localizer.Format("#LOC_PartInfoInPAW_Engine2Info_GroupTitle");
						}
					}
				}
				else
				{
					Fields["engine2Propellants"].guiActiveEditor = false;
					Fields["engine2Thrust"].guiActiveEditor = false;
					Fields["engine2MinThrust"].guiActiveEditor = false;
					Fields["engine2ISP"].guiActiveEditor = false;
					Fields["engine2Gimbal"].guiActiveEditor = false;
				}
			}
			InfoUpdated = true;
		}

		private string GetPartModName()
		{
			string url = part.partInfo.partUrl;
			foreach (var mod in ModsWithComplexFoldersStruct)
			{
				if (mod.URLMatches(url))
				{
					return "\n" + mod.BuildModName(url);
				}
			}
			return url.Split('/')[0];
		}

		private string GetConfigNodeText()
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

		public override string GetInfo()
		{
			if (partName == "")
			{
				partName = part.GetPartName();
			}
			string[] urlSegments = part.partInfo.partUrl.Split('/');
			if (urlSegments.Length > 1)
			{
				Array.Resize(ref urlSegments, urlSegments.Length - 1);
			}
			string partURL = String.Join("<color=#a0a0a0>/</color><br>", urlSegments) + "." + UrlDir.configExtension;
			return Localizer.Format("#LOC_PartInfoInPAW_PartModuleInfo", partName, partURL);
		}
	}
}
