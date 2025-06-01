using System.Collections.Generic;
using System.Globalization;
using GameData;
using GameEvent;
using UnityEngine;

public class PMEventScript : MonoBehaviour
{
	public enum eScriptFunc
	{
		NONE = -1,
		CAMERA_MOVE,
		CAMERA_MOVE_RELATIVE,
		CAMERA_MOVE_OBJ,
		CAMERA_PRESET_MOVE_AND_ROTATE,
		CAMERA_PRESET_MOVE,
		CAMERA_PRESET_ROTATE,
		CAMERA_VIEW,
		CAMERA_VIEW_AND_FOV,
		CAMERA_ROTATE,
		CAMERA_DOLLYZOOM,
		CAMERA_FOV,
		CHAR_MOVE,
		CHAR_MOVE_DETAIL,
		CHAR_MOVE_REVERT,
		CHAR_OUT,
		CHAR_ZOOM,
		CHAR_ESCAPE,
		CHAR_ESCAPE_ALL,
		CHAR_ROTATE,
		CHAR_ROTATE_REVERT,
		CHAR_CREATE,
		DELAY_TIME,
		DELAY_BUTTON,
		EVT_CHAR_OBJ_CREATE,
		EVT_OBJ_CREATE,
		EVT_OBJ_MOVE,
		EVT_OBJ_PLAY_MOTION,
		EVT_OBJ_ZOOM,
		EVT_OBJ_DELETE,
		EVT_OBJ_ROTATE,
		EVT_OBJ_REVERT_Y,
		EVT_OBJ_SHOW_COLLECT_IMG,
		EVT_OBJ_HIDE_COLLECT_IMG,
		PRINT_TALK,
		PRINT_TALK_WITHOUT_NAME,
		PRINT_TALK_WITHOUT_NAME_FORCE,
		PRINT_TALK_SHUTUP,
		PRINT_TALK_FORCE,
		PRINT_NARRATION,
		PRINT_NARRATION_FORCE,
		PRINT_NARRATION_ONCE,
		PRINT_STACK_ALL,
		CONVERSATION_SIGN_KEYWORD,
		CONVERSATION_SIGN_PROFILE,
		CONVERSATION_SIGN_OFF,
		SET_SELECT,
		SWITCH_MENTAL_FULLPOWER,
		SWITCH_MENTAL_ADD,
		SWITCH_KEYWORD_POP,
		SWITCH_PROFILE_POP,
		THREAD_CLOSE,
		THREAD_OPEN,
		TRANS_FADE_IN,
		TRANS_FADE_OUT,
		TRANS_COVER_IN,
		TRANS_COVER_OUT,
		TRANS_CIRCLE_IN,
		TRANS_CIRCLE_OUT,
		TRANS_FLASH_IN,
		TRANS_FLASH_OUT,
		TRANS_COVER_BRUSH_IN,
		TRANS_COVER_BRUSH_OUT,
		TRANS_COVER_FLAT_IN,
		TRANS_COVER_FLAT_OUT,
		BLUR_GAUSSIAN_IN,
		BLUR_GAUSSIAN_OUT,
		BLUR_RADIAL_IN,
		BLUR_RADIAL_OUT,
		LINE_BG_STREAM_IN,
		LINE_BG_STREAM_OUT,
		LINE_FOCUSLINE_IN,
		LINE_FOCUSLINE_OUT,
		LINE_HFOCUSLINE_IN,
		LINE_HFOCUSLINE_OUT,
		EFFECT_DOUBLE_IN,
		EFFECT_DOUBLE_OUT,
		EFFECT_GLITCH_IN,
		EFFECT_GLITCH_OUT,
		EFFECT_WIGGLE_IN,
		EFFECT_WIGGLE_OUT,
		EFFECT_TV_IN,
		EFFECT_TV_OUT,
		EFFECT_SCREENOVERLAY_IN,
		EFFECT_SCREENOVERLAY_OUT,
		EFFECT_CAMERA_FLARE_IN,
		EFFECT_CAMERA_FLARE_OUT,
		EFFECT_FALLING_STONE,
		EFFECT_FALLING_STONE_BLUR,
		EFFECT_EYE_OPEN_IN,
		EFFECT_EYE_OPEN_OUT,
		EFFECT_COVER_CONCRETE_IN,
		EFFECT_COVER_CONCRETE_OUT,
		EFFECT_COVER_CONCRETE_BLUR_IN,
		EFFECT_COVER_CONCRETE_BLUR_OUT,
		EFFECT_FLAT_BLUR_IN,
		EFFECT_FLAT_BLUR_OUT,
		COLOR_GRAY_IN,
		COLOR_GRAY_OUT,
		COLOR_NEGATIVE_IN,
		COLOR_NEGATIVE_OUT,
		COLOR_CURVE_IN,
		COLOR_CURVE_OUT,
		VIDEO_PLAY,
		SET_KEYWORD_SEL,
		SET_KEYWORD_CHAR_MOVE,
		FATER_OPEN_REPLY,
		FATER_OPEN_KEYWORD,
		FATER_FLOATING_ON,
		FATER_FLOATING_OFF,
		MSG_OPEN,
		MSG_FLOATING_ON,
		MSG_FLOATING_OFF,
		MSG_FLOATING_DELAY_ON,
		MSG_FLOATING_DELAY_CANCEL,
		FLOATING_EVENT_DISAPPEAR,
		FLOATING_EVENT_MOVE,
		FLOATING_EVENT_ROTATE,
		FLOATING_EVENT_ZOOM,
		FLOATING_EVENT_MOTION,
		GOTO_GAME_SCENE_SHOW_PARTY,
		SET_KEYWORD_USE,
		SET_KEYWORD_REUSE,
		SET_KEYWORD_EXPLAIN_ONOFF,
		SET_KEYWORD_EXPLAIN_STATE,
		SET_KEYWORD_EXPLAIN_OX_MOT,
		SHOW_SEQUENCE_START,
		SHOW_SEQUENCE_END,
		SHOW_TALK_START,
		SHOW_TALK_RESULT,
		CALL_FINISH,
		CALL_BG_ON,
		CALL_BG_OFF,
		CALL_SENDING_FOR_SMARTPHONE,
		CALL_ENGAGE_FOR_SMARTPHONE,
		CALL_FINISH_FOR_SMARTPHONE,
		CALL_HIDE,
		CALL_REAPPEARANCE,
		ADD_CHAR_RELATION,
		SAVE_START_OBJ,
		SAVE_COLLECTION,
		LOAD_GAME,
		SPLIT_SCREEN_REACTION,
		STAFF_ROLL,
		TUTORIAL_POP,
		INTRODUCE_CHAR_APPEAR,
		INTRODUCE_CHAR_DISAPPEAR,
		BROADCAST_TITLE_APPEAR,
		BROADCAST_TITLE_DISPPEAR,
		POPUP,
		ACTION_POPUP,
		CHAR_LUX_IN,
		CHAR_LUX_OUT,
		OBJ_LUX_IN,
		OBJ_LUX_OUT,
		SPOT_LOADING_SCREEN,
		SPOT_LOAD_CLOSING_SCREEN,
		REFRESH_FIND_MARKER,
		VITA_DISTANT_VIEW_ON,
		VITA_DISTANT_VIEW_MOVE,
		VITA_DISTANT_VIEW_ROTATE,
		VITA_DISTANT_VIEW_ZOOM,
		ADD_FATER_CONTENT
	}

	private readonly int MAX_OBJ_CNT = 200;

	private List<eScriptFunc> m_listEventFunc;

	private bool m_isThreadOpen;

	private readonly int MAX_CHECK_SWITCH_CNT = 100;

	private int[,] m_iCheckSwitchIdx;

	private static int CHECK_KEYWORD_CNT = 10;

	private string[] m_strCheckKeywordUse = new string[CHECK_KEYWORD_CNT];

	private bool[] m_isCheckTrue;

	private int m_iCheckCntIdx;

	private bool[] m_isarrSwitch;

	private bool m_isRunConti;

	private bool m_isKRunConti;

	private bool m_isRunKRelationConti;

	private string m_strKEndEvent;

	private string m_strKArg;

	private bool m_isBackGoKeywordMenu;

	private ConstGameSwitch.eSELECT m_eSelSelect;

	private bool m_isGoToGameSceneSetFadeOut;

	private string m_strLoadLevel;

	private string[] m_strLoadAddLevel = new string[3];

	private bool m_isHideEffect_Ing;

	private bool m_isCallReappearance_Ing;

	private bool m_isSpotLoadingScreen;

	private bool m_isSpotLoadClosingScreen;

	private GameSwitch m_GameSwitch;

	private EventEngine m_EventEngine;

	private GameMain m_GameMain;

	public AudioManager m_AudioManager;

	private SaveLoad m_SaveLoad;

	public GameObject m_goPlayMakerObj;

	public GameObject m_goEventCanvasEff;

	public GameObject m_goEventCanvas;

	public GameObject m_goEachSceneEventObj;

	private bool m_isCheckBefScript;

	private string[] m_strKSelEndEvt = new string[4];

	private string[] m_strKSelArg = new string[4];

	private string m_strKSelID;

	private float m_fKSelTime;

	private string m_strPhoneConti;

	private int m_iBefKeywordPoint;

	private bool m_isPlayKeywordPoint;

	private float m_fRefreshFindMarkerDelayTime = 0.5f;

	private float m_fRefreshTime;

	private bool m_isActivatedSNSMenu;

	private bool m_isSaveStartObjComp;

	private bool m_isLoadGame;

	private void OnEnable()
	{
		m_GameSwitch = GameSwitch.GetInstance();
		m_EventEngine = EventEngine.GetInstance();
		m_EventEngine.SetEventCanvas(m_goEventCanvasEff, m_goEventCanvas);
		m_EventEngine.SetPMObjParent(m_goPlayMakerObj, m_goEachSceneEventObj);
		m_GameMain = m_EventEngine.m_GameMain;
		m_AudioManager = AudioManager.instance;
		m_SaveLoad = SaveLoad.GetInstance();
		NewScriptValue();
	}

	private void OnDisable()
	{
		BacklogDataManager.ClearBacklogDatas();
		if (m_listEventFunc != null)
		{
			m_listEventFunc.Clear();
		}
		m_listEventFunc = null;
		m_iCheckSwitchIdx = null;
		m_isCheckTrue = null;
		m_isarrSwitch = null;
		m_strKEndEvent = null;
		m_strKArg = null;
		m_strLoadLevel = null;
		m_GameSwitch = null;
		m_EventEngine = null;
		m_GameMain = null;
		m_AudioManager = null;
		m_SaveLoad = null;
	}

	private void Update()
	{
		if (!base.gameObject.activeSelf || m_GameSwitch == null)
		{
			return;
		}
		bool flag = false;
		if (m_isRunConti || m_isKRunConti)
		{
			if (ContiDataHandler.UpdateContiProc())
			{
				OnScreenLog.SetContiName(null);
				if (m_isRunConti)
				{
					SetRunConti(isSet: false);
					CallEventScriptEndForConti();
				}
				else
				{
					m_isKRunConti = false;
					CallEndForKRunConti();
				}
				BacklogDataManager.ClearBacklogDatas();
			}
		}
		else
		{
			ProcScript();
		}
		m_EventEngine.ProcSetEvent();
	}

	private void LateUpdate()
	{
		RenderManager.instance.Render();
	}

	private void SetRunConti(bool isSet)
	{
		m_isRunConti = isSet;
	}

	private void CheckBeforeScriptInit(bool isInitAll)
	{
		if (isInitAll)
		{
			bool flag = true;
			flag = m_GameMain != null && m_GameMain.IsClosedKeywordMenu();
			if (!flag || (flag && !m_GameSwitch.IsExistPartyIn()))
			{
				m_EventEngine.m_TalkChar.CharAllHide();
			}
		}
		CheckDialogue();
	}

	private void CheckDialogue()
	{
		bool flag = false;
		int count = m_listEventFunc.Count;
		for (int i = 0; i < count; i++)
		{
			eScriptFunc eScriptFunc = m_listEventFunc[i];
			if (eScriptFunc >= eScriptFunc.PRINT_TALK && eScriptFunc <= eScriptFunc.PRINT_STACK_ALL)
			{
				flag = true;
				break;
			}
		}
		if (!flag && Event_TalkDialogue.instance != null && Event_TalkDialogue.instance.m_isOpenPrintWindow)
		{
			Event_TalkDialogue.instance.InitTalkWindow();
		}
	}

	private void InitSpecialCharScriptIntArr(int[,] iArrA, int iMultiFirstIdx, int iSpecialChar)
	{
		for (int i = 0; i < MAX_CHECK_SWITCH_CNT; i++)
		{
			iArrA[iMultiFirstIdx, i] = iSpecialChar;
		}
	}

	private void InitSpecialCharScriptBoolArr(bool[] isArrA, bool isTrue)
	{
		for (int i = 0; i < MAX_CHECK_SWITCH_CNT; i++)
		{
			isArrA[i] = isTrue;
		}
	}

	private int SetStringNumberArgument(string strArgument)
	{
		InitSpecialCharScriptIntArr(m_iCheckSwitchIdx, m_iCheckCntIdx, -1);
		m_isCheckTrue[m_iCheckCntIdx] = true;
		char[] array = strArgument.ToCharArray();
		char[] array2 = new char[10];
		int num = array.Length;
		int num2 = 0;
		int num3 = 0;
		bool flag = false;
		int iCheckCntIdx = m_iCheckCntIdx;
		if (array[0] == '-')
		{
			m_isCheckTrue[m_iCheckCntIdx] = false;
		}
		for (int i = 0; i < num; i++)
		{
			if ((array[i] >= '0' && array[i] <= '9') || array[i] == '-')
			{
				array2[num3++] = array[i];
				if (i == num - 1 || array[i] == '-')
				{
					flag = true;
				}
			}
			else if (array[i] == ':' || array[i] == ',' || array[i] == '\0')
			{
				flag = true;
			}
			if (flag)
			{
				array2[num3++] = '\0';
				m_iCheckSwitchIdx[m_iCheckCntIdx, num2++] = int.Parse(new string(array2));
				num3 = 0;
				if (num2 >= MAX_CHECK_SWITCH_CNT)
				{
					break;
				}
				flag = false;
			}
		}
		m_iCheckCntIdx++;
		return iCheckCntIdx;
	}

	private void PushThreadFunc(eScriptFunc eScrFunc)
	{
		m_isCheckBefScript = false;
		switch (eScrFunc)
		{
		case eScriptFunc.THREAD_OPEN:
			m_isThreadOpen = true;
			break;
		case eScriptFunc.THREAD_CLOSE:
			m_isThreadOpen = false;
			break;
		default:
			AddThreadFunc(eScrFunc);
			break;
		}
	}

	private void NewScriptValue()
	{
		m_listEventFunc = new List<eScriptFunc>();
		m_iCheckSwitchIdx = new int[MAX_CHECK_SWITCH_CNT, MAX_CHECK_SWITCH_CNT];
		m_isCheckTrue = new bool[MAX_CHECK_SWITCH_CNT];
		m_isarrSwitch = new bool[MAX_OBJ_CNT];
		m_iCheckCntIdx = 0;
		SetRunConti(isSet: false);
		m_isKRunConti = false;
		m_isRunKRelationConti = false;
		InitScriptValue();
	}

	private void InitScriptValue()
	{
		for (int i = 0; i < MAX_CHECK_SWITCH_CNT; i++)
		{
			InitSpecialCharScriptIntArr(m_iCheckSwitchIdx, i, -1);
		}
		InitSpecialCharScriptBoolArr(m_isCheckTrue, isTrue: true);
		m_isarrSwitch.Initialize();
		m_listEventFunc.Clear();
		m_isThreadOpen = false;
		m_iCheckCntIdx = 0;
	}

	private void AddThreadFunc(eScriptFunc eAddScrFunc)
	{
		if (m_GameMain != null && m_GameMain.GetGameMainState() != GameMain.eGameMainState.RunEvent && eAddScrFunc != eScriptFunc.SET_SELECT && eAddScrFunc != eScriptFunc.SET_KEYWORD_SEL && eAddScrFunc != eScriptFunc.SET_KEYWORD_USE && eAddScrFunc != eScriptFunc.SET_KEYWORD_REUSE)
		{
			m_GameMain.SetGameMainState(GameMain.eGameMainState.RunEvent);
		}
		int count = m_listEventFunc.Count;
		for (int i = 0; i < count; i++)
		{
			if (eAddScrFunc == m_listEventFunc[i])
			{
				return;
			}
		}
		m_listEventFunc.Add(eAddScrFunc);
	}

