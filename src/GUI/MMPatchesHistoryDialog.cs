using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using KSP.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PartInfoInPAW
{
	public static class MMPatchesHistoryDialog
	{
		private static PopupDialog dialog;
		private static List<MMPatchInfo> patchesList;

		private static bool IncludePartCFGInPatchesCode = false;

		public static async void ShowDialog(string partName, string partTitle, Part part)
		{
			if (dialog != null)
			{
				dialog.Dismiss();
			}
			DialogGUIBase[] GUIPatchesList;
			UIStyle smallBtnStyle = new UIStyle(HighLogic.UISkin.button)
			{
				fontStyle = FontStyle.Normal,
				fontSize = 11
			};
			UIStyle titleLabelsStyle = new UIStyle(HighLogic.UISkin.label)
			{
				fontStyle = FontStyle.Normal,
				alignment = TextAnchor.MiddleLeft,
				fontSize = 12
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
						new MMPatchesCollapseExpandGUIButton(patchesList, i - 1, 18f, 18f, smallBtnStyle),
						new MMPatchesPatchGUILabel(patchesList, i - 1, true, false),
						new DialogGUIFlexibleSpace()
					)
				);
			}
			DialogGUIToggle guiToggleIncludePartCFG = new DialogGUIToggle(
				IncludePartCFGInPatchesCode,
				Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_CopyPatchesWithPartCFG"),
				(bool newValue) => { IncludePartCFGInPatchesCode = newValue; },
				380f
			);
			guiToggleIncludePartCFG.guiStyle = titleLabelsStyle;
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
							new DialogGUILabel(
								MMLogParser.GetStatusMsg(partName),
								titleLabelsStyle,
								true,
								true
							),
							new DialogGUIFlexibleSpace(),
							guiToggleIncludePartCFG,
							new DialogGUISpace(5f)
						),
						new DialogGUIHorizontalLayout(
							new DialogGUISpace(5f),
							new DialogGUIButton(
								Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_ExpandAllPatchesBtn"),
								() => {
									if (patchesList != null && patchesList.Count > 0)
									{
										patchesList.ForEach(p => p.Expand());
									}
								},
								190.0f, 20.0f, false, smallBtnStyle
							),
							new DialogGUIButton(
								Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_CollapseAllPatchesBtn"),
								() => {
									if (patchesList != null && patchesList.Count > 0)
									{
										patchesList.ForEach(p => p.Collapse());
									}
								},
								190.0f, 20.0f, false, smallBtnStyle
							),
							new DialogGUIFlexibleSpace(),
							new DialogGUIButton(
								Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_CopyPatchesHistoryFileBtn"),
								() => {
									string patchesHistory = Utils.GetPartMMPatchesHistory(part, patchesList, IncludePartCFGInPatchesCode);
									if (!String.IsNullOrEmpty(patchesHistory))
									{
										try
										{
											GUIUtility.systemCopyBuffer = patchesHistory;
											Utils.Log($"Part {part.partInfo.name} : MM patches history copied to clipboard");
											Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CopyToClipboard_MMPatchesHistory_SuccessMsg", partName), 2.0f);
										}
										catch (Exception e)
										{
											Utils.LogError($"Part {part.partInfo.name} : failed to copy MM patches history to clipboard: " + e.Message);
											Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CopyToClipboard_MMPatchesHistory_FailureMsg", partName));
										}
									}
								},
								() => patchesList.Count >= 0 || IncludePartCFGInPatchesCode,
								190.0f, 20.0f, false, smallBtnStyle
							),
							new DialogGUIButton(
								Localizer.Format("#LOC_PartInfoInPAW_PartMMPatchesHistory_OpenPatchesHistoryBtn"),
								() => {
									string patchesHistory = Utils.GetPartMMPatchesHistory(part, patchesList, IncludePartCFGInPatchesCode);
									if (!String.IsNullOrEmpty(patchesHistory))
									{
										string fileName = partName;
										if (fileName == "")
										{
											fileName = part.GetPartName();
										}
										foreach (var c in Path.GetInvalidFileNameChars())
										{
											fileName = fileName.Replace(c, '-');
										}
										string filePath = Path.Combine(Path.GetTempPath(), Process.GetCurrentProcess().Id.ToString() +
											"_" + fileName + "_MMPatchesHistory" + ((IncludePartCFGInPatchesCode) ? "WithOrigCFG." : ".") + UrlDir.configExtension);
										if (!File.Exists(filePath))
										{
											try
											{
												File.WriteAllText(filePath, patchesHistory, Encoding.UTF8);
											}
											catch (Exception e)
											{
												Utils.LogError($"Could not write part MM patches history to file {filePath}: " + e.Message);
												Utils.OnScreenMsg(Localizer.Format("#LOC_PartInfoInPAW_CantWriteToTempFile_MMPatchesHistory_FailureMsg", filePath));
												return;
											}
										}
										Utils.ShellOpenFile(filePath);
									}
								},
								() => patchesList.Count >= 0 || IncludePartCFGInPatchesCode,
								190.0f, 20.0f, false, smallBtnStyle
							),
							new DialogGUISpace(5f)
						),
						new DialogGUIVerticalLayout(
							new DialogGUIScrollList(
								new Vector2(980f, 560f),
								false,
								true,
								new DialogGUIVerticalLayout(10f, 530f, 4f, new RectOffset(10, 10, 0, 0), TextAnchor.UpperLeft, GUIPatchesList)
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
