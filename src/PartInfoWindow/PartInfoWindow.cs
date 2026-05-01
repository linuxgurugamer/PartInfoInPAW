using ClickThroughFix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PartInfoInPAW.PartInfoWindow
{

	internal class FloatCurveWrapper
	{
		internal string label;
		internal FloatCurveGraph curveGraph;

		internal FloatCurveWrapper(string label, FloatCurveGraph curveGraph)
		{
			this.label = label;
			this.curveGraph = curveGraph;
		}
	}


	internal class StanzaClass
	{
		internal StringBuilder stanza;
		internal List<FloatCurveWrapper> listFloatCurveGraph;

		internal StanzaClass()
		{
			stanza = new StringBuilder();
			listFloatCurveGraph = new List<FloatCurveWrapper>();
		}
	}

	public class PartInfoWindow : MonoBehaviour
	{
		string filePath;

		List<StanzaClass> listStanzas;

		internal static void AddPartInfoWindow(Part part, string filePath)
		{
			var w = part.gameObject.AddComponent<PartInfoWindow>();
			PartInfoWindow.InitStanzas(w, filePath, part);
		}

		static List<string> engineTypes = new List<string>
			{ "name = ModuleEngines", "name = ModuleEnginesFX", "name = ModuleRCS", "name = ModuleRCSFX"};

		internal static void InitStanzas(PartInfoWindow w, string filePath, Part part)
		{
			w.filePath = filePath;
			List<string>[] stanzas = KspPartStanzaReader.ReadStanzas(filePath);
			w.part = part;
			w.listStanzas = new List<StanzaClass>();

			int pmCnt = 0;

			foreach (var s in stanzas)
			{
				if (s[0] != "{" || s.Count != 1)
				{
					StringBuilder sb = new StringBuilder();
					StanzaClass stanza = new StanzaClass();

					foreach (var t in s)
					{
						string str = t;
						if (!string.IsNullOrEmpty(str) && str[0] == '\t')
							str = str.Substring(1);
						sb.Append(str + Environment.NewLine);

						if (engineTypes.Any(a => str.Contains(a)))
						{
							// need to find next module which matches
							// assumes that the order in the config will match the order in the list
							var pm = part.Modules[pmCnt];
							while (!(pm is ModuleEngines) && !(pm is ModuleEnginesFX) && !(pm is ModuleRCS) && !(pm is ModuleRCSFX))
							{
								pmCnt++;
								pm = part.Modules[pmCnt];
							}
							if (true)
							{
								int i1 = 0;

								i1 = 100;
								if (pm is ModuleEngines || pm is ModuleEnginesFX)
								{
									Utils.Log("ModuleEngines found: " + pm.ToString());
									for (int curveCnt = 0; curveCnt < 5; curveCnt++)
									{
										switch (curveCnt)
										{
											case 0:
												if (((ModuleEngines)pm).useThrustCurve)
												{
													if (((ModuleEngines)pm).useThrustCurve)
													{
														stanza.listFloatCurveGraph.Add(
															new FloatCurveWrapper(
																"Use Thrust Curve: " + ((ModuleEngines)pm).useThrustCurve,
																new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)pm).thrustCurve)
															)
														);
													}
												}
												break;
											case 1:
												if (((ModuleEngines)pm).useVelCurve)
												{
													stanza.listFloatCurveGraph.Add(
														new FloatCurveWrapper("Use Velocity Curve: " + ((ModuleEngines)pm).useVelCurve,
															new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)pm).velCurve)));
												}
												break;
											case 2:
												if (((ModuleEngines)pm).useThrottleIspCurve)
												{
													stanza.listFloatCurveGraph.Add(
															new FloatCurveWrapper("Use Throttle Isp Curve: " + ((ModuleEngines)pm).useThrottleIspCurve,
															new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)pm).throttleIspCurve)));
												}
												break;
											case 3:
												if (((ModuleEngines)pm).useAtmCurve)
												{
													stanza.listFloatCurveGraph.Add(
														new FloatCurveWrapper("Use Atmo Curve: " + ((ModuleEngines)pm).useAtmCurve,
															new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)pm).atmCurve)));
												}
												break;
											case 4:
												if (((ModuleEngines)pm).useAtmCurveIsp)
												{
													stanza.listFloatCurveGraph.Add(
														new FloatCurveWrapper("Use Atmo ISP Curve: " + ((ModuleEngines)pm).useAtmCurveIsp,
														new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)pm).atmCurveIsp)));
												}
												break;
										}
									}
								}

								if (pm is ModuleRCS || pm is ModuleRCSFX)
								{
									for (int curveCnt = 0; curveCnt < 2; curveCnt++)
									{

										switch (curveCnt)
										{
											case 0:
												if (((ModuleRCS)pm).useThrustCurve)
												{
													stanza.listFloatCurveGraph.Add(
														new FloatCurveWrapper("Use Thrust Curve: " + ((ModuleRCS)pm).useThrustCurve,
														new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleRCS)pm).thrustCurve)));
												}
												break;

											case 1:
												stanza.listFloatCurveGraph.Add(
													new FloatCurveWrapper("Atmo curve",
													new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleRCS)pm).atmosphereCurve)));
												break;
										}
									}
								}
							}
						}
					}
					stanza.stanza = sb;
					w.listStanzas.Add(stanza);
				}
			}
		}

		static Dictionary<uint, PartInfoWindow> instanceList;

		const int WIDTH = 600;
		float HEIGHT = Screen.height * 0.75f;

		private Rect winRect;
		bool showFull = false;
		bool showFullToggle = true;

		string bold = "<b>", unbold = "</b>";

		float maxPrintWidth = 0;

		StringBuilder tmpSb = new StringBuilder();

		Vector2 scrollPos;

		const int MAXMODULES = 100;
		bool copyAll = true;

		Part part;

		void Start()
		{
			if (!HighLogic.LoadedSceneIsEditor && !HighLogic.CurrentGame.Parameters.CustomParams<PartInfoInPAWGameSettings_PartInfoInFlight>().showPartInfoInFlight)
				Destroy(this);

			if (instanceList == null)
			{
				instanceList = new Dictionary<uint, PartInfoWindow>();
			}

			if (instanceList.ContainsKey(part.persistentId))
			{
				part = null;
				Destroy(this);
				return;
			}

			instanceList.Add(part.persistentId, this);

			winRect = new Rect(0, 0, WIDTH, HEIGHT);
			winRect.x = (Screen.width - WIDTH) / 2;
			winRect.y = (Screen.height - HEIGHT) / 2;
			showFullToggle = true;

			GameEvents.onEditorPartEvent.Add(onEditorPartEvent);
		}

		void onEditorPartEvent(ConstructionEventType cet, Part e)
		{
			Debug.Log("PartInfo:  onEditorPartEvent");
			if (listStanzas == null)
			{
				Debug.Log("PartInfo:  onEditorPartEvent, float listStanzas is null");

				listStanzas = new List<StanzaClass>();
			}

		}

		private void OnGUI()
		{
			{
				if (!HighLogic.CurrentGame.Parameters.CustomParams<PartInfoInPAWGameSettings_PartInfo>().useAltSkin)
				{
					GUI.skin = HighLogic.Skin;
					bold = "<b>";
					unbold = "</b>";
				}
				else
				{
					bold = "";
					unbold = "";
				}

				winRect = ClickThruBlocker.GUILayoutWindow((int)part.persistentId * 10, winRect, Window, "Part Information");
			}
		}

		void CalcWindowSize()
		{
			foreach (var m in part.Modules)
			{
				var info = m.GetInfo().TrimEnd(' ', '\r', '\n');
				info = info.Replace(@"\n", "\n");
				tmpSb.AppendLine(info);

				string str = tmpSb.ToString();
				GUIContent tmpContent = new GUIContent(str);
				Vector2 tmpSize = GUI.skin.textArea.CalcSize(tmpContent);
				maxPrintWidth = Math.Max(tmpSize.x + 10, maxPrintWidth);
				tmpSb.Clear();
			}

		}

		string FormatMass(double mass)
		{
			if (mass < 1)
				return (mass * 1000).ToString("F2") + " kg";
			return mass.ToString("F3") + " t";
		}

		private string StripHtml(string source)
		{
			string output;

			//get rid of HTML tags
			var output1 = Regex.Replace(source, "<[^>]*>", string.Empty);

			//get rid of multiple blank lines
			var output2 = Regex.Replace(output1, @"^\s*$\n", string.Empty, RegexOptions.Multiline);

			output = output2.Replace("\t", new string(' ', HighLogic.CurrentGame.Parameters.CustomParams<PartInfoInPAWGameSettings_PartInfo>().spacesPerTab));
			return output;
		}

		void Window(int id)
		{
			//sb.Clear();
			tmpSb.Clear();
			if (maxPrintWidth == 0)
			{
				CalcWindowSize();
			}

			using (new GUILayout.VerticalScope())
			{
				int cnt = 0;
				//winRect.height = (float)(Screen.height * HighLogic.CurrentGame.Parameters.CustomParams<PartInfoSettings>().WindowHeightPercentage);
				winRect.height = (float)(Screen.height * .75f);
				showFull = HighLogic.LoadedSceneIsEditor ? (EditorLogic.RootPart == this.part || this.part.parent != null) : true;
				if (showFull)
					showFullToggle = GUILayout.Toggle(showFullToggle, "Expanded Window");
				else
					showFullToggle = false;
				if (!showFullToggle)
					winRect.height /= 3;

				scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(winRect.height - 70));

				if (showFull)
				{
					using (new GUILayout.VerticalScope())
					{
						for (int i = 0; i < listStanzas.Count; i++)
						{
							tmpSb.Clear();
							tmpSb.Append(listStanzas[i].stanza);

							using (new GUILayout.HorizontalScope())
							{
								GUILayout.TextArea(StripHtml(tmpSb.ToString()), GUILayout.Width(winRect.width /*  - 80 */));
							}
							cnt++;

							int i1 = 0;
							if (!HighLogic.CurrentGame.Parameters.CustomParams<PartInfoInPAWGameSettings_PartInfo>().useAltSkin)
								i1 = 110;
							else
								i1 = 100;

							for (int j = 0; j < listStanzas[i].listFloatCurveGraph.Count; j++)
							{
								using (new GUILayout.HorizontalScope())
								{
									GUILayout.Space(i1 - 60);
									GUILayout.Box(listStanzas[i].listFloatCurveGraph[j].curveGraph.graph);

									listStanzas[i].listFloatCurveGraph[j].curveGraph.graph.Apply();
								}
								using (new GUILayout.HorizontalScope())
								{
									//sb.Append(listStanzas[i].listFloatCurveGraph[j].label);
									GUILayout.Space(i1 - 60);

									GUILayout.TextArea(StripHtml(listStanzas[i].listFloatCurveGraph[j].curveGraph.floatCurveString.ToString()), GUILayout.Width(winRect.width - 80));
								}
							}
						}
					}
				}

				GUILayout.EndScrollView();
				GUILayout.FlexibleSpace();
				//using (new GUILayout.HorizontalScope())
				//{
				if (GUILayout.Button("Close"))
				{
					Destroy(this);
				}
				//}
			}
			GUI.DragWindow();
		}
		void OnDestroy()
		{
			if (part != null)
				instanceList.Remove(part.persistentId);
		}
	}

	internal static class StringStuff
	{
		public static void CopyToClipboard(this string s)
		{
			TextEditor te = new TextEditor();
			te.text = s;
			te.SelectAll();
			te.Copy();
		}
	}
}

