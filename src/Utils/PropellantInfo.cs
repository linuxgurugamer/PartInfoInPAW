using System;
using System.Text.RegularExpressions;

namespace PartInfoInPAW
{
	internal struct PropellantInfo : IComparable<PropellantInfo>
	{
		public string name;
		public string displayName;
		public string color;
		public Propellant propellant;
		readonly int sortWeight;

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
				// Toxic oxidizers
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
				// Pressurization gases
				case "Helium":
				case "Nitrogen":
					return "#84d9ff";
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
}
