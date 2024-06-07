using KSP.Localization;
using System;
using System.Diagnostics;
using System.IO;
using UniLinq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PartInfoInPAW
{
	public static class Utils
	{
		public const bool LogDebugMsgs = true;
		public static bool ModuleManagerInstalled = false;

		public struct PropellantInfo: IComparable<PropellantInfo>
		{
			public string name;
			public string displayName;
			public string color;
			public Propellant propellant;
			int sortWeight;

			public PropellantInfo(Propellant propellant)
			{
				this.propellant = propellant;
				name = propellant.name;
				displayName = propellant.displayName;
				string propellantName = (propellant.resourceDef != null) ? propellant.resourceDef.displayName : propellant.displayName;
				displayName = Regex.Replace(propellantName, @"\^.$", ""); // remove some silly "^<letter>" stuff from propellant names in russian
				color = GetColor(name);
				sortWeight = GetSortWeight(name);
			}

			public static string GetColor(string propellantName)
			{
				switch (propellantName)
				{
					// Kerosene
					case "LiquidFuel":
					case "Kerosene":
					case "CooledKerosene":
					case "RP-1":
					case "CooledRP-1":
					case "RG-1":
					case "CooledRG-1":
					case "Syntin":
					case "CooledSyntin":
						return "#dfc780";
					// Avgas
					case "AvGas":
						return "#d8bf78";
					// O2
					case "Oxidizer":
					case "LqdOxygen":
					case "CooledLqdOxygen":
					case "Oxygen":
						return "#43b9ec";
					// H2
					case "LqdHydrogen":
					case "Hydrogen":
						return "#b9ecff";
					// Methane
					case "LqdMethane":
					case "Methane":
						return "#20dfaf";
					// Ammonia
					case "LqdAmmonia":
					case "Ammonia":
						return "#22af3e";
					// Alcohols
					case "Ethanol":
					case "Ethanol75":
					case "Ethanol90":
					case "Methanol":
					case "Turpentine":
						return "#ffc5cb";
					// Hydrazine and toxic monopropellants
					case "MonoPropellant":
					case "Hydrazine":
					case "CaveaB":
						return "#cfb343";
					// Toxic fuels
					case "Aerozine50":
					case "Aniline":
					case "Furfuryl":
					case "Hydyne":
					case "MMH":
					case "ANFA22":
					case "ANFA37":
					case "MHF3":
					case "Pentaborane":
					case "Tonka250":
					case "Tonka500":
					case "UDMH":
					case "UH25":
					case "PB-1":
					case "CooledAerozine50":
						return "#ba9941";
					// Toxic oxidizers:
					case "NitricAcid":
					case "AK20":
					case "AK27":
					case "ClF3":
					case "ClF5":
					case "IRFNA-III":
					case "IRFNA-IV":
					case "IWFNA":
					case "NitrousOxide":
					case "NTO":
					case "LqdFluorine":
					case "MON1":
					case "MON3":
					case "MON10":
					case "MON15":
					case "MON20":
					case "MON25":
					case "CooledNTO":
						return "#cade37";
					// Non-toxic monopropellants
					case "HTP":
					case "ASCENT":
						return "#ffffec";
					// Electric charge, megajoules, waste heat, ...
					case "ElectricCharge":
					case "ChargedParticles":
					case "Megajoules":
					case "ThermalPower":
					case "WasteHeat":
						return "#ffff53";
					// Resources from intakes
					case "IntakeAir":
					case "IntakeAtm":
					case "CompressedAir":
						return "#64b9ff";
					case "IntakeLqd":
						return "#54a9ef";
					case "FanIntakeAir":
						return "#84c9df";
					// Solid fuels
					case "SolidFuel":
					case "Aluminium":
					case "HTPB":
					case "PBAN":
					case "PSPC":
					case "NGNC":
						return "#dddddd";
					// Rocky stuff
					case "Ablator":
					case "Ore":
					case "Rock":
					case "Regolith":
						return "#b0a090";
					// Precious propellants
					case "XenonGas":
					case "LqdXenon":
						return "#23ff6b";
					case "ArgonGas":
					case "LqdArgon":
						return "#ff6b23";
					case "KryptonGas":
					case "LqdKrypton":
					case "NeonGas":
					case "LqdNeon":
						return "#ff236b";
					case "Lithium":
					case "Lithium6":
					case "LithiumHydride":
					case "LithiumDeuteride":
						return "#ffd4d0";
					case "Deuterium":
					case "LqdDeuterium":
						return "#d7e9ff";
					case "Tritium":
					case "LqdTritium":
						return "#e7f9ff";
					case "Helium3":
					case "LqdHe3":
						return "#f9bfff";
					// Nuclear and exotic propellants
					case "EnrichedUranium":
					case "UraniumNitride":
					case "Plutonium-238":
					case "FusionPellets":
					case "FissionPellets":
					case "FissionParticles":
					case "NuclearSaltWater":
					case "MtlHydrogen":
					case "SolidHydrogen":
					case "Karborundum":
					case "UF4":
						return "#d0a8ff";
					// Antimatter and exotic matter
					case "Antimatter":
					case "AntiHydrogen":
					case "ExoticMatter":
					case "VacuumPlasma":
						return "#ff8080";
					default:
						return null;
				}
			}

			public static int GetSortWeight(string propellantName)
			{
				switch (propellantName)
				{
					// Oxidizers
					case "Oxidizer":
					case "LqdOxygen":
					case "CooledLqdOxygen":
					case "Oxygen":
					case "NitricAcid":
					case "AK20":
					case "AK27":
					case "ClF3":
					case "ClF5":
					case "IRFNA-III":
					case "IRFNA-IV":
					case "IWFNA":
					case "NitrousOxide":
					case "NTO":
					case "LqdFluorine":
					case "MON1":
					case "MON3":
					case "MON10":
					case "MON15":
					case "MON20":
					case "MON25":
					case "CooledNTO":
						return 10;
					// Pressurization gases and stuff to power engine pump
					case "HTP":
					case "Helium":
					case "Nitrogen":
						return 20;
					// Precious propellants
					case "Deuterium":
					case "LqdDeuterium":
					case "Tritium":
					case "LqdTritium":
					case "Helium3":
					case "LqdHe3":
					case "Lithium":
					case "Lithium6":
					case "LithiumHydride":
					case "LithiumDeuteride":
					case "XenonGas":
					case "LqdXenon":
					case "ArgonGas":
					case "LqdArgon":
					case "KryptonGas":
					case "LqdKrypton":
					case "NeonGas":
					case "LqdNeon":
						return 30;
					// Nuclear and exotic propellants
					case "EnrichedUranium":
					case "UraniumNitride":
					case "Plutonium-238":
					case "FusionPellets":
					case "FissionPellets":
					case "FissionParticles":
					case "NuclearSaltWater":
					case "MtlHydrogen":
					case "SolidHydrogen":
					case "Karborundum":
					case "UF4":
					case "Thorium":
					case "ThF4":
						return 40;
					// Antimatter and exotic matter
					case "Antimatter":
					case "AntiHydrogen":
					case "ExoticMatter":
					case "VacuumPlasma":
						return 50;
					// Resources from intakes
					case "IntakeAir":
					case "IntakeAtm":
					case "FanIntakeAir":
					case "IntakeLqd":
					case "CompressedAir":
						return 60;
					// Electric charge, megajoules, waste heat, ...
					case "ElectricCharge":
					case "ChargedParticles":
					case "Megajoules":
					case "ThermalPower":
					case "WasteHeat":
						return 100;
					default:
						return 0;
				}
			}

			public int CompareTo(PropellantInfo other)
			{
				if (sortWeight != other.sortWeight)
				{
					return sortWeight.CompareTo(other.sortWeight);
				}
				else
				{
					return name.CompareTo(other.name);
				}
			}
		}

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

		public static string ReplaceFirstOccurrence(string source, string find, string replace)
		{
			int n = source.IndexOf(find);
			string result = source.Remove(n, find.Length).Insert(n, replace);
			return result;
		}

		public static string GetPartFilePath(this Part part)
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

		public static string GetPropellantsInfo(this ModuleEngines engine)
		{
			if (engine.propellants.Count > 0)
			{
				List<PropellantInfo> propellantsInfo = new List<PropellantInfo>();
				List<string> propellantsInfoStr = new List<string>();
				engine.propellants.ForEach(delegate(Propellant p)
				{
					propellantsInfo.Add(new PropellantInfo(p));
				});
				propellantsInfo.Sort();
				string[] showFuelFlowForPropellants = new string[] { "ElectricCharge", "IntakeAir", "IntakeAtm" };
				foreach (PropellantInfo p in propellantsInfo)
				{
					if (showFuelFlowForPropellants.Contains(p.name))
					{
						float propellantFlow = engine.getMaxFuelFlow(p.propellant) * engine.thrustPercentage / 100f;
						propellantsInfoStr.Add(
							WrapStringWithBoldColorTags(
								Localizer.Format(
									"#LOC_PartInfoInPAW_EnginePropellantFlow_Format",
									p.displayName,
									propellantFlow.ToString("N2")
								),
								p.color
							)
						);
					}
					else
					{
						propellantsInfoStr.Add(WrapStringWithBoldColorTags(p.displayName, p.color));
					}
				}
				int pCount = propellantsInfoStr.Count;
				if (pCount > 2)
				{
					int n = pCount / 2;
					List<string> lines = new List<string>();
					for (int i = 0; i <= n; i++)
					{
						if ((pCount - i * 2) > 0)
						{
							lines.Add(String.Join(", ", propellantsInfoStr.GetRange(i * 2, Math.Min(2, pCount - i * 2))));
						}
					}
					return "\n" + String.Join(",\n", lines);
				}
				else
				{
					return "\n" + String.Join(", ", propellantsInfoStr);
				}
			}
			return "";
		}

		public static string GetThrustInfo(this ModuleEngines engine)
		{
			float thrustVac = engine.GetEngineThrust(engine.atmosphereCurve.Evaluate(0f), engine.thrustPercentage / 100f);
			float thrustASL = engine.GetEngineThrust(engine.atmosphereCurve.Evaluate(1f), engine.thrustPercentage / 100f);
			bool airBreathingEngine = false;
			float maxMach = 0f;
			if (engine.useVelCurve)
			{
				airBreathingEngine = true;
				thrustASL *= engine.velCurve.Evaluate(0f);
				engine.velCurve.FindMinMaxValue(out _, out float maxCoeff, out _, out maxMach);
				thrustVac *= maxCoeff;
			}
			if (Math.Max(thrustVac, thrustASL) < 1.0f)
			{
				// thrust in newtons
				if (airBreathingEngine)
				{
					if (Math.Abs(thrustVac - thrustASL) < float.Epsilon)
					{
						return Localizer.Format(
							"#LOC_PartInfoInPAW_EngineThrust_FormatSimple",
							(thrustASL * 1000f).ToString("N1"),
							Localizer.Format("#LOC_PartInfoInPAW_N_Unit")
						);
					}
					else
					{
						return Localizer.Format(
							"#LOC_PartInfoInPAW_EngineThrust_FormatAirBreathing",
							(thrustASL * 1000f).ToString("N1"),
							(thrustVac * 1000f).ToString("N1"),
							Localizer.Format("#LOC_PartInfoInPAW_N_Unit"),
							maxMach.ToString("N1")
						);
					}
				}
				else
				{
					return Localizer.Format(
						"#LOC_PartInfoInPAW_EngineThrust_Format",
						(thrustVac * 1000f).ToString("N1"),
						(thrustASL * 1000f).ToString("N1"),
						Localizer.Format("#LOC_PartInfoInPAW_N_Unit")
					);
				}
			}
			else if (Math.Min(thrustVac, thrustASL) > 2000.0f)
			{
				// thrust in meganewtons
				if (airBreathingEngine)
				{
					if (Math.Abs(thrustVac - thrustASL) < float.Epsilon)
					{
						return Localizer.Format(
							"#LOC_PartInfoInPAW_EngineThrust_FormatSimple",
							(thrustASL / 1000f).ToString("N3"),
							Localizer.Format("#LOC_PartInfoInPAW_MN_Unit")
						);
					}
					else
					{
						return Localizer.Format(
							"#LOC_PartInfoInPAW_EngineThrust_FormatAirBreathing",
							(thrustASL / 1000f).ToString("N3"),
							(thrustVac / 1000f).ToString("N3"),
							Localizer.Format("#LOC_PartInfoInPAW_MN_Unit"),
							maxMach.ToString("N1")
						);
					}
				}
				else
				{
					return Localizer.Format(
						"#LOC_PartInfoInPAW_EngineThrust_Format",
						(thrustVac / 1000f).ToString("N3"),
						(thrustASL / 1000f).ToString("N3"),
						Localizer.Format("#LOC_PartInfoInPAW_MN_Unit")
					);
				}
			}
			else
			{
				// thrust in kilonewtons
				if (airBreathingEngine)
				{
					if (Math.Abs(thrustVac - thrustASL) < float.Epsilon)
					{
						return Localizer.Format(
							"#LOC_PartInfoInPAW_EngineThrust_FormatSimple",
							(thrustASL).ToString("N2"),
							Localizer.Format("#LOC_PartInfoInPAW_KN_Unit")
						);
					}
					else
					{
						return Localizer.Format(
							"#LOC_PartInfoInPAW_EngineThrust_FormatAirBreathing",
							(thrustASL).ToString("N2"),
							(thrustVac).ToString("N2"),
							Localizer.Format("#LOC_PartInfoInPAW_KN_Unit"),
							maxMach.ToString("N1")
						);
					}
				}
				else
				{
					return Localizer.Format(
						"#LOC_PartInfoInPAW_EngineThrust_Format",
						(thrustVac).ToString("N2"),
						(thrustASL).ToString("N2"),
						Localizer.Format("#LOC_PartInfoInPAW_KN_Unit")
					);
				}
			}
		}

		public static string GetISPInfo(this ModuleEngines engine)
		{
			float ISPVac = engine.atmosphereCurve.Evaluate(0f);
			float ISPASL = engine.atmosphereCurve.Evaluate(1f);
			if (Math.Abs(ISPASL - ISPVac) < float.Epsilon)
			{
				return Localizer.Format("#LOC_PartInfoInPAW_EngineISP_FormatSimple", ISPVac.ToString("N0"));
			}
			else
			{
				return Localizer.Format("#LOC_PartInfoInPAW_EngineISP_Format", ISPVac.ToString("N0"), ISPASL.ToString("N0"));
			}
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

		public static string WrapStringWithBoldColorTags(string strToWrap, string color)
		{
			if (color == null)
			{
				return "<b>" + strToWrap + "</b>";
			}
			return "<color=" + color + "><b>" + strToWrap + "</b></color>";
		}

		public static string GetOrigPartConfigNodeText(this Part part)
		{
			string origFilePath = part.GetPartFilePath();
			string origFileContent;
			try
			{
				origFileContent = File.ReadAllText(origFilePath);
			}
			catch (Exception e)
			{
				LogError($"Could not read part CFG file {origFilePath}: " + e.Message);
				OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_OpenFile_PartOrigFile_FailureMsg", origFilePath));
				return "";
			}
			return origFileContent;
		}

		public static string GetPartMMPatchesHistory(Part part, List<MMPatchInfo> patchesList, bool includePartOrigCFG)
		{
			string result = "";
			if (includePartOrigCFG)
			{
				result = "// Original part CFG" + Environment.NewLine + part.GetOrigPartConfigNodeText() + Environment.NewLine;
			}
			foreach (MMPatchInfo patchInfo in patchesList)
			{
				result += patchInfo.GetPatchStr();
			}
			return result;
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
	}
}
