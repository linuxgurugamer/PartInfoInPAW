using System.Collections.Generic;

namespace PartInfoInPAW
{
	internal class MMPatchesCollapseExpandGUIButton: DialogGUIButton
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
			return patchesList[index].IsCollapsed() ? expandText : collapseText;
		}

		private static void Toggle(List<MMPatchInfo> patchesList, int index)
		{
			patchesList[index].ToggleCollapsed();
		}
	}
}
