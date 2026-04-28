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

										bool useCurve = false;
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

														useCurve = true;
													}
												}
												break;
											case 1:
												if (((ModuleEngines)pm).useVelCurve)
												{
													stanza.listFloatCurveGraph.Add(
														new FloatCurveWrapper("Use Velocity Curve: " + ((ModuleEngines)pm).useVelCurve,
															new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)pm).velCurve)));
													useCurve = true;
												}
												break;
											case 2:
												if (((ModuleEngines)pm).useThrottleIspCurve)
												{
													stanza.listFloatCurveGraph.Add(
															new FloatCurveWrapper("Use Throttle Isp Curve: " + ((ModuleEngines)pm).useThrottleIspCurve,
															new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)pm).throttleIspCurve)));
													useCurve = true;
												}
												break;
											case 3:
												if (((ModuleEngines)pm).useAtmCurve)
												{
													stanza.listFloatCurveGraph.Add(
														new FloatCurveWrapper("Use Atmo Curve: " + ((ModuleEngines)pm).useAtmCurve,
															new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)pm).atmCurve)));
													useCurve = true;
												}
												break;
											case 4:
												if (((ModuleEngines)pm).useAtmCurveIsp)
												{
													stanza.listFloatCurveGraph.Add(
														new FloatCurveWrapper("Use Atmo ISP Curve: " + ((ModuleEngines)pm).useAtmCurveIsp,
														new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)pm).atmCurveIsp)));
													useCurve = true;
												}
												break;
										}
#if false
										if (useCurve)
										{
											using (new GUILayout.HorizontalScope())
											{
												GUILayout.Space(i1 - 60);
												GUILayout.Box(stanza.listFloatCurveGraph[curveCnt].graph);
												stanza.listFloatCurveGraph[curveCnt].graph.Apply();
											}
											using (new GUILayout.HorizontalScope())
											{
												sb.Append(stanza.listFloatCurveGraph[curveCnt].floatCurveString);
												GUILayout.Space(i1 - 60);

												GUILayout.TextArea(w.StripHtml(stanza.listFloatCurveGraph[curveCnt].floatCurveString.ToString()), GUILayout.Width(w.winRect.width - 80));

												w.sbPrint.Append(stanza.listFloatCurveGraph[curveCnt].floatCurveString);
											}
										}
#endif
									}
								}

								if (pm is ModuleRCS || pm is ModuleRCSFX)
								{
									for (int curveCnt = 0; curveCnt < 2; curveCnt++)
									{

										bool useCurve = false;
										switch (curveCnt)
										{
											case 0:
												if (((ModuleRCS)pm).useThrustCurve)
												{
													stanza.listFloatCurveGraph.Add(
														new FloatCurveWrapper("Use Thrust Curve: " + ((ModuleRCS)pm).useThrustCurve,
														new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleRCS)pm).thrustCurve)));
													useCurve = true;
												}
												break;

											case 1:
												stanza.listFloatCurveGraph.Add(
													new FloatCurveWrapper("Atmo curve",
													new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleRCS)pm).atmosphereCurve)));
												useCurve = true;
												break;
										}
#if false
										if (useCurve)
										{
											using (new GUILayout.HorizontalScope())
											{
												GUILayout.Space(i1 - 60);
												GUILayout.Box(stanza.listFloatCurveGraph[curveCnt].graph);
												stanza.listFloatCurveGraph[curveCnt].graph.Apply();
											}
											using (new GUILayout.HorizontalScope())
											{
												sb.Append(stanza.listFloatCurveGraph[curveCnt].floatCurveString);
												GUILayout.Space(i1 - 60);

												//if (!HighLogic.CurrentGame.Parameters.CustomParams<PartInfoSettings>().useAltSkin)
												//	GUILayout.TextArea(StripHtml(floatCurveGraphs[i, curveCnt].floatCurveString.ToString()), GUILayout.Width(winRect.width - 90));
												//else
												GUILayout.TextArea(w.StripHtml(stanza.listFloatCurveGraph[curveCnt].floatCurveString.ToString()), GUILayout.Width(w.winRect.width - 80));

												w.sbPrint.Append(stanza.listFloatCurveGraph[curveCnt].floatCurveString);
											}
										}
