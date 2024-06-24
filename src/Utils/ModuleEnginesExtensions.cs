using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartInfoInPAW
{
	internal static class ModuleEnginesExtensions
	{
		public static string GetPropellantsInfo(this ModuleEngines engine)
		{
			if (engine.propellants.Count > 0)
			{
				List<PropellantInfo> propellantsInfo = new List<PropellantInfo>();
				List<string> propellantsInfoStr = new List<string>();
				engine.propellants.ForEach(delegate (Propellant p)
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
							Utils.WrapStringWithBoldColorTags(
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
						propellantsInfoStr.Add(Utils.WrapStringWithBoldColorTags(p.displayName, p.color));
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
			if (Math.Max(thrustVac, thrustASL) < 0.001f)
			{
				// thrust in millinewtons
				if (airBreathingEngine)
				{
					if (Math.Abs(thrustVac - thrustASL) < float.Epsilon)
					{
						return Localizer.Format(
							"#LOC_PartInfoInPAW_EngineThrust_FormatSimple",
							(thrustASL * 1000000f).ToString("N1"),
							Localizer.Format("#LOC_PartInfoInPAW_mN_Unit")
						);
					}
					else
					{
						return Localizer.Format(
							"#LOC_PartInfoInPAW_EngineThrust_FormatAirBreathing",
							(thrustASL * 1000000f).ToString("N1"),
							(thrustVac * 1000000f).ToString("N1"),
							Localizer.Format("#LOC_PartInfoInPAW_mN_Unit"),
							maxMach.ToString("N1")
						);
					}
				}
				else
				{
					return Localizer.Format(
						"#LOC_PartInfoInPAW_EngineThrust_Format",
						(thrustVac * 1000000f).ToString("N1"),
						(thrustASL * 1000000f).ToString("N1"),
						Localizer.Format("#LOC_PartInfoInPAW_mN_Unit")
					);
				}
			}
			else if (Math.Max(thrustVac, thrustASL) < 1.0f)
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
	}
}
