using System;
using System.Collections;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class SWatchMenuPlus : MonoBehaviour
{
	public enum Modes
	{
		Unknown = -1,
		Home,
		SNS,
		Messenger,
		Rank,
		Memo,
		Address,
		AutoWrite,
		Phone,
		Record,
		Config,
		Count
	}

	public enum SpecialAniState
	{
		move_left_start,
		move_left_appear,
		move_left_idle,
		move_left_return,
		move_smaller_start,
		move_smaller_idle,
		move_smaller_return,
		move_smaller_appear,
		move_smaller_disappear
	}

	public enum State
	{
		Hided,
		Idle,
		Appear,
		Disappear,
		IdleLeftSide,
		MoveToLeftSide,
		MoveToCenter,
		IdleSmall,
		MoveToSmall,
		MoveToNormal,
		AppearSmall,
		DisappearSmall
	}

	public enum ModeState
	{
		Unknown,
		Idle,
		Appear,
		Disappear,
		WaitForNextMode
	}

	[Serializable]
	public class HomeMenuButton
	{
		public Button m_Button;

		public Text m_ButtonText;

		public GameObject m_SelectionCursor;

		public Button m_SelectionIconButton;

		public GameObject m_BannerObj;

		public Text m_BannerText;

		public GameObject m_LinkedMenuRoot;
	}

	public GameObject m_RootObject;

	public GameObject m_BackGound;

	public GameObject m_BGUIImageObj;

	public Image m_BGCoverImage;

	public GameObject m_WatchRootObject;

	public GameObject m_InputBlockObject;

	public Animator m_BackButtonAnimator;

	public GameMain m_GameMain;

	[Header("Title")]
	public GameObject m_TitleRoot;

	public Text m_TitleText;

	[Header("Home Menu Buttons")]
	public HomeMenuButton m_SNSButton = new HomeMenuButton();

	public HomeMenuButton m_MessengerButton = new HomeMenuButton();

	public HomeMenuButton m_RankButton = new HomeMenuButton();

	public HomeMenuButton m_MemoButton = new HomeMenuButton();

	public HomeMenuButton m_AddressButton = new HomeMenuButton();

	public HomeMenuButton m_AutoWriteButton = new HomeMenuButton();

	public HomeMenuButton m_PhoneButton = new HomeMenuButton();

	public HomeMenuButton m_RecordButton = new HomeMenuButton();

	public HomeMenuButton m_ConfigButton = new HomeMenuButton();

	private HomeMenuButton[] m_HomeMenuButtons;

	private HomeMenuButton m_OnCursorButton;

	public float m_ScrollRepeatTime = 0.2f;

	private float m_fScrollButtonPusingTime;

	[Header("Home Menu Others")]
	public Text m_TimeText;

	public Image m_BGImage;

	[Header("Watch Sub Contents")]
	public GameObject m_HomeModeRoot;

	public GameObject m_SNSModeRoot;

	public GameObject m_MessengerModeRoot;

	public GameObject m_RankModeRoot;

	public GameObject m_MemoModeRoot;

	public GameObject m_AddressModeRoot;

	public GameObject m_AutoWriteModeRoot;

	public GameObject m_PhoneModeRoot;

	public GameObject m_RecordModeRoot;

	public GameObject m_ConfigModeRoot;

	private GameObject[] m_SubModeRoots;

	private const string c_XlsDataName_TitleText = "SMART_WATCH_TITLE";

	private const string c_XlsDataName_TimeFormat = "SMART_WATCH_TIME_FORMAT";

	private const string c_XlsDataName_ButtonTextSNS = "SMART_WATCH_BUTTON_SNS";

	private const string c_XlsDataName_ButtonTextMSG = "SMART_WATCH_BUTTON_MSG";

	private const string c_XlsDataName_ButtonTextRank = "SMART_WATCH_BUTTON_RANK";

	private const string c_XlsDataName_ButtonTextMeno = "SMART_WATCH_BUTTON_MEMO";

	private const string c_XlsDataName_ButtonTextAddress = "SMART_WATCH_BUTTON_ADDRESS";

	private const string c_XlsDataName_ButtonTextAutoWrite = "SMART_WATCH_BUTTON_AUTOWRITE";

	private const string c_XlsDataName_ButtonTextPhone = "SMART_WATCH_BUTTON_PHONE";

	private const string c_XlsDataName_ButtonTextRecord = "SMART_WATCH_BUTTON_RECORD";

	private const string c_XlsDataName_ButtonTextConfig = "SMART_WATCH_BUTTON_CONFIG";

	private State m_curState;

	private ModeState m_curModeState;

	private Modes m_curMode = Modes.Unknown;

	private Modes m_nextMode = Modes.Unknown;

	private bool m_isChangeMode_EnableAnimation;

	private Animator m_AnimatorWatchSelf;

	private Animator m_AnimatorModeStateCheck;

	private GameDefine.EventProc m_fpClosedCallBack;

	private AudioManager m_AudioManager;

	private bool m_isInputedByPadOrKey;

	private CommonButtonGuide m_ButtonGuide;

	private bool m_isInitializedButtonGuidText;

	private string m_buttonGuideText_MoveCursor = string.Empty;

	private string m_buttonGuideText_Submit = string.Empty;

	private string m_buttonGuideText_Exit = string.Empty;

	private bool m_isShowPhoneFromSmartMenu;

	private bool m_isPlaingBGCoverTrans;

	private bool m_isBGCoverTrans_Appear;

	private float m_bgCoverTrans_TargetTime;

	private float m_bgCoverTrans_CurrentTime;

	private float m_bgCoverTrans_DefalutTime;

	public State curState => m_curState;

	public Animator animatorWatchSelf => m_AnimatorWatchSelf;

	private void Awake()
	{
		int num = 10;
		m_HomeMenuButtons = new HomeMenuButton[num];
		m_HomeMenuButtons[1] = m_SNSButton;
		m_HomeMenuButtons[2] = m_MessengerButton;
		m_HomeMenuButtons[3] = m_RankButton;
		m_HomeMenuButtons[4] = m_MemoButton;
		m_HomeMenuButtons[5] = m_AddressButton;
		m_HomeMenuButtons[6] = m_AutoWriteButton;
		m_HomeMenuButtons[7] = m_PhoneButton;
		m_HomeMenuButtons[8] = m_RecordButton;
		m_HomeMenuButtons[9] = m_ConfigButton;
		m_SubModeRoots = new GameObject[num];
		m_SubModeRoots[0] = m_HomeModeRoot;
		m_SubModeRoots[1] = m_SNSModeRoot;
		m_SubModeRoots[2] = m_MessengerModeRoot;
		m_SubModeRoots[3] = m_RankModeRoot;
		m_SubModeRoots[4] = m_MemoModeRoot;
		m_SubModeRoots[5] = m_AddressModeRoot;
		m_SubModeRoots[6] = m_AutoWriteModeRoot;
		m_SubModeRoots[7] = m_PhoneModeRoot;
		m_SubModeRoots[8] = m_RecordModeRoot;
		m_SubModeRoots[9] = m_ConfigModeRoot;
		GameObject gameObject = null;
		for (int i = 0; i < num; i++)
		{
			gameObject = m_SubModeRoots[i];
			if (gameObject == null)
			{
			}
		}
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		InitTextComponents();
	}

	private void OnDestroy()
	{
		m_SubModeRoots = null;
		m_AnimatorWatchSelf = null;
		m_AnimatorModeStateCheck = null;
		m_fpClosedCallBack = null;
		m_AudioManager = null;
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.ClearContents();
		}
		m_ButtonGuide = null;
	}

	private void Update()
	{
		if (m_isPlaingBGCoverTrans)
		{
			UpdateUBCoverImageTrans();
		}
		if ((bool)m_AnimatorModeStateCheck)
		{
			switch (m_curModeState)
			{
			case ModeState.Appear:
				if (m_AnimatorModeStateCheck.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()))
				{
					m_curModeState = ModeState.Idle;
				}
				break;
			case ModeState.Disappear:
			{
				AnimatorStateInfo currentAnimatorStateInfo = m_AnimatorModeStateCheck.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
				{
					m_curModeState = ((m_curMode != m_nextMode && m_nextMode != Modes.Unknown) ? ModeState.WaitForNextMode : ModeState.Unknown);
				}
				break;
			}
			case ModeState.WaitForNextMode:
				ChangeNextMode();
				break;
			}
		}
		if (m_AnimatorWatchSelf != null)
		{
			switch (m_curState)
			{
			case State.Appear:
				if (m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()))
				{
					if (m_BGUIImageObj != null)
					{
						m_BGUIImageObj.SetActive(value: true);
					}
					ChangeState(State.Idle);
				}
				break;
			case State.Disappear:
			{
				AnimatorStateInfo currentAnimatorStateInfo2 = m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo2.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo2.normalizedTime >= 0.99f)
				{
					if (m_ButtonGuide != null)
					{
						m_ButtonGuide.SetShow(isShow: false);
					}
					if (m_BGImage != null)
					{
						m_BGImage.sprite = null;
					}
					m_RootObject.SetActive(value: false);
					base.gameObject.SetActive(value: false);
					if (m_fpClosedCallBack != null)
					{
						m_fpClosedCallBack(this, null);
						InitClosedCallBack();
					}
					Resources.UnloadUnusedAssets();
					return;
				}
				break;
			}
			case State.MoveToLeftSide:
				if (m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0).IsName(SpecialAniState.move_left_idle.ToString()))
				{
					ChangeState(State.IdleLeftSide);
				}
				break;
			case State.MoveToCenter:
				if (m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()))
				{
					ChangeState(State.Idle);
				}
				break;
			case State.MoveToSmall:
				if (m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0).IsName(SpecialAniState.move_smaller_idle.ToString()))
				{
					ChangeState(State.IdleSmall);
				}
				break;
			case State.MoveToNormal:
				if (m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()))
				{
					ChangeState(State.Idle);
				}
				break;
			case State.AppearSmall:
				if (m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0).IsName(SpecialAniState.move_smaller_idle.ToString()))
				{
					ChangeState(State.IdleSmall);
				}
				break;
			}
		}
		if (!m_InputBlockObject.activeInHierarchy && m_curModeState == ModeState.Idle && !ButtonPadInput.IsPlayingButPressAnim() && !PopupDialoguePlus.IsAnyPopupActivated() && m_curMode == Modes.Home)
		{
			PadInputProc_HomeMode();
		}
	}

	private void PadInputProc_HomeMode()
	{
		if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton))
		{
			m_ButtonGuide.SetContentActivate(m_buttonGuideText_Exit, isActivate: true);
			Close();
		}
		else
		{
			if (m_OnCursorButton == null)
			{
				return;
			}
			if (ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_OnCursorButton.m_Button, m_OnCursorButton.m_SelectionIconButton))
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuideText_Submit, isActivate: true);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Push_WatchOK");
				}
				m_isInputedByPadOrKey = true;
				return;
			}
			float fAxisX = 0f;
			float fAxisY = 0f;
			if (!GamePadInput.GetLStickMove(out fAxisX, out fAxisY))
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			if (Mathf.Abs(fAxisX) >= Mathf.Abs(fAxisY))
			{
				if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
				{
					num = -1;
					m_fScrollButtonPusingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
				{
					num = 1;
					m_fScrollButtonPusingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Pushing))
				{
					if (IsOverScrollButtonPushingTime())
					{
						num = -1;
					}
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
				{
					num = 1;
				}
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				num2 = -1;
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				num2 = 1;
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				if (IsOverScrollButtonPushingTime())
				{
					num2 = -1;
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
			{
				num2 = 1;
			}
			if (num == 0 && num2 == 0)
			{
				return;
			}
			int num3 = 3;
			int num4 = 3;
			int num5 = Array.IndexOf(m_HomeMenuButtons, m_OnCursorButton) - 1;
			int num6 = num5 / num3;
			int num7 = num5 % num3;
			if (num != 0)
			{
				num7 += num;
				if (num7 < 0)
				{
					num7 = num3 - 1;
				}
				else if (num7 >= num3)
				{
					num7 = 0;
				}
			}
			if (num2 != 0)
			{
				num6 += num2;
				if (num6 < 0)
				{
					num6 = num4 - 1;
				}
				else if (num6 >= num4)
				{
					num6 = 0;
				}
			}
			int num8 = num6 * num3 + num7 + 1;
			ChangeOnCursorButton(m_HomeMenuButtons[num8]);
			m_ButtonGuide.SetContentActivate(m_buttonGuideText_MoveCursor, isActivate: true);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Push_WatchCursor");
			}
		}
	}

	private void PadInputProc_PhoneMode()
	{
	}

	private bool IsOverScrollButtonPushingTime()
	{
		m_fScrollButtonPusingTime += Time.deltaTime;
		if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
		{
			m_fScrollButtonPusingTime = 0f;
			return true;
		}
		return false;
	}

	public void InitClosedCallBack()
	{
		m_fpClosedCallBack = null;
	}

	public IEnumerator Show(GameDefine.EventProc fpClosedCB = null)
	{
		if (m_BGUIImageObj != null)
		{
			m_BGUIImageObj.SetActive(value: false);
		}
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		m_fpClosedCallBack = ((fpClosedCB == null) ? null : new GameDefine.EventProc(fpClosedCB.Invoke));
		Sprite bgSprite = null;
		yield return MainLoadThing.instance.StartCoroutine(GameGlobalUtil.GetSWatchBGImageSpriteRequest());
		bgSprite = GameGlobalUtil.m_sprLoadFromImgXls;
		if (m_BGImage != null && bgSprite != null)
		{
			m_BGImage.sprite = bgSprite;
		}
		InitButtonGuide();
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		m_curModeState = ModeState.Unknown;
		m_curMode = Modes.Unknown;
		ChangeMode(Modes.Home, isEnableAnimation: false);
		ShowCommonMembers(isShow: true);
		ChangeState(State.Appear);
		ChangeOnCursorButton(m_SNSButton);
		GameSwitch.GetInstance().AddTrophyCnt("trp_00027");
		yield return null;
	}

	public void Close(bool isPlayUISound = true)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.Stop(7);
			if (isPlayUISound)
			{
				m_AudioManager.PlayUISound("Push_WatchCancel");
			}
		}
		m_isShowPhoneFromSmartMenu = false;
		ChangeState(State.Disappear);
		ChangeMode(Modes.Unknown, isEnableAnimation: true);
		ShowCommonMembers(isShow: false);
		if (m_BGUIImageObj != null)
		{
			m_BGUIImageObj.SetActive(value: false);
		}
	}

	public void InitTextComponents()
	{
		Xls.ProgramText programText = null;
		FontManager.ResetTextFontByCurrentLanguage(m_TitleText);
		FontManager.ResetTextFontByCurrentLanguage(m_TimeText);
		if (m_TitleText != null)
		{
			programText = Xls.ProgramText.GetData_byKey("SMART_WATCH_TITLE");
			if (programText != null)
			{
				m_TitleText.text = programText.m_strTxt;
			}
		}
		m_TimeText.text = GameGlobalUtil.GetCurrentGameTimeString(GameGlobalUtil.GetXlsProgramText("SMART_WATCH_TIME_FORMAT"));
		string[] array = new string[10]
		{
			string.Empty,
			"SMART_WATCH_BUTTON_SNS",
			"SMART_WATCH_BUTTON_MSG",
			"SMART_WATCH_BUTTON_RANK",
			"SMART_WATCH_BUTTON_MEMO",
			"SMART_WATCH_BUTTON_ADDRESS",
			"SMART_WATCH_BUTTON_AUTOWRITE",
			"SMART_WATCH_BUTTON_PHONE",
			"SMART_WATCH_BUTTON_RECORD",
			"SMART_WATCH_BUTTON_CONFIG"
		};
		HomeMenuButton homeMenuButton = null;
		int num = Mathf.Min(m_HomeMenuButtons.Length, array.Length);
		for (int i = 0; i < num; i++)
		{
			homeMenuButton = m_HomeMenuButtons[i];
			if (homeMenuButton != null)
			{
				FontManager.ResetTextFontByCurrentLanguage(homeMenuButton.m_ButtonText);
				FontManager.ResetTextFontByCurrentLanguage(homeMenuButton.m_BannerText);
				programText = Xls.ProgramText.GetData_byKey(array[i]);
				homeMenuButton.m_ButtonText.text = ((programText == null) ? string.Empty : programText.m_strTxt);
				homeMenuButton.m_BannerObj.SetActive(value: false);
			}
		}
		m_bgCoverTrans_DefalutTime = GameGlobalUtil.GetXlsProgramDefineStrToFloat("SWATCH_BG_COVER_TIME");
	}

	private void InitButtonGuide()
	{
		if (m_ButtonGuide == null)
		{
			m_ButtonGuide = ((!(m_GameMain != null)) ? GameGlobalUtil.GetCommonButtonGuide() : m_GameMain.m_CommonButtonGuide);
		}
		if (!m_isInitializedButtonGuidText)
		{
			m_buttonGuideText_MoveCursor = GameGlobalUtil.GetXlsProgramText("SWATCH_BTNGUIDE_SEL_MENU");
			m_buttonGuideText_Submit = GameGlobalUtil.GetXlsProgramText("SWATCH_BTNGUIDE_SUBMIT");
			m_buttonGuideText_Exit = GameGlobalUtil.GetXlsProgramText("SWATCH_BTNGUIDE_EXIT");
			m_isInitializedButtonGuidText = true;
		}
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuideText_MoveCursor, PadInput.GameInput.LStickY, isIngoreAxis: true);
		m_ButtonGuide.AddContent(m_buttonGuideText_Submit, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuideText_Exit, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuideText_Exit, PadInput.GameInput.SquareButton);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Center);
		m_ButtonGuide.SetShow(isShow: true);
	}

	public void OnClick_HomeMenuButton(string buttonName)
	{
		Modes modes = (Modes)Enum.Parse(typeof(Modes), buttonName, ignoreCase: true);
		int tutoMenuState = GameSwitch.GetInstance().GetTutoMenuState();
		HomeMenuButton homeMenuButton = GatModeHomeButtonInfo(modes);
		if (homeMenuButton != null)
		{
			ChangeOnCursorButton(homeMenuButton);
			switch (modes)
			{
			case Modes.SNS:
				if (tutoMenuState > 1)
				{
					GameDefine.EventProc fpChangedSelContent2 = null;
					GameObject modeRootObject2 = GetModeRootObject(modes);
					if (modeRootObject2 != null)
					{
						SWSub_SNSMenu component2 = modeRootObject2.GetComponent<SWSub_SNSMenu>();
						if (component2 != null)
						{
							fpChangedSelContent2 = component2.OnProc_ChnagedSelectContent;
						}
					}
					StartCoroutine(SNSMenuPlus.ShowSNSMenu_FormAssetBundle(SNSMenuPlus.Mode.WatchMenu, OnClosed_InnerMenu, fpChangedSelContent2));
					break;
				}
				m_InputBlockObject.SetActive(value: true);
				PopupDialoguePlus.ShowActionPopup_OK(GameGlobalUtil.GetXlsProgramText("SWATCH_POPUP_TEXT_SNS_BLOCKED"), OnClosed_BlockedMenuPopup);
				return;
			case Modes.Messenger:
				if (tutoMenuState > 1)
				{
					SWSub_MSGMenu sWSub_MSGMenu = null;
					GameDefine.EventProc fpChangedSelContent3 = null;
					GameObject modeRootObject3 = GetModeRootObject(modes);
					if (modeRootObject3 != null)
					{
						sWSub_MSGMenu = modeRootObject3.GetComponent<SWSub_MSGMenu>();
						if (sWSub_MSGMenu != null)
						{
							fpChangedSelContent3 = sWSub_MSGMenu.OnProc_ChangedSelectContent;
						}
					}
					StartCoroutine(MSGMenuPlus.ShowMSGMenu_FormAssetBundle(OnClosed_InnerMenu, fpChangedSelContent3, sWSub_MSGMenu));
					break;
				}
				m_InputBlockObject.SetActive(value: true);
				PopupDialoguePlus.ShowActionPopup_OK(GameGlobalUtil.GetXlsProgramText("SWATCH_POPUP_TEXT_MSG_BLOCKED"), OnClosed_BlockedMenuPopup);
				return;
			case Modes.Rank:
				if (tutoMenuState > 1)
				{
					StartCoroutine(RankMenu.ShowRankMenu_FormAssetBundle(OnClosed_InnerMenu));
					break;
				}
				m_InputBlockObject.SetActive(value: true);
				PopupDialoguePlus.ShowActionPopup_OK(GameGlobalUtil.GetXlsProgramText("SWATCH_POPUP_TEXT_RANK_BLOCKED"), OnClosed_BlockedMenuPopup);
				return;
			case Modes.Memo:
			{
				EventEngine.GetInstance().m_GameMain.ShowKeywordMenu(isShow: true, isKeywordMenu: true);
				KeywordMenuPlus component4 = homeMenuButton.m_LinkedMenuRoot.GetComponent<KeywordMenuPlus>();
				component4.SetClosedFunc(OnClosed_InnerMenu);
				break;
			}
			case Modes.Address:
			{
				GameDefine.EventProc fpChangedSelContent5 = null;
				GameObject modeRootObject5 = GetModeRootObject(modes);
				if (modeRootObject5 != null)
				{
					SWSub_ProfileMenu component5 = modeRootObject5.GetComponent<SWSub_ProfileMenu>();
					if (component5 != null)
					{
						fpChangedSelContent5 = component5.OnProc_ChnagedSelectContent;
					}
				}
				StartCoroutine(ProfileMenuPlus.ShowProfileMenu_FormAssetBundle(OnClosed_InnerMenu, fpChangedSelContent5));
				break;
			}
			case Modes.AutoWrite:
				if (tutoMenuState > 1)
				{
					StartCoroutine(AutoWrite.ShowAutoWrite_FormAssetBundle(OnClosed_InnerMenu));
					break;
				}
				m_InputBlockObject.SetActive(value: true);
				PopupDialoguePlus.ShowActionPopup_OK(GameGlobalUtil.GetXlsProgramText("SWATCH_POPUP_TEXT_AUTO_WRITE_BLOCKED"), OnClosed_BlockedMenuPopup);
				return;
			case Modes.Phone:
				if (tutoMenuState > 2)
				{
					SWSub_PhoneMenu component6 = GetModeRootObject(modes).GetComponent<SWSub_PhoneMenu>();
					m_isShowPhoneFromSmartMenu = true;
					StartCoroutine(component6.ShowSelectionMode(this, OnClosed_InnerMenu));
					break;
				}
				m_InputBlockObject.SetActive(value: true);
				PopupDialoguePlus.ShowActionPopup_OK(GameGlobalUtil.GetXlsProgramText("SWATCH_POPUP_TEXT_PHONE_BLOCKED"), OnClosed_BlockedMenuPopup);
				return;
			case Modes.Record:
			{
				GameDefine.EventProc fpChangedSelContent4 = null;
				GameObject modeRootObject4 = GetModeRootObject(modes);
				if (modeRootObject4 != null)
				{
					SWSub_RecordMenu component3 = modeRootObject4.GetComponent<SWSub_RecordMenu>();
					if (component3 != null)
					{
						fpChangedSelContent4 = component3.OnProc_ChnagedSelectContent;
					}
				}
				StartCoroutine(RecordMenuPlus.ShowRecordMenu_FormAssetBundle(OnClosed_InnerMenu, fpChangedSelContent4));
				break;
			}
			case Modes.Config:
			{
				GameDefine.EventProc fpChangedSelContent = null;
				GameObject modeRootObject = GetModeRootObject(modes);
				if (modeRootObject != null)
				{
					SWSub_ConfigMenu component = modeRootObject.GetComponent<SWSub_ConfigMenu>();
					if (component != null)
					{
						fpChangedSelContent = component.OnProc_ChangedSelectContent;
					}
				}
				StartCoroutine(SWatchConfigMenu.ShowSWConfigMenu_FormAssetBundle(OnClosed_InnerMenu, fpChangedSelContent, OnChanged_Wallpaper));
				break;
			}
			}
		}
		ChangeMode(modes, isEnableAnimation: true);
		if (modes != Modes.Phone)
		{
			ChangeState(State.MoveToLeftSide);
			ShowCommonMembers(isShow: false);
		}
	}

	public void OnClick_BackButton()
	{
		if (m_curMode != Modes.Phone)
		{
			Close();
			return;
		}
		SWSub_PhoneMenu component = GetModeRootObject(Modes.Phone).GetComponent<SWSub_PhoneMenu>();
		component.Close();
	}

	private void OnClosed_BlockedMenuPopup(PopupDialoguePlus.Result result)
	{
		m_InputBlockObject.SetActive(value: false);
	}

	private void OnClosed_InnerMenu(object sender, object arg)
	{
		HomeMenuButton homeMenuButton = GatModeHomeButtonInfo(m_curMode);
		if (homeMenuButton != null && homeMenuButton.m_LinkedMenuRoot != null)
		{
			homeMenuButton.m_LinkedMenuRoot.SetActive(value: false);
		}
		InitButtonGuide();
		if (m_curMode != Modes.Phone)
		{
			ChangeState(State.MoveToCenter);
			ShowCommonMembers(isShow: true);
		}
		else
		{
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText("SMART_WATCH_TITLE");
		}
		if (m_curMode != Modes.Home)
		{
			ChangeMode(Modes.Home, isEnableAnimation: true);
			return;
		}
		m_nextMode = Modes.Home;
		ChangeNextMode();
	}

	private IEnumerator OnChanged_Wallpaper(object sender, object arg)
	{
		Sprite bgSprite = null;
		yield return MainLoadThing.instance.StartCoroutine(GameGlobalUtil.GetSWatchBGImageSpriteRequest());
		bgSprite = GameGlobalUtil.m_sprLoadFromImgXls;
		if (m_BGImage != null && bgSprite != null)
		{
			m_BGImage.sprite = bgSprite;
		}
		yield return null;
	}

	private void ChangeMode(Modes nextMode, bool isEnableAnimation)
	{
		if (m_curMode != nextMode)
		{
			m_nextMode = nextMode;
			m_isChangeMode_EnableAnimation = isEnableAnimation;
			GameObject modeRootObject = GetModeRootObject(m_curMode);
			if (!isEnableAnimation || m_curMode == Modes.Unknown || modeRootObject == null)
			{
				ChangeNextMode();
			}
			else if (m_curMode != Modes.Phone)
			{
				m_AnimatorModeStateCheck = GameGlobalUtil.PlayUIAnimation_WithChidren(modeRootObject, GameDefine.UIAnimationState.disappear.ToString());
				m_curModeState = ModeState.Disappear;
			}
			else
			{
				ChangeNextMode();
			}
		}
	}

	private void ChangeNextMode()
	{
		GameObject modeRootObject = GetModeRootObject(m_curMode);
		if (modeRootObject != null)
		{
			modeRootObject.SetActive(value: false);
		}
		modeRootObject = GetModeRootObject(m_nextMode);
		if (modeRootObject == null)
		{
			m_curModeState = ModeState.Idle;
		}
		else
		{
			modeRootObject.SetActive(value: true);
			string strMot = ((!m_isChangeMode_EnableAnimation) ? GameDefine.UIAnimationState.idle.ToString() : GameDefine.UIAnimationState.appear.ToString());
			Animator animator = GameGlobalUtil.PlayUIAnimation_WithChidren(modeRootObject, strMot);
			m_AnimatorModeStateCheck = ((!m_isChangeMode_EnableAnimation) ? null : animator);
			m_curModeState = ((!m_isChangeMode_EnableAnimation) ? ModeState.Idle : ModeState.Appear);
		}
		m_curMode = m_nextMode;
	}

	public void ChangeState(State nextState)
	{
		if (m_curState == nextState)
		{
			return;
		}
		string text = string.Empty;
		switch (nextState)
		{
		case State.Idle:
			text = GameDefine.UIAnimationState.idle.ToString();
			break;
		case State.Appear:
			text = GameDefine.UIAnimationState.appear.ToString();
			break;
		case State.Disappear:
			text = GameDefine.UIAnimationState.disappear.ToString();
			if (m_BackButtonAnimator != null)
			{
				GameGlobalUtil.PlayUIAnimation(m_BackButtonAnimator, text);
			}
			break;
		case State.IdleLeftSide:
			text = SpecialAniState.move_left_idle.ToString();
			break;
		case State.MoveToLeftSide:
			text = ((m_curState != State.Hided) ? SpecialAniState.move_left_start.ToString() : SpecialAniState.move_left_appear.ToString());
			if (m_BackButtonAnimator != null)
			{
				GameGlobalUtil.PlayUIAnimation(m_BackButtonAnimator, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear));
			}
			break;
		case State.MoveToCenter:
			text = SpecialAniState.move_left_return.ToString();
			if (m_BackButtonAnimator != null)
			{
				m_BackButtonAnimator.gameObject.SetActive(value: true);
				GameGlobalUtil.PlayUIAnimation(m_BackButtonAnimator, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.appear));
			}
			break;
		case State.IdleSmall:
			text = SpecialAniState.move_smaller_idle.ToString();
			break;
		case State.MoveToSmall:
			text = SpecialAniState.move_smaller_start.ToString();
			break;
		case State.MoveToNormal:
			text = SpecialAniState.move_smaller_return.ToString();
			break;
		case State.AppearSmall:
			text = SpecialAniState.move_smaller_appear.ToString();
			break;
		case State.DisappearSmall:
			text = SpecialAniState.move_smaller_disappear.ToString();
			break;
		}
		if (m_AnimatorWatchSelf == null)
		{
			m_AnimatorWatchSelf = ((!(m_WatchRootObject != null)) ? null : m_WatchRootObject.GetComponentInChildren<Animator>());
		}
		if (!string.IsNullOrEmpty(text) && m_AnimatorWatchSelf != null)
		{
			GameGlobalUtil.PlayUIAnimation(m_AnimatorWatchSelf, text);
		}
		m_curState = nextState;
		if (m_InputBlockObject != null)
		{
			m_InputBlockObject.SetActive(m_curState != State.Idle);
		}
	}

	private void ChangeOnCursorButton(HomeMenuButton newButton)
	{
		if (m_OnCursorButton != null)
		{
			m_OnCursorButton.m_SelectionCursor.SetActive(value: false);
		}
		newButton?.m_SelectionCursor.SetActive(value: true);
		m_OnCursorButton = newButton;
	}

	private void ShowCommonMembers(bool isShow, bool isEnableAnimation = true)
	{
		if (isShow)
		{
			if (m_BackGound != null)
			{
				m_BackGound.SetActive(value: true);
			}
			if (m_TitleRoot != null)
			{
				m_TitleRoot.SetActive(value: true);
			}
			if (isEnableAnimation)
			{
				string strMot = ((!isEnableAnimation) ? GameDefine.UIAnimationState.idle.ToString() : GameDefine.UIAnimationState.appear.ToString());
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackGound, strMot);
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_TitleRoot, strMot);
			}
		}
		else if (isEnableAnimation)
		{
			string strMot2 = GameDefine.UIAnimationState.disappear.ToString();
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackGound, strMot2);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_TitleRoot, strMot2);
		}
		else
		{
			if (m_BackGound != null)
			{
				m_BackGound.SetActive(value: false);
			}
			if (m_TitleRoot != null)
			{
				m_TitleRoot.SetActive(value: false);
			}
		}
	}

	private GameObject GetModeRootObject(Modes mode)
	{
		if (m_SubModeRoots == null)
		{
			return null;
		}
		return (mode < Modes.Home || (int)mode >= m_SubModeRoots.Length) ? null : m_SubModeRoots[(int)mode];
	}

	private HomeMenuButton GatModeHomeButtonInfo(Modes mode)
	{
		if (m_HomeMenuButtons == null)
		{
			return null;
		}
		return (mode < Modes.Home || (int)mode >= m_HomeMenuButtons.Length) ? null : m_HomeMenuButtons[(int)mode];
	}

	public void SetPhoneState_Sending(string xlsCharKey)
	{
		SetPhoneState(SWSub_PhoneMenu.EngageState.Sending, xlsCharKey);
	}

	public void SetPhoneState_SendingSmall(string xlsCharKey = null)
	{
		SetPhoneState(SWSub_PhoneMenu.EngageState.SendingSmall, xlsCharKey);
	}

	public void SetPhoneState_Ringing(string xlsCharKey)
	{
		SetPhoneState(SWSub_PhoneMenu.EngageState.Ringing, xlsCharKey);
	}

	public void SetPhoneState_Engaged()
	{
		SetPhoneState(SWSub_PhoneMenu.EngageState.Engaged);
	}

	public void SetPhoneState_Finish()
	{
		SetPhoneState(SWSub_PhoneMenu.EngageState.Finish);
	}

	public void SetPhoneState_EngagedShow(GameDefine.EventProc fpCompleteCB = null)
	{
		SetPhoneState(SWSub_PhoneMenu.EngageState.EngagedShow, null, fpCompleteCB);
		if (m_BackGound != null)
		{
			m_BackGound.SetActive(value: true);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackGound, GameDefine.UIAnimationState.appear.ToString());
		}
	}

	public void SetPhoneState_EngagedHide(GameDefine.EventProc fpCompleteCB = null)
	{
		SetPhoneState(SWSub_PhoneMenu.EngageState.EngagedHide, null, fpCompleteCB);
		if (m_BackGound != null)
		{
			m_BackGound.SetActive(value: true);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackGound, GameDefine.UIAnimationState.disappear.ToString());
		}
	}

	public void PlayAniPhoneEngaging(bool isPlay)
	{
		SWSub_PhoneMenu component = GetModeRootObject(Modes.Phone).GetComponent<SWSub_PhoneMenu>();
		if (!(component == null))
		{
			component.PlayAniPhoneEngaging(isPlay);
		}
	}

	public string GetCurSelCharName()
	{
		string result = null;
		SWSub_PhoneMenu component = GetModeRootObject(Modes.Phone).GetComponent<SWSub_PhoneMenu>();
		if (component != null)
		{
			result = component.GetCurSelChar();
		}
		return result;
	}

	public void SetPhoneState(SWSub_PhoneMenu.EngageState engageState, string xlsCharKey = null, GameDefine.EventProc fpCompleteCB = null)
	{
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
		bool activeSelf = base.gameObject.activeSelf;
		if (!activeSelf)
		{
			base.gameObject.SetActive(value: true);
			m_RootObject.SetActive(value: true);
		}
		Modes curMode = m_curMode;
		if (curMode != Modes.Phone)
		{
			ChangeMode(Modes.Phone, isEnableAnimation: false);
		}
		string strMot = ((!activeSelf) ? GameDefine.UIAnimationState.appear.ToString() : GameDefine.UIAnimationState.idle.ToString());
		m_BackGound.SetActive(value: true);
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackGound, strMot);
		if (m_isShowPhoneFromSmartMenu)
		{
			m_TitleRoot.SetActive(value: true);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_TitleRoot, strMot);
		}
		else
		{
			m_TitleRoot.SetActive(value: false);
		}
		if (!activeSelf)
		{
			ChangeState(State.Appear);
		}
		SWSub_PhoneMenu component = GetModeRootObject(Modes.Phone).GetComponent<SWSub_PhoneMenu>();
		component.onChangeStateComplete = fpCompleteCB;
		if (curMode != Modes.Phone)
		{
			StartCoroutine(component.ShowEngageMode(this, xlsCharKey, engageState));
		}
		else
		{
			component.ChangeEngageState(engageState);
		}
	}

	public SWSub_PhoneMenu.EngageState GetCurrentPhoneState()
	{
		if (!base.gameObject.activeSelf)
		{
			return SWSub_PhoneMenu.EngageState.Unknown;
		}
		if (m_curMode != Modes.Phone)
		{
			return SWSub_PhoneMenu.EngageState.Unknown;
		}
		SWSub_PhoneMenu component = GetModeRootObject(Modes.Phone).GetComponent<SWSub_PhoneMenu>();
		return component.curEngageState;
	}

	public void SetPhoneMode_Selection()
	{
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
		bool activeSelf = base.gameObject.activeSelf;
		if (!activeSelf)
		{
			base.gameObject.SetActive(value: true);
			m_RootObject.SetActive(value: true);
		}
		Modes curMode = m_curMode;
		if (curMode != Modes.Phone)
		{
			ChangeMode(Modes.Phone, isEnableAnimation: false);
		}
		string strMot = ((!activeSelf) ? GameDefine.UIAnimationState.appear.ToString() : GameDefine.UIAnimationState.idle.ToString());
		m_BackGound.SetActive(value: true);
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackGound, strMot);
		if (m_isShowPhoneFromSmartMenu)
		{
			m_TitleRoot.SetActive(value: true);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_TitleRoot, strMot);
		}
		else
		{
			m_TitleRoot.SetActive(value: false);
		}
		if (!activeSelf)
		{
			ChangeState(State.Appear);
		}
		SWSub_PhoneMenu component = GetModeRootObject(Modes.Phone).GetComponent<SWSub_PhoneMenu>();
		if (curMode != Modes.Phone)
		{
			StartCoroutine(component.ShowSelectionMode(this));
		}
		else
		{
			component.ChangeMode_Selection();
		}
	}

	public void BGCoverAppear(float fTime = -1f)
	{
		if (!(m_BGCoverImage == null))
		{
			if (fTime < 0.0001f)
			{
				fTime = m_bgCoverTrans_DefalutTime;
			}
			m_bgCoverTrans_CurrentTime = 0f;
			m_bgCoverTrans_TargetTime = fTime;
			m_BGCoverImage.gameObject.SetActive(value: true);
			if (GameGlobalUtil.IsAlmostSame(fTime, 0f) || fTime < 0f)
			{
				m_BGCoverImage.color = new Color(0f, 0f, 0f, 1f);
				m_isPlaingBGCoverTrans = false;
			}
			else
			{
				m_BGCoverImage.color = new Color(0f, 0f, 0f, 0f);
				m_isPlaingBGCoverTrans = true;
				m_isBGCoverTrans_Appear = true;
			}
		}
	}

	public void BGCoverDisappear(float fTime = -1f)
	{
		if (!(m_BGCoverImage == null) && m_BGCoverImage.gameObject.activeSelf)
		{
			if (fTime < 0.0001f)
			{
				fTime = m_bgCoverTrans_DefalutTime;
			}
			m_bgCoverTrans_CurrentTime = 0f;
			m_bgCoverTrans_TargetTime = fTime;
			if (GameGlobalUtil.IsAlmostSame(fTime, 0f) || fTime < 0f)
			{
				m_BGCoverImage.color = new Color(0f, 0f, 0f, 0f);
				m_BGCoverImage.gameObject.SetActive(value: false);
				m_isPlaingBGCoverTrans = false;
			}
			else
			{
				m_BGCoverImage.color = new Color(0f, 0f, 0f, 1f);
				m_isPlaingBGCoverTrans = true;
				m_isBGCoverTrans_Appear = false;
			}
		}
	}

	private void UpdateUBCoverImageTrans()
	{
		if (m_BGCoverImage == null || !m_isPlaingBGCoverTrans)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		m_bgCoverTrans_CurrentTime += ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		if (m_isBGCoverTrans_Appear)
		{
			if (m_bgCoverTrans_CurrentTime >= m_bgCoverTrans_TargetTime)
			{
				m_BGCoverImage.color = new Color(0f, 0f, 0f, 1f);
				m_isPlaingBGCoverTrans = false;
			}
			else
			{
				float a = Mathf.Lerp(0f, 1f, m_bgCoverTrans_CurrentTime / m_bgCoverTrans_TargetTime);
				m_BGCoverImage.color = new Color(0f, 0f, 0f, a);
			}
		}
		else if (m_bgCoverTrans_CurrentTime >= m_bgCoverTrans_TargetTime)
		{
			m_BGCoverImage.color = new Color(0f, 0f, 0f, 0f);
			m_BGCoverImage.gameObject.SetActive(value: false);
			m_isPlaingBGCoverTrans = false;
		}
		else
		{
			float a2 = Mathf.Lerp(1f, 0f, m_bgCoverTrans_CurrentTime / m_bgCoverTrans_TargetTime);
			m_BGCoverImage.color = new Color(0f, 0f, 0f, a2);
		}
	}

	public bool OnProc_IsCompleteCoverTrans()
	{
		return !m_isPlaingBGCoverTrans;
	}

	public SWSub_MemoMenu GetMemoMenu()
	{
		return m_MemoModeRoot.GetComponent<SWSub_MemoMenu>();
	}

	public void TouchHomeMenuButton(string buttonName)
	{
		if (!m_isInputedByPadOrKey && m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Push_WatchOK");
		}
		m_isInputedByPadOrKey = false;
		OnClick_HomeMenuButton(buttonName);
	}

	public void TouchBackButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClick_BackButton();
		}
	}
}
