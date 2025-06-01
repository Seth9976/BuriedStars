using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using GameData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameEvent;

public class EventEngine
{
	private class clEnableObj
	{
		public string m_strEnableObj;

		public string m_strSendEvent;

		public clEnableObj(string strObjName, string strSendEvent)
		{
			m_strEnableObj = strObjName;
			m_strSendEvent = strSendEvent;
		}
	}

	private class LoadedObjectState
	{
		public string m_strLevelName;

		public Dictionary<GameObject, bool> m_dicRootsObjsStates = new Dictionary<GameObject, bool>();
	}

	private enum eBottomGuide
	{
		Skip,
		SkipCancel
	}

	public delegate IEnumerator CBDoneLoadResource(UnityEngine.Object obj);

	public delegate void CBFuncLoadDone();

	private static EventEngine s_instance;

	private PlayMakerFSM m_pmRunFsm;

	private List<clEnableObj> m_listEnableObjList;

	private bool m_isGoMainMenu;

	public GameMain m_GameMain;

	private GameSwitch m_GameSwitch;

	public TalkChar m_TalkChar;

	public EventCamera m_EventCamera;

	public EventObject m_EventObject;

	public GameObject m_goPMEventParent;

	public GameObject m_goEachSceneEventParent;

	private GameObject m_goEventCanvasEff;

	private GameObject m_goEventCanvas;

	public static float EVENT_IMMEDIATE_TIME;

	private bool m_isSkip;

	private bool m_isShowSkipBut = true;

	private bool m_isAuto;

	private bool m_isEventPlaying;

	private bool m_isRunEndMental;

	private bool m_isDisableRunMentalZero;

	public static string m_strLoadedLevel;

	public static string[] m_strLoadedSubLevel;

	public static string m_strNeedLoadLevel;

	private LoadedObjectState[] m_loadedObjsStates;

	private float m_fDelaySetTime;

	private float m_fDelayPassTime;

	private float m_fRelationAnimatorSkipSpeed = 1f;

	private float m_fAnimatorSkipSpeed = 1f;

	private float m_fLerpSkipSpeed = 1f;

	private CommonButtonGuide m_ButtonGuide;

	private static bool m_isLoadIng;

	private bool m_isSaveFileLoad;

	private bool m_isMentalZeroEvtFromSNSFater;

	private GameDefine.EventProc m_fpCompRecFaterEventCB;

	public CBDoneLoadResource m_CBLoadResourceDone;

	private bool m_isLoadLevelShowLoadingIcon = true;

	private static float m_fLoadLevelDelayTime;

	private static float LOADLEVEL_DELAYTIME = 1f;

	private static bool m_isFirstLoad;

	private CBFuncLoadDone m_cbFuncLoadDone;

	public const string c_ScenePath = "Scene/";

	private static string s_strLoadedAssetBundleScene;

	private static string[] s_strLoadedAssetBundleSubScene;

	public EventEngine()
	{
		InitEventEngine();
	}

	public void SetIsSaveFileLoad(bool isSet)
	{
		m_isSaveFileLoad = isSet;
	}

	public bool GetIsSaveFileLoad()
	{
		return m_isSaveFileLoad;
	}

	public static EventEngine GetInstance(bool isCreate = true)
	{
		if (isCreate && s_instance == null)
		{
			s_instance = new EventEngine();
		}
		return s_instance;
	}

	public static void ReleaseInstance()
	{
		m_strLoadedSubLevel = null;
		if (s_instance != null)
		{
			s_instance.Free();
		}
		s_instance = null;
	}

	public void SetPMObjParent(GameObject goPMObjParent, GameObject goEachSceneEventParent)
	{
		m_goPMEventParent = goPMObjParent;
		m_goEachSceneEventParent = goEachSceneEventParent;
	}

	public void SetEventCanvas(GameObject goEventCanvasEff, GameObject goEventCanvas)
	{
		m_goEventCanvasEff = goEventCanvasEff;
		m_goEventCanvas = goEventCanvas;
		RenderManager.instance.ReflashRenderCamera();
		m_goEventCanvasEff.GetComponent<Canvas>().worldCamera = RenderManager.instance.GetCamera_byLayerId(GameDefine.GetLayerID_byCharLayer(2));
	}

	public GameObject GetEventCanvasEff()
	{
		return m_goEventCanvasEff;
	}

	public GameObject GetEventCanvas()
	{
		return m_goEventCanvas;
	}

	private void InitEventEngine()
	{
		m_GameSwitch = GameSwitch.GetInstance();
		InitAllStatic();
		m_TalkChar = new TalkChar();
		m_EventCamera = new EventCamera();
		m_EventObject = new EventObject();
		if (m_listEnableObjList == null)
		{
			m_listEnableObjList = new List<clEnableObj>();
		}
		m_isGoMainMenu = false;
		m_isSkip = false;
		m_isShowSkipBut = true;
		m_isAuto = false;
		m_fRelationAnimatorSkipSpeed = GameGlobalUtil.GetXlsProgramDefineStrToFloat("RELATION_ANIMATOR_SKIP_SPEED");
		m_fAnimatorSkipSpeed = GameGlobalUtil.GetXlsProgramDefineStrToFloat("ANIMATOR_SKIP_SPEED");
		m_fLerpSkipSpeed = GameGlobalUtil.GetXlsProgramDefineStrToFloat("LERP_SKIP_SPEED");
		m_ButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
	}

