namespace PartInfoInPAW
{

	// http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
	// search for "Mod integration into Stock Settings

	internal class PartInfoInPAWGameSettings_PartInfo : GameParameters.CustomParameterNode
	{
		public override string DisplaySection { get { return Section; } }
		public override string Section { get { return "#LOC_PartInfoInPAW_Settings_Title"; } }
		public override string Title { get { return "#LOC_PartInfoInPAW_Settings_PartInfo_Title"; } }
		public override int SectionOrder { get { return 1; } }
		public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
		public override bool HasPresets { get { return false; } }
		public override bool Enabled(System.Reflection.MemberInfo member, GameParameters parameters) { return true; }

		public override bool Interactible(System.Reflection.MemberInfo member, GameParameters parameters)
		{
			if (member.Name == "showCopyPartNodeBtn" || member.Name == "showOpenPartCFGInEditorBtn" || member.Name == "showPartMMPatchesHistoryBtn")
				return PartInfoInPAW.IsModuleManagerPresent();
			return true;
		}

		[GameParameters.CustomParameterUI("Use alternate skin",
			toolTip = "Use a more minimiliast skin, no color")]
		public bool useAltSkin = true;


		[GameParameters.CustomIntParameterUI("#LOC_PartInfoInPAW_Settings_TabIndentSizeTitle", toolTip = "#LOC_PartInfoInPAW_Settings_TabIndentSizeTooltip", minValue = 2, maxValue = 16, displayFormat = "0")]
		public int spacesPerTab = 8;

