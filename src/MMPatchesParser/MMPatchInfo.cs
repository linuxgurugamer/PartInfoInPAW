using KSP.Localization;
using System;

namespace PartInfoInPAW
{
	internal class MMPatchInfo
	{
		public string PatchFilePath { get; private set; }
		public string Patch { get; private set; }
		public string PatchBody { get; private set; }
		public bool PatchBodyCollapsed { get; private set; }

		public MMPatchInfo(string patchFilePath, string patch, string patchBody)
		{
			PatchFilePath = patchFilePath;
			Patch = patch;
			PatchBody = patchBody;
			PatchBodyCollapsed = true;
		}

		public static string AddExtension(string patch)
		{
			string result = patch;
			if (result[0] == '/')
			{
				result = result.Substring(1);
			}
			result += "." + UrlDir.configExtension;
			return result;
		}

		public void ToggleCollapsed()
		{
			PatchBodyCollapsed = !PatchBodyCollapsed;
		}

		public void Collapse()
		{
			PatchBodyCollapsed = true;
		}

		public void Expand()
		{
			PatchBodyCollapsed = false;
		}

		public bool IsCollapsed()
		{
			return PatchBodyCollapsed;
		}

		public string GetPatchStr()
		{
			if (PatchBody == null)
			{
				return "// " + PatchFilePath + Environment.NewLine +
					Patch + Environment.NewLine +
					"{} // Could not parse patch CFG from " + PatchFilePath + Environment.NewLine + Environment.NewLine;
			}
			return "// " + PatchFilePath + Environment.NewLine +
				Patch + Environment.NewLine +
				PatchBody + Environment.NewLine;
		}

		public override string ToString()
		{
			if (PatchBodyCollapsed)
			{
				return Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_PatchFileName", PatchFilePath);
			}
			else
			{
				if (PatchBody == null)
				{
					return Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_PatchFileName", PatchFilePath) +
						Environment.NewLine + Environment.NewLine +
						Patch + Environment.NewLine +
						Localizer.Format("#LOC_PartInfoInPAW_CantParsePatchCFG_ErrorMsg");
				}
				return Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_PatchFileName", PatchFilePath) +
					Environment.NewLine + Environment.NewLine +
					Patch + Environment.NewLine +
					PatchBody;
			}
		}
	}
}