	public void ClearCurrentLevelEvent(string strLevel = null)
	{
		m_EventCamera.InitCameraEvtEnd();
	}

	public void InitEnterGame()
	{
		m_isRunEndMental = false;
		m_isDisableRunMentalZero = false;
	}

	public void UnloadEachLoad()
	{
		if (m_TalkChar != null)
		{
			m_TalkChar.UnloadRes();
		}
		if (m_EventObject != null)
		{
			m_EventObject.Free();
		}
	}

	public IEnumerator InitAfterLoadLevel()
	{
		m_TalkChar.InitAfterLoad();
		m_EventCamera.InitAfterLoad();
		yield return m_GameMain.StartCoroutine(m_EventObject.InitAfterLoad());
	}

	public void AfterLoadSetColor()
	{
		m_TalkChar.AfterLoadSetColor();
		m_EventObject.AfterLoadSetColor();
	}

	public void Free()
	{
		Resources.UnloadUnusedAssets();
		UnloadEachLoad();
		m_pmRunFsm = null;
		if (m_listEnableObjList != null)
		{
			m_listEnableObjList.Clear();
		}
		m_listEnableObjList = null;
		m_GameMain = null;
		if (m_GameSwitch != null)
		{
			m_GameSwitch.FreeGoToMain();
		}
		m_GameSwitch = null;
		if (m_TalkChar != null)
		{
			m_TalkChar.UnloadRes();
		}
		m_TalkChar = null;
		if (m_EventObject != null)
		{
			m_EventObject.Free();
		}
		m_EventObject = null;
		m_goPMEventParent = null;
		m_goEachSceneEventParent = null;
		if (m_EventCamera != null)
		{
			m_EventCamera.Free();
		}
		EventCameraEffect.Instance.Clear();
		m_EventCamera = null;
		m_goEventCanvasEff = null;
		m_goEventCanvas = null;
		if (m_loadedObjsStates != null)
		{
			int num = m_loadedObjsStates.Length;
			for (int i = 0; i < num; i++)
			{
				m_loadedObjsStates[i] = null;
			}
			m_loadedObjsStates = null;
		}
	}

	public void OnlyFreeResForGoToMain()
	{
		m_EventObject.Free();
		m_TalkChar.UnloadRes();
	}

	public bool FinishRunEvent(PMEventScript pmEventScript, bool isOpenKeyword, ref bool isGoMainMenu)
	{
		if (m_pmRunFsm != null)
		{
			m_pmRunFsm.gameObject.SetActive(value: false);
		}
		m_GameSwitch.SetRunEventObj(GameSwitch.eEventRunType.NONE);
		m_pmRunFsm = null;
		bool flag = CheckEnableObjAndRun();
		if (!flag)
		{
			if (m_isGoMainMenu)
			{
				GoMainMenu();
				isGoMainMenu = true;
				return false;
			}
			m_isEventPlaying = false;
		}
		return flag;
	}

	public void SetGoMainMenu(bool isGoMainMenu)
	{
		m_isGoMainMenu = isGoMainMenu;
	}

	public void GoMainMenu()
	{
		MainLoadThing.instance.StartCoroutine(GoMainMenu_Coroutine());
	}

	public IEnumerator GoMainMenu_Coroutine()
	{
		if (!string.IsNullOrEmpty(m_strLoadedLevel))
		{
			AsyncOperation operation = SceneManager.UnloadSceneAsync(m_strLoadedLevel);
			while (!operation.isDone)
			{
				yield return null;
			}
			m_strLoadedLevel = null;
		}
		yield return null;
		Resources.UnloadUnusedAssets();
		m_isGoMainMenu = false;
		MainMenuCommon.DestoryInstance(isImmidiate: true);
		yield return MainLoadThing.instance.StartCoroutine(GameMain.GoMainMenu());
	}

	public void StopSkip(bool isShowSkipBut = true, bool isForce = false)
	{
		if (isForce || IsEventRunning())
		{
			SetSkip(isSkip: false, isForce);
			SetShowSkipBut(isShowSkipBut);
		}
	}