		[GameParameters.CustomStringParameterUI("#LOC_PartInfoInPAW_Settings_PartInfoTitle")]
		public string advancedHeader1 = "";


		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_ShowPartInfo",
			toolTip = "#LOC_PartInfoInPAW_Settings_ShowPartInfo_Tooltip",
			autoPersistance = true)]
		public bool showPartInfo = true;

		//[GameParameters.CustomStringParameterUI("showButtonsTitle",
		//	title = "#LOC_PartInfoInPAW_Settings_ShowButtonsTitle",
		//	toolTip = "#LOC_PartInfoInPAW_Settings_ShowButtonsTitle_Tooltip",
		//	lines = 2,
		//	autoPersistance = false)]
		//public string showButtonsTitle = "";

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_CopyPartName_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_CopyPartName_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showCopyPartNameBtn = true;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_CopyPartNode_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_CopyPartNode_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showCopyPartNodeBtn = false;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_CopyOrigPartNode_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_CopyOrigPartNode_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showCopyOrigPartNodeBtn = false;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_UseExtEditorTitle", toolTip = "#LOC_PartInfoInPAW_Settings_UseExtEditorTooltip", autoPersistance = true)]
		public bool useExternalEditor = true;


		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_OpenPartCFGInEditor_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_OpenPartCFGInEditor_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showOpenPartCFGInEditorBtn = true;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_OpenOrigPartCFGInEditor_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_OpenOrigPartCFGInEditor_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showOpenOrigPartCFGInEditorBtn = true;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_ShowPartMMPatchesHistory_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_ShowPartMMPatchesHistory_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showPartMMPatchesHistoryBtn = true;

	}


	internal class PartInfoInPAWGameSettings_PartInfoInFlight : GameParameters.CustomParameterNode
	{
		public override string DisplaySection { get { return Section; } }
		public override string Section { get { return "#LOC_PartInfoInPAW_Settings_Title"; } }
		public override string Title { get { return "#LOC_PartInfoInPAW_Settings_PartInfo_TitleInFlight"; } }
		public override int SectionOrder { get { return 2; } }
		public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
		public override bool HasPresets { get { return false; } }
		public override bool Enabled(System.Reflection.MemberInfo member, GameParameters parameters) { return true; }

		public override bool Interactible(System.Reflection.MemberInfo member, GameParameters parameters)
		{
			if (member.Name == "showCopyPartNodeBtn" || member.Name == "showOpenPartCFGInEditorBtn" || member.Name == "showPartMMPatchesHistoryBtn")
				return PartInfoInPAW.IsModuleManagerPresent();
			return true;
		}


		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_ShowPartInfo", toolTip = "#LOC_PartInfoInPAW_Settings_ShowPartInfo_Tooltip", autoPersistance = true)]
		public bool showPartInfoInFlight = true;

		//[GameParameters.CustomStringParameterUI("showButtonsTitle",
		//	title = "#LOC_PartInfoInPAW_Settings_ShowButtonsTitle",
		//	toolTip = "#LOC_PartInfoInPAW_Settings_ShowButtonsTitle_Tooltip",
		//	lines = 2,
		//	autoPersistance = false)]
		//public string showButtonsTitleInFlight = "";

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_CopyPartName_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_CopyPartName_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showCopyPartNameBtnInFlight = true;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_CopyPartNode_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_CopyPartNode_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showCopyPartNodeBtnInFlight = false;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_CopyOrigPartNode_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_CopyOrigPartNode_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showCopyOrigPartNodeBtnInFlight = false;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_OpenPartCFGInEditor_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_OpenPartCFGInEditor_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showOpenPartCFGInEditorBtnInFlight = true;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_OpenOrigPartCFGInEditor_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_OpenOrigPartCFGInEditor_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showOpenOrigPartCFGInEditorBtnInFlight = true;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_ShowPartMMPatchesHistory_ShowBtn", toolTip = "#LOC_PartInfoInPAW_Settings_ShowPartMMPatchesHistory_ShowBtn_Tooltip", autoPersistance = true)]
		public bool showPartMMPatchesHistoryBtnInFlight = true;
	}

	internal class PartInfoInPAWGameSettings_EngineInfo : GameParameters.CustomParameterNode
	{
		public override string DisplaySection { get { return Section; } }
		public override string Section { get { return "#LOC_PartInfoInPAW_Settings_Title"; } }
		public override string Title { get { return "#LOC_PartInfoInPAW_Settings_EnginesInfo_Title"; } }
		public override int SectionOrder { get { return 3; } }
		public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
		public override bool HasPresets { get { return false; } }

		public override bool Enabled(System.Reflection.MemberInfo member, GameParameters parameters)
		{
			return (showEngineInfo || member.Name == "showEngineInfo");
		}

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_ShowEngineInfo", toolTip = "#LOC_PartInfoInPAW_Settings_ShowEngineInfo_Tooltip", autoPersistance = true)]
		public bool showEngineInfo = true;

		[GameParameters.CustomStringParameterUI("showEngineInfoTitle", title = "#LOC_PartInfoInPAW_Settings_ShowEngineInfoTitle", lines = 2, autoPersistance = false)]
		public string showEngineInfoTitle = "";

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_ShowEnginesPropellantsInfo", toolTip = "#LOC_PartInfoInPAW_Settings_ShowEnginesPropellantsInfo_Tooltip", autoPersistance = true)]
		public bool showEnginesPropellantsInfo = true;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_ShowEnginesThrustInfo", toolTip = "#LOC_PartInfoInPAW_Settings_ShowEnginesThrustInfo_Tooltip", autoPersistance = true)]
		public bool showEnginesThrustInfo = true;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_ShowEnginesMinThrustInfo", toolTip = "#LOC_PartInfoInPAW_Settings_ShowEnginesMinThrustInfo_Tooltip", autoPersistance = true)]
		public bool showEnginesMinThrustInfo = true;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_ShowEnginesISPInfo", toolTip = "#LOC_PartInfoInPAW_Settings_ShowEnginesISPInfo_Tooltip", autoPersistance = true)]
		public bool showEnginesISPInfo = true;

		[GameParameters.CustomParameterUI("#LOC_PartInfoInPAW_Settings_ShowEnginesGimbalInfo", toolTip = "#LOC_PartInfoInPAW_Settings_ShowEnginesGimbalInfo_Tooltip", autoPersistance = true)]
		public bool showEnginesGimbalInfo = true;
	}
}
