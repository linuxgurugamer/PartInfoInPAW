using System;
using System.Collections.Generic;

namespace PartInfoInPAW
{
	internal static class MMPatchesUtils
	{
		public static string GetPartMMPatchesHistory(Part part, List<MMPatchInfo> patchesList, bool includePartOrigCFG)
		{
			string result = "";
			if (includePartOrigCFG)
			{
				result = "// Original part CFG (" + part.GetPartGameDataFilePath() + ")" + Environment.NewLine +
					part.GetOrigPartConfigNodeText() + Environment.NewLine;
			}
			foreach (MMPatchInfo patchInfo in patchesList)
			{
				result += patchInfo.GetPatchStr();
			}
			return result;
		}
	}
}