	public void SetSkip(bool isSkip, bool isForce = false)
	{
		if ((isForce || IsEventRunning()) && (isForce || m_isSkip != isSkip))
		{
			m_isSkip = isSkip;
			SkipButtonGuide.SetSkipActivate(isSkip);
			if (MainLoadThing.instance.m_EffSkip != null)
			{
				MainLoadThing.instance.m_EffSkip.SetSkip(isSkip);
			}
			SetBottomGuide(isSkip);
			if (m_GameMain != null && m_GameMain.m_clEventTalkDialogue != null && m_GameMain.m_clEventTalkDialogue.gameObject.activeInHierarchy)
			{
				m_GameMain.m_clEventTalkDialogue.TouchPrintSkip(isSkip);
			}
			if (m_EventObject != null && !isSkip)
			{
				m_EventObject.StopSkipPlayAnimation();
			}
			AudioManager.instance.SetSkip(isSkip);
		}
	}

	public void SetShowSkipBut(bool isShowSkipBut)
	{
		m_isShowSkipBut = isShowSkipBut;
		if (isShowSkipBut)
		{
			SetBottomGuide(m_isSkip);
		}
		else
		{
			SkipButtonGuide.Hide();
		}
	}

	public bool GetShowSkipBut()
	{
		return m_isShowSkipBut;
	}

	public bool GetSkip()
	{
		return m_isSkip;
	}

	public float GetLerpSkipValue()
	{
		return (!GetSkip()) ? 1f : m_fLerpSkipSpeed;
	}

	public float GetAnimatorSkipValue()
	{
		return (!GetSkip()) ? 1f : m_fAnimatorSkipSpeed;
	}

	public float GetRelationAnimatorSkipValue()
	{
		return (!GetSkip()) ? 1f : m_fRelationAnimatorSkipSpeed;
	}

	public void SetEventBotGuide(bool isShow)
	{
		if (isShow)
		{
			SetBottomGuide(m_isSkip);
		}
		else
		{
			SetShowSkipBut(isShowSkipBut: false);
		}
	}

	private void SetBottomGuide(bool isSkip)
	{
		SkipButtonGuide.Show();
	}

	private void ActivateBottomGuide()
	{
		SkipButtonGuide.SetPressState(isPressed: true);
	}

	public void PressSkipKey()
	{
		if (!m_isShowSkipBut)
		{
			return;
		}
		AudioManager instance = AudioManager.instance;
		if (instance.IsPlayingVideo())
		{
			GameSwitch.eUIButType uIButType = GameSwitch.GetInstance().GetUIButType();
			if ((uIButType == GameSwitch.eUIButType.KEYMOUSE && GamePadInput.IsButtonState_Down(PadInput.GameInput.SkipButton)) || (uIButType != GameSwitch.eUIButType.KEYMOUSE && GamePadInput.IsButtonState_Down(PadInput.GameInput.OptionButton)))
			{
				SetBottomGuide(!m_isSkip);
				ActivateBottomGuide();
				SetSkip(!m_isSkip);
			}
		}
	}

	public void TouchSkipKey()
	{
		if (MainLoadThing.instance.IsTouchableState() && m_isShowSkipBut)
		{
			AudioManager instance = AudioManager.instance;
			if (!instance.IsPlayingVideo())
			{
				instance.SetSkipVideo();
				return;
			}
			SetBottomGuide(!m_isSkip);
			ActivateBottomGuide();
			SetSkip(!m_isSkip);
		}
	}

	public void ToggleAuto()
	{
		SetAuto(!m_isAuto);
	}

	public void SetAuto(bool isAuto)
	{
		if (m_isAuto != isAuto)
		{
			m_isAuto = isAuto;
		}
	}

	public bool GetAuto()
	{
		return m_isAuto;
	}

	public bool IsEventRunning()
	{
		return m_isEventPlaying;
	}

	public void ClearEnableObjList()
	{
		m_listEnableObjList.Clear();
	}

	public void StopFSM()
	{
		if (m_pmRunFsm != null)
		{
			m_pmRunFsm.Fsm.Stop();
			m_pmRunFsm.gameObject.SetActive(value: false);
			m_pmRunFsm = null;
		}
	}

