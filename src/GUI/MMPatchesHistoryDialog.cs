using System;
using System.Collections.Generic;
using KSP.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PartInfoInPAW
{
	public static class MMPatchesHistoryDialog
	{
		private static PopupDialog dialog;

		public static async void ShowDialog(string partName, string partTitle)
		{
			List<MMPatchInfo> patchesList = new List<MMPatchInfo>();
			DialogGUIBase[] GUIPatchesList;
			UIStyle smallBtnStyle = new UIStyle(HighLogic.UISkin.button)
			{
				fontStyle = FontStyle.Normal,
				fontSize = 11
			};

			try
			{
				patchesList = await MMLogParser.ParseLogFile(partName);
			}
			catch (Exception e)
			{
				Utils.LogError($"Part {partName} : failed to get MM patches history: " + e.Message);
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CantParsePartPatchesHistory_FailureMsg", partName));
				return;
			}
			if (MMLogParser.GetStatus() != MMLogParser.ParserStatus.LogParsed)
			{
				Utils.LogError($"Part {partName} : failed to get MM patches history");
				Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CantParsePartPatchesHistory_FailureMsg", partName));
				return;
			}

			GUIPatchesList = new DialogGUIBase[patchesList.Count + 1];
			GUIPatchesList[0] = new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize, true);
			for (int i = 1; i <= patchesList.Count; i++)
			{
				GUIPatchesList[i] = new DialogGUIVerticalLayout(
					true, false, 4, new RectOffset(5, 5, 5, 5),
					TextAnchor.UpperLeft,
					new DialogGUIHorizontalLayout(
						new DialogGUILabel(patchesList[i - 1].PatchFilePath + "<br>  " + patchesList[i - 1].Patch, expandW: true),
						new DialogGUIFlexibleSpace(),
						new DialogGUIButton("C", () => { }, 30.0f, 20.0f, false, smallBtnStyle),
						new DialogGUIButton("E", () => { }, 30.0f, 20.0f, false, smallBtnStyle)
					)
				);
			}
			dialog = PopupDialog.SpawnPopupDialog(
				new Vector2(0.5f, 0.5f),
				new Vector2(0.5f, 0.5f),
				new MultiOptionDialog(
					"PartMMPatchesHistory",
					"",
					Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_Title", partTitle),
					HighLogic.UISkin,
					new Rect(0.5f, 0.5f, 1000f, 60f),
					new DialogGUIVerticalLayout(
						new DialogGUIHorizontalLayout(
							new DialogGUISpace(5f),
							new DialogGUILabel(MMLogParser.GetStatusMsg(partName),
								new UIStyle(HighLogic.UISkin.label),
								expandW: true
							),
							new DialogGUIFlexibleSpace(),
							new DialogGUIButton(
								Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_CopyPatchesHistoryFileBtn"),
								() => { },
								160.0f, 20.0f, false, smallBtnStyle
							),
							new DialogGUIButton(
								Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_OpenPatchesHistoryBtn"),
								() => { },
								160.0f, 20.0f, false, smallBtnStyle
							),
							new DialogGUISpace(5f)
						),
						new DialogGUIVerticalLayout(
							new DialogGUIScrollList(
								new Vector2(980f, 560f),
								false,
								true,
								new DialogGUIVerticalLayout(10, 100, 4, new RectOffset(5, 15, 0, 0), TextAnchor.UpperLeft, GUIPatchesList)
							)
						),
						new DialogGUIVerticalLayout(
							new DialogGUIHorizontalLayout(
								new DialogGUIFlexibleSpace(),
								new DialogGUIButton(
									Localizer.Format("#LOC_PartInfoInPAW_CloseBtn"),
									() => { },
									160.0f, 30.0f, true
								),
								new DialogGUIFlexibleSpace()
							)
						)
					)
				),
				false,
				HighLogic.UISkin
			);
		}
	}
}
