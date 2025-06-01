using System.Collections;
using System.Collections.Generic;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
	public enum eGameMainState
	{
		None = -1,
		Def,
		RunEvent,
		RunKeywordEvent,
		LoadLevel,
		LoadDefScene,
		KeywordMenu,
		SNSFaterMenu,
		SystemMenu,
		SmartWatchMenu
	}

	public enum eDefType
	{
		Talk,
		Investigate
	}

	public enum eGameMainUI
	{
		SmartWatch,
		MenuButton,
		Mental,
		Consider,
		KeywordGetPoint,
		QuickSave,
		All
	}

	public enum eProcFunc
	{
		CHAR_ZOOM,
		CHAR_MOVE
	}

	public enum eFromMenu
	{
		NONE,
		KEYWORD
	}

	public enum eGoMenu
	{
		None = -1,
		SmartWatchMenu,
		EventKeywordMenu
	}

	private delegate void CallBackEndMenuZoom();

	[Header("Start Level Setting")]
	public string STR_FIRST_LEVEL = string.Empty;

	[HideInInspector]
	public GameSwitch m_GameSwitch;

	[HideInInspector]
	public EventEngine m_EventEngine;

	[HideInInspector]
	public AudioManager m_AudioManager;

	[HideInInspector]
	public CommonButtonGuide m_CommonButtonGuide;

	[Header("Keyword Menu Setting")]
	public KeywordMenuPlus m_KeywordMenu;

	private eGameMainState m_eGameMainState = eGameMainState.None;

	private bool m_isDefStateComp;

	public static eDefType m_eDefType;

	[Header("GameUI / Root")]
	public GameObject m_goGameMainRoot;

	public GameMainMenu m_gameMainMenu;

	public SWatchMenuPlus m_SmartWatchRoot;

	public GameObject m_goKeywordMenuRoot;

	public KeywordUseExplain m_KeywordUseExplain;

	[Header("Event UI")]
	public GameObject m_goEventSelectRoot;

	public EventEngine_SelectPlus m_clEventSelect;

	public Event_TalkDialogue m_clEventTalkDialogue;

	public ConversationSign m_clConversationSign;

	public GameObject m_goEventTalkWindowRoot;

	[Header("ConnectScene")]
	public GameObject m_TempCam;

	public GameObject m_goFullCover;

	public Image m_imgFullCover;

	[Header("Marker")]
	public GameObject m_goDetectIcon;

	public GameObject m_goConversationIcon;

	[Header("Sequence Start/End")]
	public SequenceStartEnd m_SequenceStartEnd;

	public SequenceResultRank m_SequenceResultRank;

	public TalkCutStart m_TalkCutStart;

	public TalkCutResult m_TalkCutResult;

	[Header("Event Icon/Marker")]
	public GameObject m_goMentalDown;

	public GameObject m_goMentalUp;

	public Image m_imgMentalDown;

	public Image m_imgMentalUp;

	public Image[] m_imgDownLevel = new Image[3];

	public Image[] m_imgUpLevel = new Image[3];

	public Animator m_animMentalDown;

	public Animator m_animMentalUp;

	private GameObject m_goMentalPlay;

	private Animator m_animMentalPlay;

	public Canvas m_canvasMentalPlay;

	[Header("UI_Marker")]
	public GameObject m_goUIMarkerRoot;

	[Header("UI_DialogEnd")]
	public DialogEnd m_DialogEnd;

	private List<eProcFunc> m_listProcFunc;

	[HideInInspector]
	public eFromMenu m_eFromMenu;

	private bool m_isPressingMenu;

	private bool m_isZoomingMenu;

	private const string m_strCharRelationTutoID = "tuto_00008";

	private bool m_isLoadAssetBundle;

	private const string c_RankCharacterImageBundleName = "image/smartwatch_rank";

	private ContentThumbnailManager m_RankcharacterImageMgr;

	private static GameMain s_Instance;

	private static float m_fLoadLevelDelayTime;

	private static float LOADLEVEL_DELAYTIME = 0.5f;

	private const string c_strGameMainPath = "Prefabs/InGame/Game/UI_InGameMain";

	private const string c_strEventTalkWindowPath = "Prefabs/InGame/Game/UI_Event_Talk_Window";

	private const string c_strEventSelectPath = "Prefabs/InGame/Game/UI_Event_Select";

	private const string c_strNoticePath = "Prefabs/InGame/Menu/UI_Notice";

	private const string c_strSequenceStartEndPath = "Prefabs/InGame/Menu/UI_SequenceStartEnd";

	private const string c_strSequenceResultRankPath = "Prefabs/InGame/Menu/UI_SequenceResultRank";

	private const string c_strTalkCutStartPath = "Prefabs/InGame/Game/UI_TalkCutStart";

	private const string c_strTalkCutResultPath = "Prefabs/InGame/Game/UI_TalkCutResult";

	private const string c_strKeywordMenuPath = "Prefabs/InGame/Menu/UI_KeywordMenu";

	private const string c_strKeywordUseExplainPath = "Prefabs/InGame/Game/UI_KeywordUseExplain";

	private const string c_strBackLogMenuPath = "Prefabs/InGame/Menu/UI_BackLogMenu";

	private const string c_strConditionPath = "Prefabs/InGame/Game/Icon_Condition";

	private const string c_strStaffRollPath = "Prefabs/InGame/Game/UI_StaffRoll";

	private const string c_strDialogEndPath = "Prefabs/InGame/Game/UI_DialogEnd";

	private const string c_SceneGameMainName = "Scene/GameMain";

	[Header("FOR PREFAB LOAD")]
	public Object m_objGameMain;

	public Object m_objEventTalkWindow;

	public Object m_objEventSelect;

	public Object m_objNotice;

	public Object m_objSequenceStartEnd;

	public Object m_objSequenceResultRank;

	public Object m_objTalkCutStart;

	public Object m_objTalkCutResult;

	public Object m_objKeywordMenu;

	public Object m_objKeywordUseExplain;

	public Object m_objBackLogMenu;

	public Object m_objCondition;

	public Object m_objStaffRoll;

	public Object m_objDialogEnd;

	public Object m_objSceneGameMain;

	public Object m_prefabSplitReaction;

	private bool m_isLoadDone;

	private EventEngine.CBFuncLoadDone m_cbfpReloadDone;

	private eGoMenu m_eGoMenuType = eGoMenu.None;

	private bool m_isBottomGuideShowBefBackLog;

	private float m_curFadeOutTime;

	private float m_targetFadeOutTime;

	private bool m_isProcFadeOut;

	private GameDefine.EventProc m_fpFadeOutComplete;

	private bool m_isActiveSequenceResult;

	private bool m_isShowingTalkCutResult;

	private bool m_isShowRelationiTuto;

	private bool m_isRunTuto;

	private GameDefine.eAnimChangeState m_eChgStateChrRelation;

	private CallBackEndMenuZoom m_cbEndMenuZoom;

	private float m_fBefFov;

	private float m_fMoveFov = -1f;

	private float m_fMoveFovTime = 0.15f;

	private string m_strSpeed = "감속";

	public ContentThumbnailManager RankCharImageManager => m_RankcharacterImageMgr;

	public static GameMain instance => s_Instance;

	private void Awake()
	{
		s_Instance = this;
		s_Instance.m_isLoadAssetBundle = false;
	}

	private void OnDestroy()
	{
		Free();
		s_Instance = null;
	}

	private void OnDisable()
	{
		UnloadGameMainAssetBundle();
	}

	private void Free()
	{
		m_GameSwitch = null;
		m_EventEngine = null;
		m_AudioManager = null;
		m_CommonButtonGuide = null;
		m_gameMainMenu = null;
		m_goGameMainRoot = null;
		m_goEventTalkWindowRoot = null;
		m_clEventTalkDialogue = null;
		m_clConversationSign = null;
		m_goEventSelectRoot = null;
		m_clEventSelect = null;
		m_SequenceStartEnd = null;
		m_SequenceResultRank = null;
		m_TalkCutStart = null;
		m_TalkCutResult = null;
		m_SmartWatchRoot = null;
		m_goKeywordMenuRoot = null;
		m_KeywordMenu = null;
		m_KeywordUseExplain = null;
		m_DialogEnd = null;
		m_eFromMenu = eFromMenu.NONE;
		if (m_listProcFunc != null)
		{
			m_listProcFunc.Clear();
			m_listProcFunc = null;
		}
	}

	private void Start()
	{
		m_eGameMainState = eGameMainState.None;
		StartCoroutine(LoadGameMainLevel());
	}

	private IEnumerator LoadGameMainLevel()
	{
		m_fLoadLevelDelayTime = 0f;
		LoadingScreen.SetLoadingPercent(1);
		GameObject goTemp = Object.Instantiate(m_objGameMain) as GameObject;
		goTemp.name = "UI_InGameMain";
		m_goGameMainRoot = goTemp;
		m_gameMainMenu = m_goGameMainRoot.transform.GetChild(0).GetComponent<GameMainMenu>();
		m_gameMainMenu.m_GameMain = this;
		m_goGameMainRoot.SetActive(value: true);
		m_gameMainMenu.gameObject.SetActive(value: true);
		yield return null;
		LoadingScreen.SetLoadingPercent(3);
		goTemp = Object.Instantiate(m_objEventTalkWindow) as GameObject;
		goTemp.name = "UI_Event_Talk_Window";
		goTemp.SetActive(value: true);
		int iChildCount = goTemp.transform.childCount;
		for (int i = 0; i < iChildCount; i++)
		{
			Transform tfTemp = goTemp.transform.GetChild(i);
			string strGoName = tfTemp.name;
			if (strGoName == "MainTalkWindow")
			{
				tfTemp = goTemp.transform.GetChild(i);
				m_goEventTalkWindowRoot = tfTemp.gameObject;
				m_clEventTalkDialogue = tfTemp.GetComponent<Event_TalkDialogue>();
				m_goEventTalkWindowRoot.SetActive(value: true);
			}
			else if (strGoName == "ConversationSign")
			{
				m_clConversationSign = tfTemp.GetComponent<ConversationSign>();
				m_clConversationSign.gameObject.SetActive(value: false);
			}
		}
		yield return null;
		LoadingScreen.SetLoadingPercent(6);
		goTemp = Object.Instantiate(m_objEventSelect) as GameObject;
		goTemp.name = "UI_Event_Select";
		m_goEventSelectRoot = goTemp.transform.GetChild(0).gameObject;
		m_clEventSelect = m_goEventSelectRoot.GetComponent<EventEngine_SelectPlus>();
		m_goEventSelectRoot.SetActive(value: false);
		yield return null;
		LoadingScreen.SetLoadingPercent(9);
		goTemp = Object.Instantiate(m_objNotice) as GameObject;
		goTemp.name = "UI_Notice";
		yield return null;
		LoadingScreen.SetLoadingPercent(12);
		goTemp = Object.Instantiate(m_objSequenceStartEnd) as GameObject;
		goTemp.name = "UI_SequenceStartEnd";
		m_SequenceStartEnd = goTemp.GetComponent<SequenceStartEnd>();
		goTemp.SetActive(value: false);
		yield return null;
		LoadingScreen.SetLoadingPercent(15);
		goTemp = Object.Instantiate(m_objSequenceResultRank) as GameObject;
		goTemp.name = "UI_SequenceResultRank";
		m_SequenceResultRank = goTemp.GetComponent<SequenceResultRank>();
		goTemp.SetActive(value: false);
		yield return null;
		LoadingScreen.SetLoadingPercent(18);
		goTemp = Object.Instantiate(m_objTalkCutStart) as GameObject;
		goTemp.name = "UI_TalkCutStart";
		m_TalkCutStart = goTemp.GetComponent<TalkCutStart>();
		goTemp.SetActive(value: false);
		yield return null;
		LoadingScreen.SetLoadingPercent(21);
		goTemp = Object.Instantiate(m_objTalkCutResult) as GameObject;
		goTemp.name = "UI_TalkCutResult";
		m_TalkCutResult = goTemp.GetComponent<TalkCutResult>();
		goTemp.SetActive(value: false);
		yield return null;
		LoadingScreen.SetLoadingPercent(24);
		goTemp = Object.Instantiate(m_objKeywordMenu) as GameObject;
		m_goKeywordMenuRoot = goTemp.transform.GetChild(0).gameObject;
		m_KeywordMenu = m_goKeywordMenuRoot.GetComponent<KeywordMenuPlus>();
		m_SmartWatchRoot.m_MemoButton.m_LinkedMenuRoot = m_goKeywordMenuRoot;
		goTemp.name = "UI_KeywordMenu";
		goTemp.SetActive(value: true);
		yield return null;
		LoadingScreen.SetLoadingPercent(27);
		goTemp = Object.Instantiate(m_objKeywordUseExplain) as GameObject;
		goTemp.name = "UI_KeywordUseExplain";
		m_KeywordUseExplain = goTemp.GetComponent<KeywordUseExplain>();
		m_KeywordUseExplain.m_KeywordMenuPlus = m_goKeywordMenuRoot.GetComponent<KeywordMenuPlus>();
		goTemp.SetActive(value: false);
		yield return null;
		LoadingScreen.SetLoadingPercent(30);
		goTemp = Object.Instantiate(m_objBackLogMenu) as GameObject;
		goTemp.name = "UI_BackLogMenu";
		goTemp.SetActive(value: false);
		goTemp.GetComponent<BackLogMenuPlus>().SetInstance();
		yield return null;
		LoadingScreen.SetLoadingPercent(33);
		goTemp = Object.Instantiate(m_objCondition) as GameObject;
		goTemp.transform.SetParent(m_goUIMarkerRoot.transform);
		goTemp.name = "Icon_Condition";
		goTemp.SetActive(value: true);
		yield return null;
		LoadingScreen.SetLoadingPercent(36);
		goTemp = Object.Instantiate(m_objStaffRoll) as GameObject;
		goTemp.name = "StaffRoll";
		goTemp.SetActive(value: true);
		yield return null;
		LoadingScreen.SetLoadingPercent(37);
		goTemp = Object.Instantiate(m_objDialogEnd) as GameObject;
		goTemp.name = "UI_DialogEnd";
		m_DialogEnd = goTemp.GetComponent<DialogEnd>();
		yield return StartCoroutine(SplitScreenReaction.PreloadFromAsset());
		yield return null;
		LoadingScreen.SetLoadingPercent(40);
		yield return StartCoroutine(MainMenuCommon.PreloadToInGameMain());
		yield return null;
		LoadingScreen.SetLoadingPercent(43);
		yield return StartCoroutine(KeywordGetPopupPlus.Create());
		yield return null;
		LoadingScreen.SetLoadingPercent(46);
		yield return StartCoroutine(ProfileGetPopup.Create());
		yield return null;
		LoadingScreen.SetLoadingPercent(49);
		yield return StartCoroutine(MentalGageRenewal.CreateInstance());
		yield return null;
		LoadingScreen.SetLoadingPercent(52);
		m_RankcharacterImageMgr = new ContentThumbnailManager("image/smartwatch_rank");
		yield return StartCoroutine(m_RankcharacterImageMgr.LoadAssetsAll(this));
		while (m_fLoadLevelDelayTime <= LOADLEVEL_DELAYTIME)
		{
			m_fLoadLevelDelayTime += Time.deltaTime;
			yield return null;
		}
		LoadingScreen.SetLoadingPercent(55);
		while (!MainLoadThing.instance.isCompleteFaterProfileImageLoad)
		{
			yield return null;
		}
		LoadingScreen.SetLoadingPercent(58);
		m_AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
		m_CommonButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
		yield return StartCoroutine(ContiDataHandler.Init("ContiData"));
		m_GameSwitch = GameSwitch.GetInstance();
		m_GameSwitch.SetFirst();
		m_EventEngine = EventEngine.GetInstance();
		m_goGameMainRoot.SetActive(value: true);
		m_eGameMainState = eGameMainState.Def;
		bool isFirstLevel = EventEngine.m_strNeedLoadLevel == null;
		string strStartEvent = GameGlobalUtil.GetXlsProgramDefineStr("PM_EVENT_START_EVENT");
		EventEngine.LoadLevel((!isFirstLevel) ? EventEngine.m_strNeedLoadLevel : STR_FIRST_LEVEL, null, this, (!isFirstLevel || strStartEvent == null) ? null : strStartEvent, OnLoadLevelDone, isFirstLevel && strStartEvent != null, isFirstLoad: true);
		InitValue();
		yield return null;
		m_isLoadAssetBundle = true;
		yield return null;
	}

	private void OnLoadLevelDone()
	{
		m_listProcFunc = new List<eProcFunc>();
		m_gameMainMenu.SetTutoObj(m_GameSwitch.GetTutoMenuState());
		MainMenuCommon.UnloadScene();
		Resources.UnloadUnusedAssets();
	}

	public void AddProcEvent(eProcFunc eAddFunc, eFromMenu eBefMenu = eFromMenu.NONE)
	{
		if (m_listProcFunc == null)
		{
			return;
		}
		bool flag = true;
		int count = m_listProcFunc.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_listProcFunc[i] == eAddFunc)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			m_listProcFunc.Add(eAddFunc);
		}
		m_eFromMenu = eBefMenu;
	}

	public void ProcEvent()
	{
		bool flag = true;
		bool flag2 = true;
		if (m_listProcFunc == null || m_listProcFunc.Count <= 0)
		{
			return;
		}
		int count = m_listProcFunc.Count;
		for (int i = 0; i < count; i++)
		{
			switch (m_listProcFunc[i])
			{
			case eProcFunc.CHAR_ZOOM:
				flag2 = m_EventEngine.m_TalkChar.ProcCharZoom();
				break;
			case eProcFunc.CHAR_MOVE:
				flag2 = m_EventEngine.m_TalkChar.ProcCharMove();
				break;
			}
			flag = flag && flag2;
		}
		if (flag)
		{
			m_listProcFunc.Clear();
			if (m_eFromMenu == eFromMenu.KEYWORD)
			{
				m_eFromMenu = eFromMenu.NONE;
			}
		}
	}

	public void SetLoadDone(bool isLoadDone)
	{
		m_isLoadDone = isLoadDone;
	}

	private void Update()
	{
		if (m_isProcFadeOut)
		{
			UpdateFullCover_FadeOut();
			return;
		}
		if (m_isZoomingMenu)
		{
			UpdateEnterMenuZoom();
			return;
		}
		bool flag = false;
		bool flag2 = false;
		switch (m_eGameMainState)
		{
		case eGameMainState.LoadLevel:
			if (m_isLoadDone)
			{
				if (m_EventEngine.IsEventRunning())
				{
					SetGameMainState(eGameMainState.RunEvent);
				}
				else if (m_GameSwitch.IsExistPartyIn() || m_GameSwitch.IsExistInvestObj())
				{
					SetGameMainState(eGameMainState.LoadDefScene);
				}
				else
				{
					SetGameMainState(eGameMainState.Def);
				}
			}
			break;
		case eGameMainState.LoadDefScene:
			switch (m_eDefType)
			{
			case eDefType.Talk:
				flag = m_EventEngine.m_TalkChar.ProcShowPartyChar();
				break;
			case eDefType.Investigate:
				flag = true;
				break;
			}
			if (flag)
			{
				SetGameMainState(eGameMainState.Def);
			}
			break;
		case eGameMainState.Def:
		case eGameMainState.KeywordMenu:
		case eGameMainState.SNSFaterMenu:
			ProcEvent();
			break;
		}
		if (PopupDialoguePlus.IsAnyPopupActivated() || IsPressTalkOkButton())
		{
			return;
		}
		switch (m_eGameMainState)
		{
		case eGameMainState.Def:
			if ((m_GameSwitch.GetCutConsider() && m_GameSwitch.GetTutorial(m_gameMainMenu.m_strExamineTuto) == 0) || !TutorialPopup.IsTutorialPopupEnd() || m_gameMainMenu.m_ePushState != GameMainMenu.ePushState.None || m_gameMainMenu.IsGameMainMenuKeyLock())
			{
				break;
			}
			if (m_eDefType == eDefType.Talk && m_GameSwitch.IsExistPartyIn())
			{
				switch (GamePadInput.GetLStickDownDir())
				{
				case GamePadInput.StickDir.Left:
					AudioManager.instance.PlayUISound("Select_Marker");
					m_GameSwitch.MoveTalkSelIdx(isLeft: true);
					m_EventEngine.m_TalkChar.SetTalkMarkAnim();
					break;
				case GamePadInput.StickDir.Right:
					AudioManager.instance.PlayUISound("Select_Marker");
					m_GameSwitch.MoveTalkSelIdx(isLeft: false);
					m_EventEngine.m_TalkChar.SetTalkMarkAnim();
					break;
				case GamePadInput.StickDir.None:
					if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
					{
						m_EventEngine.m_TalkChar.TalkPressOKButton();
					}
					break;
				}
			}
			else if (m_eDefType == eDefType.Investigate && m_GameSwitch.GetInvestObjCnt() > 0)
			{
				GamePadInput.StickDir lStickDownDir = GamePadInput.GetLStickDownDir();
				if (lStickDownDir != GamePadInput.StickDir.None)
				{
					m_GameSwitch.MoveInvestXYPlusMinus(lStickDownDir == GamePadInput.StickDir.Left || lStickDownDir == GamePadInput.StickDir.Right, lStickDownDir == GamePadInput.StickDir.Right || lStickDownDir == GamePadInput.StickDir.Down);
					m_EventEngine.m_EventObject.SetInvestButAnim();
				}
				else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
				{
					m_EventEngine.m_EventObject.TouchFindMarker();
				}
			}
			break;
		case eGameMainState.RunEvent:
			if (!IsActiveBackLogMenu() && m_EventEngine != null)
			{
				m_EventEngine.PressSkipKey();
			}
			break;
		}
	}

	public bool IsPressTalkOkButton()
	{
		bool result = false;
		if (m_EventEngine != null)
		{
			if (m_eDefType == eDefType.Investigate)
			{
				if (m_EventEngine.m_EventObject != null)
				{
					result = m_EventEngine.m_EventObject.IsPressInvestOKButton();
				}
			}
			else if (m_EventEngine.m_TalkChar != null)
			{
				result = m_EventEngine.m_TalkChar.IsPressTalkOKButton();
			}
		}
		return result;
	}

	public static void UnloadGameMainAssetBundle()
	{
		if (!(s_Instance == null) && (!(s_Instance != null) || s_Instance.m_isLoadAssetBundle))
		{
			if (s_Instance != null && s_Instance.m_RankcharacterImageMgr != null)
			{
				s_Instance.m_RankcharacterImageMgr.ClearThumbnailCaches();
				s_Instance.m_RankcharacterImageMgr.UnloadThumbnailBundle();
				s_Instance.m_RankcharacterImageMgr = null;
			}
			SplitScreenReaction.UnloadLoadedAssetBundle();
			KeywordGetPopupPlus.Destroy();
			ProfileGetPopup.UnloadAssetBundle();
			MentalGageRenewal.Free();
			if (s_Instance != null)
			{
				s_Instance.m_isLoadAssetBundle = false;
			}
		}
	}

	public void ReLoadGameScene(int iSlotIdx, bool isEvent, EventEngine.CBFuncLoadDone cbLoadDone = null)
	{
		m_EventEngine.StopSkip();
		m_EventEngine.StopEvent();
		ContiDataHandler.FinishContiForced();
		m_EventEngine.ClearCurrentLevelEvent();
		m_EventEngine.OnlyFreeResForGoToMain();
		m_GameSwitch.FreeGoToMain();
		m_GameSwitch.InitEventObjName();
		m_GameSwitch.FreeForCurKeywordEvtVal();
		m_cbfpReloadDone = cbLoadDone;
		if (isEvent)
		{
			if (m_GameSwitch.GetReLoadSlotIdx() == -1)
			{
				GameSwitch.GetInstance().InitGameVal(ConstGameSwitch.eSTARTTYPE.RESTART, 0, OnGameSaveReloadDone);
			}
			else
			{
				GameSwitch.GetInstance().InitGameVal(ConstGameSwitch.eSTARTTYPE.CONTINUE, m_GameSwitch.GetReLoadSlotIdx(), OnGameSaveReloadDone);
			}
		}
		else
		{
			GameSwitch.GetInstance().InitGameVal(ConstGameSwitch.eSTARTTYPE.CONTINUE, iSlotIdx, OnGameSaveReloadDone);
		}
	}

	private void OnGameSaveReloadDone(bool isExistErr)
	{
		bool flag = EventEngine.m_strNeedLoadLevel == null;
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("PM_EVENT_START_EVENT");
		EventEngine.LoadLevel((!flag) ? EventEngine.m_strNeedLoadLevel : STR_FIRST_LEVEL, null, this, (!flag || xlsProgramDefineStr == null) ? null : xlsProgramDefineStr, OnReloadAllDone);
	}

	private void OnReloadAllDone()
	{
		if (m_cbfpReloadDone != null)
		{
			m_cbfpReloadDone();
		}
	}

	public static IEnumerator GoMainMenu()
	{
		AudioManager.instance.Stop();
		EventEngine eventEngine = EventEngine.GetInstance();
		if (eventEngine != null)
		{
			eventEngine.SetSkip(isSkip: false, isForce: true);
			eventEngine.InitGoMainMenu();
			CommonButtonGuide commonButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
			if (commonButtonGuide != null)
			{
				commonButtonGuide.SetShow(isShow: false);
			}
		}
		LoadingScreen.Show();
		if (SkipButtonGuide.instance != null)
		{
			SkipButtonGuide.instance.gameObject.SetActive(value: false);
		}
		AsyncOperation operation = MainMenuCommon.LoadScene(isAsync: true);
		MainLoadThing.instance.StartCoroutine(MainMenuCommon.ChangeMode(MainMenuCommon.Mode.MainMenu));
		MainMenuCommon.SetBGType(MainMenuCommon.BGType.ImageBG);
		if (operation != null)
		{
			while (!operation.isDone)
			{
				yield return null;
			}
		}
		yield return null;
		UnloadGameMainAssetBundle();
		yield return null;
		Resources.UnloadUnusedAssets();
	}

	public eGameMainState GetGameMainState()
	{
		return m_eGameMainState;
	}

	private void cbfpShowPartyChar(object sender, object arg)
	{
		m_isDefStateComp = true;
	}

	public void SetGameMainState(eGameMainState eState)
	{
		if (eState == m_eGameMainState)
		{
			return;
		}
		switch (m_eGameMainState)
		{
		case eGameMainState.Def:
			ShowInGameMainUI(isShow: false);
			if (m_eDefType == eDefType.Investigate)
			{
				m_EventEngine.m_EventObject.PlayAppearMot_FindObj(isAppear: false);
				break;
			}
			m_EventEngine.m_TalkChar.SetTalkMarkAppear(isAppear: false);
			ButtonSpriteSwap.DeleteAllButton();
			break;
		case eGameMainState.RunEvent:
			m_EventEngine.SetEventBotGuide(isShow: false);
			break;
		}
		switch (eState)
		{
		case eGameMainState.LoadLevel:
			SetLoadDone(isLoadDone: false);
			break;
		case eGameMainState.LoadDefScene:
			if (m_eDefType == eDefType.Talk)
			{
				if (!IsClosedKeywordMenu())
				{
					m_EventEngine.m_TalkChar.RevertPartyCharKeywordSet(m_GameSwitch.GetRunKeywordCharKey());
					break;
				}
				m_isDefStateComp = false;
				StartCoroutine(m_EventEngine.m_TalkChar.ShowPartyChar(isShowTalkIcon: true, cbfpShowPartyChar));
			}
			else if (m_eDefType == eDefType.Investigate)
			{
				m_isDefStateComp = true;
				m_EventEngine.m_EventObject.ShowFindMarker();
			}
			break;
		case eGameMainState.RunEvent:
			m_EventEngine.SetShowSkipBut(isShowSkipBut: true);
			m_EventEngine.SetEventBotGuide(isShow: true);
			ShowInGameMainUI(isShow: false);
			m_EventEngine.m_TalkChar.HidePartyTalkIcon();
			break;
		case eGameMainState.Def:
			GameSwitch.GetInstance().CheckShowSNSButton();
			if (m_eDefType == eDefType.Investigate)
			{
				SetGameMainSub(isShow: true, eGameMainUI.MenuButton);
				SetGameMainSub(isShow: true, eGameMainUI.Mental);
				SetGameMainSub(isShow: true, eGameMainUI.QuickSave);
			}
			else
			{
				ShowInGameMainUI(isShow: true);
			}
			break;
		case eGameMainState.KeywordMenu:
			if (KeywordMenuPlus.m_eKeywordState == KeywordMenuPlus.KeywordState.TALK)
			{
				m_EventEngine.m_TalkChar.MovePartyCharKeywordSet(m_GameSwitch.GetRunKeywordCharKey());
			}
			break;
		}
		m_eGameMainState = eState;
	}

	public bool IsGameMainStateDef()
	{
		return m_eGameMainState == eGameMainState.Def && m_isDefStateComp;
	}

	public static void SetDefaultSceneType(string strSceneType)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSceneType);
		if (xlsScriptKeyValue != -1)
		{
			SetDefaultSceneType((eDefType)xlsScriptKeyValue);
		}
	}

	public static void SetDefaultSceneType(eDefType defSceneType)
	{
		m_eDefType = defSceneType;
	}

	public static eDefType GetDefaultSceneType()
	{
		return m_eDefType;
	}

	private void SetGameMainSub(bool isShow, eGameMainUI eUI)
	{
		m_gameMainMenu.ShowAnimation(isShow, (int)eUI);
	}

	public void ShowInGameMainUI(bool isShow, eGameMainUI eVal = eGameMainUI.All, bool isRemainFalse = false)
	{
		if (eVal != eGameMainUI.All)
		{
			if (isRemainFalse)
			{
				for (eGameMainUI eGameMainUI = eGameMainUI.SmartWatch; eGameMainUI < eGameMainUI.All; eGameMainUI++)
				{
					if (eGameMainUI != eVal)
					{
						SetGameMainSub(isShow: false, eGameMainUI);
					}
				}
			}
			SetGameMainSub(isShow, eVal);
		}
		else
		{
			for (eGameMainUI eGameMainUI2 = eGameMainUI.SmartWatch; eGameMainUI2 < eGameMainUI.All; eGameMainUI2++)
			{
				SetGameMainSub(isShow, eGameMainUI2);
			}
		}
	}

	public void OnClickMenuButton()
	{
		ShowKeywordMenu(isShow: true, isKeywordMenu: true);
	}

	public bool IsClosedEventSelect()
	{
		if (m_goEventSelectRoot == null)
		{
			return true;
		}
		return !m_goEventSelectRoot.activeSelf;
	}

	public void ShowKeywordMenu(bool isShow, bool isKeywordMenu, string strCharKey = null, bool isFromKeywordEvt = false, bool isShowBackButton = true, bool isFinalSeq = false)
	{
		if (!m_isPressingMenu && !m_goKeywordMenuRoot.activeInHierarchy && m_eGameMainState != eGameMainState.SNSFaterMenu && m_eGameMainState != eGameMainState.SystemMenu)
		{
			m_isPressingMenu = true;
			m_KeywordMenu.SetMenuState((!isKeywordMenu) ? KeywordMenuPlus.KeywordState.TALK : KeywordMenuPlus.KeywordState.MEMO, strCharKey, this, isFromKeywordEvt, isShowBackButton, isFinalSeq);
			m_goKeywordMenuRoot.SetActive(isShow);
			if (isShow)
			{
				SetGameMainState(eGameMainState.KeywordMenu);
			}
			m_isPressingMenu = false;
		}
	}

	public void ShowKeywordUseMenu(bool isShow, bool isReUse, string strKeywordUseID = null, int iSelCnt = 0)
	{
		if (m_goKeywordMenuRoot.activeInHierarchy)
		{
			return;
		}
		if (isShow)
		{
			if (isReUse)
			{
				m_KeywordMenu.SetReUseKeyword(this);
			}
			else
			{
				m_KeywordMenu.SetUseKeyword(strKeywordUseID, iSelCnt, this);
			}
		}
		m_goKeywordMenuRoot.SetActive(isShow);
		if (isShow)
		{
			SetGameMainState(eGameMainState.KeywordMenu);
		}
	}

	private void OnClosed_SmartWatchMenu(object sender, object arg)
	{
		EndEnterMenuZoomBack();
	}

	public void ShowEnterMenuZoom(eGoMenu eGoMenuType)
	{
		m_eGoMenuType = eGoMenuType;
		switch (eGoMenuType)
		{
		case eGoMenu.SmartWatchMenu:
			ShowEnterMenuZoom(ShowSmartWatchMenu);
			break;
		case eGoMenu.EventKeywordMenu:
			ShowEnterMenuZoom(ShowEventKeyword);
			break;
		}
	}

	public void ShowSmartWatchMenu()
	{
		if (!m_isPressingMenu && !(m_SmartWatchRoot == null) && m_eGameMainState != eGameMainState.KeywordMenu && m_eGameMainState != eGameMainState.SNSFaterMenu && m_eGameMainState != eGameMainState.SystemMenu)
		{
			m_isPressingMenu = true;
			SetGameMainState(eGameMainState.SmartWatchMenu);
			StartCoroutine(m_SmartWatchRoot.Show(OnClosed_SmartWatchMenu));
			m_isPressingMenu = false;
		}
	}

	public void ShowEventKeyword()
	{
		if (!m_isPressingMenu && !m_goKeywordMenuRoot.activeInHierarchy && m_eGameMainState != eGameMainState.SNSFaterMenu && m_eGameMainState != eGameMainState.SystemMenu)
		{
			m_isPressingMenu = true;
			m_KeywordMenu.SetMenuState(KeywordMenuPlus.KeywordState.EVENT_KEYWORD, null, this);
			m_goKeywordMenuRoot.SetActive(value: true);
			SetGameMainState(eGameMainState.KeywordMenu);
			m_isPressingMenu = false;
		}
	}

	public void NotSetOpenKeyword(bool isShow)
	{
		if (!m_goKeywordMenuRoot.activeSelf)
		{
			m_KeywordMenu.SetFromEndRunKeyword(this);
			m_goKeywordMenuRoot.SetActive(isShow);
			if (isShow)
			{
				SetGameMainState(eGameMainState.KeywordMenu);
			}
		}
	}

	public bool IsClosedKeywordMenu()
	{
		if (m_goKeywordMenuRoot == null)
		{
			return true;
		}
		return !m_goKeywordMenuRoot.activeSelf;
	}

	public IEnumerator ShowSystemMenu()
	{
		if (!m_isPressingMenu && m_eGameMainState != eGameMainState.KeywordMenu && m_eGameMainState != eGameMainState.SmartWatchMenu && m_eGameMainState != eGameMainState.SNSFaterMenu)
		{
			m_isPressingMenu = true;
			SetGameMainState(eGameMainState.SystemMenu);
			if (MainMenuCommon.instance == null)
			{
				yield return StartCoroutine(MainMenuCommon.PreloadToInGameMain());
			}
			MainMenuCommon.eventCloseComplete = OnClosed_SystemMenu;
			MainMenuCommon.SetBGType(MainMenuCommon.BGType.ScreenShotBG);
			yield return StartCoroutine(MainMenuCommon.ChangeMode(MainMenuCommon.Mode.SystemMenu));
			m_isPressingMenu = false;
		}
	}

	public IEnumerator ShowQuickSaveMenu()
	{
		if (!m_isPressingMenu && m_eGameMainState != eGameMainState.KeywordMenu && m_eGameMainState != eGameMainState.SmartWatchMenu && m_eGameMainState != eGameMainState.SNSFaterMenu)
		{
			m_isPressingMenu = true;
			SetGameMainState(eGameMainState.SystemMenu);
			if (MainMenuCommon.instance == null)
			{
				yield return StartCoroutine(MainMenuCommon.PreloadToInGameMain());
			}
			MainMenuCommon.eventCloseComplete = OnClosed_SystemMenu;
			MainMenuCommon.SetBGType(MainMenuCommon.BGType.ScreenShotBG);
			yield return StartCoroutine(MainMenuCommon.ChangeMode(MainMenuCommon.Mode.QuickSave));
			m_isPressingMenu = false;
		}
	}

	public void OnClosed_SystemMenu(object sender, object arg)
	{
		SetGameMainState(eGameMainState.LoadDefScene);
	}

	public IEnumerator ShowSNSMenu(bool isShow, SNSMenuPlus.Mode mode, GameDefine.EventProc fpClosed = null)
	{
		if (!m_isPressingMenu && m_eGameMainState != eGameMainState.KeywordMenu && m_eGameMainState != eGameMainState.SmartWatchMenu && m_eGameMainState != eGameMainState.SystemMenu && !SNSMenuPlus.IsActivated)
		{
			if (fpClosed != null)
			{
				SetGameMainState(eGameMainState.SNSFaterMenu);
			}
			m_isPressingMenu = true;
			yield return StartCoroutine(SNSMenuPlus.ShowSNSMenu_FormAssetBundle(mode, fpClosed));
			m_isPressingMenu = false;
		}
	}

	public void CBCloseSNSMenu(object sender, object arg)
	{
		bool flag = false;
		if (m_GameSwitch.GetMental() <= ConstGameSwitch.MIN_MENTAL_POINT)
		{
			flag = m_EventEngine.RunMentalZero(isSNSFaterEvt: true);
		}
		if (!flag)
		{
			SetGameMainState(eGameMainState.LoadDefScene);
		}
	}

	public bool IsClosedSNSMenu()
	{
		return !SNSMenuPlus.IsActivated;
	}

	public void ShowMessengerMenu(bool isShow)
	{
		if (!MSGMenuPlus.IsActivated)
		{
			StartCoroutine(MSGMenuPlus.ShowMSGMenu_FormAssetBundle());
		}
	}

	public bool IsClosedMessengerMenu()
	{
		return !MSGMenuPlus.IsActivated;
	}

	public static void ShowBackLogMenu(bool isShow)
	{
		if (BackLogMenuPlus.instance == null || (EventEngine.GetInstance() != null && EventEngine.GetInstance().GetSkip()))
		{
			return;
		}
		CommonButtonGuide commonButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
		BackLogMenuPlus.instance.gameObject.SetActive(value: true);
		if (instance != null)
		{
			instance.m_isBottomGuideShowBefBackLog = commonButtonGuide != null && commonButtonGuide.IsShow();
		}
		if (isShow)
		{
			BackLogMenuPlus.instance.Show(BackLogCallBack);
			if (instance != null && instance.m_isBottomGuideShowBefBackLog && commonButtonGuide != null)
			{
				commonButtonGuide.SetShow(isShow: false);
			}
		}
	}

	private static void BackLogCallBack(object sender, object arg)
	{
		Event_TalkDialogue.instance.CloseBacklogCB();
		if (instance != null && instance.m_isBottomGuideShowBefBackLog)
		{
			GameGlobalUtil.GetCommonButtonGuide().SetShow(isShow: true);
		}
	}

	public static bool IsActiveBackLogMenu()
	{
		return BackLogMenuPlus.instance != null && BackLogMenuPlus.instance.gameObject.activeInHierarchy;
	}

	public void SetFullCover(bool isActive)
	{
		m_goFullCover.SetActive(isActive);
		if (isActive)
		{
			Color fillCameraColor = EventCameraEffect.Instance.GetFillCameraColor();
			fillCameraColor.a = 1f;
			m_imgFullCover.color = fillCameraColor;
		}
	}

	public void StartFullCover_FadeOut(float time, GameDefine.EventProc fpCompleteCB = null)
	{
		if (!(m_imgFullCover == null) && !m_isProcFadeOut)
		{
			m_curFadeOutTime = 0f;
			m_targetFadeOutTime = time;
			m_isProcFadeOut = true;
			m_fpFadeOutComplete = ((fpCompleteCB == null) ? null : new GameDefine.EventProc(fpCompleteCB.Invoke));
			Color color = m_imgFullCover.color;
			color.a = 0f;
			m_imgFullCover.color = color;
			m_imgFullCover.gameObject.SetActive(value: true);
		}
	}

	private void UpdateFullCover_FadeOut()
	{
		m_curFadeOutTime += Time.deltaTime;
		Color color = m_imgFullCover.color;
		color.a = ((!(m_curFadeOutTime < m_targetFadeOutTime)) ? 1f : (m_curFadeOutTime / m_targetFadeOutTime));
		m_imgFullCover.color = color;
		if (GameGlobalUtil.IsAlmostSame(color.a, 1f) || color.a > 1f)
		{
			m_isProcFadeOut = false;
			if (m_fpFadeOutComplete != null)
			{
				m_fpFadeOutComplete(null, null);
			}
		}
	}

	public void ShowSequenceStart()
	{
		if (!(m_SequenceStartEnd == null))
		{
			m_SequenceStartEnd.ShowStartSequence(GameSwitch.GetInstance().GetCurSequence());
		}
	}

	public void ShowSequenceEnd()
	{
		if (!(m_SequenceStartEnd == null))
		{
			m_isActiveSequenceResult = true;
			m_SequenceStartEnd.ShowEndSequence(ProcEvent_ClosedUI);
		}
	}

	public bool IsClosedSequenceStart()
	{
		if (m_SequenceStartEnd == null)
		{
			return true;
		}
		return !m_SequenceStartEnd.gameObject.activeSelf;
	}

	public bool IsClosedSequenceEnd()
	{
		return !m_isActiveSequenceResult;
	}

	private void ProcEvent_ClosedUI(object sender, object arg)
	{
		if (sender is SequenceStartEnd && (SequenceStartEnd)sender == m_SequenceStartEnd && m_SequenceStartEnd != null)
		{
			StartCoroutine(m_SequenceResultRank.Show(ProcEvent_ClosedUI));
		}
		else if (sender is SequenceResultRank && (SequenceResultRank)sender == m_SequenceResultRank && m_SequenceResultRank != null)
		{
			StartCoroutine(m_TalkCutResult.Show(TalkCutResult.Mode.SurvivorResult, ProcEvent_ClosedUI));
		}
		else if (sender is TalkCutResult && (TalkCutResult)sender == m_TalkCutResult && m_TalkCutResult != null)
		{
			m_isActiveSequenceResult = false;
		}
		else
		{
			m_isActiveSequenceResult = false;
		}
	}

	public void ShowTalkCutStart()
	{
		if (!(m_TalkCutStart == null))
		{
			m_TalkCutStart.Show();
		}
	}

	public bool IsClosedTalkCutStart()
	{
		return !(m_TalkCutStart != null) || !m_TalkCutStart.gameObject.activeSelf;
	}

	public void ShowTalkCutResult()
	{
		if (!(m_TalkCutResult == null))
		{
			m_isShowingTalkCutResult = true;
			StartCoroutine(m_TalkCutResult.Show(TalkCutResult.Mode.TalkCutResult, OnClosed_TalkCutResult));
		}
	}

	private void OnClosed_TalkCutResult(object sender, object arg)
	{
		m_isShowingTalkCutResult = false;
	}

	public bool IsClosedTalkCutResult()
	{
		return !m_isShowingTalkCutResult;
	}

	public void PlayCharMental(GameObject goChar, int iRelation)
	{
		bool flag = iRelation > 0;
		m_goMentalPlay = ((!flag) ? m_goMentalDown : m_goMentalUp);
		m_animMentalPlay = ((!flag) ? m_animMentalDown : m_animMentalUp);
		float xlsProgramDefineStrToFloat = GameGlobalUtil.GetXlsProgramDefineStrToFloat("RELATION_DELTA_SMALL_BOUND");
		float xlsProgramDefineStrToFloat2 = GameGlobalUtil.GetXlsProgramDefineStrToFloat("RELATION_DELTA_MIDIUM_BOUND");
		int num = Mathf.Abs(iRelation);
		int num2 = 0;
		num2 = ((!((float)num <= xlsProgramDefineStrToFloat)) ? (((float)num <= xlsProgramDefineStrToFloat2) ? 1 : 2) : 0);
		if (flag)
		{
			m_imgMentalUp.sprite = m_imgUpLevel[num2].sprite;
		}
		else
		{
			m_imgMentalDown.sprite = m_imgDownLevel[num2].sprite;
		}
		RectTransform component = m_EventEngine.GetEventCanvas().GetComponent<RectTransform>();
		Vector2 canvViewPosByWorldPos = m_GameSwitch.GetCanvViewPosByWorldPos(Camera.main, component, goChar.transform.position);
		RectTransform component2 = m_goMentalPlay.GetComponent<RectTransform>();
		component2.anchoredPosition = new Vector2(canvViewPosByWorldPos.x, component2.anchoredPosition.y);
		m_isShowRelationiTuto = m_GameSwitch.GetTutorial("tuto_00008") == 0;
		StartCharMental();
	}

	public bool ProcCharMental()
	{
		if (m_isShowRelationiTuto && !TutorialPopup.IsTutorialPopupEnd())
		{
			return false;
		}
		if (m_animMentalPlay == null)
		{
			return true;
		}
		m_animMentalPlay.speed = m_EventEngine.GetRelationAnimatorSkipValue();
		GameGlobalUtil.CheckPlayEndUIAnimation(m_animMentalPlay, GameDefine.UIAnimationState.idle2, ref m_eChgStateChrRelation);
		if (m_eChgStateChrRelation == GameDefine.eAnimChangeState.play_end)
		{
			m_eChgStateChrRelation = GameDefine.eAnimChangeState.none;
			m_goMentalPlay.SetActive(value: false);
			if (!m_isShowRelationiTuto)
			{
				return true;
			}
			if (!m_isRunTuto)
			{
				m_isRunTuto = true;
				bool flag = TutorialPopup.isShowAble("tuto_00008");
				if (flag)
				{
					StartCoroutine(TutorialPopup.Show("tuto_00008", cbFcCharMentalTutorialExit, m_canvasMentalPlay));
				}
				return !flag;
			}
			return m_isRunTuto && m_isShowRelationiTuto && TutorialPopup.IsTutorialPopupEnd();
		}
		return false;
	}

	private void StartCharMental()
	{
		m_goMentalPlay.SetActive(value: true);
		GameGlobalUtil.PlayUIAnimation(m_animMentalPlay, GameDefine.UIAnimationState.idle2, ref m_eChgStateChrRelation);
	}

	private void cbFcCharMentalTutorialExit(object sender, object arg)
	{
	}

	public SWSub_MemoMenu GetSmartWatchSubMemoMenu()
	{
		return m_SmartWatchRoot.GetMemoMenu();
	}

	public bool GetIsZoomingMenu()
	{
		return m_isZoomingMenu;
	}

	private void InitValue()
	{
		m_fMoveFov = GameGlobalUtil.GetXlsProgramDefineStrToFloat("GAME_MENU_ENTER_ZOOM_FOV");
		m_fMoveFovTime = GameGlobalUtil.GetXlsProgramDefineStrToFloat("GAME_MENU_ENTER_ZOOM_TIME");
		m_strSpeed = GameGlobalUtil.GetXlsProgramDefineStr("GAME_MENU_ENTER_ZOOM_SPEED");
	}

	private void ShowEnterMenuZoom(CallBackEndMenuZoom cbEndMenuZoom = null)
	{
		m_isZoomingMenu = true;
		m_fBefFov = Camera.main.fieldOfView;
		m_EventEngine.m_EventCamera.SetFov(m_fBefFov - m_fMoveFov, m_fMoveFovTime, m_strSpeed);
		m_cbEndMenuZoom = cbEndMenuZoom;
	}

	public void EndEnterMenuZoomBack()
	{
		m_isZoomingMenu = true;
		m_EventEngine.m_EventCamera.SetFov(m_fBefFov, m_fMoveFovTime, m_strSpeed);
		m_cbEndMenuZoom = EndMenuGoDefScene;
	}

	private void EndMenuGoDefScene()
	{
		SetGameMainState(eGameMainState.LoadDefScene);
	}

	private void UpdateEnterMenuZoom()
	{
		if (m_EventEngine.m_EventCamera.ProcFov())
		{
			EndEnterMenuZoom();
		}
	}

	private void EndEnterMenuZoom()
	{
		if (m_cbEndMenuZoom != null)
		{
			m_cbEndMenuZoom();
			m_isZoomingMenu = false;
		}
	}

	public void DialogEndOn(bool isShow)
	{
		if (isShow)
		{
			if (!IsCheckDialogEndOn())
			{
				m_DialogEnd.gameObject.SetActive(value: true);
			}
		}
		else
		{
			m_DialogEnd.gameObject.SetActive(value: false);
		}
	}

	public bool IsCheckDialogEndOn()
	{
		return m_DialogEnd.gameObject.activeInHierarchy;
	}
}