#endif
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

		StringBuilder sb = new StringBuilder();
		StringBuilder tmpSb = new StringBuilder();

		Vector2 scrollPos;

		const int MAXMODULES = 100;
		//bool[] printModule = null;
		bool copyAll = true;

		//ModulePartInfo mpi;
		Part part;


		//internal static Log Log = null;

		void Start()
		{
			//if (!HighLogic.LoadedSceneIsEditor && !HighLogic.CurrentGame.Parameters.CustomParams<PartInfoSettings>().availableInFlight)
			//	Destroy(this);
			//if (Log == null)
			//    Log = new Log("PartInfo", Log.LEVEL.INFO);
			//Log.Info("Start, ModulePartInfo.currentPart.persistentId: " + ModulePartInfo.currentPart.persistentId);

			if (instanceList == null)
			{
				//Log.Info("Creating new instanceList");
				instanceList = new Dictionary<uint, PartInfoWindow>();
			}
			//mpi = ModulePartInfo.currentModule;

			if (instanceList.ContainsKey(part.persistentId))
			{
				part = null;
				//mpi = null;
				Destroy(this);
				return;
			}

			instanceList.Add(part.persistentId, this);

			winRect = new Rect(0, 0, WIDTH, HEIGHT);
			winRect.x = (Screen.width - WIDTH) / 2;
			winRect.y = (Screen.height - HEIGHT) / 2;
			//showFullToggle = HighLogic.CurrentGame.Parameters.CustomParams<PartInfoSettings>().showFullWindow;
			showFullToggle = true;

			//if (printModule == null)
			//printModule = new bool[MAXMODULES];
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
				//if (!HighLogic.CurrentGame.Parameters.CustomParams<PartInfoSettings>().useAltSkin)
				//{
				//	GUI.skin = HighLogic.Skin;
				//	bold = "<b>";
				//	unbold = "</b>";
				//}
				//else
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

		string GetResourceValues()
		{
			tmpSb.Clear();
			tmpSb.AppendLine(bold + "Mass: " + unbold + FormatMass(part.mass));

			if (part.Resources.Count > 0)
			{

				tmpSb.AppendLine(bold + "Resources:" + unbold);
				foreach (PartResource r in part.Resources)
				{
					double mass = r.amount * r.info.density;
					tmpSb.AppendLine("    " + r.resourceName + ": " + r.amount.ToString("F1") + "/" + r.maxAmount.ToString("F1") + ", mass: " + FormatMass(mass));
				}
			}
			return tmpSb.ToString();
		}


		private string StripHtml(string source)
		{
			string output;

			//get rid of HTML tags
			output = Regex.Replace(source, "<[^>]*>", string.Empty);

			//get rid of multiple blank lines
			output = Regex.Replace(output, @"^\s*$\n", string.Empty, RegexOptions.Multiline);

			return output;
		}

		void AddDashedLine()
		{
			//sbPrint.AppendLine("-----------------------------------------------");
		}

		void Window(int id)
		{
			sb.Clear();
			tmpSb.Clear();
			//sbPrint.Clear();
			if (maxPrintWidth == 0)
			{
				CalcWindowSize();
			}

			string resVal = GetResourceValues();
			sb.Append(tmpSb);
			//sbPrint.Append(tmpSb);
			AddDashedLine();

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

				//using (new GUILayout.HorizontalScope())
				//{
				//	bool newCopyAll = GUILayout.Toggle(copyAll, "Copy All");
				//
				//	if (newCopyAll != copyAll)
				//	{
				//		for (int i = 0; i < part.Modules.Count; i++)
				//			printModule[i] = newCopyAll;
				//		copyAll = newCopyAll;
				//	}
				//}
				if (showFull)
				{
					using (new GUILayout.VerticalScope())
					{
						for (int i = 0; i < listStanzas.Count; i++)
						{
							tmpSb.Clear();
							//tmpSb.AppendLine(bold + m.moduleName + unbold);
							//tmpSb.AppendLine();

							tmpSb.Append(listStanzas[i].stanza);
							//Debug.Log("PartInfoInPAW.PartInfoWindow, str: " + tmpSb.ToString());

							sb.Append(tmpSb);
							//if (printModule[cnt] || copyAll)
							//{
							//	sbPrint.Append(tmpSb);
							//	AddDashedLine();
							//}

							using (new GUILayout.HorizontalScope())
							{
								//printModule[cnt] = GUILayout.Toggle(printModule[cnt], "");
								//if (!printModule[cnt])
								//	copyAll = false;
								//if (!HighLogic.CurrentGame.Parameters.CustomParams<PartInfoSettings>().useAltSkin)
								//	GUILayout.TextArea(StripHtml(tmpSb.ToString()), GUILayout.Width(winRect.width - 90));
								//else
								GUILayout.TextArea(StripHtml(tmpSb.ToString()), GUILayout.Width(winRect.width - 80));
							}
							cnt++;

							int i1 = 0;
							//if (!HighLogic.CurrentGame.Parameters.CustomParams<PartInfoSettings>().useAltSkin)
							//	i1 = 110;
							//else
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
									sb.Append(listStanzas[i].listFloatCurveGraph[j].label);
									GUILayout.Space(i1 - 60);

									GUILayout.TextArea(StripHtml(listStanzas[i].listFloatCurveGraph[j].curveGraph.floatCurveString.ToString()), GUILayout.Width(winRect.width - 80));

									//sbPrint.Append(listStanzas[i].listFloatCurveGraph[j].curveGraph.floatCurveString);
								}

							}

#if false
								if (m is ModuleEngines || m is ModuleEnginesFX || m is ModuleRCS || m is ModuleRCSFX)
								{
									int i1 = 0;

									//if (!HighLogic.CurrentGame.Parameters.CustomParams<PartInfoSettings>().useAltSkin)
									//	i1 = 110;
									//else
									i1 = 100;
									if (m is ModuleEngines || m is ModuleEnginesFX)
									{
										for (int curveCnt = 0; curveCnt < 5; curveCnt++)
										{

											bool useCurve = false;
											switch (curveCnt)
											{
												case 0:
													if (((ModuleEngines)m).useThrustCurve)
													{
														using (new GUILayout.HorizontalScope())
														{
															GUILayout.Space(i1 - 60);
															GUILayout.Label("Use Thrust Curve: " + ((ModuleEngines)m).useThrustCurve);
														}

														if (((ModuleEngines)m).useThrustCurve)
														{
															if (floatCurveGraphs[i, curveCnt] == null)
																floatCurveGraphs[i, curveCnt] = new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)m).thrustCurve);
															useCurve = true;
														}
													}
													break;
												case 1:
													if (((ModuleEngines)m).useVelCurve)
													{
														using (new GUILayout.HorizontalScope())
														{
															GUILayout.Space(i1 - 60);
															GUILayout.Label("Use Velocity Curve: " + ((ModuleEngines)m).useVelCurve);
														}
														if (((ModuleEngines)m).useVelCurve)
														{
															if (floatCurveGraphs[i, curveCnt] == null)
																floatCurveGraphs[i, curveCnt] = new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)m).velCurve);
															useCurve = true;
														}
													}
													break;
												case 2:
													if (((ModuleEngines)m).useThrottleIspCurve)
													{
														using (new GUILayout.HorizontalScope())
														{
															GUILayout.Space(i1 - 60);
															GUILayout.Label("Use Throttle Isp Curve: " + ((ModuleEngines)m).useThrottleIspCurve);
														}
														if (((ModuleEngines)m).useThrottleIspCurve)
														{
															if (floatCurveGraphs[i, curveCnt] == null)
																floatCurveGraphs[i, curveCnt] = new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)m).throttleIspCurve);
															useCurve = true;
														}
													}
													break;
												case 3:
													if (((ModuleEngines)m).useAtmCurve)
													{
														using (new GUILayout.HorizontalScope())
														{
															GUILayout.Space(i1 - 60);
															GUILayout.Label("Use Atmo Curve: " + ((ModuleEngines)m).useAtmCurve);
														}
														if (((ModuleEngines)m).useAtmCurve)
														{
															if (floatCurveGraphs[i, curveCnt] == null)
																floatCurveGraphs[i, curveCnt] = new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)m).atmCurve);
															useCurve = true;
														}
													}
													break;
												case 4:
													if (((ModuleEngines)m).useAtmCurveIsp)
													{
														using (new GUILayout.HorizontalScope())
														{
															GUILayout.Space(i1 - 60);
															GUILayout.Label("Use Atmo ISP Curve: " + ((ModuleEngines)m).useAtmCurveIsp);
														}
														if (((ModuleEngines)m).useAtmCurveIsp)
														{
															if (floatCurveGraphs[i, curveCnt] == null)
																floatCurveGraphs[i, curveCnt] = new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleEngines)m).atmCurveIsp);
															useCurve = true;
														}
													}
													break;
											}

											if (useCurve)
											{
												using (new GUILayout.HorizontalScope())
												{
													GUILayout.Space(i1 - 60);
													GUILayout.Box(floatCurveGraphs[i, curveCnt].graph);
													floatCurveGraphs[i, curveCnt].graph.Apply();
												}
												using (new GUILayout.HorizontalScope())
												{
													sb.Append(floatCurveGraphs[i, curveCnt].floatCurveString);
													GUILayout.Space(i1 - 60);

													//if (!HighLogic.CurrentGame.Parameters.CustomParams<PartInfoSettings>().useAltSkin)
													//	GUILayout.TextArea(StripHtml(floatCurveGraphs[i, curveCnt].floatCurveString.ToString()), GUILayout.Width(winRect.width - 90));
													//else
													GUILayout.TextArea(StripHtml(floatCurveGraphs[i, curveCnt].floatCurveString.ToString()), GUILayout.Width(winRect.width - 80));

													sbPrint.Append(floatCurveGraphs[i, curveCnt].floatCurveString);
												}
											}
										}
									}

									if (m is ModuleRCS || m is ModuleRCSFX)
									{
										for (int curveCnt = 0; curveCnt < 2; curveCnt++)
										{

											bool useCurve = false;
											switch (curveCnt)
											{
												case 0:
													if (((ModuleRCS)m).useThrustCurve)
													{
														using (new GUILayout.HorizontalScope())
														{
															GUILayout.Space(i1 - 60);
															GUILayout.Label("Use Thrust Curve: " + ((ModuleRCS)m).useThrustCurve);
														}

														if (((ModuleRCS)m).useThrustCurve)
														{
															if (floatCurveGraphs[i, curveCnt] == null)
																floatCurveGraphs[i, curveCnt] = new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleRCS)m).thrustCurve);
															useCurve = true;
														}
													}
													break;
												case 1:
													using (new GUILayout.HorizontalScope())
													{
														GUILayout.Space(i1 - 60);
														GUILayout.Label("Atmo curve");
													}
													if (floatCurveGraphs[i, curveCnt] == null)
														floatCurveGraphs[i, curveCnt] = new FloatCurveGraph(WIDTH - i1 + 10, ((ModuleRCS)m).atmosphereCurve);
													useCurve = true;
													break;
											}

											if (useCurve)
											{
												using (new GUILayout.HorizontalScope())
												{
													GUILayout.Space(i1 - 60);
													GUILayout.Box(floatCurveGraphs[i, curveCnt].graph);
													floatCurveGraphs[i, curveCnt].graph.Apply();
												}
												using (new GUILayout.HorizontalScope())
												{
													sb.Append(floatCurveGraphs[i, curveCnt].floatCurveString);
													GUILayout.Space(i1 - 60);

													//if (!HighLogic.CurrentGame.Parameters.CustomParams<PartInfoSettings>().useAltSkin)
													//	GUILayout.TextArea(StripHtml(floatCurveGraphs[i, curveCnt].floatCurveString.ToString()), GUILayout.Width(winRect.width - 90));
													//else
													GUILayout.TextArea(StripHtml(floatCurveGraphs[i, curveCnt].floatCurveString.ToString()), GUILayout.Width(winRect.width - 80));

													sbPrint.Append(floatCurveGraphs[i, curveCnt].floatCurveString);
												}
											}
										}
									}

								}
#endif
						}

					}
				}

				GUILayout.EndScrollView();
				GUILayout.FlexibleSpace();
				using (new GUILayout.HorizontalScope())
				{
					if (GUILayout.Button("Close"))
					{
						Destroy(this);
						//printModule = null;
					}


					GUIContent strContent;
					if (copyAll)
						strContent = new GUIContent("Copy all to clipboard");
					else
						strContent = new GUIContent("Copy to clipboard");
					var size = GUI.skin.button.CalcSize(strContent);

					if (GUILayout.Button(strContent, GUILayout.Width(size.x + 20)))
					{
						//sbPrint.ToString().CopyToClipboard();
					}
				}
			}
			GUI.DragWindow();
		}
		void OnDestroy()
		{
			//Log.Info("OnDestroy");
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

