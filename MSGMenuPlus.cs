using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class MSGMenuPlus : MonoBehaviour
{
	[Serializable]
	public class ContainerInfo
	{
		public GameObject m_ScrollbarRoot;

		public ScrollRect m_ScrollRect;

		public RectTransform m_ContainerRT;

		public float m_ContentInterval;

		public bool m_ScrollLoopEnable = true;

		public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

		public float m_ScrollRepeatTime = 0.2f;

		[NonSerialized]
		public float m_fScrollButtonPusingTime;
	}

	public enum Mode
	{
		Unknown,
		SelChatRoom,
		InChatRoom
	}

	private enum ChatroomAniState
	{
		idle,
		active_in,
		active,
		active_out
	}

	public class ContentInfo
	{
		public Xls.MessengerTalkData xlsData;

		public MSGContentPlus.MSGContentType contentType;

		public float pos;

		public float height;

		public float scaledHeight;

		public bool isValid;

		public bool isNew;
	}

	private struct ContentSlotLayoutInfo
	{
		public float heightWithoutText;

		public float heightThumbnailImage;

		public float scaleY;

		public TextGenerationSettings textGenSetting_WithImage;

		public TextGenerationSettings textGenSetting_WithoutImage;
	}

	public GameObject m_RootObject;

	public Text m_TitleText;

	public CommonTabContainerPlus m_TabContainer;

	public GameObject m_BackButtonObj;

	[Header("Chatroom Container")]
	public RectTransform m_ChatroomRootRT;

	public Animator m_ChatroomAnimator;

	public Text m_ChatroomTitleText;

	public GameObject m_ChatroomButtonSrcObj;

	public ContainerInfo m_ChatroomContainer = new ContainerInfo();

	[Header("Content Container")]
	public GameObject m_ContentsRootObj;

	public GameObject m_NoMessgeTextRoot;

	public Text m_NoMessageText;

	public GameObject m_ChangingTabCover;

	public NextContentsLoadButtton m_NextPageButton;

	public int m_ContentPageSize = 20;

	public float m_BottomSpacing = 80f;

	public ContainerInfo m_ContentContainer = new ContainerInfo();

	public Button m_ScrollButtonToFirst;

	public Button m_ScrollButtonRStick;

	[Header("Linked Objects")]
	public ShowImageOriginSize m_ShowImageOrigin;

	private Mode m_curMode;

	private Canvas m_Canvas;

	private SWSub_MSGMenu m_swSubMsgMenu;

	private AudioManager m_AudioManager;

	private CommonButtonGuide m_ButtonGuide;

	private bool m_isNeedContentAlign;

	private bool m_isInputBlock;

	private bool m_isPageScrolling;

	private bool m_isTutorialActivated;

	private Animator m_StateCheckAnimator;

	private GameDefine.EventProc m_fpClosedFP;

	private GameDefine.EventProc m_fpChangedSelectContent;

	private List<MSGChatRoomButton> m_ChatroomButtons = new List<MSGChatRoomButton>();

	private MSGChatRoomButton m_SelectedChatroomButton;

	private MSGChatRoomButton m_OnCursorChatroomButton;

	private List<Xls.MessengerTalkData> m_curActiveMsgDatas;

	private MSGContentPlus m_OnCursorContent;

	private List<ContentInfo> m_curContentInfos = new List<ContentInfo>();

	private TextGenerator m_textGenerator;

	private int m_curBaseContentIdx = -1;

	private int m_onCursorContentIdx = -1;

	private int m_validContentIndexFirst = -1;

	private int m_validContentIndexLast = -1;

	private List<MSGContentPlus> m_MeContentSlots = new List<MSGContentPlus>();

	private List<MSGContentPlus> m_PartnerContentSlots = new List<MSGContentPlus>();

	private List<MSGContentPlus> m_PartnerFirstContentSlots = new List<MSGContentPlus>();

	private List<MSGContentPlus> m_activedContentSlots = new List<MSGContentPlus>();

	private bool m_isInitailizedSlots;

	private ContentSlotLayoutInfo m_MeContentSlotLayoutInfo = default(ContentSlotLayoutInfo);

	private ContentSlotLayoutInfo m_PartnerContentSlotLayoutInfo = default(ContentSlotLayoutInfo);

	private ContentSlotLayoutInfo m_PartnerFirstContentSlotLayoutInfo = default(ContentSlotLayoutInfo);

	private string m_EmptyContentText;

	private LoadingSWatchIcon m_LoadingIcon;

	private bool m_isInitedButtonGuidText;

	private string m_btnGuide_SelChatroom;

	private string m_btnGuide_SubmitChatroom;

	private string m_btnGuide_ExitMenu;

	private string m_btnGuide_ExitChatroom;

	private string m_btnGuide_SelContent;

	private string m_btnGuide_ShowImage;

	private string m_btnGuide_ScrollToTop;

	private string m_btnGuide_SelectTab;

	private string m_btnGuide_SubmitTab;

	private string m_btnGuide_CancelTab;

	private static MSGMenuPlus s_activedInstance;

	private const string c_assetBundleName = "prefabs/ingame/menu/ui_msgmenu";

	private static AssetBundleObjectHandler s_assetBundleHdr;

	private AssetBundleObjectHandler m_showingImageAssetObjHdr;

	public static MSGMenuPlus instance => s_activedInstance;

	public static bool IsActivated => s_activedInstance != null && s_activedInstance.gameObject.activeSelf;

	private void Awake()
	{
		if (m_TabContainer != null)
		{
			m_TabContainer.getTabCreateInfoFP = SetTabCreateInfos;
			m_TabContainer.onChangedSelectTab = OnChangedSelectTab;
			m_TabContainer.onChangingSelectTab = OnChangingSelectTab;
			m_TabContainer.onPressTabButton = OnProc_PressedTabButtons;
			m_TabContainer.m_AutoBuild = false;
		}
		m_ChatroomContainer.m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Vertical, m_ChatroomContainer.m_ScrollRect, m_ChatroomContainer.m_ContainerRT);
		m_ChatroomContainer.m_ScrollHandler.scrollPos = 0f;
		m_ContentContainer.m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Vertical, m_ContentContainer.m_ScrollRect, m_ContentContainer.m_ContainerRT, null, null, m_ScrollButtonToFirst);
		m_ContentContainer.m_ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
		m_textGenerator = new TextGenerator();
		m_Canvas = base.gameObject.GetComponentInChildren<Canvas>();
		m_NextPageButton.gameObject.SetActive(value: false);
	}

	private void OnDisable()
	{
		if (m_LoadingIcon != null)
		{
			UnityEngine.Object.Destroy(m_LoadingIcon.gameObject);
			m_LoadingIcon = null;
		}
	}

	private void OnDestroy()
	{
		if (m_ChatroomContainer != null && m_ChatroomContainer.m_ScrollHandler != null)
		{
			m_ChatroomContainer.m_ScrollHandler.ReleaseScroll();
		}
		if (m_ContentContainer != null && m_ContentContainer.m_ScrollHandler != null)
		{
			m_ContentContainer.m_ScrollHandler.ReleaseScroll();
		}
		ClearChatroomButtons();
		if (m_curActiveMsgDatas != null)
		{
			m_curActiveMsgDatas.Clear();
		}
		if (m_curContentInfos != null)
		{
			m_curContentInfos.Clear();
		}
		if (m_MeContentSlots != null)
		{
			m_MeContentSlots.Clear();
		}
		if (m_PartnerContentSlots != null)
		{
			m_PartnerContentSlots.Clear();
		}
		if (m_PartnerFirstContentSlots != null)
		{
			m_PartnerFirstContentSlots.Clear();
		}
		if (m_activedContentSlots != null)
		{
			m_activedContentSlots.Clear();
		}
		m_StateCheckAnimator = null;
		m_fpClosedFP = null;
		m_fpChangedSelectContent = null;
		m_SelectedChatroomButton = null;
		m_OnCursorChatroomButton = null;
		m_Canvas = null;
		m_swSubMsgMenu = null;
		m_AudioManager = null;
		m_ButtonGuide = null;
		m_SelectedChatroomButton = null;
		m_OnCursorChatroomButton = null;
		m_OnCursorContent = null;
		m_textGenerator = null;
		m_EmptyContentText = null;
		if (m_LoadingIcon != null)
		{
			UnityEngine.Object.Destroy(m_LoadingIcon.gameObject);
		}
		m_LoadingIcon = null;
		s_assetBundleHdr = null;
		s_activedInstance = null;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_StateCheckAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_StateCheckAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				CallClosedCallback();
			}
			return;
		}
		switch (m_curMode)
		{
		case Mode.SelChatRoom:
			Update_SelChatroomMode();
			break;
		case Mode.InChatRoom:
			Update_InChatroomMode();
			break;
		}
	}

	private void Update_SelChatroomMode()
	{
		if (m_ChatroomAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_ChatroomAnimator.GetCurrentAnimatorStateInfo(0);
			if (m_ContentsRootObj.activeSelf && currentAnimatorStateInfo.IsName(ChatroomAniState.active.ToString()))
			{
				m_ContentsRootObj.SetActive(value: false);
			}
		}
		if (m_ChatroomContainer.m_ScrollHandler.IsScrolling)
		{
			m_ChatroomContainer.m_ScrollHandler.Update();
			SetActivedContents_byScrollPos(m_ChatroomContainer.m_ScrollHandler.scrollPos);
			bool flag = true;
			if (m_ChatroomContainer.m_ScrollHandler.CurScrollEvent == UIUtil_ScrollHandler.ScrollEvent.None)
			{
				Vector2 zero = Vector2.zero;
				if (GamePadInput.GetLStickMove(out zero.x, out zero.y) || GamePadInput.GetLStickMove(out zero.x, out zero.y))
				{
					m_ChatroomContainer.m_ScrollRect.velocity = Vector2.zero;
					flag = false;
				}
			}
			if (flag)
			{
				return;
			}
		}
		if (m_isTutorialActivated || m_isInputBlock || ButtonPadInput.IsPlayingButPressAnim() || PopupDialoguePlus.IsAnyPopupActivated())
		{
			return;
		}
		if (m_ChatroomButtons.Count > 1)
		{
			if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				ChangeOnCursorChatroom(isUpSide: true);
				m_ChatroomContainer.m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				ChangeOnCursorChatroom(isUpSide: false);
				m_ChatroomContainer.m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				m_ChatroomContainer.m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_ChatroomContainer.m_fScrollButtonPusingTime >= m_ChatroomContainer.m_ScrollRepeatTime)
				{
					m_ChatroomContainer.m_fScrollButtonPusingTime = 0f;
					ChangeOnCursorChatroom(isUpSide: true);
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing))
			{
				m_ChatroomContainer.m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_ChatroomContainer.m_fScrollButtonPusingTime >= m_ChatroomContainer.m_ScrollRepeatTime)
				{
					m_ChatroomContainer.m_fScrollButtonPusingTime = 0f;
					ChangeOnCursorChatroom(isUpSide: false);
				}
			}
		}
		if (m_OnCursorChatroomButton != null && GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
		{
			SetSelectedChatroom(m_OnCursorChatroomButton);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_OK");
			}
		}
		else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
		{
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_btnGuide_ExitMenu, isActivate: true);
			}
			CloseMSGMenu();
		}
	}

	private void Update_InChatroomMode()
	{
		if (m_ContentContainer.m_ScrollHandler.IsScrolling)
		{
			m_ContentContainer.m_ScrollHandler.Update();
			SetActivedContents_byScrollPos(m_ContentContainer.m_ScrollHandler.scrollPos);
			bool flag = true;
			if (m_ContentContainer.m_ScrollHandler.CurScrollEvent == UIUtil_ScrollHandler.ScrollEvent.None && m_onCursorContentIdx >= 0 && m_onCursorContentIdx < m_curContentInfos.Count)
			{
				ChangeOnCursorContent_byScrollPos();
				Vector2 zero = Vector2.zero;
				if (GamePadInput.GetLStickMove(out zero.x, out zero.y) || GamePadInput.GetLStickMove(out zero.x, out zero.y))
				{
					m_ContentContainer.m_ScrollRect.velocity = Vector2.zero;
					flag = false;
				}
			}
			if (flag)
			{
				return;
			}
		}
		if (m_isTutorialActivated || m_isInputBlock || ButtonPadInput.IsPlayingButPressAnim() || m_TabContainer.isChaningTab || PopupDialoguePlus.IsAnyPopupActivated())
		{
			return;
		}
		float num = 0f;
		if (m_ContentContainer.m_ScrollHandler.isScrollable)
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.TriangleButton, m_ScrollButtonToFirst);
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.TriangleButton))
			{
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetContentActivate(m_btnGuide_ScrollToTop, isActivate: true);
				}
				OnClick_ContentScrollToTop();
			}
			bool flag2 = false;
			num = GamePadInput.GetAxisValue(PadInput.GameInput.RStickY);
			if (!GameGlobalUtil.IsAlmostSame(num, 0f))
			{
				if (m_ContentContainer.m_ScrollHandler.scrollPos <= 0f && num < 0f && m_onCursorContentIdx != m_validContentIndexFirst)
				{
					SetOnCursorContentIndex(m_validContentIndexFirst);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
				}
				else if (m_ContentContainer.m_ScrollHandler.scrollPos >= m_ContentContainer.m_ScrollHandler.scrollPos_Max && num > 0f && m_onCursorContentIdx != m_validContentIndexLast)
				{
					SetOnCursorContentIndex(m_validContentIndexLast);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
				}
				else
				{
					m_ContentContainer.m_ScrollHandler.ScrollByDirection(num > 0f, Mathf.Abs(num));
					SetActivedContents_byScrollPos(m_ContentContainer.m_ScrollHandler.scrollPos);
					ChangeOnCursorContent_byScrollPos();
				}
			}
			else
			{
				if (!m_ContentContainer.m_ScrollHandler.IsScrolling && m_isPageScrolling)
				{
					m_isPageScrolling = false;
				}
				if (!m_ContentContainer.m_ScrollHandler.IsScrolling)
				{
					num = GamePadInput.GetAxisValue(PadInput.GameInput.LStickX);
					if (GameGlobalUtil.IsAlmostSame(num, 0f))
					{
						num = 0f - GamePadInput.GetMouseWheelScrollDelta();
					}
					if (num > 0.9f)
					{
						m_isPageScrolling = ChangeScrollPage(isUpSide: false);
					}
					else if (num < -0.9f)
					{
						m_isPageScrolling = ChangeScrollPage(isUpSide: true);
					}
				}
			}
		}
		if (m_ContentContainer.m_ScrollHandler.IsScrolling)
		{
			return;
		}
		num = GamePadInput.GetAxisValue(PadInput.GameInput.LStickY);
		if (Mathf.Abs(num) > 0.5f)
		{
			if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				ChangeOnCursorContentIndex(isUpSide: true);
				m_ContentContainer.m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				ChangeOnCursorContentIndex(isUpSide: false);
				m_ContentContainer.m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				m_ContentContainer.m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_ContentContainer.m_fScrollButtonPusingTime >= m_ContentContainer.m_ScrollRepeatTime)
				{
					m_ContentContainer.m_fScrollButtonPusingTime = 0f;
					ChangeOnCursorContentIndex(isUpSide: true);
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing))
			{
				m_ContentContainer.m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_ContentContainer.m_fScrollButtonPusingTime >= m_ContentContainer.m_ScrollRepeatTime)
				{
					m_ContentContainer.m_fScrollButtonPusingTime = 0f;
					ChangeOnCursorContentIndex(isUpSide: false);
				}
			}
		}
		if (m_OnCursorContent != null && m_OnCursorContent.isExistImage)
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.SquareButton, m_OnCursorContent.m_ImageViewButton);
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton))
			{
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetContentActivate(m_btnGuide_ShowImage, isActivate: true);
				}
				StartCoroutine(OnProc_ViewImageDetail(m_OnCursorContent));
			}
		}
		if (!GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
		{
			return;
		}
		if (m_ChatroomButtons.Count > 1)
		{
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_btnGuide_ExitChatroom, isActivate: true);
			}
			ChangeMode(Mode.SelChatRoom);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Cancel");
			}
		}
		else
		{
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_btnGuide_ExitMenu, isActivate: true);
			}
			CloseMSGMenu();
		}
	}

	public static IEnumerator ShowMSGMenu_FormAssetBundle(GameDefine.EventProc fpClosed = null, GameDefine.EventProc fpChangedSelContent = null, SWSub_MSGMenu swSubMsgMenu = null)
	{
		if (s_activedInstance == null)
		{
			s_activedInstance = (UnityEngine.Object.Instantiate(MainLoadThing.instance.m_prefabMSGMenu) as GameObject).GetComponent<MSGMenuPlus>();
			yield return null;
		}
		yield return MainLoadThing.instance.StartCoroutine(s_activedInstance.InitMSGMenu(fpClosed, fpChangedSelContent, swSubMsgMenu));
	}

	public IEnumerator InitMSGMenu(GameDefine.EventProc fpClosedCB = null, GameDefine.EventProc fpChangedSelContent = null, SWSub_MSGMenu swSubMsgMenu = null)
	{
		base.gameObject.SetActive(value: true);
		m_fpClosedFP = ((fpClosedCB == null) ? null : new GameDefine.EventProc(fpClosedCB.Invoke));
		m_fpChangedSelectContent = ((fpChangedSelContent == null) ? null : new GameDefine.EventProc(fpChangedSelContent.Invoke));
		m_swSubMsgMenu = swSubMsgMenu;
		Text[] textComps = new Text[3] { m_TitleText, m_ChatroomTitleText, m_NoMessageText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		if (m_TitleText != null)
		{
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText("MSGMENU_TITLE");
		}
		if (m_ChatroomTitleText != null)
		{
			m_ChatroomTitleText.text = GameGlobalUtil.GetXlsProgramText("MSGMENU_CHATROOM_SEL_TEXT");
		}
		if (m_NextPageButton != null)
		{
			FontManager.ResetTextFontByCurrentLanguage(m_NextPageButton.m_LoadingText);
			FontManager.ResetTextFontByCurrentLanguage(m_NextPageButton.m_SelectedText);
			FontManager.ResetTextFontByCurrentLanguage(m_NextPageButton.m_NotSelectedText);
			m_NextPageButton.nextPageText = GameGlobalUtil.GetXlsProgramText("MSGMENU_LOAD_PREV_PAGE");
			m_NextPageButton.loadingText = GameGlobalUtil.GetXlsProgramText("MSGMENU_LOADING_PREV_PAGE");
		}
		if (m_TabContainer != null)
		{
			m_TabContainer.BuildTabButtonObjects();
			m_TabContainer.isInputBlock = true;
			m_TabContainer.gameObject.SetActive(value: false);
		}
		if (m_ContentContainer.m_ScrollbarRoot != null)
		{
			m_ContentContainer.m_ScrollbarRoot.SetActive(value: false);
		}
		Xls.MessengerTalkData xlsTalkData = null;
		List<Xls.MessengerTalkData> activedTalkDatas = new List<Xls.MessengerTalkData>();
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		int[] msgOrderArray = gameSwitch.GetMessageOrderArray();
		if (msgOrderArray != null && msgOrderArray.Length > 0)
		{
			int num = Mathf.Min(msgOrderArray.Length, gameSwitch.GetMessageGetCount());
			for (int i = 0; i < num; i++)
			{
				int switchIdx = msgOrderArray[i];
				xlsTalkData = Xls.MessengerTalkData.GetData_bySwitchIdx(switchIdx);
				if (xlsTalkData != null)
				{
					activedTalkDatas.Add(xlsTalkData);
				}
			}
		}
		yield return StartCoroutine(InitChatroomButtons(activedTalkDatas));
		yield return null;
		if (0 == 0 && m_ChatroomButtons.Count > 0)
		{
			SetOnCursorChatroom(m_ChatroomButtons[0]);
		}
		InitButtonGuide();
		if (m_ChatroomButtons.Count == 1)
		{
			SetSelectedChatroomAtInit(m_ChatroomButtons[0]);
		}
		else
		{
			ChangeMode(Mode.SelChatRoom);
		}
		GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.appear.ToString());
		m_isInputBlock = false;
	}

	public void CloseMSGMenu(bool isEnableAnimation = true)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Cancel");
		}
		m_isInputBlock = true;
		m_TabContainer.isInputBlock = true;
		if (!isEnableAnimation)
		{
			CallClosedCallback();
			return;
		}
		m_StateCheckAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
		if (m_StateCheckAnimator == null)
		{
			CallClosedCallback();
		}
	}

	private void CallClosedCallback()
	{
		m_StateCheckAnimator = null;
		m_TabContainer.ClearTabButtonObjects();
		ClearContentSlots();
		ClearChatroomButtons();
		ChangeMode(Mode.Unknown);
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
		UnityEngine.Object.Destroy(base.gameObject);
		if (m_fpClosedFP != null)
		{
			m_fpClosedFP(this, null);
		}
	}

	private void InitButtonGuide()
	{
		if (m_ButtonGuide == null)
		{
			m_ButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
			if (m_ButtonGuide == null)
			{
				return;
			}
		}
		m_btnGuide_SelChatroom = GameGlobalUtil.GetXlsProgramText("MSGMENU_BTNGUIDE_SEL_CHATROOM");
		m_btnGuide_SubmitChatroom = GameGlobalUtil.GetXlsProgramText("MSGMENU_BTNGUIDE_SUBMIT_CHATROOM");
		m_btnGuide_ExitMenu = GameGlobalUtil.GetXlsProgramText("MSGMENU_BTNGUIDE_EXIT_MENU");
		m_btnGuide_ExitChatroom = GameGlobalUtil.GetXlsProgramText("MSGMENU_BTNGUIDE_EXIT_CHATROOM");
		m_btnGuide_SelContent = GameGlobalUtil.GetXlsProgramText("MSGMENU_BTNGUIDE_SEL_CONTENT");
		m_btnGuide_ShowImage = GameGlobalUtil.GetXlsProgramText("MSGMENU_BTNGUIDE_SHOW_IMAGE");
		m_btnGuide_ScrollToTop = GameGlobalUtil.GetXlsProgramText("MSGMENU_BTNGUIDE_SCROLL_TO_TOP");
		m_btnGuide_SelectTab = GameGlobalUtil.GetXlsProgramText("MSGMENU_BTNGUIDE_SEL_TAB");
		m_btnGuide_SubmitTab = GameGlobalUtil.GetXlsProgramText("MSGMENU_BTNGUIDE_SUBMIT_TAB");
		m_btnGuide_CancelTab = GameGlobalUtil.GetXlsProgramText("MSGMENU_BTNGUIDE_CANCEL_TAB");
		m_isInitedButtonGuidText = true;
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_btnGuide_SelChatroom, PadInput.GameInput.LStickY);
		m_ButtonGuide.AddContent(m_btnGuide_SubmitChatroom, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_btnGuide_SelContent, PadInput.GameInput.LStickY);
		m_ButtonGuide.AddContent(m_btnGuide_ExitMenu, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_btnGuide_ExitChatroom, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_btnGuide_ScrollToTop, PadInput.GameInput.TriangleButton);
		m_ButtonGuide.AddContent(m_btnGuide_SubmitTab, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_btnGuide_CancelTab, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_btnGuide_SelectTab, PadInput.GameInput.L1Button);
		m_ButtonGuide.AddContent(m_btnGuide_SelectTab, PadInput.GameInput.R1Button);
		m_ButtonGuide.AddContent(m_btnGuide_ShowImage, PadInput.GameInput.SquareButton);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Left);
		m_ButtonGuide.SetShow(isShow: true);
	}

	private IEnumerator InitChatroomButtons(List<Xls.MessengerTalkData> activedTalkDatas)
	{
		ClearChatroomButtons();
		SortedDictionary<string, object> validChatroomIDs = new SortedDictionary<string, object>();
		Xls.MessengerTalkData xlsTalkData = null;
		int dataCount = activedTalkDatas.Count;
		for (int i = 0; i < dataCount; i++)
		{
			xlsTalkData = activedTalkDatas[i];
			if (!validChatroomIDs.ContainsKey(xlsTalkData.m_strIDchat))
			{
				validChatroomIDs.Add(xlsTalkData.m_strIDchat, null);
			}
		}
		if (validChatroomIDs.Count < 0)
		{
			yield break;
		}
		List<Xls.MessengerChatroomData> xlsChatroomDatas = Xls.MessengerChatroomData.datas;
		Xls.MessengerChatroomData xlsChatroomData = null;
		GameObject newGameObject = null;
		MSGChatRoomButton msgChatroomButton = null;
		Vector3 vBasePos = Vector3.zero;
		Quaternion qBaseRot = Quaternion.identity;
		float fCurY = 0f;
		float fTotalHeight = 0f;
		SortedDictionary<string, MSGChatRoomButton> dicChatroomInfos = new SortedDictionary<string, MSGChatRoomButton>();
		dataCount = xlsChatroomDatas.Count;
		for (int j = 0; j < dataCount; j++)
		{
			xlsChatroomData = xlsChatroomDatas[j];
			if (validChatroomIDs.ContainsKey(xlsChatroomData.m_strID))
			{
				newGameObject = UnityEngine.Object.Instantiate(m_ChatroomButtonSrcObj, vBasePos, qBaseRot);
				newGameObject.name = $"ChatroomButton_{j}";
				msgChatroomButton = newGameObject.GetComponent<MSGChatRoomButton>();
				m_ChatroomButtons.Add(msgChatroomButton);
				dicChatroomInfos.Add(xlsChatroomData.m_strID, msgChatroomButton);
				yield return StartCoroutine(msgChatroomButton.InitValue(xlsChatroomData));
				msgChatroomButton.rectTransform.SetParent(m_ChatroomContainer.m_ContainerRT, worldPositionStays: false);
				msgChatroomButton.rectTransform.anchoredPosition = new Vector2(msgChatroomButton.rectTransform.anchoredPosition.x, fCurY);
				msgChatroomButton.onClickedProc = OnClick_ChatroomButton;
				fCurY -= msgChatroomButton.rectTransform.rect.height + m_ChatroomContainer.m_ContentInterval;
				fTotalHeight += msgChatroomButton.rectTransform.rect.height;
			}
		}
		if (dataCount > 1)
		{
			fTotalHeight += m_ChatroomContainer.m_ContentInterval * (float)(dataCount - 1);
		}
		float fContanierHeight = Mathf.Max(m_ChatroomContainer.m_ScrollRect.viewport.rect.height, fTotalHeight);
		m_ChatroomContainer.m_ContainerRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fContanierHeight);
		m_ChatroomContainer.m_ScrollHandler.ResetScrollRange();
		m_ChatroomContainer.m_ScrollHandler.scrollPos = 0f;
		dataCount = activedTalkDatas.Count;
		for (int k = 0; k < dataCount; k++)
		{
			xlsTalkData = activedTalkDatas[k];
			if (dicChatroomInfos.ContainsKey(xlsTalkData.m_strIDchat))
			{
				dicChatroomInfos[xlsTalkData.m_strIDchat].xlsTalkDatas.Add(xlsTalkData);
			}
		}
	}

	private void ClearChatroomButtons()
	{
		int count = m_ChatroomButtons.Count;
		for (int i = 0; i < count; i++)
		{
			UnityEngine.Object.Destroy(m_ChatroomButtons[i].gameObject);
		}
		m_ChatroomButtons.Clear();
	}

	private MSGChatRoomButton GetChatroomButton_byKey(string key)
	{
		int count = m_ChatroomButtons.Count;
		MSGChatRoomButton mSGChatRoomButton = null;
		for (int i = 0; i < count; i++)
		{
			mSGChatRoomButton = m_ChatroomButtons[i];
			if (mSGChatRoomButton.xlsChatroomData.m_strID == key)
			{
				return mSGChatRoomButton;
			}
		}
		return null;
	}

	private void SetEnableChatroomCursor(bool _enable)
	{
		int count = m_ChatroomButtons.Count;
		for (int i = 0; i < count; i++)
		{
			m_ChatroomButtons[i].acitveOnCursor = _enable;
		}
	}

	private void OnClick_ChatroomButton(object sender, object args)
	{
		if (sender == null || !(sender is MSGChatRoomButton))
		{
			return;
		}
		MSGChatRoomButton mSGChatRoomButton = sender as MSGChatRoomButton;
		if (!(mSGChatRoomButton == m_SelectedChatroomButton))
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_OK");
			}
			SetOnCursorChatroom(mSGChatRoomButton);
			SetSelectedChatroom(mSGChatRoomButton);
		}
	}

	private void ChangeMode(Mode nextMode)
	{
		if (m_curMode == nextMode)
		{
			return;
		}
		switch (m_curMode)
		{
		case Mode.SelChatRoom:
			SetEnableChatroomCursor(_enable: false);
			if (m_ChatroomTitleText != null)
			{
				m_ChatroomTitleText.gameObject.SetActive(value: false);
			}
			if (m_ChatroomAnimator != null)
			{
				m_ChatroomAnimator.Play(ChatroomAniState.active_out.ToString());
			}
			break;
		case Mode.InChatRoom:
		{
			string strMot = GameDefine.UIAnimationState.disappear.ToString();
			if (m_TabContainer != null)
			{
				m_TabContainer.isInputBlock = true;
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_TabContainer.gameObject, strMot);
			}
			if (m_ContentsRootObj != null)
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_ContentsRootObj, strMot);
			}
			if (m_NoMessgeTextRoot != null)
			{
				m_NoMessgeTextRoot.gameObject.SetActive(value: false);
			}
			SetOnCursorContentIndex(-1);
			break;
		}
		}
		switch (nextMode)
		{
		case Mode.SelChatRoom:
			m_SelectedChatroomButton = null;
			SetEnableChatroomCursor(_enable: true);
			if (m_ChatroomTitleText != null)
			{
				m_ChatroomTitleText.gameObject.SetActive(value: true);
			}
			foreach (MSGChatRoomButton chatroomButton in m_ChatroomButtons)
			{
				chatroomButton.gameObject.SetActive(value: true);
			}
			if (m_ChatroomAnimator != null)
			{
				m_ChatroomAnimator.Play(ChatroomAniState.active_in.ToString());
			}
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentEnable(m_btnGuide_SelChatroom, isEnable: true, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_SubmitChatroom, isEnable: true, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_ExitMenu, isEnable: true, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_ExitChatroom, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_SelContent, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_ShowImage, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_ScrollToTop, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_SelectTab, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_SubmitTab, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_CancelTab, isEnable: false);
			}
			break;
		case Mode.InChatRoom:
		{
			if (m_ContentContainer.m_ScrollbarRoot != null)
			{
				m_ContentContainer.m_ScrollbarRoot.SetActive(value: true);
			}
			m_ContentContainer.m_ContainerRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
			m_ContentContainer.m_ScrollHandler.ResetScrollRange();
			string strMot2 = GameDefine.UIAnimationState.appear.ToString();
			if (m_TabContainer != null)
			{
				m_TabContainer.gameObject.SetActive(value: true);
				m_TabContainer.isInputBlock = false;
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_TabContainer.gameObject, strMot2);
			}
			if (m_ContentsRootObj != null)
			{
				m_ContentsRootObj.gameObject.SetActive(value: true);
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_ContentsRootObj.gameObject, strMot2);
			}
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentEnable(m_btnGuide_SelChatroom, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_SubmitChatroom, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_ExitMenu, m_ChatroomButtons.Count <= 1, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_ExitChatroom, m_ChatroomButtons.Count > 1, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_SelContent, isEnable: true, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_ShowImage, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_ScrollToTop, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_SelectTab, m_TabContainer.tabButtonInfos.Count > 1, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_SubmitTab, isEnable: false, isNeedAlign: false);
				m_ButtonGuide.SetContentEnable(m_btnGuide_CancelTab, isEnable: false);
			}
			break;
		}
		}
		m_curMode = nextMode;
		if (m_swSubMsgMenu != null)
		{
			m_swSubMsgMenu.OnNotice_ChangedMessengerMode(m_curMode);
		}
	}

	private void SetOnCursorChatroom(MSGChatRoomButton onCursorChatroom, bool isAdjustScrollPos = true)
	{
		if (!(m_OnCursorChatroomButton == onCursorChatroom))
		{
			if (m_OnCursorChatroomButton != null)
			{
				m_OnCursorChatroomButton.selected = false;
			}
			if (onCursorChatroom != null)
			{
				onCursorChatroom.selected = true;
			}
			m_OnCursorChatroomButton = onCursorChatroom;
		}
	}

	private bool ChangeOnCursorChatroom(bool isUpSide, bool isAdjustScrollPos = true)
	{
		int count = m_ChatroomButtons.Count;
		if (m_OnCursorChatroomButton == null)
		{
			if (count > 0)
			{
				SetOnCursorChatroom(m_ChatroomButtons[count - 1], isAdjustScrollPos: false);
			}
			return false;
		}
		int num = m_ChatroomButtons.IndexOf(m_OnCursorChatroomButton);
		num = ((!isUpSide) ? (num + 1) : (num - 1));
		if (num < 0)
		{
			if (!m_ChatroomContainer.m_ScrollLoopEnable)
			{
				return false;
			}
			num = count - 1;
		}
		else if (num >= count)
		{
			if (!m_ChatroomContainer.m_ScrollLoopEnable)
			{
				return false;
			}
			num = 0;
		}
		SetOnCursorChatroom(m_ChatroomButtons[num], isAdjustScrollPos);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentActivate(m_btnGuide_SelChatroom, isActivate: true);
		}
		return true;
	}

	private void SetSelectedChatroomAtInit(MSGChatRoomButton chatroomButton)
	{
		foreach (MSGChatRoomButton chatroomButton2 in m_ChatroomButtons)
		{
			chatroomButton2.gameObject.SetActive(value: true);
		}
		SetOnCursorChatroom(chatroomButton);
		SetSelectedChatroom(chatroomButton);
	}

	private void SetSelectedChatroom(MSGChatRoomButton chatroomButton)
	{
		if (m_SelectedChatroomButton == chatroomButton)
		{
			return;
		}
		SetOnCursorChatroom(chatroomButton);
		ChangeMode(Mode.InChatRoom);
		m_SelectedChatroomButton = chatroomButton;
		if (m_SelectedChatroomButton == null)
		{
			return;
		}
		List<CommonTabContainerPlus.TabButtonInfo> tabButtonInfos = m_TabContainer.tabButtonInfos;
		int count = tabButtonInfos.Count;
		List<Xls.MessengerTalkData> xlsTalkDatas = m_SelectedChatroomButton.xlsTalkDatas;
		int count2 = xlsTalkDatas.Count;
		CommonTabContainerPlus.TabButtonInfo tabButtonInfo = null;
		if (count2 > 0 && count > 0)
		{
			Xls.SequenceData sequenceData = null;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int i = 0; i < count; i++)
			{
				sequenceData = tabButtonInfos[i].tag as Xls.SequenceData;
				dictionary.Add(sequenceData.m_strKey, 0);
			}
			Xls.MessengerTalkData messengerTalkData = null;
			for (int j = 0; j < count2; j++)
			{
				messengerTalkData = xlsTalkDatas[j];
				if (dictionary.ContainsKey(messengerTalkData.m_strIDSeq))
				{
					dictionary[messengerTalkData.m_strIDSeq]++;
				}
			}
			CommonTabContainerPlus.TabButtonInfo tabButtonInfo2 = null;
			int num = count;
			while (num > 0)
			{
				num--;
				tabButtonInfo2 = tabButtonInfos[num];
				sequenceData = tabButtonInfo2.tag as Xls.SequenceData;
				if (dictionary[sequenceData.m_strKey] > 0)
				{
					tabButtonInfo2.tabButtonComp.isEnableButton = true;
					if (tabButtonInfo == null)
					{
						tabButtonInfo = tabButtonInfo2;
					}
				}
				else
				{
					tabButtonInfo2.tabButtonComp.isEnableButton = false;
				}
			}
		}
		else
		{
			for (int k = 0; k < count; k++)
			{
				tabButtonInfos[k].tabButtonComp.isEnableButton = false;
			}
		}
		if (count > 0)
		{
			if (tabButtonInfo == null)
			{
				tabButtonInfo = tabButtonInfos[count - 1];
			}
			m_TabContainer.SetSelectedTab_byObject(tabButtonInfo);
		}
	}

	private void AdjustScrollPos_byOnCursorChatRoom()
	{
		if (!(m_OnCursorChatroomButton == null))
		{
			float height = m_ChatroomContainer.m_ScrollRect.viewport.rect.height;
			float num = 0f - m_ChatroomContainer.m_ContainerRT.offsetMax.y;
			float num2 = num - height;
			RectTransform rectTransform = m_OnCursorChatroomButton.rectTransform;
			float y = rectTransform.offsetMax.y;
			float num3 = rectTransform.offsetMax.y - rectTransform.rect.height * rectTransform.localScale.y;
			if (num3 < num2)
			{
				float fTargetPos = num3 + height;
				m_ChatroomContainer.m_ScrollHandler.ScrollToTargetPos(fTargetPos);
			}
			else if (y > num)
			{
				float fTargetPos2 = y;
				m_ChatroomContainer.m_ScrollHandler.ScrollToTargetPos(fTargetPos2);
			}
		}
	}

	private List<CommonTabContainerPlus.TabCreateInfo> SetTabCreateInfos()
	{
		List<CommonTabContainerPlus.TabCreateInfo> list = new List<CommonTabContainerPlus.TabCreateInfo>();
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		if (gameSwitch != null)
		{
			List<GameSwitch.SequenceInfo> sequencceList = gameSwitch.GetSequencceList();
			if (sequencceList != null && sequencceList.Count > 0)
			{
				GameSwitch.SequenceInfo sequenceInfo = null;
				Xls.SequenceData sequenceData = null;
				Xls.TextData textData = null;
				int count = sequencceList.Count;
				for (int i = 0; i < count; i++)
				{
					sequenceInfo = sequencceList[i];
					sequenceData = Xls.SequenceData.GetData_bySwitchIdx(sequenceInfo.m_iSeqIdx);
					if (sequenceData != null)
					{
						textData = Xls.TextData.GetData_byKey(sequenceData.m_strIDName);
						CommonTabContainerPlus.TabCreateInfo tabCreateInfo = new CommonTabContainerPlus.TabCreateInfo();
						list.Add(tabCreateInfo);
						tabCreateInfo.m_Text = ((textData == null) ? string.Empty : textData.m_strTxt);
						tabCreateInfo.m_Tag = sequenceData;
					}
				}
			}
		}
		return list;
	}

	private void OnChangedSelectTab(object sender, object arg)
	{
		if (sender is CommonTabContainerPlus.TabButtonInfo { tag: not null, tag: Xls.SequenceData activeMsgDatas })
		{
			SetActiveMsgDatas(activeMsgDatas);
			if (!m_isInitailizedSlots)
			{
				StartCoroutine(CreateContentSlots());
			}
			else
			{
				SetCurrentContentInfo();
			}
		}
	}

	private void OnChangingSelectTab(object sender, object arg)
	{
		bool flag = (bool)arg;
		if (m_ChangingTabCover != null)
		{
			m_ChangingTabCover.SetActive(flag);
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentEnable(m_btnGuide_SubmitTab, flag, isNeedAlign: false);
			m_ButtonGuide.SetContentEnable(m_btnGuide_CancelTab, flag, isNeedAlign: false);
			m_ButtonGuide.SetContentEnable(m_btnGuide_SelContent, !flag, isNeedAlign: false);
			m_ButtonGuide.SetContentEnable((m_ChatroomButtons.Count <= 1) ? m_btnGuide_ExitMenu : m_btnGuide_ExitChatroom, !flag, isNeedAlign: false);
			m_ButtonGuide.SetContentEnable(m_btnGuide_ScrollToTop, !flag && m_ContentContainer.m_ScrollHandler.isScrollable, isNeedAlign: false);
			m_ButtonGuide.SetContentEnable(m_btnGuide_ShowImage, !flag && m_OnCursorContent != null && m_OnCursorContent.isExistImage);
		}
	}

	private void OnProc_PressedTabButtons(object sender, object arg)
	{
		switch ((PadInput.GameInput)arg)
		{
		case PadInput.GameInput.CircleButton:
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_btnGuide_SubmitTab, isActivate: true);
			}
			break;
		case PadInput.GameInput.CrossButton:
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_btnGuide_CancelTab, isActivate: true);
			}
			break;
		case PadInput.GameInput.L1Button:
		case PadInput.GameInput.R1Button:
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_btnGuide_SelectTab, isActivate: true);
			}
			break;
		case PadInput.GameInput.TriangleButton:
		case PadInput.GameInput.SquareButton:
			break;
		}
	}

	private void SetActiveMsgDatas(Xls.SequenceData xlsSequenceData)
	{
		if (m_curActiveMsgDatas != null)
		{
			m_curActiveMsgDatas.Clear();
		}
		if (!(m_SelectedChatroomButton == null))
		{
			m_curActiveMsgDatas = m_SelectedChatroomButton.GetTalkData_bySequence(xlsSequenceData.m_strKey);
		}
	}

	private IEnumerator CreateContentSlots()
	{
		if (!MSGContentPlus.initializedPrefabs)
		{
			yield return StartCoroutine(MSGContentPlus.LoadMsgContentPrefabs());
		}
		m_isInputBlock = true;
		m_TabContainer.isInputBlock = true;
		if (m_LoadingIcon == null)
		{
			m_LoadingIcon = LoadingSWatchIcon.Create(m_ContentContainer.m_ScrollRect.gameObject);
		}
		m_LoadingIcon.gameObject.SetActive(value: true);
		ClearContentSlots();
		MSGContentPlus contentSlot = null;
		for (int i = 0; i < 10; i++)
		{
			contentSlot = MSGContentPlus.CreateMSGContentSlot(MSGContentPlus.MSGContentType.Me);
			m_MeContentSlots.Add(contentSlot);
			contentSlot.transform.SetParent(m_ContentContainer.m_ContainerRT, worldPositionStays: false);
			contentSlot.gameObject.SetActive(value: false);
			contentSlot.type = MSGContentPlus.MSGContentType.Me;
			contentSlot.linkedMsgMenu = this;
			yield return null;
		}
		for (int j = 0; j < 5; j++)
		{
			contentSlot = MSGContentPlus.CreateMSGContentSlot(MSGContentPlus.MSGContentType.Partner);
			m_PartnerContentSlots.Add(contentSlot);
			contentSlot.transform.SetParent(m_ContentContainer.m_ContainerRT, worldPositionStays: false);
			contentSlot.gameObject.SetActive(value: false);
			contentSlot.type = MSGContentPlus.MSGContentType.Partner;
			contentSlot.linkedMsgMenu = this;
			yield return null;
		}
		for (int k = 0; k < 5; k++)
		{
			contentSlot = MSGContentPlus.CreateMSGContentSlot(MSGContentPlus.MSGContentType.PartnerFirst);
			m_PartnerFirstContentSlots.Add(contentSlot);
			contentSlot.transform.SetParent(m_ContentContainer.m_ContainerRT, worldPositionStays: false);
			contentSlot.gameObject.SetActive(value: false);
			contentSlot.type = MSGContentPlus.MSGContentType.PartnerFirst;
			contentSlot.linkedMsgMenu = this;
			yield return null;
		}
		SetContentSlotLayoutInfo(m_MeContentSlots[0], ref m_MeContentSlotLayoutInfo);
		SetContentSlotLayoutInfo(m_PartnerContentSlots[0], ref m_PartnerContentSlotLayoutInfo);
		SetContentSlotLayoutInfo(m_PartnerFirstContentSlots[0], ref m_PartnerFirstContentSlotLayoutInfo);
		SetCurrentContentInfo();
		m_isInitailizedSlots = true;
		m_isInputBlock = false;
		m_TabContainer.isInputBlock = false;
		m_LoadingIcon.gameObject.SetActive(value: false);
	}

	private void ClearContentSlots()
	{
		MSGContentPlus mSGContentPlus = null;
		List<MSGContentPlus>.Enumerator enumerator = m_MeContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			mSGContentPlus = enumerator.Current;
			if (!(mSGContentPlus == null))
			{
				UnityEngine.Object.Destroy(mSGContentPlus.gameObject);
			}
		}
		enumerator = m_PartnerContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			mSGContentPlus = enumerator.Current;
			if (!(mSGContentPlus == null))
			{
				UnityEngine.Object.Destroy(mSGContentPlus.gameObject);
			}
		}
		enumerator = m_PartnerFirstContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			mSGContentPlus = enumerator.Current;
			if (!(mSGContentPlus == null))
			{
				UnityEngine.Object.Destroy(mSGContentPlus.gameObject);
			}
		}
		m_MeContentSlots.Clear();
		m_PartnerContentSlots.Clear();
		m_PartnerFirstContentSlots.Clear();
		m_activedContentSlots.Clear();
		m_isInitailizedSlots = false;
	}

	private void SetContentSlotLayoutInfo(MSGContentPlus contentSlot, ref ContentSlotLayoutInfo layoutInfo)
	{
		RectTransform foRectTransform = contentSlot.foRectTransform;
		RectTransform component = contentSlot.m_ImageRoot.m_ContentText.gameObject.GetComponent<RectTransform>();
		RectTransform component2 = contentSlot.m_ContentImageRoot.GetComponent<RectTransform>();
		layoutInfo.heightWithoutText = foRectTransform.rect.height - component.rect.height;
		layoutInfo.heightThumbnailImage = component2.rect.height;
		layoutInfo.scaleY = foRectTransform.localScale.y;
		layoutInfo.textGenSetting_WithImage = contentSlot.m_ImageRoot.m_ContentText.GetGenerationSettings(component.rect.size);
		layoutInfo.textGenSetting_WithImage.scaleFactor = 1f;
		layoutInfo.textGenSetting_WithImage.verticalOverflow = VerticalWrapMode.Overflow;
		layoutInfo.textGenSetting_WithImage.generationExtents.x -= 15f;
		component = contentSlot.m_NoImageRoot.m_ContentText.gameObject.GetComponent<RectTransform>();
		layoutInfo.textGenSetting_WithoutImage = contentSlot.m_NoImageRoot.m_ContentText.GetGenerationSettings(component.rect.size);
		layoutInfo.textGenSetting_WithoutImage.scaleFactor = 1f;
		layoutInfo.textGenSetting_WithoutImage.verticalOverflow = VerticalWrapMode.Overflow;
		layoutInfo.textGenSetting_WithoutImage.generationExtents.x -= 15f;
	}

	private void ClearActivedContentSlots()
	{
		MSGContentPlus mSGContentPlus = null;
		List<MSGContentPlus>.Enumerator enumerator = m_activedContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			mSGContentPlus = enumerator.Current;
			if (!(mSGContentPlus == null))
			{
				mSGContentPlus.gameObject.SetActive(value: false);
				mSGContentPlus.SetContentData(null);
			}
		}
		m_activedContentSlots.Clear();
		m_curBaseContentIdx = -1;
	}

	private void SetCurrentContentInfo()
	{
		m_curContentInfos.Clear();
		m_validContentIndexFirst = -1;
		m_validContentIndexLast = -1;
		ClearActivedContentSlots();
		if (m_curActiveMsgDatas != null && m_curActiveMsgDatas.Count > 0)
		{
			Xls.MessengerTalkData messengerTalkData = null;
			MSGContentPlus.MSGContentType mSGContentType = MSGContentPlus.MSGContentType.Unknown;
			bool flag = false;
			float num = 0f;
			float scale = 1f;
			List<Xls.MessengerTalkData>.Enumerator enumerator = m_curActiveMsgDatas.GetEnumerator();
			while (enumerator.MoveNext())
			{
				messengerTalkData = enumerator.Current;
				if (messengerTalkData != null)
				{
					flag = string.Equals("acc_00000", messengerTalkData.m_strIDAcc);
					ContentInfo contentInfo = new ContentInfo();
					contentInfo.xlsData = messengerTalkData;
					contentInfo.contentType = (flag ? ((mSGContentType == MSGContentPlus.MSGContentType.Me || mSGContentType == MSGContentPlus.MSGContentType.MeFrist) ? MSGContentPlus.MSGContentType.Me : MSGContentPlus.MSGContentType.MeFrist) : ((mSGContentType != MSGContentPlus.MSGContentType.Partner && mSGContentType != MSGContentPlus.MSGContentType.PartnerFirst) ? MSGContentPlus.MSGContentType.PartnerFirst : MSGContentPlus.MSGContentType.Partner));
					mSGContentType = contentInfo.contentType;
					contentInfo.isValid = true;
					contentInfo.pos = num;
					contentInfo.height = CalculateContentHeight(messengerTalkData, contentInfo.contentType, out scale);
					contentInfo.scaledHeight = contentInfo.height * scale;
					int count = m_curContentInfos.Count;
					if (m_validContentIndexFirst < 0)
					{
						m_validContentIndexFirst = count;
					}
					if (m_validContentIndexLast < count)
					{
						m_validContentIndexLast = count;
					}
					m_curContentInfos.Add(contentInfo);
					num += ((!contentInfo.isValid) ? 0f : contentInfo.scaledHeight);
				}
			}
			num += m_BottomSpacing;
			float size = Mathf.Max(num, m_ContentContainer.m_ScrollRect.viewport.rect.height);
			m_ContentContainer.m_ContainerRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			m_ContentContainer.m_ScrollHandler.ResetScrollRange();
			m_ContentContainer.m_ScrollHandler.scrollPos = 0f;
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentEnable(m_btnGuide_ScrollToTop, m_ContentContainer.m_ScrollHandler.isScrollable);
			}
			if (m_validContentIndexLast >= 0)
			{
				if (m_ContentContainer.m_ScrollbarRoot != null)
				{
					m_ContentContainer.m_ScrollbarRoot.SetActive(value: true);
				}
				m_ContentContainer.m_ScrollHandler.scrollPos = m_ContentContainer.m_ScrollHandler.scrollPos_Max;
				SetCurrentBaseContentIndex(GetBaseContentIndex_byScrollPos(m_ContentContainer.m_ScrollHandler.scrollPos_Max), isIgnoreAni: false);
				SetOnCursorContentIndex(m_validContentIndexLast);
			}
		}
		else if (m_validContentIndexFirst < 0)
		{
			SetCurrentBaseContentIndex(-1);
		}
	}

	private ContentInfo GetContentInfo_byTalkData(Xls.MessengerTalkData talkData)
	{
		if (talkData == null)
		{
			return null;
		}
		ContentInfo contentInfo = null;
		List<ContentInfo>.Enumerator enumerator = m_curContentInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			contentInfo = enumerator.Current;
			if (contentInfo.xlsData == talkData)
			{
				return contentInfo;
			}
		}
		return null;
	}

	private int GetContentIndex_byTalkData(Xls.MessengerTalkData talkData)
	{
		if (talkData == null)
		{
			return -1;
		}
		ContentInfo contentInfo = null;
		int count = m_curContentInfos.Count;
		for (int i = 0; i < count; i++)
		{
			contentInfo = m_curContentInfos[i];
			if (contentInfo.xlsData == talkData)
			{
				return i;
			}
		}
		return -1;
	}

	private float CalculateContentHeight(Xls.MessengerTalkData xlsData, MSGContentPlus.MSGContentType contentType, out float scale)
	{
		scale = 1f;
		ContentSlotLayoutInfo contentSlotLayoutInfo;
		switch (contentType)
		{
		case MSGContentPlus.MSGContentType.Me:
		case MSGContentPlus.MSGContentType.MeFrist:
			contentSlotLayoutInfo = m_MeContentSlotLayoutInfo;
			break;
		case MSGContentPlus.MSGContentType.Partner:
			contentSlotLayoutInfo = m_PartnerContentSlotLayoutInfo;
			break;
		case MSGContentPlus.MSGContentType.PartnerFirst:
			contentSlotLayoutInfo = m_PartnerFirstContentSlotLayoutInfo;
			break;
		default:
			return 0f;
		}
		scale = contentSlotLayoutInfo.scaleY;
		Xls.TextData data_byKey = Xls.TextData.GetData_byKey(xlsData.m_strIDText);
		string str = ((data_byKey == null) ? string.Empty : data_byKey.m_strTxt);
		bool flag = !string.IsNullOrEmpty(xlsData.m_strIDImg);
		float preferredHeight = m_textGenerator.GetPreferredHeight(str, (!flag) ? contentSlotLayoutInfo.textGenSetting_WithoutImage : contentSlotLayoutInfo.textGenSetting_WithImage);
		float num = ((!flag) ? preferredHeight : Mathf.Max(preferredHeight, contentSlotLayoutInfo.heightThumbnailImage));
		return num + contentSlotLayoutInfo.heightWithoutText;
	}

	private int GetBaseContentIndex_byScrollPos(float scrollPos)
	{
		return __GetBaseContentIndex_byScrollPos(scrollPos, 0, m_curContentInfos.Count);
	}

	private int __GetBaseContentIndex_byScrollPos(float scrollPos, int startIdx, int endIdx)
	{
		if (endIdx <= startIdx)
		{
			return -1;
		}
		int num = ((endIdx <= startIdx + 1) ? startIdx : (startIdx + (endIdx - startIdx) / 2));
		if (num < 0 || num >= m_curContentInfos.Count)
		{
			return -1;
		}
		ContentInfo contentInfo = m_curContentInfos[num];
		if (scrollPos >= contentInfo.pos && scrollPos < contentInfo.pos + contentInfo.scaledHeight)
		{
			return (!contentInfo.isValid) ? __GetBaseContentIndex_byScrollPos(scrollPos, num + 1, endIdx) : num;
		}
		return (!(scrollPos >= contentInfo.pos + contentInfo.scaledHeight)) ? __GetBaseContentIndex_byScrollPos(scrollPos, startIdx, num) : __GetBaseContentIndex_byScrollPos(scrollPos, num + 1, endIdx);
	}

	private void SetActivedContents_byScrollPos(float scrollPos)
	{
		int baseContentIndex_byScrollPos = GetBaseContentIndex_byScrollPos(scrollPos);
		if (m_curBaseContentIdx != baseContentIndex_byScrollPos)
		{
			SetCurrentBaseContentIndex(baseContentIndex_byScrollPos);
		}
	}

	private MSGContentPlus GetEmptyContentSlot(MSGContentPlus.MSGContentType contentType)
	{
		List<MSGContentPlus> list = null;
		switch (contentType)
		{
		case MSGContentPlus.MSGContentType.Me:
		case MSGContentPlus.MSGContentType.MeFrist:
			list = m_MeContentSlots;
			break;
		case MSGContentPlus.MSGContentType.Partner:
			list = m_PartnerContentSlots;
			break;
		case MSGContentPlus.MSGContentType.PartnerFirst:
			list = m_PartnerFirstContentSlots;
			break;
		default:
			return null;
		}
		MSGContentPlus mSGContentPlus = null;
		List<MSGContentPlus>.Enumerator enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			mSGContentPlus = enumerator.Current;
			if (!mSGContentPlus.gameObject.activeSelf || mSGContentPlus.xlsData == null)
			{
				mSGContentPlus.type = contentType;
				return mSGContentPlus;
			}
		}
		mSGContentPlus = MSGContentPlus.CreateMSGContentSlot(contentType);
		mSGContentPlus.transform.SetParent(m_ContentContainer.m_ContainerRT, worldPositionStays: false);
		mSGContentPlus.gameObject.SetActive(value: false);
		mSGContentPlus.type = contentType;
		mSGContentPlus.linkedMsgMenu = this;
		list.Add(mSGContentPlus);
		return mSGContentPlus;
	}

	private void ReleaseActivedContentSlot(MSGContentPlus activedSlot)
	{
		if (m_activedContentSlots.Contains(activedSlot))
		{
			m_activedContentSlots.Remove(activedSlot);
		}
		activedSlot.gameObject.SetActive(value: false);
		activedSlot.SetContentData(null);
		activedSlot.indexInContentInfos = -1;
	}

	private void SetCurrentBaseContentIndex(int idx, bool isIgnoreAni = true)
	{
		int count = m_curContentInfos.Count;
		if (idx < 0 || idx >= count)
		{
			ClearActivedContentSlots();
			SetOnCursorContentIndex(-1);
			if (m_NoMessgeTextRoot != null)
			{
				m_NoMessgeTextRoot.gameObject.SetActive(value: true);
			}
			if (m_ContentContainer.m_ScrollbarRoot != null)
			{
				m_ContentContainer.m_ScrollbarRoot.SetActive(value: false);
			}
			return;
		}
		int curBaseContentIdx = m_curBaseContentIdx;
		if (m_curBaseContentIdx != idx)
		{
			if (m_NoMessgeTextRoot != null)
			{
				m_NoMessgeTextRoot.gameObject.SetActive(value: false);
			}
			float num = m_ContentContainer.m_ScrollRect.viewport.rect.height * 1.5f;
			float num2 = 0f;
			string strMot = GameDefine.UIAnimationState.idle.ToString();
			int count2 = m_activedContentSlots.Count;
			if (m_curBaseContentIdx >= 0 && count2 > 0)
			{
				int indexInContentInfos = m_activedContentSlots[0].indexInContentInfos;
				int indexInContentInfos2 = m_activedContentSlots[count2 - 1].indexInContentInfos;
				if (idx > indexInContentInfos2)
				{
					ClearActivedContentSlots();
				}
				else if (idx >= indexInContentInfos && idx <= indexInContentInfos2)
				{
					int num3 = count2;
					while (num3 > 0)
					{
						num3--;
						MSGContentPlus mSGContentPlus = m_activedContentSlots[num3];
						if (mSGContentPlus.indexInContentInfos >= idx)
						{
							num2 += m_curContentInfos[mSGContentPlus.indexInContentInfos].scaledHeight;
						}
						else
						{
							ReleaseActivedContentSlot(mSGContentPlus);
						}
					}
					idx = indexInContentInfos2 + 1;
				}
				else
				{
					int num4 = 0;
					for (int i = idx; i < count; i++)
					{
						ContentInfo contentInfo = m_curContentInfos[i];
						if (contentInfo.isValid)
						{
							num4 = i;
							num2 += contentInfo.scaledHeight;
							if (num2 >= num)
							{
								break;
							}
						}
					}
					if (indexInContentInfos < num4)
					{
						int num5 = count2;
						while (num5 > 0)
						{
							num5--;
							MSGContentPlus mSGContentPlus2 = m_activedContentSlots[num5];
							if (mSGContentPlus2.indexInContentInfos > num4)
							{
								ReleaseActivedContentSlot(mSGContentPlus2);
							}
						}
						int num6 = indexInContentInfos;
						while (num6 > idx)
						{
							num6--;
							ContentInfo contentInfo2 = m_curContentInfos[num6];
							if (contentInfo2.isValid)
							{
								MSGContentPlus emptyContentSlot = GetEmptyContentSlot(contentInfo2.contentType);
								emptyContentSlot.SetContentData(contentInfo2.xlsData, 0f - contentInfo2.pos, contentInfo2.height);
								emptyContentSlot.indexInContentInfos = num6;
								emptyContentSlot.gameObject.SetActive(value: true);
								m_activedContentSlots.Insert(0, emptyContentSlot);
								if (isIgnoreAni)
								{
									GameGlobalUtil.PlayUIAnimation_WithChidren(emptyContentSlot.gameObject, strMot);
								}
							}
						}
						goto IL_03f7;
					}
					ClearActivedContentSlots();
					num2 = 0f;
				}
			}
			for (int j = idx; j < count; j++)
			{
				ContentInfo contentInfo3 = m_curContentInfos[j];
				if (contentInfo3.isValid)
				{
					MSGContentPlus emptyContentSlot2 = GetEmptyContentSlot(contentInfo3.contentType);
					emptyContentSlot2.SetContentData(contentInfo3.xlsData, 0f - contentInfo3.pos, contentInfo3.height);
					emptyContentSlot2.indexInContentInfos = j;
					emptyContentSlot2.gameObject.SetActive(value: true);
					m_activedContentSlots.Add(emptyContentSlot2);
					if (isIgnoreAni)
					{
						GameGlobalUtil.PlayUIAnimation_WithChidren(emptyContentSlot2.gameObject, strMot);
					}
					num2 += contentInfo3.scaledHeight;
					if (num2 >= num)
					{
						break;
					}
				}
			}
			m_curBaseContentIdx = ((m_activedContentSlots.Count <= 0) ? (-1) : m_activedContentSlots[0].indexInContentInfos);
		}
		goto IL_03f7;
		IL_03f7:
		if (m_onCursorContentIdx >= 0)
		{
			MSGContentPlus contentSlot_byContentInfoIndex = GetContentSlot_byContentInfoIndex(m_onCursorContentIdx);
			if (contentSlot_byContentInfoIndex != null && contentSlot_byContentInfoIndex != m_OnCursorContent)
			{
				SetOnCursorContentIndex(m_onCursorContentIdx, isAdjustScrollPos: false);
			}
		}
		if (m_fpChangedSelectContent != null && curBaseContentIdx != m_curBaseContentIdx)
		{
			m_fpChangedSelectContent(this, m_curBaseContentIdx);
		}
	}

	private MSGContentPlus GetContentSlot_byContentInfoIndex(int idx)
	{
		if (idx < 0 || idx >= m_curContentInfos.Count)
		{
			return null;
		}
		int count = m_activedContentSlots.Count;
		if (count <= 0)
		{
			return null;
		}
		if (idx < m_activedContentSlots[0].indexInContentInfos || idx > m_activedContentSlots[count - 1].indexInContentInfos)
		{
			return null;
		}
		List<MSGContentPlus>.Enumerator enumerator = m_activedContentSlots.GetEnumerator();
		MSGContentPlus mSGContentPlus = null;
		while (enumerator.MoveNext())
		{
			mSGContentPlus = enumerator.Current;
			if (mSGContentPlus.indexInContentInfos == idx)
			{
				return mSGContentPlus;
			}
		}
		return null;
	}

	private void SetOnCursorContentIndex(int idx, bool isAdjustScrollPos = true, bool isSendNotice_ContentChanged = true)
	{
		int onCursorContentIdx = m_onCursorContentIdx;
		m_onCursorContentIdx = ((idx >= 0 && idx < m_curContentInfos.Count && m_curContentInfos[idx].isValid) ? idx : (-1));
		MSGContentPlus contentSlot_byContentInfoIndex = GetContentSlot_byContentInfoIndex(idx);
		SetOnCursorContentSlot(contentSlot_byContentInfoIndex);
		if (isAdjustScrollPos && m_onCursorContentIdx >= 0)
		{
			AdjustScrollPos_byOnCursorContent();
		}
	}

	private void SetOnCursorContentSlot(MSGContentPlus contentSlot)
	{
		if (!(m_OnCursorContent == contentSlot))
		{
			if (m_OnCursorContent != null)
			{
				m_OnCursorContent.onCursor = false;
			}
			if (contentSlot != null)
			{
				contentSlot.onCursor = true;
			}
			m_OnCursorContent = contentSlot;
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentEnable(m_btnGuide_ShowImage, m_OnCursorContent != null && m_OnCursorContent.isExistImage);
			}
		}
	}

	private bool ChangeOnCursorContentIndex(bool isUpSide, bool isAdjustScrollPos = true)
	{
		int count = m_curContentInfos.Count;
		if (m_onCursorContentIdx < 0)
		{
			if (m_validContentIndexFirst >= 0)
			{
				SetOnCursorContentIndex(m_validContentIndexFirst);
			}
			return false;
		}
		int num = m_onCursorContentIdx;
		do
		{
			num += ((!isUpSide) ? 1 : (-1));
			if (num < 0)
			{
				if (!m_ContentContainer.m_ScrollLoopEnable)
				{
					return false;
				}
				num = count - 1;
			}
			else if (num >= count)
			{
				if (!m_ContentContainer.m_ScrollLoopEnable)
				{
					return false;
				}
				num = 0;
			}
			if (m_curContentInfos[num].isValid)
			{
				SetOnCursorContentIndex(num);
				m_AudioManager.PlayUISound("Menu_Select");
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetContentActivate(m_btnGuide_SelContent, isActivate: true);
				}
				return true;
			}
		}
		while (num != m_onCursorContentIdx);
		return false;
	}

	private void SetEmptyContents()
	{
		if (m_NoMessgeTextRoot == null || m_NoMessageText == null)
		{
			return;
		}
		if (m_EmptyContentText == null)
		{
			Xls.ProgramText data_byKey = Xls.ProgramText.GetData_byKey("MSGMENU_EMPTY_CONTENT");
			if (data_byKey != null)
			{
				m_EmptyContentText = data_byKey.m_strTxt;
			}
		}
		m_NoMessageText.text = ((m_EmptyContentText == null) ? string.Empty : m_EmptyContentText);
		m_NoMessageText.gameObject.SetActive(value: true);
		m_NoMessgeTextRoot.SetActive(value: true);
		m_ContentContainer.m_ContainerRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
		m_ContentContainer.m_ScrollHandler.ResetScrollRange();
		if (m_ContentContainer.m_ScrollbarRoot != null)
		{
			m_ContentContainer.m_ScrollbarRoot.SetActive(value: false);
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentEnable(m_btnGuide_ScrollToTop, isEnable: false);
		}
		m_isInputBlock = false;
		m_TabContainer.isInputBlock = false;
		m_isNeedContentAlign = false;
		SetOnCursorContentIndex(-1);
	}

	private bool ChangeScrollPage(bool isUpSide)
	{
		if (!m_ContentContainer.m_ScrollHandler.isScrollable || m_ContentContainer.m_ScrollHandler.IsScrolling)
		{
			return false;
		}
		if (m_onCursorContentIdx < 0 || m_onCursorContentIdx >= m_curContentInfos.Count)
		{
			return false;
		}
		if (isUpSide && m_ContentContainer.m_ScrollHandler.scrollPos <= 0f)
		{
			if (m_onCursorContentIdx > m_validContentIndexFirst)
			{
				SetOnCursorContentIndex(m_validContentIndexFirst);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Select");
				}
			}
			return false;
		}
		if (!isUpSide && m_ContentContainer.m_ScrollHandler.scrollPos >= m_ContentContainer.m_ScrollHandler.scrollPos_Max)
		{
			if (m_onCursorContentIdx < m_validContentIndexLast)
			{
				SetOnCursorContentIndex(m_validContentIndexLast);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Select");
				}
			}
			return false;
		}
		float num = m_ContentContainer.m_ScrollRect.viewport.rect.height - m_BottomSpacing;
		float value = m_ContentContainer.m_ScrollHandler.scrollPos + ((!isUpSide) ? num : (0f - num));
		value = Mathf.Clamp(value, 0f, m_ContentContainer.m_ScrollHandler.scrollPos_Max);
		int num2 = GetBaseContentIndex_byScrollPos(value);
		if (num2 < 0 || num2 >= m_curContentInfos.Count)
		{
			return false;
		}
		if (isUpSide)
		{
			ContentInfo contentInfo = null;
			do
			{
				contentInfo = m_curContentInfos[num2];
				if (contentInfo != null && contentInfo.isValid && contentInfo.pos >= value)
				{
					break;
				}
				num2++;
			}
			while (num2 < m_onCursorContentIdx);
		}
		if (num2 == m_onCursorContentIdx)
		{
			if (!isUpSide && num2 < m_validContentIndexLast)
			{
				SetOnCursorContentIndex(m_validContentIndexLast);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Select");
				}
			}
			return false;
		}
		if (m_onCursorContentIdx != num2 && m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		SetOnCursorContentIndex(num2, isAdjustScrollPos: false);
		m_ContentContainer.m_ScrollHandler.ScrollToTargetPos(0f - m_curContentInfos[num2].pos);
		return true;
	}

	private void ChangeOnCursorContent_byScrollPos()
	{
		if (m_onCursorContentIdx < 0 || m_onCursorContentIdx >= m_curContentInfos.Count)
		{
			return;
		}
		float height = m_ContentContainer.m_ScrollRect.viewport.rect.height;
		float num = 0f - m_ContentContainer.m_ContainerRT.offsetMax.y;
		float f = num - height;
		int num2 = Mathf.CeilToInt(num);
		int num3 = Mathf.FloorToInt(f);
		ContentInfo contentInfo = m_curContentInfos[m_onCursorContentIdx];
		float num4 = 0f - contentInfo.pos;
		float num5 = num4 - contentInfo.scaledHeight;
		num5 -= m_BottomSpacing;
		if (Mathf.CeilToInt(num5) < num3)
		{
			int num6 = m_onCursorContentIdx;
			while (num6 > 0)
			{
				num6--;
				contentInfo = m_curContentInfos[num6];
				if (contentInfo.isValid)
				{
					num5 = 0f - (contentInfo.pos + contentInfo.scaledHeight);
					num5 -= m_BottomSpacing;
					if (Mathf.CeilToInt(num5) >= num3)
					{
						SetOnCursorContentIndex(num6, isAdjustScrollPos: false);
						m_AudioManager.PlayUISound("Menu_Select");
						break;
					}
				}
			}
		}
		else
		{
			if (Mathf.FloorToInt(num4) <= num2)
			{
				return;
			}
			int count = m_curContentInfos.Count;
			for (int i = m_onCursorContentIdx + 1; i < count; i++)
			{
				contentInfo = m_curContentInfos[i];
				if (contentInfo.isValid)
				{
					num4 = 0f - contentInfo.pos;
					if (Mathf.FloorToInt(num4) <= num2)
					{
						SetOnCursorContentIndex(i, isAdjustScrollPos: false);
						m_AudioManager.PlayUISound("Menu_Select");
						break;
					}
				}
			}
		}
	}

	private void AdjustScrollPos_byOnCursorContent()
	{
		if (m_onCursorContentIdx >= 0 && m_onCursorContentIdx < m_curContentInfos.Count)
		{
			float height = m_ContentContainer.m_ScrollRect.viewport.rect.height;
			float num = 0f - m_ContentContainer.m_ContainerRT.offsetMax.y;
			float num2 = num - height;
			ContentInfo contentInfo = m_curContentInfos[m_onCursorContentIdx];
			float num3 = 0f - contentInfo.pos;
			float num4 = num3 - contentInfo.scaledHeight;
			num4 -= m_BottomSpacing;
			if (num4 < num2)
			{
				float fTargetPos = num4 + height;
				m_ContentContainer.m_ScrollHandler.ScrollToTargetPos(fTargetPos);
			}
			else if (num3 > num)
			{
				float fTargetPos2 = num3;
				m_ContentContainer.m_ScrollHandler.ScrollToTargetPos(fTargetPos2);
			}
		}
	}

	public IEnumerator OnProc_ViewImageDetail(MSGContentPlus msgContent)
	{
		if (msgContent == null)
		{
			yield break;
		}
		Xls.CollImages xlsCollectionImage = Xls.CollImages.GetData_byKey(msgContent.xlsData.m_strIDImg);
		if (xlsCollectionImage == null)
		{
			yield break;
		}
		if (string.IsNullOrEmpty(xlsCollectionImage.m_strIDColImageDest))
		{
			if (m_ShowImageOrigin == null)
			{
				yield break;
			}
			Xls.ImageFile xlsImageFile = Xls.ImageFile.GetData_byKey(xlsCollectionImage.m_strIDImg);
			if (xlsImageFile == null)
			{
				yield break;
			}
			m_isInputBlock = true;
			m_TabContainer.isInputBlock = true;
			m_showingImageAssetObjHdr = new AssetBundleObjectHandler(xlsImageFile.m_strAssetPath);
			yield return StartCoroutine(m_showingImageAssetObjHdr.LoadAssetBundle());
			Sprite spr = m_showingImageAssetObjHdr.GetLoadedAsset_ToSprite();
			if (spr == null)
			{
				OnProc_ClosedViewImageDetail(null, null);
				yield break;
			}
			m_ShowImageOrigin.gameObject.SetActive(value: true);
			m_ShowImageOrigin.ShowImage(isShow: true, spr, OnProc_ClosedViewImageDetail, xlsCollectionImage);
			if (m_BackButtonObj != null)
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackButtonObj, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear));
			}
		}
		else
		{
			ImageDetailViewer imageDetailViewer = MainLoadThing.instance.imageDetailViewer;
			if (imageDetailViewer == null)
			{
				yield break;
			}
			m_isInputBlock = true;
			m_TabContainer.isInputBlock = true;
			imageDetailViewer.SetCanvasOrder((!(m_Canvas != null)) ? 100000 : (m_Canvas.sortingOrder + 1));
			yield return StartCoroutine(imageDetailViewer.ShowImage(xlsCollectionImage, OnProc_ClosedViewImageDetail));
		}
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Detail");
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
	}

	private void OnProc_ClosedViewImageDetail(object sender, object arg)
	{
		if (m_showingImageAssetObjHdr != null)
		{
			m_showingImageAssetObjHdr.UnloadAssetBundle();
		}
		m_isInputBlock = false;
		m_TabContainer.isInputBlock = false;
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: true);
		}
		if (sender != null && sender is ShowImageOriginSize && m_BackButtonObj != null)
		{
			m_BackButtonObj.SetActive(value: true);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackButtonObj, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.appear));
		}
	}

	public void OnClick_ContentScrollToTop()
	{
		if (m_ContentContainer.m_ScrollHandler.isScrollable)
		{
			m_ContentContainer.m_ScrollHandler.ScrollToTargetPos(0f);
			if (m_validContentIndexFirst >= 0)
			{
				SetOnCursorContentIndex(m_validContentIndexFirst, isAdjustScrollPos: false);
			}
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Scroll_Page");
			}
		}
	}

	public ContentInfo GetTalkDataNearBy(int baseContentIndex, int offset)
	{
		int count = m_curContentInfos.Count;
		if (count <= 0)
		{
			return null;
		}
		if (baseContentIndex < 0 || baseContentIndex >= count)
		{
			return null;
		}
		if (offset > 0)
		{
			ContentInfo contentInfo = null;
			for (int i = baseContentIndex + 1; i < count; i++)
			{
				contentInfo = m_curContentInfos[i];
				if (contentInfo.isValid)
				{
					offset--;
					if (offset <= 0)
					{
						return contentInfo;
					}
				}
			}
		}
		else if (offset < 0)
		{
			ContentInfo contentInfo2 = null;
			int num = baseContentIndex;
			while (num > 0)
			{
				num--;
				contentInfo2 = m_curContentInfos[num];
				if (contentInfo2.isValid)
				{
					offset++;
					if (offset >= 0)
					{
						return contentInfo2;
					}
				}
			}
		}
		else
		{
			ContentInfo contentInfo3 = m_curContentInfos[baseContentIndex];
			if (contentInfo3.isValid)
			{
				return contentInfo3;
			}
		}
		return null;
	}

	private void ShowTutorialPopup()
	{
		string strTutorialKey = "tuto_00017";
		m_isTutorialActivated = TutorialPopup.isShowAble(strTutorialKey);
		if (m_isTutorialActivated)
		{
			StartCoroutine(TutorialPopup.Show(strTutorialKey, OnClosed_TutorialPopup, base.gameObject.GetComponentInChildren<Canvas>()));
		}
		if (m_isTutorialActivated && m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
	}

	private void OnClosed_TutorialPopup(object sender, object arg)
	{
		m_isTutorialActivated = false;
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: true);
		}
	}

	public void OnClickBackButton()
	{
		if (m_curMode == Mode.InChatRoom && m_ChatroomButtons.Count > 1)
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Cancel");
			}
			ChangeMode(Mode.SelChatRoom);
		}
		else
		{
			CloseMSGMenu();
		}
	}

	public void OnClick_Content(MSGContentPlus contentSlot)
	{
		if (m_OnCursorContent != contentSlot && contentSlot != null && m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		int contentIndex_byTalkData = GetContentIndex_byTalkData(contentSlot.xlsData);
		SetOnCursorContentIndex(contentIndex_byTalkData);
	}
}
