using KSP.Localization;
using System;
using System.Reflection;
using UnityEngine;

namespace PartInfoInPAW
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class PartInfoInPAW: MonoBehaviour
	{
		void Awake()
		{
			if (PartLoader.LoadedPartsList == null)
				return;
			foreach (AvailablePart part in PartLoader.LoadedPartsList)
			{
				if (part.partPrefab.GetComponents<ModulePartInfoInPAW>().Length == 0 )
				{
					try
					{
						PartModule module = part.partPrefab.AddModule("ModulePartInfoInPAW", true);
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
			foreach (var a in AssemblyLoader.loadedAssemblies)
			{
				AssemblyName nameObject = new AssemblyName(a.assembly.FullName);
				string realName = nameObject.Name;

				if (realName.Equals("ModuleManager"))
				{
					Utils.ModuleManagerInstalled = true;
					break;
				}
			}
		}
	}
}
