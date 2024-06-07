using System.Collections.Generic;

namespace PartInfoInPAW
{
	public class MMPatchesCollapseExpandGUIButton: DialogGUIButton
	{
		private const string expandText = "+";
		private const string collapseText = "-";

		public MMPatchesCollapseExpandGUIButton(
			List<MMPatchInfo> patchesList,
			int index,
			float w,
			float h,
			UIStyle style
		): base(
			() => GetBtnText(patchesList, index),
			() => {
				Toggle(patchesList, index);
			},
			() => true,
			w,
			h,
			false,
			style
		)
		{
		}

		private static string GetBtnText(List<MMPatchInfo> patchesList, int index)
		{
			if (patchesList[index].IsCollapsed())
				return expandText;
			return collapseText;
		}

		private static void Toggle(List<MMPatchInfo> patchesList, int index)
		{
			patchesList[index].ToggleCollapsed();
		}
	}
}
