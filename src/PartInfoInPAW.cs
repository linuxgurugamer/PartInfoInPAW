using System;
using System.Reflection;
using UnityEngine;

namespace PartInfoInPAW
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	internal class PartInfoInPAW : MonoBehaviour
	{
		private static bool moduleManagerInstalled = false;
		private static float kerbonautMass = 0.045f;

		void Awake()
		{
			// Load settings
			Settings.Load();
			if (PartLoader.LoadedPartsList == null)
				return;
			int partsCount = PartLoader.LoadedPartsList.Count;
			ConfigNode emptyNode = new ConfigNode();
			for (int i = 0; i < partsCount; i++)
			{
				AvailablePart part = PartLoader.LoadedPartsList[i];
				if (part != null && !part.name.StartsWith("kerbalEVA") && !part.name.Equals("flag"))
				if (part.partPrefab.GetComponents<ModulePartInfoInPAW>().Length == 0)
				{
					try
					{
						PartModule module = part.partPrefab.AddModule("ModulePartInfoInPAW", true);
						// We should manually add ModulePartInfoInPAW.GetInfo() result to part modules info list
						AvailablePart.ModuleInfo moduleInfo = new AvailablePart.ModuleInfo();
						moduleInfo.moduleName = "ModulePartInfoInPAW";
						moduleInfo.moduleDisplayName = (module as ModulePartInfoInPAW).GetModuleTitle();
						moduleInfo.info = module.GetInfo();
						part.moduleInfos.Add(moduleInfo);
						Utils.LogDebugMsg("Added ModulePartInfoInPAW to part " + part.name);
					}
					catch (Exception e)
					{
						Utils.LogError("Could not add ModulePartInfoInPAW to part " + part.name + ": " + e.Message);
					}
				}
			}
		}

		public void Start()
		{
			// Determine if Module Manager present or not
			foreach (var a in AssemblyLoader.loadedAssemblies)
			{
				AssemblyName nameObject = new AssemblyName(a.assembly.FullName);
				string realName = nameObject.Name;

				if (realName.Equals("ModuleManager"))
				{
					moduleManagerInstalled = true;
					break;
				}
			}
			// Determine kerbonaut mass (it should be the same for all kerbals)
			Part p = PartLoader.getPartInfoByName("kerbalEVA").partPrefab;
			KerbalEVA m = p.FindModuleImplementing<KerbalEVA>();
			if (m != null)
			{
				kerbonautMass = m.initialMass * m.massMultiplier;
			}
		}

		public static bool IsModuleManagerPresent()
		{
			return moduleManagerInstalled;
		}

		public static float GetKerbonautMass()
		{
			return kerbonautMass;
		}
	}
}
