using System.Collections.Generic;

namespace PartInfoInPAW
{
	internal class MMPatchesPatchGUILabel: DialogGUILabel
	{
		public MMPatchesPatchGUILabel(
			List<MMPatchInfo> patchesList,
			int index,
			bool expandW = false,
			bool expandH = false
		): base(
			() => GetText(patchesList, index),
			expandW,
			expandH
		)
		{
		}

		private static string GetText(List<MMPatchInfo> patchesList, int index)
		{
			return patchesList[index].ToString();
		}
	}
}