	public void AddEnableObjList(string strEnableObj, string strSendEvent = null)
	{
		int count = m_listEnableObjList.Count;
		bool flag = false;
		if (m_pmRunFsm != null && m_pmRunFsm.name == strEnableObj)
		{
			flag = true;
		}
		if (!flag)
		{
			for (int i = 0; i < count; i++)
			{
				if (m_listEnableObjList[i].m_strEnableObj == strEnableObj)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			m_listEnableObjList.Add(new clEnableObj(strEnableObj, strSendEvent));
		}
	}

	public bool CheckEnableObjAndRun()
	{
		if (m_listEnableObjList.Count <= 0)
		{
			return false;
		}
		return EnableAndRunObj(m_listEnableObjList[0].m_strEnableObj, m_listEnableObjList[0].m_strSendEvent, isEnableObjListRemove: true);
	}

	public bool RunKeywordGameOver()
	{
		bool result = false;
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("RUN_KEYWORD_ACTPOINT_0");
		Xls.SequenceData data_bySwitchIdx = Xls.SequenceData.GetData_bySwitchIdx(m_GameSwitch.GetCurSequence());
		m_GameSwitch.InitReplyGroup();
		if (xlsProgramDefineStr != null && data_bySwitchIdx != null)
		{
			result = EnableAndRunObj(xlsProgramDefineStr + data_bySwitchIdx.m_strKey);
		}
		return result;
	}

	public void SetDisableMentalZero(bool isDisableMentalZero)
	{
		m_isDisableRunMentalZero = isDisableMentalZero;
	}

	public bool GetDisableMentalZero()
	{
		return m_isDisableRunMentalZero;
	}

	public void CloseKeywordUseExplain()
	{
		if (m_GameMain.m_KeywordUseExplain.gameObject.activeInHierarchy)
		{
			m_GameMain.m_KeywordUseExplain.CloseWindow();
		}
	}

	public bool RunMentalZero(bool isSNSFaterEvt)
	{
		bool flag = false;
		if (!m_isDisableRunMentalZero)
		{
			string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("RUN_MENTAL_0");
			if (!GetRunEndMental())
			{
				if (xlsProgramDefineStr != null)
				{
					m_GameMain.m_clConversationSign.SetDialogSignType(ConversationSign.eDialogSignType.Off, null, isSetAnim: false);
					flag = EnableAndRunObj(xlsProgramDefineStr, GameGlobalUtil.GetXlsProgramDefineStr("PLAYMAKER_TRANSITION_END_MENTAL"));
					CloseKeywordUseExplain();
					if (flag)
					{
						m_isMentalZeroEvtFromSNSFater = isSNSFaterEvt;
					}
				}
				if (flag)
				{
					SetRunEndMental(isRun: true);
				}
			}
		}
		return flag;
	}

	public void SetRunEndMental(bool isRun)
	{
		m_isRunEndMental = isRun;
	}

	public bool GetRunEndMental()
	{
		return m_isRunEndMental;
	}

	public bool GetMentalZeroEvtFromSNSFater()
	{
		return m_isMentalZeroEvtFromSNSFater;
	}

	public GameObject ExistEvtObj(string strGameObj)
	{
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		if (m_goPMEventParent == null)
		{
			return null;
		}
		int childCount = m_goPMEventParent.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			gameObject2 = m_goPMEventParent.transform.GetChild(i).gameObject;
			if (gameObject2.name.Equals(strGameObj))
			{
				gameObject = gameObject2.gameObject;
				break;
			}
		}
		if (gameObject == null && m_goEachSceneEventParent != null)
		{
			childCount = m_goEachSceneEventParent.transform.childCount;
			for (int j = 0; j < childCount; j++)
			{
				gameObject2 = m_goEachSceneEventParent.transform.GetChild(j).gameObject;
				if (gameObject2.name.Equals(strGameObj))
				{
					gameObject = gameObject2.gameObject;
					break;
				}
			}
		}
		return gameObject;
	}

	public bool CheckExistEnableObj(string strGameObj)
	{
		GameObject gameObject = ExistEvtObj(strGameObj);
		return gameObject != null;
	}