	public bool ProcScript()
	{
		bool flag = true;
		bool flag2 = true;
		eScriptFunc eScriptFunc = eScriptFunc.NONE;
		int count = m_listEventFunc.Count;
		if (count < 1)
		{
			return true;
		}
		if (m_isThreadOpen)
		{
			return true;
		}
		if (!m_isCheckBefScript)
		{
			CheckBeforeScriptInit(isInitAll: false);
			m_isCheckBefScript = true;
		}
		int count2 = m_listEventFunc.Count;
		for (int i = 0; i < count2; i++)
		{
			eScriptFunc eScriptFunc2 = m_listEventFunc[i];
			switch (eScriptFunc2)
			{
			case eScriptFunc.PRINT_TALK:
			case eScriptFunc.PRINT_TALK_WITHOUT_NAME:
			case eScriptFunc.PRINT_TALK_SHUTUP:
			case eScriptFunc.PRINT_NARRATION:
			case eScriptFunc.PRINT_STACK_ALL:
				if (Event_TalkDialogue.instance != null)
				{
					flag2 = Event_TalkDialogue.instance.ProcPrintTalk();
				}
				break;
			case eScriptFunc.PRINT_TALK_WITHOUT_NAME_FORCE:
			case eScriptFunc.PRINT_TALK_FORCE:
			case eScriptFunc.PRINT_NARRATION_FORCE:
				if (Event_TalkDialogue.instance != null)
				{
					flag2 = Event_TalkDialogue.instance.ProcPrintForceTalk();
				}
				break;
			case eScriptFunc.PRINT_NARRATION_ONCE:
				if (Event_TalkDialogue.instance != null)
				{
					flag2 = Event_TalkDialogue.instance.ProcNarrationOnce();
				}
				break;
			case eScriptFunc.CONVERSATION_SIGN_KEYWORD:
			case eScriptFunc.CONVERSATION_SIGN_PROFILE:
			case eScriptFunc.CONVERSATION_SIGN_OFF:
				flag2 = m_GameMain.m_clConversationSign.ProcCoversationSign();
				break;
			case eScriptFunc.CHAR_MOVE:
			case eScriptFunc.CHAR_MOVE_DETAIL:
			case eScriptFunc.CHAR_MOVE_REVERT:
			case eScriptFunc.CHAR_OUT:
				flag2 = m_EventEngine.m_TalkChar.ProcCharMove();
				break;
			case eScriptFunc.CHAR_ESCAPE:
			case eScriptFunc.CHAR_ESCAPE_ALL:
				flag2 = m_EventEngine.m_TalkChar.ProcCharMove(isMoveDoneDel: true);
				break;
			case eScriptFunc.CHAR_ROTATE:
			case eScriptFunc.CHAR_ROTATE_REVERT:
				flag2 = m_EventEngine.m_TalkChar.ProcCharRotate();
				break;
			case eScriptFunc.CHAR_CREATE:
				flag2 = m_EventEngine.m_TalkChar.IsCompCreateChar();
				break;
			case eScriptFunc.CHAR_ZOOM:
				flag2 = m_EventEngine.m_TalkChar.ProcCharZoom();
				break;
			case eScriptFunc.CAMERA_PRESET_MOVE_AND_ROTATE:
			case eScriptFunc.CAMERA_PRESET_MOVE:
			case eScriptFunc.CAMERA_PRESET_ROTATE:
				flag2 = m_EventEngine.m_EventCamera.ProcCameraMoveAndRotate();
				break;
			case eScriptFunc.CAMERA_VIEW:
				flag2 = m_EventEngine.m_EventCamera.ProcCameraView();
				break;
			case eScriptFunc.CAMERA_VIEW_AND_FOV:
				flag2 = m_EventEngine.m_EventCamera.ProcCameraView() & m_EventEngine.m_EventCamera.ProcFov();
				break;
			case eScriptFunc.CAMERA_MOVE:
			case eScriptFunc.CAMERA_MOVE_RELATIVE:
			case eScriptFunc.CAMERA_MOVE_OBJ:
				flag2 = m_EventEngine.m_EventCamera.ProcCameraMoveAndView();
				break;
			case eScriptFunc.CAMERA_ROTATE:
				flag2 = m_EventEngine.m_EventCamera.ProcRotate();
				break;
			case eScriptFunc.CAMERA_DOLLYZOOM:
				flag2 = m_EventEngine.m_EventCamera.ProcDollyZoom();
				break;
			case eScriptFunc.CAMERA_FOV:
				flag2 = m_EventEngine.m_EventCamera.ProcFov();
				break;
			case eScriptFunc.SET_SELECT:
			case eScriptFunc.SET_KEYWORD_SEL:
				flag2 = m_GameMain.IsClosedEventSelect();
				if (flag2)
				{
					eScriptFunc = eScriptFunc2;
					m_EventEngine.SetShowSkipBut(isShowSkipBut: true);
				}
				break;
			case eScriptFunc.SET_KEYWORD_CHAR_MOVE:
				flag2 = m_EventEngine.m_TalkChar.ProcCharMove();
				break;
			case eScriptFunc.DELAY_TIME:
				flag2 = m_EventEngine.ProcDelayTime();
				break;
			case eScriptFunc.DELAY_BUTTON:
				flag2 = false;
				if (m_EventEngine.ProcDelayTime())
				{
					if (!m_GameMain.IsCheckDialogEndOn())
					{
						m_GameMain.DialogEndOn(isShow: true);
					}
					if (m_GameMain.IsCheckDialogEndOn() && m_GameMain.m_DialogEnd.IsProcDialogEnd())
					{
						m_GameMain.DialogEndOn(isShow: false);
						flag2 = true;
					}
				}
				break;
			case eScriptFunc.EVT_CHAR_OBJ_CREATE:
				flag2 = m_EventEngine.m_EventObject.IsCharObjectCreateComp();
				break;
			case eScriptFunc.EVT_OBJ_CREATE:
				flag2 = m_EventEngine.m_EventObject.IsObjCreateComp();
				goto IL_0588;
			case eScriptFunc.EVT_OBJ_DELETE:
				flag2 = m_EventEngine.m_EventObject.IsObjDeleteComp();
				goto IL_0588;
			case eScriptFunc.EVT_OBJ_MOVE:
				flag2 = m_EventEngine.m_EventObject.IsObjectMoveSettingComp();
				if (flag2)
				{
					flag2 = m_EventEngine.m_EventObject.ProcObjMove();
				}
				break;
			case eScriptFunc.EVT_OBJ_PLAY_MOTION:
				flag2 = m_EventEngine.m_EventObject.IsObjPlayAnimationSettingComp();
				if (flag2)
				{
					flag2 = m_EventEngine.m_EventObject.ProcDelayAnimaton();
				}
				break;
			case eScriptFunc.EVT_OBJ_ROTATE:
				flag2 = m_EventEngine.m_EventObject.IsObjRotateSettingComp();
				if (flag2)
				{
					flag2 = m_EventEngine.m_EventObject.ProcRotate();
				}
				break;
			case eScriptFunc.EVT_OBJ_REVERT_Y:
				flag2 = m_EventEngine.m_EventObject.IsObjRevertYComp();
				break;
			case eScriptFunc.EVT_OBJ_ZOOM:
				flag2 = m_EventEngine.m_EventObject.IsObjZoomSettingComp();
				if (flag2)
				{
					flag2 = m_EventEngine.m_EventObject.ProcObjZoom();
				}
				break;
			case eScriptFunc.EVT_OBJ_SHOW_COLLECT_IMG:
			case eScriptFunc.EVT_OBJ_HIDE_COLLECT_IMG:
				flag2 = m_EventEngine.m_EventObject.IsCompleteCIImage();
				break;
			case eScriptFunc.SWITCH_MENTAL_FULLPOWER:
			case eScriptFunc.SWITCH_MENTAL_ADD:
				flag2 = m_GameMain.m_gameMainMenu.IsCompleteMentalGageChange();
				if (flag2)
				{
					m_GameMain.ShowInGameMainUI(isShow: false, GameMain.eGameMainUI.Mental);
				}
				break;
			case eScriptFunc.SWITCH_KEYWORD_POP:
				flag2 = KeywordGetPopupPlus.IsInactivated();
				if (!flag2)
				{
					break;
				}
				if (!m_isPlayKeywordPoint)
				{
					if (m_GameSwitch.GetMustGetKeywordCnt() != m_iBefKeywordPoint)
					{
						AudioManager.instance.PlayUISound("View_KeyCount");
						m_GameMain.m_gameMainMenu.PlayAddPointAnim(m_iBefKeywordPoint, m_GameSwitch.GetMustGetKeywordCnt());
						m_isPlayKeywordPoint = true;
						flag2 = false;
					}
				}
				else
				{
					flag2 = m_GameMain.m_gameMainMenu.ProcAddPointAnim();
				}
				break;
			case eScriptFunc.SWITCH_PROFILE_POP:
				flag2 = ProfileGetPopup.IsProfilePopupEnd();
				break;
			case eScriptFunc.TRANS_FADE_IN:
			case eScriptFunc.TRANS_FADE_OUT:
			case eScriptFunc.TRANS_COVER_IN:
			case eScriptFunc.TRANS_COVER_OUT:
			case eScriptFunc.TRANS_CIRCLE_IN:
			case eScriptFunc.TRANS_CIRCLE_OUT:
			case eScriptFunc.TRANS_COVER_BRUSH_IN:
			case eScriptFunc.TRANS_COVER_BRUSH_OUT:
			case eScriptFunc.TRANS_COVER_FLAT_IN:
			case eScriptFunc.TRANS_COVER_FLAT_OUT:
			case eScriptFunc.EFFECT_EYE_OPEN_IN:
			case eScriptFunc.EFFECT_EYE_OPEN_OUT:
			case eScriptFunc.EFFECT_COVER_CONCRETE_IN:
			case eScriptFunc.EFFECT_COVER_CONCRETE_OUT:
			case eScriptFunc.EFFECT_COVER_CONCRETE_BLUR_IN:
			case eScriptFunc.EFFECT_COVER_CONCRETE_BLUR_OUT:
			case eScriptFunc.EFFECT_FLAT_BLUR_IN:
			case eScriptFunc.EFFECT_FLAT_BLUR_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.Cover);
				break;
			case eScriptFunc.TRANS_FLASH_IN:
			case eScriptFunc.TRANS_FLASH_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.Flash);
				break;
			case eScriptFunc.BLUR_GAUSSIAN_IN:
			case eScriptFunc.BLUR_GAUSSIAN_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.GaussianBlur);
				break;
			case eScriptFunc.BLUR_RADIAL_IN:
			case eScriptFunc.BLUR_RADIAL_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.RadialBlur);
				break;
			case eScriptFunc.LINE_BG_STREAM_IN:
			case eScriptFunc.LINE_BG_STREAM_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.BackgroundStream);
				break;
			case eScriptFunc.LINE_FOCUSLINE_IN:
			case eScriptFunc.LINE_FOCUSLINE_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.FocusLine);
				break;
			case eScriptFunc.LINE_HFOCUSLINE_IN:
			case eScriptFunc.LINE_HFOCUSLINE_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.FocusLine_Hori);
				break;
			case eScriptFunc.EFFECT_DOUBLE_IN:
			case eScriptFunc.EFFECT_DOUBLE_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.DoubleVision);
				break;
			case eScriptFunc.EFFECT_GLITCH_IN:
			case eScriptFunc.EFFECT_GLITCH_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.Glitch);
				break;
			case eScriptFunc.EFFECT_WIGGLE_IN:
			case eScriptFunc.EFFECT_WIGGLE_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.Wiggle);
				break;
			case eScriptFunc.EFFECT_TV_IN:
			case eScriptFunc.EFFECT_TV_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.AnalogTV);
				break;
			case eScriptFunc.EFFECT_SCREENOVERLAY_IN:
			case eScriptFunc.EFFECT_SCREENOVERLAY_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.ScreenOverlay);
				break;
			case eScriptFunc.EFFECT_CAMERA_FLARE_IN:
			case eScriptFunc.EFFECT_CAMERA_FLARE_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.CameraFlare);
				break;
			case eScriptFunc.EFFECT_FALLING_STONE:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.FallingStone);
				break;
			case eScriptFunc.EFFECT_FALLING_STONE_BLUR:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.FallingStoneBlur);
				break;
			case eScriptFunc.COLOR_GRAY_IN:
			case eScriptFunc.COLOR_GRAY_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.GrayScale);
				break;
			case eScriptFunc.COLOR_NEGATIVE_IN:
			case eScriptFunc.COLOR_NEGATIVE_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.Negative);
				break;
			case eScriptFunc.COLOR_CURVE_IN:
			case eScriptFunc.COLOR_CURVE_OUT:
				flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.ColorCurve);
				break;
			case eScriptFunc.VIDEO_PLAY:
				flag2 = m_AudioManager.IsPlayingVideo();
				break;
			case eScriptFunc.FATER_OPEN_REPLY:
			case eScriptFunc.FATER_OPEN_KEYWORD:
				flag2 = !m_isActivatedSNSMenu;
				break;
			case eScriptFunc.FATER_FLOATING_ON:
			case eScriptFunc.MSG_FLOATING_ON:
				flag2 = FloatingUIHandler.IsCompleteShow();
				break;
			case eScriptFunc.FATER_FLOATING_OFF:
			case eScriptFunc.MSG_FLOATING_OFF:
				flag2 = FloatingUIHandler.IsCompleteHide();
				break;
			case eScriptFunc.MSG_FLOATING_DELAY_ON:
				flag2 = FloatingUIHandler.IsCompleteShow();
				break;
			case eScriptFunc.MSG_FLOATING_DELAY_CANCEL:
				flag2 = FloatingUIHandler.IsCompleteOnlyInput();
				break;
			case eScriptFunc.FLOATING_EVENT_DISAPPEAR:
			case eScriptFunc.FLOATING_EVENT_MOVE:
			case eScriptFunc.FLOATING_EVENT_ROTATE:
			case eScriptFunc.FLOATING_EVENT_ZOOM:
			case eScriptFunc.FLOATING_EVENT_MOTION:
				flag2 = FloatingUIHandler.IsCompleteEvents();
				break;
			case eScriptFunc.GOTO_GAME_SCENE_SHOW_PARTY:
			{
				bool flag3 = false;
				if (m_isGoToGameSceneSetFadeOut)
				{
					flag2 = EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.Cover);
					break;
				}
				if (m_EventEngine.m_TalkChar.ProcShowPartyChar() && !m_isGoToGameSceneSetFadeOut)
				{
					EventCameraEffect.Instance.Activate_FadeOut(1.5f);
					m_isGoToGameSceneSetFadeOut = true;
				}
				flag2 = false;
				break;
			}
			case eScriptFunc.MSG_OPEN:
				flag2 = m_GameMain.IsClosedMessengerMenu();
				break;
			case eScriptFunc.SET_KEYWORD_USE:
			case eScriptFunc.SET_KEYWORD_REUSE:
				flag2 = m_GameMain.IsClosedKeywordMenu();
				if (flag2)
				{
					m_EventEngine.SetShowSkipBut(isShowSkipBut: true);
				}
				break;
			case eScriptFunc.SET_KEYWORD_EXPLAIN_ONOFF:
				flag2 = m_GameMain.m_KeywordUseExplain.ProcCheckEndOrStartAnim();
				break;
			case eScriptFunc.SET_KEYWORD_EXPLAIN_STATE:
				flag2 = m_GameMain.m_KeywordUseExplain.ProcCheckSelAnim();
				break;
			case eScriptFunc.SET_KEYWORD_EXPLAIN_OX_MOT:
				flag2 = m_GameMain.m_KeywordUseExplain.ProcOXMotCheck();
				break;
			case eScriptFunc.SHOW_SEQUENCE_START:
				flag2 = m_GameMain.IsClosedSequenceStart();
				break;
			case eScriptFunc.SHOW_SEQUENCE_END:
				flag2 = m_GameMain.IsClosedSequenceEnd();
				break;
			case eScriptFunc.SHOW_TALK_START:
				flag2 = m_GameMain.IsClosedTalkCutStart();
				break;
			case eScriptFunc.SHOW_TALK_RESULT:
				flag2 = m_GameMain.IsClosedTalkCutResult();
				break;
			case eScriptFunc.CALL_FINISH:
				flag2 = m_EventEngine.ProcDelayTime();
				if (flag2)
				{
					m_GameMain.m_SmartWatchRoot.Close(isPlayUISound: false);
				}
				break;
			case eScriptFunc.CALL_BG_ON:
			case eScriptFunc.CALL_BG_OFF:
				flag2 = m_GameMain.m_SmartWatchRoot.OnProc_IsCompleteCoverTrans();
				break;
			case eScriptFunc.CALL_SENDING_FOR_SMARTPHONE:
			case eScriptFunc.CALL_ENGAGE_FOR_SMARTPHONE:
			case eScriptFunc.CALL_FINISH_FOR_SMARTPHONE:
				flag2 = m_EventEngine.ProcDelayTime();
				break;
			case eScriptFunc.ADD_CHAR_RELATION:
				flag2 = !(m_GameMain != null) || m_GameMain.ProcCharMental();
				break;
			case eScriptFunc.SAVE_START_OBJ:
			case eScriptFunc.SAVE_COLLECTION:
				flag2 = m_isSaveStartObjComp;
				break;
			case eScriptFunc.LOAD_GAME:
				flag2 = m_isLoadGame;
				break;
			case eScriptFunc.SPLIT_SCREEN_REACTION:
				flag2 = SplitScreenReaction.IsEventComplete();
				break;
			case eScriptFunc.STAFF_ROLL:
				if (StaffRoll.instance != null)
				{
					flag2 = StaffRoll.instance.IsProcStaffRoll();
				}
				break;
			case eScriptFunc.TUTORIAL_POP:
				flag2 = TutorialPopup.IsTutorialPopupEnd();
				if (flag2)
				{
					m_EventEngine.SetShowSkipBut(isShowSkipBut: true);
				}
				break;
			case eScriptFunc.BROADCAST_TITLE_APPEAR:
				flag2 = EFF_Broadcast.IsAppearEnd();
				break;
			case eScriptFunc.INTRODUCE_CHAR_APPEAR:
				flag2 = CHR_Introduce.IsAppearEnd();
				break;
			case eScriptFunc.INTRODUCE_CHAR_DISAPPEAR:
				flag2 = CHR_Introduce.IsDisappearEnd();
				break;
			case eScriptFunc.CALL_HIDE:
				flag2 = !m_isHideEffect_Ing;
				break;
			case eScriptFunc.CALL_REAPPEARANCE:
				flag2 = !m_isCallReappearance_Ing;
				break;
			case eScriptFunc.CHAR_LUX_IN:
			case eScriptFunc.CHAR_LUX_OUT:
				flag2 = m_EventEngine.m_TalkChar.ProcCharLux();
				break;
			case eScriptFunc.OBJ_LUX_IN:
			case eScriptFunc.OBJ_LUX_OUT:
				flag2 = m_EventEngine.m_EventObject.IsSetObjLuxSettingComp();
				if (flag2)
				{
					flag2 = m_EventEngine.m_EventObject.ProcObjLux();
				}
				break;
			case eScriptFunc.SPOT_LOADING_SCREEN:
				flag2 = !m_isSpotLoadingScreen;
				break;
			case eScriptFunc.SPOT_LOAD_CLOSING_SCREEN:
				flag2 = !m_isSpotLoadClosingScreen;
				if (flag2)
				{
					m_EventEngine.SetShowSkipBut(isShowSkipBut: true);
				}
				break;
			case eScriptFunc.REFRESH_FIND_MARKER:
				if (m_fRefreshTime < m_fRefreshFindMarkerDelayTime)
				{
					flag2 = false;
					m_fRefreshTime += Time.deltaTime;
				}
				else
				{
					m_EventEngine.m_EventObject.ShowFindMarker();
					flag2 = true;
				}
				break;
			case eScriptFunc.VITA_DISTANT_VIEW_ON:
				flag2 = RenderManager.instance.IsActivatingBGImage();
				break;
			case eScriptFunc.VITA_DISTANT_VIEW_MOVE:
				flag2 = RenderManager.instance.IsCompleteBGImageEvent_Move();
				break;
			case eScriptFunc.VITA_DISTANT_VIEW_ROTATE:
				flag2 = RenderManager.instance.IsCompleteBGImageEvent_Rotate();
				break;
			case eScriptFunc.VITA_DISTANT_VIEW_ZOOM:
				flag2 = RenderManager.instance.IsCompleteBGImageEvent_Zoom();
				break;
			case eScriptFunc.ADD_FATER_CONTENT:
				flag2 = SNSMenuPlus.IsFinishedMoveToAddedContent();
				break;
			default:
				{
					flag2 = true;
					break;
				}
				IL_0588:
				if (flag2)
				{
					flag2 = m_EventEngine.m_EventObject.ProcObjAlpha();
				}
				break;
			}
			flag = flag && flag2;
		}
		if (flag)
		{
			InitScriptValue();
			if (eScriptFunc == eScriptFunc.NONE)
			{
				CallEventScriptEnd();
			}
			else
			{
				CallSelectEndEvent(eScriptFunc);
			}
		}
		return flag;
	}

	private string GetTransitionString(string strKey)
	{
		string result = string.Empty;
		Xls.ProgramDefineStr data_byKey = Xls.ProgramDefineStr.GetData_byKey(strKey);
		if (data_byKey != null)
		{
			result = data_byKey.m_strTxt;
		}
		return result;
	}

	private void CallTrueFalse(bool isTrue)
	{
		InitScriptValue();
		if (!m_isRunConti && !m_isKRunConti)
		{
			if (isTrue)
			{
				PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_TRUE"));
			}
			else
			{
				PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_FALSE"));
			}
		}
	}

	private void CallKeywordUsingCheck(KeywordMenuPlus.ReturnCheckAns returnAns)
	{
		InitScriptValue();
		if (!m_isRunConti && !m_isKRunConti)
		{
			switch (returnAns)
			{
			case KeywordMenuPlus.ReturnCheckAns.WRONG_QUEST_ID:
				PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_NotAsked"));
				break;
			case KeywordMenuPlus.ReturnCheckAns.ANS_ALL_WRONG:
				PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_ElseKeyword"));
				break;
			default:
				PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_CaseKeyword") + (int)returnAns);
				break;
			}
		}
	}

	public void SetSelectResult(ConstGameSwitch.eSELECT eSel)
	{
		m_eSelSelect = eSel;
	}

	private void CallSelectEndEvent(eScriptFunc eFunc)
	{
		if (eFunc == eScriptFunc.SET_SELECT)
		{
			CallSelectEvent(m_eSelSelect);
		}
		else
		{
			CallKSelectEvent(m_eSelSelect);
		}
	}

	private void CallSelectEvent(ConstGameSwitch.eSELECT eSel)
	{
		switch (eSel)
		{
		case ConstGameSwitch.eSELECT.LEFT:
			PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_SEL_LEFT"));
			break;
		case ConstGameSwitch.eSELECT.RIGHT:
			PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_SEL_RIGHT"));
			break;
		default:
			PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_SEL_TIMEOVER"));
			break;
		}
	}

	private void CallKSelectEvent(ConstGameSwitch.eSELECT eSel)
	{
		int num = 0;
		num = (int)((eSel != ConstGameSwitch.eSELECT.NONE) ? eSel : ConstGameSwitch.eSELECT.TIME_OVER);
		SetKContiEndEvent(m_strKSelEndEvt[num], m_strKSelArg[num]);
		CallEndForKRunConti();
	}

	private void CallEventScriptEnd(eScriptFunc eScrFunc = eScriptFunc.NONE)
	{
		if (!m_isRunConti && !m_isKRunConti && !m_isThreadOpen && m_listEventFunc.Count < 1)
		{
			PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_EVENT_END"));
		}
	}

	private void CallMentalEndBroadcast(eScriptFunc eScrFunc = eScriptFunc.NONE)
	{
		if (!m_EventEngine.GetDisableMentalZero() && !m_EventEngine.GetRunEndMental())
		{
			if (m_isRunConti || m_isKRunConti)
			{
				ContiDataHandler.FinishContiForced();
			}
			m_isRunConti = (m_isKRunConti = false);
			if (m_isThreadOpen)
			{
				m_isThreadOpen = false;
			}
			if (m_listEventFunc.Count >= 1)
			{
				m_listEventFunc.Clear();
			}
			m_EventEngine.ClearEnableObjList();
			m_EventEngine.StopFSM();
			if (!m_EventEngine.FinishRecordFaterEventCBFunc())
			{
				m_EventEngine.SetRunEndMental(isRun: true);
				PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_END_MENTAL"));
				m_EventEngine.CloseKeywordUseExplain();
			}
			else
			{
				CheckBeforeScriptInit(isInitAll: true);
			}
		}
	}

	public void CallEventScriptEndForConti()
	{
		PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_EVENT_END"));
	}

	public void CallEndForKRunConti()
	{
		Xls.ScriptKeyTextValue scriptKeyTextValue = null;
		if (m_strKEndEvent != null)
		{
			scriptKeyTextValue = Xls.ScriptKeyTextValue.GetData_byKey(m_strKEndEvent);
		}
		if (scriptKeyTextValue == null)
		{
			PlayMakerFSM.BroadcastEvent(GetTransitionString("PLAYMAKER_TRANSITION_EVENT_END"));
		}
		else
		{
			PlayMakerFSM.BroadcastEvent(scriptKeyTextValue.m_strValue);
		}
	}

	public void SetKContiEndEvent(string strEndEvent, string strArg = null)
	{
		m_strKEndEvent = strEndEvent;
		m_strKArg = strArg;
		if (strEndEvent == null && m_GameSwitch.GetRunKeyword() != null)
		{
			m_GameSwitch.SetRunKeyword(null);
		}
	}

	public void SetKContiEndSelect(string strSelID, float fTime, string strLEndEvent, string strLArg, string strREndEvent, string strRArg, string strTimeEndEvent, string strTimeArg)
	{
		m_strKSelID = strSelID;
		m_fKSelTime = fTime;
		SetKContiEndEvent("선택지");
		m_strKSelEndEvt[1] = strLEndEvent;
		m_strKSelArg[1] = strLArg;
		m_strKSelEndEvt[2] = strREndEvent;
		m_strKSelArg[2] = strRArg;
		m_strKSelEndEvt[3] = strTimeEndEvent;
		m_strKSelArg[3] = strTimeArg;
	}

	public string KeywordCharMove()
	{
		PushThreadFunc(eScriptFunc.SET_KEYWORD_CHAR_MOVE);
		m_EventEngine.m_TalkChar.SetRunKeywordCharSet(m_GameSwitch.GetRunKeywordCharKey());
		return m_GameSwitch.GetRunKeywordCharKey();
	}

	public void OpenKSelect()
	{
		PushThreadFunc(eScriptFunc.SET_KEYWORD_SEL);
		m_EventEngine.StopSkip(isShowSkipBut: false, isForce: true);
		m_GameMain.m_clEventSelect.SetSelect(this, m_strKSelID, m_fKSelTime, ConstGameSwitch.eSELECT_TYPE.KEYWORD);
		m_GameSwitch.SetCollSelKeyword(m_GameSwitch.GetCurRunEventKeywordKey());
	}

	public void SetKContiEndEnableObj()
	{
		m_EventEngine.EnableAndRunObj(m_strKArg);
		SetKContiEndEvent(null);
		CallEventScriptEnd();
	}

	public void OpenKeywordMenu()
	{
		m_isBackGoKeywordMenu = true;
		SetKContiEndEvent(null);
		CallEventScriptEnd();
	}

	public void CancelOpenKeywordMenu()
	{
		m_isBackGoKeywordMenu = false;
	}

	public void CallKConti()
	{
		m_isKRunConti = true;
		string text = null;
		text = ((m_GameSwitch.GetRunKeyword() == null) ? m_strKArg : m_GameSwitch.GetRunKeyword());
		SetKContiEndEvent(null);
		if (!ContiDataHandler.ActivateConti(text, this))
		{
			CallEventScriptEnd();
		}
	}

	public void KeywordCheckRelationOpening()
	{
		string text = m_GameSwitch.CheckAndOnCharRelationContiIdx(m_GameSwitch.GetRunKeywordCharKey(), isSetSwitch: false);
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("KEYWORD_CHECK_RELATION_OPENING");
		SetKContiEndEvent(null);
		if (text != null)
		{
			FinishKeywordEventCheck(isReleaseKeywordSet: false);
			m_isKRunConti = ContiDataHandler.IsExistConti(xlsProgramDefineStr);
			if (!ContiDataHandler.ActivateConti(xlsProgramDefineStr, this))
			{
				CallTrueFalse(isTrue: false);
			}
		}
		else
		{
			CallTrueFalse(isTrue: false);
		}
	}

	public void KeywordCheckRelation()
	{
		string text = m_GameSwitch.CheckAndOnCharRelationContiIdx(m_GameSwitch.GetRunKeywordCharKey());
		SetKContiEndEvent(null);
		if (text != null)
		{
			m_isKRunConti = true;
			m_isRunKRelationConti = true;
			ContiDataHandler.ActivateConti(text, this);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	public void CallConti(string strConti)
	{
		if (ContiDataHandler.IsExistConti(strConti))
		{
			SetRunConti(isSet: true);
		}
		if (!ContiDataHandler.ActivateConti(strConti, this))
		{
			CallEventScriptEnd();
		}
	}

	public void CallPhoneConti()
	{
		if (m_strPhoneConti != null)
		{
			SetRunConti(isSet: true);
			CallConti(m_strPhoneConti);
		}
	}

	public void SetGameTime(string strTime)
	{
		SetGameTimeString(strTime);
	}

	public void A_SetGameTime(string strTime)
	{
		SetGameTimeString(strTime);
		CallEventScriptEnd();
	}

	private void SetGameTimeString(string strTime)
	{
		Xls.InGameTime data_byKey = Xls.InGameTime.GetData_byKey(strTime);
		if (data_byKey != null)
		{
			int num = SetStringNumberArgument(data_byKey.m_strInGameTime);
			m_GameSwitch.SetGameTime(m_iCheckSwitchIdx[num, 0], m_iCheckSwitchIdx[num, 1]);
		}
	}

	public void AddGameTime(string strTime)
	{
		int num = SetStringNumberArgument(strTime);
		m_GameSwitch.AddGameTime(m_isCheckTrue[num], m_iCheckSwitchIdx[num, 0], m_iCheckSwitchIdx[num, 1]);
	}

	public void CheckGameTime(bool isOver, string strTime)
	{
		int num = SetStringNumberArgument(strTime);
		bool isTrue = m_GameSwitch.CheckGameTime(isOver, m_iCheckSwitchIdx[num, 0], m_iCheckSwitchIdx[num, 1]);
		CallTrueFalse(isTrue);
	}

	public void SetMental(int iMental)
	{
		m_GameSwitch.SetMental(iMental);
	}

	public void A_SetMental(int iMental)
	{
		m_GameSwitch.SetMental(iMental);
		CallEventScriptEnd();
	}

	public void AddMental(int iAddMental)
	{
		bool flag = m_GameSwitch.AddMental(iAddMental);
		if (IsMentalZero())
		{
			CallMentalEndBroadcast();
		}
		else if (flag)
		{
			if (m_GameSwitch.IsMentalBeyoundOrBroken())
			{
				PushThreadFunc(eScriptFunc.SWITCH_MENTAL_ADD);
			}
			else
			{
				CallEventScriptEnd(eScriptFunc.SWITCH_MENTAL_ADD);
			}
		}
		else
		{
			CallEventScriptEnd(eScriptFunc.SWITCH_MENTAL_ADD);
		}
	}

	public void A_AddMental(int iAddMental)
	{
		m_GameSwitch.AddMental(iAddMental);
		if (IsMentalZero())
		{
			CallMentalEndBroadcast();
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	private bool IsMentalZero()
	{
		bool result = false;
		if (m_GameSwitch.GetMental() <= 0)
		{
			result = true;
		}
		return result;
	}

	public void MentalFullPower()
	{
		m_GameMain.ShowInGameMainUI(isShow: true, GameMain.eGameMainUI.Mental, isRemainFalse: true);
		m_GameMain.m_gameMainMenu.SetMentalPoint(ConstGameSwitch.MAX_MENTAL_POINT + 1, m_GameSwitch.GetMental());
		PushThreadFunc(eScriptFunc.SWITCH_MENTAL_FULLPOWER);
	}

	public void CheckMental(bool isOver, int iMental)
	{
		bool isTrue = m_GameSwitch.IsCheckMental(isOver, iMental);
		CallTrueFalse(isTrue);
	}

	public void CheckMentalZeroEvtType()
	{
		bool mentalZeroEvtFromSNSFater = m_EventEngine.GetMentalZeroEvtFromSNSFater();
		CallTrueFalse(mentalZeroEvtFromSNSFater);
	}

	public void DisableEndMentalEvt(bool isDisable)
	{
		m_EventEngine.SetDisableMentalZero(isDisable);
	}

	public void SetCharRelation(string strCharKey, int iValue)
	{
		m_GameSwitch.SetCharRelation(strCharKey, (byte)iValue);
	}

	public void A_SetCharRelation(string strCharKey, int iValue)
	{
		m_GameSwitch.SetCharRelation(strCharKey, (byte)iValue);
		CallEventScriptEnd();
	}

	public void AddCharRelation(string strCharKey, int iRelation)
	{
		if (m_GameSwitch.AddCharRelation(strCharKey, iRelation))
		{
			PushThreadFunc(eScriptFunc.ADD_CHAR_RELATION);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	public void A_AddCharRelation(string strCharKey, int iRelation)
	{
		m_GameSwitch.AddCharRelation(strCharKey, iRelation);
		CallEventScriptEnd();
	}

	public void CheckCharRelation(bool isOver, string strCharKey, int iRelation)
	{
		bool isTrue = m_GameSwitch.IsCheckCharRelation(isOver, strCharKey, iRelation);
		CallTrueFalse(isTrue);
	}

	public int GetCharRelationChangedValue(string strCharKey)
	{
		int cutStartRelation = m_GameSwitch.GetCutStartRelation(strCharKey);
		int charRelation = m_GameSwitch.GetCharRelation(strCharKey);
		int result = charRelation - cutStartRelation;
		CallEventScriptEnd();
		return result;
	}

	public int GetCharRelationValue(string strCharKey)
	{
		return m_GameSwitch.GetCharRelation(strCharKey);
	}

	public string A_GetLargestRelation(string strChar_0, string strChar_1, string strChar_2, string strChar_3, string strChar_4)
	{
		return m_GameSwitch.GetLargestRelation(strChar_0, strChar_1, strChar_2, strChar_3, strChar_4);
	}

	public void SetCharParty(string strCharKey, string strEventKey)
	{
		m_GameSwitch.SetCharPartyState(strCharKey, strEventKey);
	}

	public void GetCharParty(string strCharKey, string strEventKey)
	{
		bool isTrue = m_GameSwitch.IsCharPartyState(strCharKey, strEventKey);
		CallTrueFalse(isTrue);
	}

	public void SetEventSwitch(int iIndex, bool isTrue)
	{
		m_GameSwitch.SetEventSwitch(iIndex, (byte)(isTrue ? 1u : 0u));
	}

	public void A_SetEventSwitch(int iIndex, bool isTrue)
	{
		m_GameSwitch.SetEventSwitch(iIndex, (byte)(isTrue ? 1u : 0u));
		CallEventScriptEnd();
	}

	public void GetEventSwitchOn(string strArg)
	{
		bool isTrue = false;
		int num = SetStringNumberArgument(strArg);
		for (int i = 0; i < MAX_CHECK_SWITCH_CNT && m_iCheckSwitchIdx[num, i] != -1; i++)
		{
			sbyte eventSwitch = m_GameSwitch.GetEventSwitch(m_iCheckSwitchIdx[num, i]);
			if (eventSwitch == 1)
			{
				isTrue = true;
				continue;
			}
			isTrue = false;
			break;
		}
		CallTrueFalse(isTrue);
	}

	public void SetCharAnonymous(string strCharKey, bool isView)
	{
		m_GameSwitch.SetCharAnonymous(strCharKey, isView ? 1 : 0);
	}

	public void SetCharProfile(string strProfileKey)
	{
		if (m_GameSwitch.SetCharProfile(strProfileKey, 1))
		{
			PushThreadFunc(eScriptFunc.SWITCH_PROFILE_POP);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	public void SetCharProfileHidePop(string strProfileKey)
	{
		m_GameSwitch.SetCharProfile(strProfileKey, 1, isPop: false);
	}

	public void SetKeyword(string strKeyword)
	{
		m_iBefKeywordPoint = m_GameSwitch.GetMustGetKeywordCnt();
		m_isPlayKeywordPoint = false;
		if (m_GameSwitch.SetKeywordAllState(strKeyword, 1, isSetPop: true))
		{
			PushThreadFunc(eScriptFunc.SWITCH_KEYWORD_POP);
		}
		else
		{
			CallEventScriptEnd(eScriptFunc.SWITCH_KEYWORD_POP);
		}
	}

	public void SetKeywordNoPop(string strKeyword)
	{
		m_GameSwitch.SetKeywordAllState(strKeyword, 1);
	}

	public void A_SetKeywordNoPop(string strKeyword)
	{
		m_GameSwitch.SetKeywordAllState(strKeyword, 1);
		CallEventScriptEnd();
	}

	public void SetSoundColl(string strSound, bool isPopup)
	{
		m_GameSwitch.SetCollSound(strSound, 1, isPopup);
	}

	public void SetSoundInven(string strSound, bool isPopup)
	{
		m_GameSwitch.SetSoundSwitch(strSound, 1, isPopup);
	}

	public void SetCollImage(string strCollImgKey, bool isPopup)
	{
		m_GameSwitch.SetCollImage(strCollImgKey, 1, isPopup);
	}

	public void SetImageInven(string strImage, bool isPopup)
	{
		m_GameSwitch.SetImageSwitch(strImage, 1, isPopup);
	}

	public void SetCharVotesCnt(string strCharKey, int iVotesCnt)
	{
		m_GameSwitch.SetCharVotesCount(strCharKey, iVotesCnt);
	}

	public void AddCharVotesCnt(string strCharKey, int iAddCnt)
	{
		m_GameSwitch.AddCharVotesCount(strCharKey, iAddCnt);
	}

	public void AddTrophyCnt(string strTrophyKey)
	{
		m_GameSwitch.AddTrophyCnt(strTrophyKey);
	}

	public void CheckTrophySwitch(string strTrophyKey)
	{
		bool trophyComplete = m_GameSwitch.GetTrophyComplete(strTrophyKey);
		CallTrueFalse(trophyComplete);
	}

	public void CheckProfileSwitch(string strProfileKey)
	{
		sbyte charProfile = m_GameSwitch.GetCharProfile(strProfileKey);
		bool isTrue = charProfile == 1 || charProfile == 2;
		CallTrueFalse(isTrue);
	}

	public void SetSWRingtone(string strRingtone)
	{
		m_GameSwitch.SetSWRingtone(strRingtone);
	}

	public void SetSWBackImage(string strBackImage)
	{
		m_GameSwitch.SetSWBackImage(strBackImage);
	}

	public string CheckCurCutID(string strCutID)
	{
		bool isTrue = false;
		string result = string.Empty;
		int curCutIdx = m_GameSwitch.GetCurCutIdx();
		if (curCutIdx != -1)
		{
			isTrue = m_GameSwitch.CheckCurCutID(strCutID);
			Xls.TalkCutSetting data_byIdx = Xls.TalkCutSetting.GetData_byIdx(curCutIdx);
			if (data_byIdx != null)
			{
				result = data_byIdx.m_strKey;
			}
		}
		CallTrueFalse(isTrue);
		return result;
	}

	public void SetAutoText(int iIdx)
	{
		m_GameSwitch.SetAutoSNSText(iIdx);
	}

	public void CallRecordConti()
	{
		SetRunConti(isSet: true);
		string recordFaterContiName = m_GameSwitch.GetRecordFaterContiName();
		if (!ContiDataHandler.ActivateConti(recordFaterContiName, this))
		{
			CallEventScriptEnd();
		}
	}

	public void FinishRecordFaterConti()
	{
		m_EventEngine.FinishRecordFaterEventCBFunc();
	}

	public string CheckCallAble()
	{
		bool flag = m_GameSwitch.CheckCharCallable();
		bool flag2 = false;
		string curSelCharName = m_GameMain.m_SmartWatchRoot.GetCurSelCharName();
		sbyte charCallState = m_GameSwitch.GetCharCallState(curSelCharName);
		PlayRingingBell(isPlay: false, curSelCharName);
		bool flag3 = charCallState == 0;
		bool flag4 = charCallState == 2;
		if (flag)
		{
			string text = Xls.TalkCutSetting.GetData_byIdx(m_GameSwitch.GetCurCutIdx()).m_strContiPreName + curSelCharName;
			if (ContiDataHandler.IsExistConti(text))
			{
				m_strPhoneConti = text;
				flag2 = true;
			}
			else
			{
				flag3 = true;
			}
		}
		if (flag3)
		{
			string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("CONTI_DO_NOT_ANSWER_CALL");
			if (xlsProgramDefineStr != null)
			{
				m_strPhoneConti = xlsProgramDefineStr;
			}
		}
		else if (flag4)
		{
			string xlsProgramDefineStr2 = GameGlobalUtil.GetXlsProgramDefineStr("CONTI_PHONE_NO_USE");
			if (xlsProgramDefineStr2 != null)
			{
				m_strPhoneConti = xlsProgramDefineStr2;
			}
		}
		m_GameSwitch.SetCharCall(curSelCharName, 4);
		if (flag2)
		{
			m_GameSwitch.SetCutAllCharDoNotCall();
		}
		CallTrueFalse(flag2);
		return curSelCharName;
	}

	public void SetCallState(string strCharKey, string strCallState)
	{
		m_GameSwitch.SetCharCall(strCharKey, (byte)GameGlobalUtil.GetXlsScriptKeyValue(strCallState));
	}

	public void SetCallSending(string strCharKey, string strCallState = null)
	{
		m_GameMain.m_SmartWatchRoot.SetPhoneState_Sending(strCharKey);
		PlayRingingBell(isPlay: true, strCharKey, strCallState);
	}

	private void PlayRingingBell(bool isPlay, string strCharKey, string strCallState = null)
	{
		if (strCallState == null)
		{
			m_GameSwitch.PlayCharRingingBell(isPlay, strCharKey);
		}
		else
		{
			m_GameSwitch.PlayCharRingingBellByState(isPlay, (byte)GameGlobalUtil.GetXlsScriptKeyValue(strCallState));
		}
	}

	public void SetCallReceive(string strCharKey)
	{
		m_GameMain.m_SmartWatchRoot.SetPhoneState_Ringing(strCharKey);
	}

	public void SetCallSendingSmall()
	{
		m_GameMain.m_SmartWatchRoot.SetPhoneState_SendingSmall();
		CallEventScriptEnd();
	}

	public void SetCallTalking(string strCallState)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strCallState);
		m_GameSwitch.PlayCharRingingBellByState(isPlay: false, xlsScriptKeyValue);
		if (xlsScriptKeyValue == 1)
		{
			m_GameMain.m_SmartWatchRoot.SetPhoneState_Engaged();
		}
		else
		{
			m_GameMain.m_SmartWatchRoot.SetPhoneState_SendingSmall();
		}
	}

	public void SetCallFinish(float fTime)
	{
		m_GameMain.m_SmartWatchRoot.SetPhoneState_Finish();
		m_EventEngine.SetDelayTime(fTime);
		PushThreadFunc(eScriptFunc.CALL_FINISH);
	}

	public void SetCallBackOn()
	{
		m_GameMain.m_SmartWatchRoot.BGCoverAppear();
		PushThreadFunc(eScriptFunc.CALL_BG_ON);
	}

	public void SetCallBackOff()
	{
		m_GameMain.m_SmartWatchRoot.BGCoverDisappear();
		PushThreadFunc(eScriptFunc.CALL_BG_OFF);
	}

	public void SetCallHide()
	{
		m_isHideEffect_Ing = true;
		m_GameMain.m_SmartWatchRoot.SetPhoneState_EngagedHide(CallBack_CallHide);
		PushThreadFunc(eScriptFunc.CALL_HIDE);
	}

	private void CallBack_CallHide(object sender, object arg)
	{
		m_isHideEffect_Ing = false;
	}

	public void SetCallReAppearance()
	{
		m_isCallReappearance_Ing = true;
		m_GameMain.m_SmartWatchRoot.SetPhoneState_EngagedShow(CallBack_CallReAppearance);
		PushThreadFunc(eScriptFunc.CALL_REAPPEARANCE);
	}

	private void CallBack_CallReAppearance(object sender, object arg)
	{
		m_isCallReappearance_Ing = false;
	}

	public void SetCallSendingForSmartPhone()
	{
		SetCallSending(m_GameMain.m_SmartWatchRoot.GetCurSelCharName());
		m_EventEngine.SetDelayTime(GameGlobalUtil.GetXlsProgramDefineStrToFloat("PLAYMAKER_PHONE_SENDING_EFF_TIME"));
		PushThreadFunc(eScriptFunc.CALL_SENDING_FOR_SMARTPHONE);
	}

	public void SetCallEngagedForSmartPhone()
	{
		m_GameMain.m_SmartWatchRoot.SetPhoneState_Engaged();
		m_EventEngine.SetDelayTime(GameGlobalUtil.GetXlsProgramDefineStrToFloat("PLAYMAKER_PHONE_ENGAGE_TIME"));
		PushThreadFunc(eScriptFunc.CALL_ENGAGE_FOR_SMARTPHONE);
	}

	public void SetCallFinishForSmartPhone()
	{
		if (m_GameMain.m_SmartWatchRoot.GetCurrentPhoneState() != SWSub_PhoneMenu.EngageState.Finish)
		{
			m_GameMain.m_SmartWatchRoot.SetPhoneState_Finish();
			m_EventEngine.SetDelayTime(GameGlobalUtil.GetXlsProgramDefineStrToFloat("PLAYMAKER_PHONE_FINISH_TIME"));
			PushThreadFunc(eScriptFunc.CALL_FINISH_FOR_SMARTPHONE);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	public void GoPhoneSelectScreen()
	{
		m_GameMain.m_SmartWatchRoot.SetPhoneMode_Selection();
		CallEventScriptEnd();
	}

	public void AddPrintTalkWithoutName(string strTalkKey)
	{
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.AddTalkText(strTalkKey, string.Empty);
		}
	}

	public void PrintTalkWithoutName(string strTalkKey)
	{
		PushThreadFunc(eScriptFunc.PRINT_TALK_WITHOUT_NAME);
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.SetTalkWindow(string.Empty, strTalkKey, isTalkMot: false);
		}
	}

	public void PrintTalkWithoutNameForce(string strTalkKey)
	{
		PushThreadFunc(eScriptFunc.PRINT_TALK_WITHOUT_NAME_FORCE);
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.SetTalkWindow(string.Empty, strTalkKey, isTalkMot: false, isTouchableFunc: false);
		}
	}

	public void AddPrintTalk(string strCharKey, string strTalkKey)
	{
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.AddTalkText(strTalkKey, strCharKey);
		}
	}

	public void PrintStackAll()
	{
		PushThreadFunc(eScriptFunc.PRINT_STACK_ALL);
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.SetNextQueueText();
		}
	}

	public void PrintTalk(string strCharKey, string strTalkKey)
	{
		PushThreadFunc(eScriptFunc.PRINT_TALK);
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.SetTalkWindow(strCharKey, strTalkKey, isTalkMot: true);
		}
	}

	public void PrintTalkShutUp(string strCharKey, string strTalkKey)
	{
		PushThreadFunc(eScriptFunc.PRINT_TALK_SHUTUP);
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.SetTalkWindow(strCharKey, strTalkKey, isTalkMot: false);
		}
	}

	public void PrintTalkForce(string strCharKey, string strTalkKey)
	{
		PushThreadFunc(eScriptFunc.PRINT_TALK_FORCE);
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.SetTalkWindow(strCharKey, strTalkKey, isTalkMot: true, isTouchableFunc: false);
		}
	}

	public void PrintNarration(string strTalkKey, string strAlign)
	{
		PushThreadFunc(eScriptFunc.PRINT_NARRATION);
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.SetNarrationWindow(strTalkKey, strAlign);
		}
	}

	public void PrintNarrationForce(string strTalkKey, string strAlign)
	{
		PushThreadFunc(eScriptFunc.PRINT_NARRATION_FORCE);
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.SetNarrationWindow(strTalkKey, strAlign, isTouchableFunc: false);
		}
	}

	public void PrintNarrationOnce(string strTalkKey, string strAlign, float fEnterT, float fWaitT, float fExitT)
	{
		PushThreadFunc(eScriptFunc.PRINT_NARRATION_ONCE);
		if (Event_TalkDialogue.instance != null)
		{
			Event_TalkDialogue.instance.SetNarrationOnce(strTalkKey, strAlign, fEnterT, fWaitT, fExitT);
		}
	}

	public void SetSelect(string strSelectKey, float fTime)
	{
		PushThreadFunc(eScriptFunc.SET_SELECT);
		m_EventEngine.StopSkip(isShowSkipBut: false, isForce: true);
		m_GameMain.m_clEventSelect.SetSelect(this, strSelectKey, fTime);
	}

	public void CharaterCreate(string strCharKey, string strPosition, string strSize, string strMot, string strDir)
	{
		m_EventEngine.m_TalkChar.PrintCharMove("캐릭터_등장", strCharKey, strPosition, strSize, 999f, isSaveData: true);
		StartCoroutine(m_EventEngine.m_TalkChar.CreateChar(strCharKey, strPosition, strSize, strMot, strDir));
		PushThreadFunc(eScriptFunc.CHAR_CREATE);
	}

	public void CharacterMotion(string strCharKey, string strMotion, string strDir)
	{
		m_EventEngine.m_TalkChar.ChangeCharMot(strCharKey, strMotion, strDir);
		CallEventScriptEnd();
	}

	public void CharacterBlack(string strCharKey, bool isOn)
	{
		m_EventEngine.m_TalkChar.SetCharBlack(strCharKey, isOn);
		CallEventScriptEnd();
	}

	public void CharacterMove(string strCharKey, string strPosition, float fTime, string strSpeedType)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
		if (!m_EventEngine.m_TalkChar.SetCharMove(strCharKey, strPosition, fTime, xlsScriptKeyValue))
		{
			CallEventScriptEnd(eScriptFunc.CHAR_MOVE);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CHAR_MOVE);
		}
	}

	public void CharacterMoveDetailWorld(string strCharKey, float fX, float fY, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_TalkChar.SetCharMoveDetailWorld(strCharKey, fX, fY, 1f, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CHAR_MOVE_DETAIL);
			return;
		}
		m_EventEngine.m_TalkChar.PrintCharMove("캐릭터_피벗이동", strCharKey, null, null, fY, isSaveData: true);
		PushThreadFunc(eScriptFunc.CHAR_MOVE_DETAIL);
	}

	public void CharacterMoveDetailWorldForSkip(string strCharKey, float fX, float fY)
	{
		m_EventEngine.m_TalkChar.SetCharSkipWorldPos(strCharKey, fX, fY);
	}

	public void CharacterMoveDetail(string strCharKey, float fX, float fY, float fTime, string strSpeedType)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
		if (!m_EventEngine.m_TalkChar.SetCharMoveDetail(strCharKey, fX, fY, fTime, xlsScriptKeyValue))
		{
			CallEventScriptEnd(eScriptFunc.CHAR_MOVE_DETAIL);
			return;
		}
		m_EventEngine.m_TalkChar.PrintCharMove("캐릭터_피벗이동", strCharKey, null, null, fY, isSaveData: true);
		PushThreadFunc(eScriptFunc.CHAR_MOVE_DETAIL);
	}

	public void CharacterMoveRevert(string strCharKey, float fTime, string strSpeedType)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
		if (!m_EventEngine.m_TalkChar.SetCharMoveRevert(strCharKey, fTime, xlsScriptKeyValue))
		{
			CallEventScriptEnd(eScriptFunc.CHAR_MOVE_REVERT);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CHAR_MOVE_REVERT);
		}
	}

	public void CharacterDeleteAllPivot()
	{
		m_GameSwitch.InitAllPivotXY();
	}

	public void CharacterZoom(string strCharKey, string strSize, float fTime, string strZoomSpeed)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strZoomSpeed);
		if (!m_EventEngine.m_TalkChar.SetCharZoom(strCharKey, strSize, fTime, xlsScriptKeyValue))
		{
			CallEventScriptEnd(eScriptFunc.CHAR_ZOOM);
			return;
		}
		m_EventEngine.m_TalkChar.PrintCharMove("캐릭터_줌", strCharKey, null, strSize, 999f, isSaveData: true);
		PushThreadFunc(eScriptFunc.CHAR_ZOOM);
	}

	public void CharacterOutAll(string strOutType, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_TalkChar.SetCharOutAll(strOutType, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CHAR_OUT);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CHAR_OUT);
		}
	}

	public void CharacterDelete(string strCharKey)
	{
		m_EventEngine.m_TalkChar.CharDel(strCharKey);
		CallEventScriptEnd();
	}

	public void CharacterAllDelete()
	{
		m_EventEngine.m_TalkChar.CharAllHide();
		CallEventScriptEnd();
	}

	public void CharacterEscape(string strCharName, string strPos, float fTime, string strSpeed)
	{
		if (!m_EventEngine.m_TalkChar.SetCharMove(strCharName, strPos, fTime, strSpeed))
		{
			CallEventScriptEnd(eScriptFunc.CHAR_ESCAPE);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CHAR_ESCAPE);
		}
	}

	public void CharacterAllEscape(string strOutType, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_TalkChar.SetCharOutAll(strOutType, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CHAR_ESCAPE_ALL);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CHAR_ESCAPE_ALL);
		}
	}

	public void CharRotate(string strCharKey, float fRotateZ, float fTime, string strSpeed)
	{
		if (!m_EventEngine.m_TalkChar.SetRotate(strCharKey, fRotateZ, fTime, strSpeed))
		{
			CallEventScriptEnd(eScriptFunc.CHAR_ROTATE);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CHAR_ROTATE);
		}
	}

	public void CharRotateRevert(string strCharKey, float fTime, string strSpeed)
	{
		if (!m_EventEngine.m_TalkChar.SetRotate(strCharKey, 0f, fTime, strSpeed))
		{
			CallEventScriptEnd(eScriptFunc.CHAR_ROTATE_REVERT);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CHAR_ROTATE_REVERT);
		}
	}

	public void AllCharDisable()
	{
		m_EventEngine.m_EventCamera.SetCurCameraToBuf();
		m_EventEngine.m_TalkChar.CharAllDisable();
	}

	public void AllCharEnableFromSavedBuf()
	{
		m_EventEngine.m_EventCamera.SetBufToCurCamera();
		m_EventEngine.m_TalkChar.CharEnableFromSavedBuf();
	}

	public void PM_AllCharDisable()
	{
		m_EventEngine.m_EventCamera.SetCurCameraToBuf();
		m_EventEngine.m_TalkChar.CharAllDisable();
		CallEventScriptEnd();
	}

	public void PM_AllCharEnableFromSavedBuf()
	{
		m_EventEngine.m_EventCamera.SetBufToCurCamera();
		m_EventEngine.m_TalkChar.CharEnableFromSavedBuf();
		CallEventScriptEnd();
	}

	public void CameraPresetMoveAndRotate(string strObj, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_EventCamera.SetPresetMoveAndRotate(strObj, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CAMERA_PRESET_MOVE_AND_ROTATE);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CAMERA_PRESET_MOVE_AND_ROTATE);
		}
	}

	public void CameraPresetMove(string strObj, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_EventCamera.SetPresetMove(strObj, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CAMERA_PRESET_MOVE);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CAMERA_PRESET_MOVE);
		}
	}

	public void CameraPresetRotate(string strObj, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_EventCamera.SetPresetRotate(strObj, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CAMERA_PRESET_ROTATE);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CAMERA_PRESET_ROTATE);
		}
	}

	public void CameraView(string strLookObj, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_EventCamera.SetCameraView(strLookObj, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CAMERA_VIEW);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CAMERA_VIEW);
		}
	}

	public void CameraViewAndFOV(string strLookObj, float fFOV, float fTime, string strSpeedType)
	{
		bool flag = false;
		bool flag2 = false;
		flag = m_EventEngine.m_EventCamera.SetCameraView(strLookObj, fTime, strSpeedType);
		flag2 = m_EventEngine.m_EventCamera.SetFov(fFOV, fTime, strSpeedType);
		if (!flag && !flag2)
		{
			CallEventScriptEnd(eScriptFunc.CAMERA_VIEW_AND_FOV);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CAMERA_VIEW_AND_FOV);
		}
	}

	public void CameraMove(float fX, float fY, float fZ, string strLookObj, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_EventCamera.SetMoveAndLook(fX, fY, fZ, strLookObj, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CAMERA_MOVE);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CAMERA_MOVE);
		}
	}

	public void CameraMoveRelative(float fX, float fY, float fZ, string strLook, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_EventCamera.SetMoveObjRelative(fX, fY, fZ, strLook, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CAMERA_MOVE_RELATIVE);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CAMERA_MOVE_RELATIVE);
		}
	}

	public void CameraMoveObj(string strObjName, string strLookObj, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_EventCamera.SetMoveObj(strObjName, strLookObj, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CAMERA_MOVE_OBJ);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CAMERA_MOVE_OBJ);
		}
	}

	public void CameraPosInit()
	{
		m_EventEngine.m_EventCamera.InitMainCameraPos();
		CallEventScriptEnd();
	}

	public void CameraRotate(float fRotX, float fRotY, float fRotZ, float fTime, string strRotType)
	{
		if (!m_EventEngine.m_EventCamera.PlusRotate(fRotX, fRotY, fRotZ, fTime, strRotType))
		{
			CallEventScriptEnd(eScriptFunc.CAMERA_ROTATE);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CAMERA_ROTATE);
		}
	}

	public void CameraRotateInit()
	{
		m_EventEngine.m_EventCamera.InitMainCameraRotate();
		CallEventScriptEnd();
	}

	public void CameraShake(string strCameraShakeKey, string strOpt, string strDir)
	{
		m_EventEngine.m_EventCamera.ShakeCamera(isOn: true, strCameraShakeKey, strOpt, strDir);
		switch (strCameraShakeKey)
		{
		case "약":
			GamePadInput.SetVibration_On(GamePadInput.VibrationType.Weak);
			break;
		case "중":
			GamePadInput.SetVibration_On(GamePadInput.VibrationType.Middle);
			break;
		case "강":
			GamePadInput.SetVibration_On(GamePadInput.VibrationType.Strong);
			break;
		}
		CallEventScriptEnd();
	}

	public void CameraShakeOff()
	{
		m_EventEngine.m_EventCamera.ShakeCamera(isOn: false, string.Empty, string.Empty, string.Empty);
		GamePadInput.SetVibration_Off();
		CallEventScriptEnd();
	}

	public void PadShake(string strPadPower)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strPadPower);
		GamePadInput.VibrationType vibration_On = GamePadInput.VibrationType.Weak;
		switch (xlsScriptKeyValue)
		{
		case 0:
			vibration_On = GamePadInput.VibrationType.Weak;
			break;
		case 1:
			vibration_On = GamePadInput.VibrationType.Middle;
			break;
		case 2:
			vibration_On = GamePadInput.VibrationType.Strong;
			break;
		}
		GamePadInput.SetVibration_On(vibration_On);
		CallEventScriptEnd();
	}

	public void PadShakeOff()
	{
		GamePadInput.SetVibration_Off();
		CallEventScriptEnd();
	}

	public void CameraDollyZoom(float fDistance, float fMoveVal, float fTime)
	{
		PushThreadFunc(eScriptFunc.CAMERA_DOLLYZOOM);
		m_EventEngine.m_EventCamera.SetDollyZoom(fDistance, fMoveVal, fTime);
	}

	public void CameraFov(float fFov, float fTime, string strSpeedType)
	{
		if (!m_EventEngine.m_EventCamera.SetFov(fFov, fTime, strSpeedType))
		{
			CallEventScriptEnd(eScriptFunc.CAMERA_FOV);
		}
		else
		{
			PushThreadFunc(eScriptFunc.CAMERA_FOV);
		}
	}

	public void StopSkip()
	{
		m_EventEngine.StopSkip();
		CallEventScriptEnd();
	}

	public void DisableSkip()
	{
		m_EventEngine.StopSkip();
		m_EventEngine.SetShowSkipBut(isShowSkipBut: false);
		CallEventScriptEnd();
	}

	public void EnableSkip()
	{
		m_EventEngine.SetShowSkipBut(isShowSkipBut: true);
		CallEventScriptEnd();
	}

	public void LoadLevel(string strLevel)
	{
		m_EventEngine.StopSkip();
		m_EventEngine.ShowLoadingIcon(isShowLoadingIcon: true);
		SetLoadLevelStr(strLevel);
		CallEventScriptEnd();
	}

	public bool LoadLevelSpot(string strLevel)
	{
		m_EventEngine.StopSkip(isShowSkipBut: false, isForce: true);
		m_EventEngine.ShowLoadingIcon(isShowLoadingIcon: false);
		SetLoadLevelStr(strLevel);
		m_isSpotLoadingScreen = true;
		StartCoroutine(ChangeSceneScreen.Show(EndSpotLoadingScreen));
		PushThreadFunc(eScriptFunc.SPOT_LOADING_SCREEN);
		return true;
	}

	public void LoadMultiLevel(string strMainLevel, string strSubLevel1, string strSubLevel2, string strSubLevel3)
	{
		m_EventEngine.StopSkip();
		m_EventEngine.ShowLoadingIcon(isShowLoadingIcon: true);
		SetLoadLevelStr(strMainLevel, strSubLevel1, strSubLevel2, strSubLevel3);
		CallEventScriptEnd();
	}

	public void ActiveMultiLevel(string strLevelName)
	{
		EventEngine.ActiveMultiLevel(strLevelName);
	}

	private void SetLoadLevelStr(string strMainLevel, string strSubLevel1 = null, string strSubLevel2 = null, string strSubLevel3 = null)
	{
		m_strLoadLevel = strMainLevel;
		if (strSubLevel1 != null || strSubLevel2 != null || strSubLevel3 != null)
		{
			m_strLoadAddLevel = new string[3];
			m_strLoadAddLevel[0] = strSubLevel1;
			m_strLoadAddLevel[1] = strSubLevel2;
			m_strLoadAddLevel[2] = strSubLevel3;
		}
		else
		{
			m_strLoadAddLevel = null;
		}
	}

	private void EndSpotLoadingScreen(object sender, object arg)
	{
		m_isSpotLoadingScreen = false;
	}

	public void CloseLoadLevelSpot()
	{
		m_isSpotLoadClosingScreen = true;
		PushThreadFunc(eScriptFunc.SPOT_LOAD_CLOSING_SCREEN);
		Invoke("CloseLoadLevelSpotInvoke", 2f);
	}

	private void CloseLoadLevelSpotInvoke()
	{
		ChangeSceneScreen.Close(EndSpotClosingScreen);
	}

	private void EndSpotClosingScreen(object sender, object arg)
	{
		m_isSpotLoadClosingScreen = false;
	}

	public void DelayTime(float fTime)
	{
		PushThreadFunc(eScriptFunc.DELAY_TIME);
		m_EventEngine.SetDelayTime(fTime);
	}

	public void DelayButton(float fTime)
	{
		PushThreadFunc(eScriptFunc.DELAY_BUTTON);
		m_EventEngine.SetDelayTime(fTime);
	}

	public void ThreadOpen()
	{
		PushThreadFunc(eScriptFunc.THREAD_OPEN);
	}

	public void ThreadClose()
	{
		PushThreadFunc(eScriptFunc.THREAD_CLOSE);
		if (m_listEventFunc.Count < 1)
		{
			CallEventScriptEnd();
		}
	}

	public void FinishScript()
	{
		bool flag = false;
		if (m_listEventFunc != null)
		{
			int count = m_listEventFunc.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_listEventFunc[i] == eScriptFunc.LOAD_GAME)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			return;
		}
		CheckBeforeScriptInit(isInitAll: true);
		bool flag2;
		do
		{
			bool isGoMainMenu = false;
			flag2 = m_EventEngine.FinishRunEvent(this, m_isBackGoKeywordMenu, ref isGoMainMenu);
			if (flag2 || isGoMainMenu)
			{
				return;
			}
			if (m_GameMain != null)
			{
				m_GameMain.m_KeywordMenu.FreeKeywordMenuForFinishPM();
			}
			if (!flag2 && m_strLoadLevel != null)
			{
				m_EventEngine.ClearCurrentLevelEvent(EventEngine.m_strLoadedLevel);
				Xls.ProgramDefineStr data_byKey = Xls.ProgramDefineStr.GetData_byKey("PM_EVENT_START_EVENT");
				EventEngine.LoadLevel(m_strLoadLevel, m_strLoadAddLevel, m_GameMain, data_byKey?.m_strTxt, null, data_byKey != null);
				m_strLoadLevel = null;
				return;
			}
		}
		while (m_GameMain != null && m_GameSwitch.GetMental() <= ConstGameSwitch.MIN_MENTAL_POINT && m_EventEngine.RunMentalZero(isSNSFaterEvt: false));
		if (m_GameMain != null && !flag2)
		{
			m_GameMain.m_clConversationSign.SetDialogSignType(ConversationSign.eDialogSignType.Off, null, isSetAnim: false);
			if (m_isRunKRelationConti)
			{
				Resources.UnloadUnusedAssets();
				m_isRunKRelationConti = false;
			}
			if (m_isBackGoKeywordMenu)
			{
				bool flag3 = m_GameSwitch.IsExistUsableKeyword(KeywordMenuPlus.GetCharKeyIdx());
				FinishKeywordEventCheck();
				if (flag3)
				{
					m_GameMain.NotSetOpenKeyword(isShow: true);
				}
				else
				{
					m_EventEngine.m_TalkChar.RevertPartyCharKeywordSet(KeywordMenuPlus.GetCharKey());
					m_GameMain.SetGameMainState(GameMain.eGameMainState.Def);
				}
				m_isBackGoKeywordMenu = false;
			}
			else if (m_GameMain.m_SmartWatchRoot != null && m_GameMain.m_SmartWatchRoot.gameObject.activeInHierarchy)
			{
				m_GameMain.SetGameMainState(GameMain.eGameMainState.SmartWatchMenu);
			}
			else if (MainMenuCommon.GetCurMode() != MainMenuCommon.Mode.None)
			{
				m_GameMain.SetGameMainState(GameMain.eGameMainState.SystemMenu);
			}
			else if (SNSMenuPlus.IsActivated)
			{
				m_GameMain.SetGameMainState(GameMain.eGameMainState.SNSFaterMenu);
			}
			else
			{
				FinishKeywordEventCheck();
				m_GameMain.SetGameMainState(GameMain.eGameMainState.LoadDefScene);
			}
		}
		if (!flag2)
		{
			m_EventEngine.StopSkip(isShowSkipBut: true, isForce: true);
			if (m_GameMain != null)
			{
				m_GameMain.m_clConversationSign.InitDialgSignType();
			}
		}
	}

	private void FinishKeywordEventCheck(bool isReleaseKeywordSet = true)
	{
		if (m_GameSwitch.GetUseKeywordChar() != null && !m_GameSwitch.GetRunLowRelationEvt())
		{
			m_GameSwitch.SetKeyUseSwitch(isReleaseKeywordSet);
		}
	}

	public void CharEventObjCreate(string strObjName, string strCharKey, string strMotion, bool isHasTalkMot)
	{
		StartCoroutine(m_EventEngine.m_EventObject.CharObjCreate(strObjName, strCharKey, strMotion, isHasTalkMot));
		PushThreadFunc(eScriptFunc.EVT_CHAR_OBJ_CREATE);
	}

	public void CharEventObjectDelete(string strObjName)
	{
		m_EventEngine.m_EventObject.CharObjDelete(strObjName);
	}

	public void EnableObj(string strGameObject)
	{
		m_EventEngine.EnableAndRunObj(strGameObject);
		CallEventScriptEnd();
	}

	public void EnableObjAndSendEvent(string strGameObject, string strSendEvent)
	{
		m_EventEngine.EnableAndRunObj(strGameObject, strSendEvent);
		CallEventScriptEnd();
	}

	public void EventObjCreate(bool isPrefab, string strObjName, float fTime)
	{
		StartCoroutine(m_EventEngine.m_EventObject.ObjCreate(isPrefab, strObjName, fTime));
		PushThreadFunc(eScriptFunc.EVT_OBJ_CREATE);
	}

	public void EventObjCreateWithPos(bool isPrefab, string strObjName, float fX, float fY, float fTime)
	{
		StartCoroutine(m_EventEngine.m_EventObject.ObjCreate(isPrefab, strObjName, fTime, isPositionSet: true, fX, fY));
		PushThreadFunc(eScriptFunc.EVT_OBJ_CREATE);
	}

	public void EventObjPrefabCreateWithPos(string strObjName, float fX, float fY, string strCanvasType)
	{
		StartCoroutine(m_EventEngine.m_EventObject.ObjCreate(isPrefab: true, strObjName, 0f, isPositionSet: true, fX, fY, isListAdd: true, isCanvasSetting: true, GameGlobalUtil.GetXlsScriptKeyValue(strCanvasType)));
		PushThreadFunc(eScriptFunc.EVT_OBJ_CREATE);
	}

	public void EventObjPrefabCreate(string strObjName, string strCanvasType)
	{
		StartCoroutine(m_EventEngine.m_EventObject.ObjCreate(isPrefab: true, strObjName, 0f, isPositionSet: false, 0f, 0f, isListAdd: true, isCanvasSetting: true, GameGlobalUtil.GetXlsScriptKeyValue(strCanvasType)));
		PushThreadFunc(eScriptFunc.EVT_OBJ_CREATE);
	}

	public void EventObjDelete(string strObjName, float fTime)
	{
		StartCoroutine(m_EventEngine.m_EventObject.ObjDelete(strObjName, fTime));
		PushThreadFunc(eScriptFunc.EVT_OBJ_DELETE);
	}

	public void EventObjAllRelease()
	{
		m_EventEngine.m_EventObject.DeleteListObj();
	}

	public void EventObjMove(string strObjName, float fX, float fY, float fTime, string strSpeed)
	{
		StartCoroutine(m_EventEngine.m_EventObject.ObjMove(strObjName, fX, fY, fTime, strSpeed));
		PushThreadFunc(eScriptFunc.EVT_OBJ_MOVE);
	}

	public void EventObjZoom(string strObjName, float fZoomSize, float fTime, string strSpeed)
	{
		StartCoroutine(m_EventEngine.m_EventObject.ObjZoom(strObjName, fZoomSize, fTime, strSpeed));
		PushThreadFunc(eScriptFunc.EVT_OBJ_ZOOM);
	}

	public void EventObjPlayMot(string strObjName, string strMotion, bool isDelay)
	{
		StartCoroutine(m_EventEngine.m_EventObject.PlayAnimation(strObjName, strMotion, isDelay, isPlayObjMot: true));
		PushThreadFunc(eScriptFunc.EVT_OBJ_PLAY_MOTION);
	}

	public void EventObjRotate(string strObjName, float fZ, float fTime, string strSpeed, string strClockwise)
	{
		Xls.ScriptKeyValue data_byKey = Xls.ScriptKeyValue.GetData_byKey(strClockwise);
		bool isClockwise = true;
		if (data_byKey != null)
		{
			isClockwise = data_byKey.m_iValue == 1;
		}
		StartCoroutine(m_EventEngine.m_EventObject.ObjRotate(strObjName, fZ, fTime, strSpeed, isClockwise));
		PushThreadFunc(eScriptFunc.EVT_OBJ_ROTATE);
	}

	public void EventObjYRevert(string strObjName, string strRevert)
	{
		Xls.ScriptKeyValue data_byKey = Xls.ScriptKeyValue.GetData_byKey(strRevert);
		if (data_byKey != null)
		{
			StartCoroutine(m_EventEngine.m_EventObject.ObjRevertY(strObjName, data_byKey.m_iValue == 1));
		}
		PushThreadFunc(eScriptFunc.EVT_OBJ_REVERT_Y);
	}

	public void ShowObjCollectImg(string strCollImgKey, float fTime)
	{
		m_EventEngine.m_EventObject.ShowCollectImage(strCollImgKey, fTime);
		PushThreadFunc(eScriptFunc.EVT_OBJ_SHOW_COLLECT_IMG);
	}

	public void HideObjCollectImg(float fTime)
	{
		m_EventEngine.m_EventObject.HideCollectImage(fTime);
		PushThreadFunc(eScriptFunc.EVT_OBJ_HIDE_COLLECT_IMG);
	}

	public void EventObjActiveFindMarker(string strObjName, string strTextKey, bool isShow)
	{
		m_GameSwitch.AddInvestObj(strObjName, strTextKey, isShow);
	}

	public void EventObjShowFindMarker(string strObjName)
	{
		bool flag = m_GameSwitch.GetInvestListCount() == 0;
		m_GameSwitch.SetShowInvestObj(strObjName);
		m_EventEngine.m_EventObject.SetInvestOrder();
		if (flag)
		{
			m_GameSwitch.MoveInvestXYPlusMinus(isX: true, isPlus: true, isPlayUISound: false);
		}
	}

	public void RefreshFindMarker()
	{
		m_fRefreshTime = 0f;
		m_EventEngine.m_EventObject.AllFindMakrerDestroy();
		PushThreadFunc(eScriptFunc.REFRESH_FIND_MARKER);
	}

	public void Trans_FadeIn(int iLayerIdx, float fTime, float fMaxAmount)
	{
		EventCameraEffect.Instance.Activate_FadeIn(iLayerIdx, fTime, fMaxAmount);
		PushThreadFunc(eScriptFunc.TRANS_FADE_IN);
	}

	public void Trans_FadeOut(float fTime)
	{
		EventCameraEffect.Instance.Activate_FadeOut(fTime);
		PushThreadFunc(eScriptFunc.TRANS_FADE_OUT);
	}

	public void Trans_CoverIn(int iLayerIdx, string strCoverType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverIn(iLayerIdx, GameGlobalUtil.GetXlsScriptKeyValue(strCoverType), fSpeedRate));
		PushThreadFunc(eScriptFunc.TRANS_COVER_IN);
	}

	public void Trans_CoverOut(string strCoverType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverOut(GameGlobalUtil.GetXlsScriptKeyValue(strCoverType), fSpeedRate));
		PushThreadFunc(eScriptFunc.TRANS_COVER_OUT);
	}

	public void Trans_CircleIn(int iLayerIdx, float fTime)
	{
		EventCameraEffect.Instance.Activate_FadeCircleIn(iLayerIdx, fTime);
		PushThreadFunc(eScriptFunc.TRANS_CIRCLE_IN);
	}

	public void Trans_CircleOut(float fTime)
	{
		EventCameraEffect.Instance.Activate_FadeCircleOut(fTime);
		PushThreadFunc(eScriptFunc.TRANS_CIRCLE_OUT);
	}

	public void Trans_FlashIn(int iLayerIdx, float fTime, float fMaxAmount, int iR, int iG, int iB)
	{
		EventCameraEffect.Instance.Activate_FlashIn(iLayerIdx, fTime, fMaxAmount, iR, iG, iB);
		PushThreadFunc(eScriptFunc.TRANS_FLASH_IN);
	}

	public void Trans_FlashOut(float fTime)
	{
		EventCameraEffect.Instance.Activate_FlashOut(fTime);
		PushThreadFunc(eScriptFunc.TRANS_FLASH_OUT);
	}

	public void Trans_CoverBrushIn(int iLayerIdx, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverBrushIn(iLayerIdx, fSpeedRate));
		PushThreadFunc(eScriptFunc.TRANS_COVER_BRUSH_IN);
	}

	public void Trans_CoverBrushOut(float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverBrushOut(fSpeedRate));
		PushThreadFunc(eScriptFunc.TRANS_COVER_BRUSH_OUT);
	}

	public void Blur_GaussianIn(int iLayerIdx, float fTime)
	{
		EventCameraEffect.Instance.Activate_GaussianBlur(iLayerIdx, fTime);
		PushThreadFunc(eScriptFunc.BLUR_GAUSSIAN_IN);
	}

	public void Blur_GaussianOut(float fTime)
	{
		EventCameraEffect.Instance.Deactivate_GaussianBlur(fTime);
		PushThreadFunc(eScriptFunc.BLUR_GAUSSIAN_OUT);
	}

	public void Blur_RadialIn(int iLayerIdx, float fTime, float fMaxStrength)
	{
		EventCameraEffect.Instance.Activate_RadialBlur(iLayerIdx, fTime, fMaxStrength, 0.5f, 0.5f, isEnableVignette: true);
		PushThreadFunc(eScriptFunc.BLUR_RADIAL_IN);
	}

	public void Blur_RadialOut(float fTime)
	{
		EventCameraEffect.Instance.Deactivate_RadialBlur(fTime);
		PushThreadFunc(eScriptFunc.BLUR_RADIAL_OUT);
	}

	public void Line_BgStreamIn(int iLayerIdx, int iType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_BackgroundStream(iLayerIdx, iType, fSpeedRate));
		PushThreadFunc(eScriptFunc.LINE_BG_STREAM_IN);
	}

	public void Line_BgStreamOut(float fSpeedRate)
	{
		EventCameraEffect.Instance.Deactivate_BackgroundStream(fSpeedRate);
		PushThreadFunc(eScriptFunc.LINE_BG_STREAM_OUT);
	}

	public void Line_FocusLineIn(int iLayerIdx)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_FocusLine(iLayerIdx));
		PushThreadFunc(eScriptFunc.LINE_FOCUSLINE_IN);
	}

	public void Line_FocusLineOut()
	{
		EventCameraEffect.Instance.Deactivate_FocusLine();
		PushThreadFunc(eScriptFunc.LINE_FOCUSLINE_OUT);
	}

	public void Line_HFocusLineIn(int iLayerIdx)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_FocusLineHori(iLayerIdx));
		PushThreadFunc(eScriptFunc.LINE_HFOCUSLINE_IN);
	}

	public void Line_HFocusLineOut()
	{
		EventCameraEffect.Instance.Deactivate_FocusLineHori();
		PushThreadFunc(eScriptFunc.LINE_HFOCUSLINE_OUT);
	}

	public void Effect_DoubleIn(int iLayerIdx, float fTime, float fMaxAmount, float fX, float fY)
	{
		EventCameraEffect.Instance.Activate_DoubleVision(iLayerIdx, fTime, fMaxAmount, fX, fY);
		PushThreadFunc(eScriptFunc.EFFECT_DOUBLE_IN);
	}

	public void Effect_DoubleOut(float fTime)
	{
		EventCameraEffect.Instance.Deactivate_DoubleVision(fTime);
		PushThreadFunc(eScriptFunc.EFFECT_DOUBLE_OUT);
	}

	public void Effect_GlitchIn(int iLayerIdx, float fSpeed, float fDensity, float fMaxDisplacement)
	{
		EventCameraEffect.Instance.Activate_Glitch(iLayerIdx, fSpeed, fDensity, fMaxDisplacement);
		PushThreadFunc(eScriptFunc.EFFECT_GLITCH_IN);
	}

	public void Effect_GlitchOut()
	{
		EventCameraEffect.Instance.Deactivate_Glitch();
		PushThreadFunc(eScriptFunc.EFFECT_GLITCH_OUT);
	}

	public void Effect_WiggleIn(int iLayerIdx, float fTime, float fSpeed, float fFreq, float fAmplitude)
	{
		EventCameraEffect.Instance.Activate_Wiggle(iLayerIdx, fTime, fSpeed, fFreq, fAmplitude);
		PushThreadFunc(eScriptFunc.EFFECT_WIGGLE_IN);
	}

	public void Effect_WiggleOut(float fTime)
	{
		EventCameraEffect.Instance.Deactivate_Wiggle(fTime);
		PushThreadFunc(eScriptFunc.EFFECT_WIGGLE_OUT);
	}

	public void Effect_TVIn(int iLayerIdx, float fNoise)
	{
		EventCameraEffect.Instance.Activate_AnalogTV(iLayerIdx, fNoise);
		PushThreadFunc(eScriptFunc.EFFECT_TV_IN);
	}

	public void Effect_TVOut()
	{
		EventCameraEffect.Instance.Deactivate_AnalogTV();
		PushThreadFunc(eScriptFunc.EFFECT_TV_OUT);
	}

	public void Effect_ScreenOverlayIn(int iLayerIdx, float fTime, string strTextureName, float fMaxIntensity)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_ScreenOverlay(iLayerIdx, fTime, strTextureName, fMaxIntensity));
		PushThreadFunc(eScriptFunc.EFFECT_SCREENOVERLAY_IN);
	}

	public void Effect_ScreenOverlayOut(float fTime)
	{
		EventCameraEffect.Instance.Deactivate_ScreenOverlay(fTime);
		PushThreadFunc(eScriptFunc.EFFECT_SCREENOVERLAY_OUT);
	}

	public void Effect_CameraFlareIn(int iLayerIdx, float fTime, float fPosX, float fPosY, float fScaleX, float fScaleY, float fMaxAmount, string prefabName)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CameraFlare(iLayerIdx, fTime, fPosX, fPosY, fScaleX, fScaleY, fMaxAmount, prefabName));
		PushThreadFunc(eScriptFunc.EFFECT_CAMERA_FLARE_IN);
	}

	public void Effect_CameraFlareIn_WorldPos(int iLayerIdx, float fTime, float fPosX, float fPosY, float fScaleX, float fScaleY, float fMaxAmount, string prefabName)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CameraFlare_byWorld(iLayerIdx, fTime, fPosX, fPosY, fScaleX, fScaleY, fMaxAmount, prefabName));
		PushThreadFunc(eScriptFunc.EFFECT_CAMERA_FLARE_IN);
	}

	public void Effect_CameraFlareOut(float fTime)
	{
		EventCameraEffect.Instance.Deactivate_CameraFlare(fTime);
		PushThreadFunc(eScriptFunc.EFFECT_CAMERA_FLARE_OUT);
	}

	public void Effect_FallingStone(int iLayerIdx, string strType, float fSppedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_FallingStone(iLayerIdx, GameGlobalUtil.GetXlsScriptKeyValue(strType), fSppedRate));
		PushThreadFunc(eScriptFunc.EFFECT_FALLING_STONE);
	}

	public void Effect_FallingStoneOut()
	{
		EventCameraEffect.Instance.Deactivate_FallingStone();
	}

	public void Effect_FallingStoneBlur(int iLayerIdx, string strType, float fSppedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_FallingStoneBlur(iLayerIdx, GameGlobalUtil.GetXlsScriptKeyValue(strType), fSppedRate));
		PushThreadFunc(eScriptFunc.EFFECT_FALLING_STONE_BLUR);
	}

	public void Effect_FallingStoneBlurOut()
	{
		EventCameraEffect.Instance.Deactivate_FallingStoneBlur();
	}

	public void Effect_EyeOpenIn(int iLayerIdx, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_EyeClose(iLayerIdx, fSpeedRate));
		PushThreadFunc(eScriptFunc.EFFECT_EYE_OPEN_IN);
	}

	public void Effect_EyeOpenOut(int iLayerIdx)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_EyeOpen(iLayerIdx));
		PushThreadFunc(eScriptFunc.EFFECT_EYE_OPEN_OUT);
	}

	public void Effect_CoverConcreteIn(int iLayerIdx, string strType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverConcreteIn(iLayerIdx, GameGlobalUtil.GetXlsScriptKeyValue(strType), fSpeedRate));
		PushThreadFunc(eScriptFunc.EFFECT_COVER_CONCRETE_IN);
	}

	public void Effect_CoverConcreteOut(string strType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverConcreteOut(GameGlobalUtil.GetXlsScriptKeyValue(strType), fSpeedRate));
		PushThreadFunc(eScriptFunc.EFFECT_COVER_CONCRETE_OUT);
	}

	public void Effect_CoverConcreteBlurIn(int iLayerIdx, string strType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverConcreteBlurIn(iLayerIdx, GameGlobalUtil.GetXlsScriptKeyValue(strType), fSpeedRate));
		PushThreadFunc(eScriptFunc.EFFECT_COVER_CONCRETE_BLUR_IN);
	}

	public void Effect_CoverConcreteBlurOut(string strType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverConcreteBlurOut(GameGlobalUtil.GetXlsScriptKeyValue(strType), fSpeedRate));
		PushThreadFunc(eScriptFunc.EFFECT_COVER_CONCRETE_BLUR_OUT);
	}

	public void Trans_CoverFlatIn(int iLayerIdx, string strCoverType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverFlatIn(iLayerIdx, GameGlobalUtil.GetXlsScriptKeyValue(strCoverType), fSpeedRate));
		PushThreadFunc(eScriptFunc.TRANS_COVER_FLAT_IN);
	}

	public void Trans_CoverFlatOut(string strCoverType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverFlatOut(GameGlobalUtil.GetXlsScriptKeyValue(strCoverType), fSpeedRate));
		PushThreadFunc(eScriptFunc.TRANS_COVER_FLAT_OUT);
	}

	public void Effect_FlatBlurIn(int iLayerIdx, string strCoverType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverFlatBlurIn(iLayerIdx, GameGlobalUtil.GetXlsScriptKeyValue(strCoverType), fSpeedRate));
		PushThreadFunc(eScriptFunc.EFFECT_FLAT_BLUR_IN);
	}

	public void Effect_FlatBlurOut(string strCoverType, float fSpeedRate)
	{
		StartCoroutine(EventCameraEffect.Instance.Activate_CoverFlatBlurOut(GameGlobalUtil.GetXlsScriptKeyValue(strCoverType), fSpeedRate));
		PushThreadFunc(eScriptFunc.EFFECT_FLAT_BLUR_OUT);
	}

	public void Color_GrayIn(int iLayerIdx, float fTime, float fMaxAmount)
	{
		EventCameraEffect.Instance.Activate_GrayScale(iLayerIdx, fTime, fMaxAmount);
		PushThreadFunc(eScriptFunc.COLOR_GRAY_IN);
	}

	public void Color_GrayOut(float fTime)
	{
		EventCameraEffect.Instance.Deactivate_GrayScale(fTime);
		PushThreadFunc(eScriptFunc.COLOR_GRAY_OUT);
	}

	public void Color_NegativeIn(int iLayerIdx, float fTime, float fMaxAmount)
	{
		EventCameraEffect.Instance.Activate_Negative(iLayerIdx, fTime, fMaxAmount);
		PushThreadFunc(eScriptFunc.COLOR_NEGATIVE_IN);
	}

	public void Color_NegativeOut(float fTime)
	{
		EventCameraEffect.Instance.Deactivate_Negative(fTime);
		PushThreadFunc(eScriptFunc.COLOR_NEGATIVE_OUT);
	}

	public void Color_CurveIn(int iLayerIdx, float fTime, float fRedSteepness, float fRedGamma, float fGreenSteepness, float fGreenGamma, float fBlueSteepness, float fBlueGamma)
	{
		EventCameraEffect.Instance.Activate_ColorCurve(iLayerIdx, fTime, fRedSteepness, fRedGamma, fGreenSteepness, fGreenGamma, fBlueSteepness, fBlueGamma);
		PushThreadFunc(eScriptFunc.COLOR_CURVE_IN);
	}

	public void Color_CurveOut(float fTime)
	{
		EventCameraEffect.Instance.Deactivate_ColorCurve(fTime);
		PushThreadFunc(eScriptFunc.COLOR_CURVE_OUT);
	}

	public void Audio_Play(string strChannel, string strSoundKey, float fTime, float fVol, bool isLoop)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.Play(strChannel, strSoundKey, isSetVol: true, fTime, fVol, isLoop);
		}
	}

	public void Audio_PlayVoice(string strSoundKey)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayVoice(strSoundKey, isSetVol: true, 0f, 1f);
		}
	}

	public void Audio_PlayRingtone(string strChannel, float fTime, float fVol, bool isLoop)
	{
		Xls.CollSounds data_byIdx = Xls.CollSounds.GetData_byIdx(m_GameSwitch.GetSwRingtone());
		SWatchConfigMenu.PlayRingSound(data_byIdx, strChannel, fTime, setVolumn: true, fVol, isLoop);
	}

	public void Audio_Pause(string strChannel)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PauseSound(strChannel);
		}
	}

	public void Audio_Stop(string strChannel)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.Stop(strChannel);
		}
	}

	public void Audio_Vol(string strChannel, float fTime, float fVol)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.SetVol(strChannel, fVol, fTime);
		}
	}

	public void Audio_SetSnapShot(int iMixerIdx, string strSnapshotName, float fTransitionTime)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.SetSnapShot(iMixerIdx, strSnapshotName, fTransitionTime);
		}
	}

	public void Audio_SetOutputMixerGroup(string strChannel, int iMixerIdx, string strMixerGroupName)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.SetOutputMixer(strChannel, iMixerIdx, strMixerGroupName);
		}
	}

	public void Video_Play(string strFileNameKey)
	{
		m_EventEngine.StopSkip();
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayVideo(strFileNameKey);
		}
		PushThreadFunc(eScriptFunc.VIDEO_PLAY);
	}

	public void SetSequenceIdx(int iIdx, bool isShowEff)
	{
		m_GameSwitch.SetCurSequence(iIdx);
		if (isShowEff)
		{
			m_GameMain.ShowSequenceStart();
			PushThreadFunc(eScriptFunc.SHOW_SEQUENCE_START);
		}
	}

	public void CheckEvtAndAddPost(string strPostKey, bool isMoveToAddedContent)
	{
		m_GameSwitch.CheckEvtAndAddPost(strPostKey, isMoveToAddedContent, 0.5f, 3);
		PushThreadFunc(eScriptFunc.ADD_FATER_CONTENT);
	}

	public void CheckEvtAndAddPost2(string strPostKey, bool isMoveToAddedContent, float moveTime, int moveType)
	{
		m_GameSwitch.CheckEvtAndAddPost(strPostKey, isMoveToAddedContent, moveTime, moveType);
		PushThreadFunc(eScriptFunc.ADD_FATER_CONTENT);
	}

	public void ReplaceEvtPost(string strPostOrgKey, string strPostReplaceKey)
	{
		m_GameSwitch.ReplacePostSwitch(strPostOrgKey, strPostReplaceKey);
	}

	public void A_SetSequenceIdx(int iIdx)
	{
		m_GameSwitch.SetCurSequence(iIdx);
		CallEventScriptEnd();
	}

	public void ExitSequence(bool isShowEff)
	{
		if (isShowEff)
		{
			m_GameMain.ShowSequenceEnd();
			PushThreadFunc(eScriptFunc.SHOW_SEQUENCE_END);
		}
	}

	public void SetPhaseIdx(int iIdx)
	{
		m_GameSwitch.SetCurPhase(iIdx);
	}

	public void OpenFaterReply(string snsTipTextKey)
	{
		GameSwitch.GetInstance().SetOnlyPostTalkWindowString(snsTipTextKey);
		PushThreadFunc(eScriptFunc.FATER_OPEN_REPLY);
		m_GameMain.StartCoroutine(m_GameMain.ShowSNSMenu(isShow: true, SNSMenuPlus.Mode.Reply, OnClosed_SNSMenu));
		m_isActivatedSNSMenu = true;
	}

	public void OpenFaterKeyword(string snsTipTextKey)
	{
		GameSwitch.GetInstance().SetOnlyPostTalkWindowString(snsTipTextKey);
		PushThreadFunc(eScriptFunc.FATER_OPEN_KEYWORD);
		m_GameMain.StartCoroutine(m_GameMain.ShowSNSMenu(isShow: true, SNSMenuPlus.Mode.Keyword, OnClosed_SNSMenu));
		m_isActivatedSNSMenu = true;
	}

	private void OnClosed_SNSMenu(object sender, object args)
	{
		m_isActivatedSNSMenu = false;
	}

	public void AddFaterRT(string strFater, int iAddRT)
	{
		m_GameSwitch.AddPostRT(strFater, iAddRT);
	}

	public void FaterFloatingOn(string strPostKey, float fX, float fY, float fScale, int iLayerIdx, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "")
	{
		FloatingUIHandler.ShowSNSPost(strPostKey, fX, fY, fScale, iLayerIdx, fRotX, fRotY, fRotZ, tag);
		PushThreadFunc(eScriptFunc.FATER_FLOATING_ON);
	}

	public void FaterFloatingOn_WorldPos(string strPostKey, float fX, float fY, float fScale, int iLayerIdx, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "")
	{
		FloatingUIHandler.ShowSNSPost_byScreen(strPostKey, fX, fY, fScale, iLayerIdx, fRotX, fRotY, fRotZ, tag);
		PushThreadFunc(eScriptFunc.FATER_FLOATING_ON);
	}

	public void FaterFloatingScroll(string strPostKey, float fX, float fY, float fScale, int iLayerIdx, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "", float fMoveTime = 0f, string moveTypeKey = "")
	{
		MainLoadThing.instance.StartCoroutine(FloatingUIHandler.ShowSNSPost_Scroll(strPostKey, fX, fY, fScale, iLayerIdx, fRotX, fRotY, fRotZ, tag, fMoveTime, moveTypeKey));
		PushThreadFunc(eScriptFunc.FATER_FLOATING_ON);
	}

	public void FaterFloatingOff(float fSpeed)
	{
		FloatingUIHandler.HideFloatingUI_All(fSpeed);
		PushThreadFunc(eScriptFunc.FATER_FLOATING_OFF);
	}

	public void FaterRetweet(string strPostKey)
	{
		m_GameSwitch.SetPostRetweet(strPostKey, 1);
	}

	public void SetCutID(string strCutID)
	{
		m_GameSwitch.StartCutEff(strCutID);
		CallEventScriptEnd();
	}

	public void SetCutIDForConti(string strCutID)
	{
		m_GameSwitch.StartCutEff(strCutID);
	}

	public void FaterTalkWindowStringSet()
	{
		int curCutIdx = m_GameSwitch.GetCurCutIdx();
		Xls.TalkCutSetting data_bySwitchIdx = Xls.TalkCutSetting.GetData_bySwitchIdx(curCutIdx);
		if (data_bySwitchIdx != null)
		{
			m_GameSwitch.SetPostTalkWindowString(data_bySwitchIdx.m_strSNSID);
			m_GameMain.ShowTalkCutStart();
			PushThreadFunc(eScriptFunc.SHOW_TALK_START);
		}
	}

	public void A_CutStartEffOnly()
	{
		int curCutIdx = m_GameSwitch.GetCurCutIdx();
		Xls.TalkCutSetting data_bySwitchIdx = Xls.TalkCutSetting.GetData_bySwitchIdx(curCutIdx);
		if (data_bySwitchIdx != null)
		{
			m_GameSwitch.SetOnlyPostTalkWindowString(data_bySwitchIdx.m_strSNSID);
			m_GameMain.ShowTalkCutStart();
			PushThreadFunc(eScriptFunc.SHOW_TALK_START);
		}
	}

	public void ExitCutEff()
	{
		m_GameSwitch.InitCutEff();
		m_GameMain.ShowTalkCutResult();
		m_GameSwitch.SetAllKeywordUsed();
		PushThreadFunc(eScriptFunc.SHOW_TALK_RESULT);
	}

	public void MsgMenuOpen()
	{
		m_GameMain.ShowMessengerMenu(isShow: true);
		PushThreadFunc(eScriptFunc.MSG_OPEN);
	}

	public void MsgFloatingOn(string strMsgKey, float fX, float fY, float fScale, bool isFirstTalk, int iLayerIdx, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "")
	{
		m_GameSwitch.SetMessage(strMsgKey, 1);
		FloatingUIHandler.ShowMSGTalk(strMsgKey, fX, fY, fScale, isFirstTalk, iLayerIdx, fRotX, fRotY, fRotZ, tag);
		PushThreadFunc(eScriptFunc.MSG_FLOATING_ON);
	}

	public void MsgFloatingOn_WorldPos(string strMsgKey, float fX, float fY, float fScale, bool isFirstTalk, int iLayerIdx, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "")
	{
		m_GameSwitch.SetMessage(strMsgKey, 1);
		FloatingUIHandler.ShowMSGTalk_byScreen(strMsgKey, fX, fY, fScale, isFirstTalk, iLayerIdx, fRotX, fRotY, fRotZ, tag);
		PushThreadFunc(eScriptFunc.MSG_FLOATING_ON);
	}

	public void MsgFloatingOff(float fSpeed)
	{
		FloatingUIHandler.HideFloatingUI_All(fSpeed);
		PushThreadFunc(eScriptFunc.MSG_FLOATING_OFF);
	}

	public void MsgFloatingDelayedOn(string strMsgKey, float fX, float fY, float fScale, bool isFirstTalk, int iLayerIdx, float fRotX, float fRotY, float fRotZ, string strTag, float fDelayTime)
	{
		FloatingUIHandler.ShowMSGTalk_withInput(strMsgKey, fDelayTime, fX, fY, fScale, isFirstTalk, iLayerIdx, fRotX, fRotY, fRotZ, strTag);
		PushThreadFunc(eScriptFunc.MSG_FLOATING_DELAY_ON);
	}

	public void MsgFloatingDelayedOn_WorldPos(string strMsgKey, float fX, float fY, float fScale, bool isFirstTalk, int iLayerIdx, float fRotX, float fRotY, float fRotZ, string strTag, float fDelayTime)
	{
		FloatingUIHandler.ShowMSGTalk_byScreen_withInput(strMsgKey, fDelayTime, fX, fY, fScale, isFirstTalk, iLayerIdx, fRotX, fRotY, fRotZ, strTag);
		PushThreadFunc(eScriptFunc.MSG_FLOATING_DELAY_ON);
	}

	public void MsgFloatingDelayedCancel(string strMsgKey, float fX, float fY, float fScale, bool isFirstTalk, int iLayerIdx, float fRotX, float fRotY, float fRotZ, float fDelayTime)
	{
		FloatingUIHandler.ShowMSGTalk_onlyInput(strMsgKey, fDelayTime, fX, fY, fScale, isFirstTalk, iLayerIdx, fRotX, fRotY, fRotZ, string.Empty);
		PushThreadFunc(eScriptFunc.MSG_FLOATING_DELAY_CANCEL);
	}

	public void MsgFloatingScroll(string strMsgKey, float fX, float fY, float fScale, bool isFirstTalk, int iLayerIdx, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "", float fMoveTime = 0f, string moveTypeKey = "")
	{
		m_GameSwitch.SetMessage(strMsgKey, 1);
		MainLoadThing.instance.StartCoroutine(FloatingUIHandler.ShowMSGTalk_Scroll(strMsgKey, fX, fY, fScale, isFirstTalk, iLayerIdx, fRotX, fRotY, fRotZ, tag, fMoveTime, moveTypeKey));
		PushThreadFunc(eScriptFunc.MSG_FLOATING_ON);
	}

	public void MsgFloatingDelayedScroll(string strMsgKey, float fX, float fY, float fScale, bool isFirstTalk, int iLayerIdx, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "", float fDelayTime = 0f, float fMoveTime = 0f, string moveTypeKey = "")
	{
		m_GameSwitch.SetMessage(strMsgKey, 1);
		MainLoadThing.instance.StartCoroutine(FloatingUIHandler.ShowMSGTalk_withInput_Scroll(strMsgKey, fDelayTime, fX, fY, fScale, isFirstTalk, iLayerIdx, fRotX, fRotY, fRotZ, tag, fMoveTime, moveTypeKey));
		PushThreadFunc(eScriptFunc.MSG_FLOATING_DELAY_ON);
	}

	public void MsgFloatingCancelScroll(string strMsgKey, float fX, float fY, float fScale, bool isFirstTalk, int iLayerIdx, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "", float fDelayTime = 0f, float fMoveTime = 0f, string moveTypeKey = "")
	{
		MainLoadThing.instance.StartCoroutine(FloatingUIHandler.ShowMSGTalk_onlyInput_Scroll(strMsgKey, fDelayTime, fX, fY, fScale, isFirstTalk, iLayerIdx, fRotX, fRotY, fRotZ, tag, fMoveTime, moveTypeKey));
		PushThreadFunc(eScriptFunc.MSG_FLOATING_DELAY_CANCEL);
	}

	public void MsgOn(string strMsgKey)
	{
		m_GameSwitch.SetMessage(strMsgKey, 1);
	}

	public void MsgSwitchStateWritable(bool isOn)
	{
		m_GameSwitch.SetMessageSwitchSettable(isOn);
	}

	public void FloatingUIEvent_Disappear(string tag, float speedRate)
	{
		FloatingUIHandler.SetEvent_Disappear(tag, speedRate);
		PushThreadFunc(eScriptFunc.FLOATING_EVENT_DISAPPEAR);
	}

	public void FloatingUIEvent_Move(string tag, float time, float moveX, float moveY, string moveTypeKey)
	{
		int moveType = Xls.ScriptKeyValue.GetData_byKey(moveTypeKey)?.m_iValue ?? 0;
		FloatingUIHandler.SetEvent_Move(tag, time, moveX, moveY, moveType);
		PushThreadFunc(eScriptFunc.FLOATING_EVENT_MOVE);
	}

	public void FloatingUIEvent_Rotate(string tag, float time, float rotX, float rotY, float rotZ, string moveTypeKey)
	{
		int moveType = Xls.ScriptKeyValue.GetData_byKey(moveTypeKey)?.m_iValue ?? 0;
		FloatingUIHandler.SetEvent_Rotate(tag, time, rotX, rotY, rotZ, moveType);
		PushThreadFunc(eScriptFunc.FLOATING_EVENT_ROTATE);
	}

	public void FloatingUIEvent_Zoom(string tag, float time, float zoom, string moveTypeKey)
	{
		int moveType = Xls.ScriptKeyValue.GetData_byKey(moveTypeKey)?.m_iValue ?? 0;
		FloatingUIHandler.SetEvent_Zoom(tag, time, zoom, moveType);
		PushThreadFunc(eScriptFunc.FLOATING_EVENT_ZOOM);
	}

	public void FloatingUIEvent_Motion(string tag, string motionName, float speedRate, int loopCount)
	{
		FloatingUIHandler.SetEvent_Motion(tag, motionName, speedRate, loopCount);
		PushThreadFunc(eScriptFunc.FLOATING_EVENT_MOTION);
	}

	public void FloatingContainerRotation(int iLayerIdx, float fRotX, float fRotY, float fRotZ)
	{
		FloatingUIHandler.SetContainerRotation(iLayerIdx, fRotX, fRotY, fRotZ);
	}

	public void GoToGameScene()
	{
		int curCutIdx = m_GameSwitch.GetCurCutIdx();
		Xls.TalkCutSetting data_bySwitchIdx = Xls.TalkCutSetting.GetData_bySwitchIdx(curCutIdx);
		if (data_bySwitchIdx != null)
		{
			bool flag = false;
			StartCoroutine(m_EventEngine.m_TalkChar.ShowPartyChar(isShowTalkIcon: false));
			flag = true;
			m_isGoToGameSceneSetFadeOut = false;
			if (flag)
			{
				PushThreadFunc(eScriptFunc.GOTO_GAME_SCENE_SHOW_PARTY);
			}
			else
			{
				CallEventScriptEnd(eScriptFunc.GOTO_GAME_SCENE_SHOW_PARTY);
			}
		}
	}

	public void KeywordOpen(string strCharKey, bool isShowBackButton, bool isFinalSeq)
	{
		m_GameMain.ShowKeywordMenu(isShow: true, isKeywordMenu: false, strCharKey, isFromKeywordEvt: true, isShowBackButton, isFinalSeq);
	}

	public void KeywordSetCrisisLevel(int iLevel)
	{
		m_GameMain.m_KeywordMenu.SetCrisisLevel(iLevel);
	}

	public void A_SetKeywordSetCrisisLevel(int iLevel)
	{
		m_GameMain.m_KeywordMenu.SetCrisisLevel(iLevel);
		CallEventScriptEnd();
	}

	public void KeywordUse(string strKeywordID, int iSelCnt)
	{
		m_EventEngine.StopSkip(isShowSkipBut: false, isForce: true);
		m_EventEngine.SetShowSkipBut(isShowSkipBut: false);
		m_GameMain.ShowKeywordUseMenu(isShow: true, isReUse: false, strKeywordID, iSelCnt);
		PushThreadFunc(eScriptFunc.SET_KEYWORD_USE);
	}

	public void KeywordReUse()
	{
		m_EventEngine.StopSkip(isShowSkipBut: false, isForce: true);
		m_GameMain.ShowKeywordUseMenu(isShow: true, isReUse: true);
		PushThreadFunc(eScriptFunc.SET_KEYWORD_REUSE);
	}

	public void CheckKeywordAnswer(string strQuestID, string strAns0, string strAns1, string strAns2, string strAns3, string strAns4, string strAns5, string strAns6, string strAns7, string strAns8, string strAns9)
	{
		bool isTrue = m_GameMain.m_KeywordMenu.CheckKeywordUsing(strQuestID, strAns0, strAns1, strAns2, strAns3, strAns4, strAns5, strAns6, strAns7, strAns8, strAns9);
		CallTrueFalse(isTrue);
	}

	public string GetKeywordAnswer(string strQuestID)
	{
		string[] keywordUseAnswer = m_GameMain.m_KeywordMenu.GetKeywordUseAnswer(strQuestID);
		int num = keywordUseAnswer.Length;
		string text = string.Empty;
		for (int i = 0; i < num; i++)
		{
			if (keywordUseAnswer[i] != null)
			{
				text = text + keywordUseAnswer[i] + ",";
			}
		}
		CallEventScriptEnd();
		return text;
	}

	public void CheckKeywordAnswerEach(string strQuestID, string strAns0, string strAns1, string strAns2, string strAns3, string strAns4, string strAns5, string strAns6, string strAns7, string strAns8, string strAns9)
	{
		KeywordMenuPlus.ReturnCheckAns returnAns = m_GameMain.m_KeywordMenu.CheckKeywordUsingEach(strQuestID, strAns0, strAns1, strAns2, strAns3, strAns4, strAns5, strAns6, strAns7, strAns8, strAns9);
		CallKeywordUsingCheck(returnAns);
	}

	public void CheckKeywordQuestion(string strQuestID)
	{
		bool isTrue = m_GameMain.m_KeywordMenu.CheckLastQuestID(strQuestID);
		CallTrueFalse(isTrue);
	}

	public void KeywordExplainOn(bool isOn, string strQuestID)
	{
		if (isOn)
		{
			m_GameMain.m_KeywordUseExplain.SetQuestID(strQuestID);
		}
		else
		{
			m_EventEngine.CloseKeywordUseExplain();
		}
		PushThreadFunc(eScriptFunc.SET_KEYWORD_EXPLAIN_ONOFF);
	}

	public void KeywordSwitchCheck(string strKeywordKey)
	{
		bool isTrue = m_GameSwitch.GetKeywordAllState(strKeywordKey) >= 1;
		CallTrueFalse(isTrue);
	}

	public void KeywordExplainState(string strKeywordKey, bool isRightAnswer)
	{
		m_GameMain.m_KeywordUseExplain.SetSelAnswer(strKeywordKey, isRightAnswer);
		PushThreadFunc(eScriptFunc.SET_KEYWORD_EXPLAIN_STATE);
	}

	public void KeywordExplainOX()
	{
		m_GameMain.m_KeywordUseExplain.SetOXMotion();
		PushThreadFunc(eScriptFunc.SET_KEYWORD_EXPLAIN_OX_MOT);
	}

	public void SaveGame(int iSlotIdx)
	{
	}

	public void SaveStartObj(string strStartObj, string strSendEvent)
	{
		m_EventEngine.StopSkip();
		m_isSaveStartObjComp = false;
		PushThreadFunc(eScriptFunc.SAVE_START_OBJ);
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("PM_FUNC_A_SAVE_EMPTY");
		if (xlsProgramDefineStr != null && strStartObj == xlsProgramDefineStr)
		{
			m_GameSwitch.SetRunEventObj(GameSwitch.eEventRunType.NONE, null, null, isForEventSave: true);
		}
		else
		{
			m_GameSwitch.SetRunEventObj(GameSwitch.eEventRunType.ENABLE_OBJ, strStartObj, strSendEvent, isForEventSave: true);
		}
		NoticeUIManager.ActiveNotice_S(NoticeUIManager.NoticeType.Normal, GameGlobalUtil.GetXlsProgramText("NOTICE_AUTO_SAVE_ALLERT"));
		m_SaveLoad.SaveLoadWhat(SaveLoad.eSaveWhat.eSaveGameInfoColl, 0, OnCompSaveStartObj);
	}

	public void SaveCollection()
	{
		m_isSaveStartObjComp = false;
		PushThreadFunc(eScriptFunc.SAVE_COLLECTION);
		m_SaveLoad.SaveLoadWhat(SaveLoad.eSaveWhat.eSaveColl, 0, OnCompSaveStartObj);
	}

	public void OnCompSaveStartObj(bool isExistErr)
	{
		m_isSaveStartObjComp = true;
	}

	public void LoadGame(int iSlotIdx)
	{
		m_isLoadGame = false;
		PushThreadFunc(eScriptFunc.LOAD_GAME);
		m_GameMain.SetFullCover(isActive: true);
		m_GameMain.ReLoadGameScene(iSlotIdx, isEvent: true, OnLoadLevelDone);
	}

	private void OnLoadLevelDone()
	{
		m_isLoadGame = true;
	}

	public void Game_GoMainMenu()
	{
		m_EventEngine.SetGoMainMenu(isGoMainMenu: true);
	}

	public void Game_Synch()
	{
	}

	public void Game_SetGameMainType(string strGameMainType)
	{
		GameMain.SetDefaultSceneType(strGameMainType);
	}

	public void SetKeyTalkSign()
	{
		SetDialogType_Keyword(m_GameMain.m_KeywordMenu.m_strRunKeywordKey);
	}

	public void SetDialogType_Keyword(string strKeyword)
	{
		if (m_GameMain.m_clConversationSign.SetDialogSignType(ConversationSign.eDialogSignType.Keyword, strKeyword))
		{
			PushThreadFunc(eScriptFunc.CONVERSATION_SIGN_KEYWORD);
		}
		else
		{
			CallEventScriptEnd(eScriptFunc.CONVERSATION_SIGN_KEYWORD);
		}
	}

	public void SetDialogType_Profile(string strProfile)
	{
		if (m_GameMain.m_clConversationSign.SetDialogSignType(ConversationSign.eDialogSignType.Profile, strProfile))
		{
			PushThreadFunc(eScriptFunc.CONVERSATION_SIGN_PROFILE);
		}
		else
		{
			CallEventScriptEnd(eScriptFunc.CONVERSATION_SIGN_PROFILE);
		}
	}

	public void SetDialogType()
	{
		if (m_GameMain.m_clConversationSign.SetDialogSignType(ConversationSign.eDialogSignType.Off))
		{
			PushThreadFunc(eScriptFunc.CONVERSATION_SIGN_OFF);
		}
		else
		{
			CallEventScriptEnd(eScriptFunc.CONVERSATION_SIGN_OFF);
		}
	}

	public void SetTutoMenuObj(int iTutoIdx)
	{
		m_GameSwitch.SetTutoMenuObj(iTutoIdx);
	}

	public void A_SetTutoMenuObj(int iTutoIdx)
	{
		m_GameSwitch.SetTutoMenuObj(iTutoIdx);
		CallEventScriptEnd();
	}

	public void SplitScreenReation(string strMotion)
	{
		StartCoroutine(SplitScreenReaction.Show(strMotion));
		PushThreadFunc(eScriptFunc.SPLIT_SCREEN_REACTION);
	}

	public void ShareScreenShot(bool isOn)
	{
	}

	public void ShareRecordLive(bool isOn)
	{
	}

	public void PlayStaffRoll(string strStaffRoll)
	{
		StaffRoll.instance.SetStaffRoll(strStaffRoll);
		PushThreadFunc(eScriptFunc.STAFF_ROLL);
	}

	public void PlayTutorial(string strTutorial)
	{
		if (TutorialPopup.isShowAble(strTutorial))
		{
			StartCoroutine(TutorialPopup.Show(strTutorial));
			m_EventEngine.StopSkip(isShowSkipBut: false, isForce: true);
			PushThreadFunc(eScriptFunc.TUTORIAL_POP);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	public void IntroduceCharEffAppear(string strCharKey, string strTextListKey)
	{
		if (CHR_Introduce.GetRankNum(strCharKey, strTextListKey) != 0)
		{
			StartCoroutine(CHR_Introduce.Show(strCharKey, strTextListKey));
			PushThreadFunc(eScriptFunc.INTRODUCE_CHAR_APPEAR);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	public void IntroduceCharEffDisappear()
	{
		if (CHR_Introduce.Disappear())
		{
			PushThreadFunc(eScriptFunc.INTRODUCE_CHAR_DISAPPEAR);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	public void BroadcastLogoAppear(string strLogoType, string strTextListKey)
	{
		if (EFF_Broadcast.isShowAble(strLogoType, strTextListKey))
		{
			StartCoroutine(EFF_Broadcast.Show(strLogoType, strTextListKey));
			PushThreadFunc(eScriptFunc.BROADCAST_TITLE_APPEAR);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	public void BroadcastLogoDisappear()
	{
		if (EFF_Broadcast.Disappear())
		{
			PushThreadFunc(eScriptFunc.BROADCAST_TITLE_DISPPEAR);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	public void SetTrickObjTime(string strAMPM, int iH, int iM)
	{
		m_GameSwitch.SetTrickObjTime(GameGlobalUtil.GetXlsScriptKeyValue(strAMPM), iH, iM);
	}

	public void SetPopup(string strTextKey, bool isYesNoPop)
	{
		Xls.TextData data_byKey = Xls.TextData.GetData_byKey(strTextKey);
		if (data_byKey != null)
		{
			m_EventEngine.StopSkip(isShowSkipBut: false, isForce: true);
			string strTxt = data_byKey.m_strTxt;
			if (isYesNoPop)
			{
				PopupDialoguePlus.ShowPopup_YesNo(strTxt, CallBack_PopupResult);
			}
			else
			{
				PopupDialoguePlus.ShowPopup_OK(strTxt, CallBack_PopupResult);
			}
		}
		else if (isYesNoPop)
		{
			CallTrueFalse(isTrue: false);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	private void CallBack_PopupResult(PopupDialoguePlus.Result result)
	{
		m_EventEngine.SetShowSkipBut(isShowSkipBut: true);
		switch (result)
		{
		case PopupDialoguePlus.Result.OK:
			CallEventScriptEnd();
			break;
		case PopupDialoguePlus.Result.Yes:
		case PopupDialoguePlus.Result.No:
			CallTrueFalse(result == PopupDialoguePlus.Result.Yes);
			break;
		}
	}

	public void SetActionPopup(string strTextKey, bool isYesNoPop)
	{
		Xls.TextData data_byKey = Xls.TextData.GetData_byKey(strTextKey);
		if (data_byKey != null)
		{
			m_EventEngine.StopSkip(isShowSkipBut: false, isForce: true);
			string strTxt = data_byKey.m_strTxt;
			if (isYesNoPop)
			{
				PopupDialoguePlus.ShowActionPopup_YesNo(strTxt, CallBack_PopupResult);
			}
			else
			{
				PopupDialoguePlus.ShowActionPopup_OK(strTxt, CallBack_PopupResult);
			}
		}
		else if (isYesNoPop)
		{
			CallTrueFalse(isTrue: false);
		}
		else
		{
			CallEventScriptEnd();
		}
	}

	private int ChangeStringToIntColor(string strColor)
	{
		int result = 16777215;
		if (strColor.Length > 6)
		{
			strColor = strColor.Substring(0, 6);
		}
		if (strColor.Length == 6)
		{
			string text = strColor.Substring(0, 2);
			string text2 = strColor.Substring(2, 2);
			string text3 = strColor.Substring(4, 2);
			result = int.Parse(text + text2 + text3, NumberStyles.HexNumber);
		}
		return result;
	}

	public void SetCharLuxIn(string strCharKey, string strColor, float fTime)
	{
		m_EventEngine.m_TalkChar.SetCharLux(strCharKey, ChangeStringToIntColor(strColor), fTime);
		if (strCharKey == "한도윤")
		{
			SetObjLuxIn("CHR_han", strColor, fTime);
		}
		PushThreadFunc(eScriptFunc.CHAR_LUX_IN);
	}

	public void SetCharLuxOut(string strCharKey, float fTime)
	{
		m_EventEngine.m_TalkChar.SetCharLux(strCharKey, 16777215, fTime);
		if (strCharKey == "한도윤")
		{
			SetObjLuxOut("CHR_han", fTime);
		}
		PushThreadFunc(eScriptFunc.CHAR_LUX_OUT);
	}

	public void SetObjLuxIn(string strObjName, string strColor, float fTime)
	{
		StartCoroutine(m_EventEngine.m_EventObject.SetObjLux(strObjName, ChangeStringToIntColor(strColor), fTime));
		PushThreadFunc(eScriptFunc.OBJ_LUX_IN);
	}

	public void SetObjLuxOut(string strObjName, float fTime)
	{
		StartCoroutine(m_EventEngine.m_EventObject.SetObjLux(strObjName, 16777215, fTime));
		PushThreadFunc(eScriptFunc.OBJ_LUX_OUT);
	}

	public void VitaDistantViewIn(string strKeyImageData)
	{
		PushThreadFunc(eScriptFunc.VITA_DISTANT_VIEW_ON);
		StartCoroutine(RenderManager.instance.ActivateBGImage(strKeyImageData));
	}

	public void VitaDistantViewOut()
	{
		RenderManager.instance.DeactivateBGImage();
	}

	public void VitaDistanctViewMove(float fX, float fY, float fTime, string strType)
	{
		RenderManager.instance.MoveBGImage(fX, fY, fTime, GameGlobalUtil.GetXlsScriptKeyValue(strType));
		PushThreadFunc(eScriptFunc.VITA_DISTANT_VIEW_MOVE);
	}

	public void VitaDistantViewRotate(float fX, float fY, float fTime, string strType)
	{
		RenderManager.instance.RotateBGImage(fX, fY, fTime, GameGlobalUtil.GetXlsScriptKeyValue(strType));
		PushThreadFunc(eScriptFunc.VITA_DISTANT_VIEW_ROTATE);
	}

	public void VitaDistantViewZoom(float fZoomFactor, float fTime, string strType)
	{
		RenderManager.instance.ZoomBGImage(fZoomFactor, fTime, GameGlobalUtil.GetXlsScriptKeyValue(strType));
		PushThreadFunc(eScriptFunc.VITA_DISTANT_VIEW_ZOOM);
	}

	public void ShowEnding()
	{
		m_GameSwitch.SetShowEnding(isShowEnding: true);
	}
}