	public GameObject ExistEvtCanvasObj(string strGameObj)
	{
		GameObject result = null;
		GameObject gameObject = null;
		if (strGameObj == null)
		{
			return null;
		}
		int childCount = m_goEventCanvas.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			gameObject = m_goEventCanvas.transform.GetChild(i).gameObject;
			if (gameObject.name == strGameObj)
			{
				result = gameObject.gameObject;
				break;
			}
		}
		return result;
	}

	public void EnableRecordFaterEventObj(string strObj, GameDefine.EventProc fpComp = null)
	{
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr(strObj);
		if (xlsProgramDefineStr != null)
		{
			EnableAndRunObj(xlsProgramDefineStr);
			m_fpCompRecFaterEventCB = fpComp;
		}
		else
		{
			fpComp?.Invoke(null, null);
		}
	}

	public bool FinishRecordFaterEventCBFunc()
	{
		if (m_fpCompRecFaterEventCB != null)
		{
			m_fpCompRecFaterEventCB(null, null);
			m_fpCompRecFaterEventCB = null;
			return true;
		}
		return false;
	}

	public bool EnableAndRunObj(string strGameObj, string strSendEvent = null, bool isEnableObjListRemove = false)
	{
		if (m_pmRunFsm != null)
		{
			AddEnableObjList(strGameObj, strSendEvent);
			return false;
		}
		if (isEnableObjListRemove)
		{
			int count = m_listEnableObjList.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					if (m_listEnableObjList[i].m_strEnableObj == strGameObj)
					{
						m_listEnableObjList.Remove(m_listEnableObjList[i]);
						break;
					}
				}
			}
		}
		bool result = false;
		GameObject gameObject = ExistEvtObj(strGameObj);
		if (gameObject != null)
		{
			m_GameSwitch.SetPMSendEvent(strSendEvent);
			PlayMakerFSM playMakerFSM = RunEvent(GameSwitch.eEventRunType.ENABLE_OBJ, gameObject, strSendEvent != null);
			if (playMakerFSM != null)
			{
				result = true;
			}
		}
		return result;
	}

	public PlayMakerFSM GetCurFSM()
	{
		return m_pmRunFsm;
	}

	public void StopEvent()
	{
		if (m_pmRunFsm != null)
		{
			m_pmRunFsm.Fsm.Stop();
			m_pmRunFsm.Fsm.InitData();
			m_pmRunFsm.gameObject.SetActive(value: false);
			m_pmRunFsm = null;
		}
		m_isEventPlaying = false;
	}

	public PlayMakerFSM RunEvent(GameSwitch.eEventRunType eRunType, GameObject go, bool isSendEvent = false)
	{
		GameObject gameObject = null;
		PlayMakerFSM playMakerFSM = null;
		StopEvent();
		if (eRunType == GameSwitch.eEventRunType.TOUCH_OBJ)
		{
			if (go.tag == "TOUCHABLE OBJECT" && go.transform.childCount > 0)
			{
				gameObject = go.transform.GetChild(0).gameObject;
			}
		}
		else
		{
			gameObject = go;
		}
		if (gameObject != null)
		{
			playMakerFSM = (m_pmRunFsm = gameObject.GetComponent<PlayMakerFSM>());
			gameObject.SetActive(value: true);
			if (playMakerFSM != null && isSendEvent)
			{
				string pMSendEvent = m_GameSwitch.GetPMSendEvent();
				if (pMSendEvent != string.Empty && pMSendEvent != null)
				{
					playMakerFSM.SendEvent(pMSendEvent);
					if (m_GameSwitch.GetLoadEventData())
					{
						m_GameSwitch.SetLoadEventData(isLoad: false);
						switch (pMSendEvent)
						{
						default:
							AudioManager.instance.InitAfterFirstLoad();
							break;
						case "AutoSave_Talk01":
						case "AutoSave_Talk02":
						case "AutoSave_Talk03":
						case "AutoSaveSearch":
							break;
						}
					}
				}
			}
			m_GameSwitch.SetRunEventObj(eRunType, go.name);
			if (playMakerFSM != null && !m_isEventPlaying)
			{
				if (m_GameMain != null)
				{
					m_GameMain.ShowInGameMainUI(isShow: false);
					m_GameMain.SetGameMainState(GameMain.eGameMainState.RunEvent);
				}
				m_isEventPlaying = true;
			}
		}
		return playMakerFSM;
	}

	public bool RunSaveSkipEvent()
	{
		string strEventObjName = m_GameSwitch.GetStrEventObjName();
		bool result = false;
		if (m_GameSwitch.m_eEventRunType == GameSwitch.eEventRunType.ENABLE_OBJ && strEventObjName != null)
		{
			result = RunEvent(m_GameSwitch.m_eEventRunType, ExistEvtObj(strEventObjName), isSendEvent: true);
			m_GameSwitch.SetStrEventObjName(null);
		}
		return result;
	}

	public void SetTempCam(bool isActive, bool isFullCoverActive = true)
	{
		if (!(m_GameMain == null))
		{
			m_GameMain.m_TempCam.SetActive(isActive);
			if (isFullCoverActive)
			{
				m_GameMain.SetFullCover(isActive);
			}
		}
	}

	public void ProcSetEvent()
	{
		if (m_EventCamera != null)
		{
			m_EventCamera.ProcShakeCamera();
		}
		EventCameraEffect.Instance.Update();
		if (m_EventObject != null)
		{
			m_EventObject.Proc();
		}
		if (m_TalkChar != null)
		{
			m_TalkChar.Update();
		}
	}

	public void LoadResourceAsync(string strRes, CBDoneLoadResource cbDoneFunc)
	{
		m_CBLoadResourceDone = null;
		m_CBLoadResourceDone = cbDoneFunc;
		m_GameMain.StartCoroutine(CheckLoadResourceAsync(strRes));
	}

	private IEnumerator CheckLoadResourceAsync(string strRes)
	{
		AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(strRes, typeof(GameObject));
		if (request != null)
		{
			while (!request.IsDone())
			{
				yield return null;
			}
			GameObject prefab = request.GetAsset<GameObject>();
			if (prefab != null && m_CBLoadResourceDone != null)
			{
				m_GameMain.StartCoroutine(m_CBLoadResourceDone(prefab));
			}
		}
	}

	public static void SetLoadIng(bool isIng, bool isOnlyBoolValSet = false)
	{
		m_isLoadIng = isIng;
		if (!isOnlyBoolValSet)
		{
			GetInstance().SetTempCam(isIng);
		}
	}

	public static bool GetLoadIng()
	{
		return m_isLoadIng;
	}

	public void ShowLoadingIcon(bool isShowLoadingIcon)
	{
		m_isLoadLevelShowLoadingIcon = isShowLoadingIcon;
	}

	public static void LoadLevel(string strLevel, string[] strSubLevels = null, GameMain clGameMain = null, string strStartEvent = null, CBFuncLoadDone cbFunc = null, bool isPlayStartEvent = false, bool isFirstLoad = false)
	{
		AudioManager instance = AudioManager.instance;
		if (instance != null)
		{
			instance.Stop(6);
			instance.Stop(5);
		}
		m_isFirstLoad = isFirstLoad;
		EventEngine instance2 = GetInstance();
		if (instance2.m_isLoadLevelShowLoadingIcon)
		{
			LoadingScreen.Show();
			LoadingScreen.ActiveLoadingPercent(isOn: true);
		}
		instance2.m_cbFuncLoadDone = cbFunc;
		if (clGameMain != null)
		{
			instance2.m_GameMain = clGameMain;
		}
		instance2.m_GameMain.SetGameMainState(GameMain.eGameMainState.LoadLevel);
		if (strStartEvent != null)
		{
			GameSwitch.GetInstance().SetRunEventObj(GameSwitch.eEventRunType.ENABLE_OBJ, strStartEvent);
		}
		if (instance2.m_GameMain != null)
		{
			instance2.m_GameMain.StartCoroutine(SceneLoad(strLevel, strSubLevels));
		}
	}

	private static IEnumerator SceneLoad(string strLoadLevel, string[] strSubLevels)
	{
		string strPrevLevel = m_strLoadedLevel;
		EventEngine eEngine = GetInstance();
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		SetLoadIng(isIng: true, isOnlyBoolValSet: true);
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 3 : 58);
		if (m_strLoadedLevel != null)
		{
			if (!eEngine.GetIsSaveFileLoad())
			{
				eEngine.OnlyFreeResForGoToMain();
				eEngine.UnloadEachLoad();
			}
			AsyncOperation unloadAsync = SceneManager.UnloadSceneAsync(m_strLoadedLevel);
			while (!unloadAsync.isDone || unloadAsync.progress < 1f)
			{
				yield return null;
			}
			if (m_strLoadedSubLevel != null)
			{
				int iLen = m_strLoadedSubLevel.Length;
				for (int i = 0; i < iLen; i++)
				{
					if (!string.IsNullOrEmpty(m_strLoadedSubLevel[i]))
					{
						unloadAsync = SceneManager.UnloadSceneAsync(m_strLoadedSubLevel[i]);
						while (!unloadAsync.isDone || unloadAsync.progress < 1f)
						{
							yield return null;
						}
					}
				}
			}
			EventCameraEffect.Instance.Clear();
			m_strLoadedLevel = null;
			m_strLoadedSubLevel = null;
		}
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 6 : 61);
		eEngine.SetIsSaveFileLoad(isSet: false);
		SetLoadIng(isIng: true);
		m_fLoadLevelDelayTime = 0f;
		yield return null;
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 15 : 64);
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 20 : 67);
		AsyncOperation loadAsync = SceneManager.LoadSceneAsync("Scene/" + strLoadLevel, LoadSceneMode.Additive);
		while (!loadAsync.isDone || loadAsync.progress < 1f)
		{
			yield return null;
		}
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 30 : 70);
		if (strSubLevels != null)
		{
			int iLen2 = strSubLevels.Length;
			for (int j = 0; j < iLen2; j++)
			{
				if (!string.IsNullOrEmpty(strSubLevels[j]))
				{
					loadAsync = SceneManager.LoadSceneAsync("Scene/" + strSubLevels[j], LoadSceneMode.Additive);
					while (!loadAsync.isDone || loadAsync.progress < 1f)
					{
						yield return null;
					}
				}
			}
		}
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 40 : 73);
		eEngine.SaveDefLevelRootsObjStates(strLoadLevel, strSubLevels);
		eEngine.SetTempCam(isActive: false, isFullCoverActive: false);
		yield return null;
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 50 : 76);
		while (m_fLoadLevelDelayTime <= LOADLEVEL_DELAYTIME)
		{
			m_fLoadLevelDelayTime += Time.deltaTime;
			yield return null;
		}
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 60 : 79);
		while (Camera.main == null)
		{
			yield return null;
		}
		yield return MainLoadThing.instance.StartCoroutine(eEngine.InitAfterLoadLevel());
		if (gameSwitch.m_isNeedLoadKeywordChar)
		{
			Xls.CharData chData = Xls.CharData.GetData_bySwitchIdx(KeywordMenuPlus.GetCharKeyIdx());
			if (chData != null)
			{
				string strCharKey = chData.m_strKey;
				yield return eEngine.m_GameMain.StartCoroutine(eEngine.m_TalkChar.ShowPartyChar(isShowTalkIcon: false));
				while (!eEngine.m_TalkChar.GetAllPartyCharLoad())
				{
					yield return null;
				}
				eEngine.m_TalkChar.CharAllHide();
				yield return null;
				int iSize = GameGlobalUtil.GetXlsScriptKeyValue("중");
				int iPos = GameGlobalUtil.GetXlsScriptKeyValue("중앙");
				yield return eEngine.m_GameMain.StartCoroutine(eEngine.m_TalkChar.CreateChar(strCharKey, iPos, iSize, gameSwitch.GetCharPartyMotion(strCharKey), gameSwitch.GetCharPartyDir(strCharKey)));
				gameSwitch.m_isNeedLoadKeywordChar = false;
			}
		}
		yield return null;
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 65 : 82);
		int iPlusPercent = 0;
		for (int k = 0; k <= 5; k++)
		{
			yield return eEngine.m_GameMain.StartCoroutine(eEngine.m_TalkChar.PreLoadChar(k));
			yield return null;
			iPlusPercent += 2;
			LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? (65 + iPlusPercent) : (82 + iPlusPercent));
		}
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 75 : 92);
		Resources.UnloadUnusedAssets();
		GC.Collect();
		eEngine.AfterLoadSetColor();
		string strEventObjName = gameSwitch.GetStrEventObjName();
		if (strEventObjName != null)
		{
			eEngine.ExistEvtObj(strEventObjName);
		}
		yield return null;
		LoadingScreen.SetLoadingPercent((!m_isFirstLoad) ? 90 : 95);
		if (gameSwitch.GetLoadEventData())
		{
			yield return MainLoadThing.instance.StartCoroutine(EventCameraEffect.Instance.ActivateLoadedData());
		}
		gameSwitch.SetPMSendEventWhenSaveNotExist();
		LoadingScreen.SetLoadingPercent(100);
		eEngine.RunSaveSkipEvent();
		m_strLoadedLevel = strLoadLevel;
		m_strLoadedSubLevel = strSubLevels;
		ActiveMultiLevel(m_strLoadedLevel);
		eEngine.m_GameMain.SetLoadDone(isLoadDone: true);
		if (eEngine.m_GameSwitch != null)
		{
			eEngine.m_GameSwitch.SetLoadDataToCamVal();
		}
		m_fLoadLevelDelayTime = 0f;
		while (m_fLoadLevelDelayTime <= LOADLEVEL_DELAYTIME)
		{
			m_fLoadLevelDelayTime += Time.deltaTime;
			yield return null;
		}
		SetLoadIng(isIng: false);
		if (eEngine.m_GameMain.GetGameMainState() == GameMain.eGameMainState.LoadLevel)
		{
			eEngine.m_GameMain.SetGameMainState(GameMain.eGameMainState.LoadDefScene);
		}
		if (eEngine.m_cbFuncLoadDone != null)
		{
			eEngine.m_cbFuncLoadDone();
			eEngine.m_cbFuncLoadDone = null;
		}
		m_strNeedLoadLevel = null;
		LoadingScreen.Close();
	}

	private void SaveDefLevelRootsObjStates(string strMainLevel, string[] strSubLevels)
	{
		int num = 0;
		if (m_loadedObjsStates != null)
		{
			num = m_loadedObjsStates.Length;
			for (int i = 0; i < num; i++)
			{
				m_loadedObjsStates[i].m_strLevelName = null;
				m_loadedObjsStates[i].m_dicRootsObjsStates.Clear();
			}
			m_loadedObjsStates = null;
		}
		if (m_loadedObjsStates == null)
		{
			int num2 = 1;
			if (strSubLevels != null)
			{
				num2 += strSubLevels.Length;
			}
			m_loadedObjsStates = new LoadedObjectState[num2];
			for (int j = 0; j < num2; j++)
			{
				m_loadedObjsStates[j] = new LoadedObjectState();
			}
		}
		SaveRootsObjState(strMainLevel);
		if (strSubLevels != null)
		{
			num = strSubLevels.Length;
			for (int k = 0; k < num; k++)
			{
				SaveRootsObjState(strSubLevels[k]);
			}
		}
	}

	private void SaveRootsObjState(string strScene)
	{
		if (string.IsNullOrEmpty(strScene))
		{
			return;
		}
		int num = m_loadedObjsStates.Length;
		for (int i = 0; i < num; i++)
		{
			if (!string.IsNullOrEmpty(m_loadedObjsStates[i].m_strLevelName) && !string.IsNullOrEmpty(strScene) && m_loadedObjsStates[i].m_strLevelName.CompareTo(strScene) == 0)
			{
				return;
			}
		}
		LoadedObjectState loadedObjectState = null;
		for (int j = 0; j < num; j++)
		{
			if (string.IsNullOrEmpty(m_loadedObjsStates[j].m_strLevelName))
			{
				m_loadedObjsStates[j].m_strLevelName = strScene;
				loadedObjectState = m_loadedObjsStates[j];
				break;
			}
		}
		if (loadedObjectState != null)
		{
			GameObject[] rootGameObjects = SceneManager.GetSceneByName(strScene).GetRootGameObjects();
			num = rootGameObjects.Length;
			for (int k = 0; k < num; k++)
			{
				loadedObjectState.m_dicRootsObjsStates.Add(rootGameObjects[k], rootGameObjects[k].activeInHierarchy);
			}
		}
	}

	public static void ActiveMultiLevel(string strSceneName)
	{
		ActiveScene(m_strLoadedLevel, strSceneName == m_strLoadedLevel, isMain: true);
		if (m_strLoadedSubLevel == null)
		{
			return;
		}
		int num = m_strLoadedSubLevel.Length;
		for (int i = 0; i < num; i++)
		{
			if (!string.IsNullOrEmpty(m_strLoadedSubLevel[i]))
			{
				ActiveScene(m_strLoadedSubLevel[i], strSceneName == m_strLoadedSubLevel[i], isMain: false);
			}
		}
	}

	private static void ActiveScene(string strScene, bool isActive, bool isMain)
	{
		GameObject[] rootGameObjects = SceneManager.GetSceneByName(strScene).GetRootGameObjects();
		int num = rootGameObjects.Length;
		string text = null;
		if (!isActive)
		{
			for (int i = 0; i < num; i++)
			{
				text = rootGameObjects[i].name;
				if (!isMain || (!(rootGameObjects[i].GetComponent<Camera>() != null) && text.CompareTo("Game_EventPrefab") != 0 && text.CompareTo("EventSystem") != 0 && !(rootGameObjects[i].tag == "GameEventObj")))
				{
					rootGameObjects[i].SetActive(isActive);
				}
			}
			return;
		}
		LoadedObjectState loadedObjectState = null;
		EventEngine instance = GetInstance();
		if (instance != null)
		{
			int num2 = instance.m_loadedObjsStates.Length;
			for (int j = 0; j < num2; j++)
			{
				if (instance.m_loadedObjsStates[j].m_strLevelName == strScene)
				{
					loadedObjectState = instance.m_loadedObjsStates[j];
					break;
				}
			}
			if (loadedObjectState == null)
			{
				return;
			}
			for (int k = 0; k < num; k++)
			{
				if (loadedObjectState.m_dicRootsObjsStates.ContainsKey(rootGameObjects[k]))
				{
					rootGameObjects[k].SetActive(loadedObjectState.m_dicRootsObjsStates[rootGameObjects[k]]);
				}
			}
		}
		SetRenderSetting(strScene);
	}

	private static void SetRenderSetting(string strLevel)
	{
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(strLevel));
	}

	public void SetDelayTime(float fTime)
	{
		m_fDelaySetTime = fTime;
		m_fDelayPassTime = 0f;
	}

	public bool ProcDelayTime()
	{
		m_fDelayPassTime += Time.deltaTime * GetLerpSkipValue();
		if (m_fDelayPassTime >= m_fDelaySetTime)
		{
			m_fDelayPassTime = m_fDelaySetTime;
		}
		return m_fDelayPassTime >= m_fDelaySetTime;
	}

	public bool ProcTime(int iType, ref float fSmoothStepFactor, ref float fPassTime, float fTime)
	{
		fPassTime += Time.deltaTime * GetLerpSkipValue();
		if (fPassTime >= fTime)
		{
			fSmoothStepFactor = 1f;
			fPassTime = fTime;
			return true;
		}
		float num = (fSmoothStepFactor = fPassTime / fTime);
		switch (iType)
		{
		case 0:
			fSmoothStepFactor = num;
			break;
		case 1:
			fSmoothStepFactor = Mathf.Pow(num, 2f);
			break;
		case 2:
			fSmoothStepFactor = 0.3f * (6f + Mathf.Log10(0.002f * Mathf.Pow(num, 2f)));
			break;
		case 3:
			fSmoothStepFactor = 1f - Mathf.Pow(Mathf.Cos(Mathf.Pow(num, 1.5f)), 8f);
			break;
		}
		return false;
	}

	public void Popup_Show(int iPopupType, string strPopup)
	{
		if (iPopupType == 0)
		{
			PopupDialoguePlus.ShowPopup_OK(strPopup, CB_PopupResult);
		}
		else
		{
			PopupDialoguePlus.ShowPopup_YesNo(strPopup, CB_PopupResult);
		}
	}

	private void CB_PopupResult(PopupDialoguePlus.Result result)
	{
		if (result == PopupDialoguePlus.Result.Close)
		{
		}
	}

	public void InitGoMainMenu()
	{
		Free();
		ReleaseInstance();
	}

	public void InitAllStatic()
	{
		m_strLoadedLevel = null;
		m_strNeedLoadLevel = null;
	}

	public void InitForConverLoad()
	{
		InitAllStatic();
		m_GameSwitch.InitGameSwitch(isRestart: true);
		m_GameSwitch.InitForConvertLoad();
		if (m_TalkChar != null)
		{
			m_TalkChar.InitForConvertFile();
		}
		if (m_EventObject != null)
		{
			m_EventObject.InitForConvertFile();
		}
		EventCameraEffect.Instance.InitForConvertLoad();
	}
}
