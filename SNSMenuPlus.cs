using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class SNSMenuPlus : MonoBehaviour
{
	public enum Mode
	{
		WatchMenu,
		Reply,
		Keyword
	}

	private class ContentInfo
	{
		public Xls.SNSPostData xlsData;

		public float pos;

		public float height;

		public float scaledHeight;

		public bool isValid;

		public bool isNew;

		public bool isHide;
	}

	private struct ContentSlotLayoutInfo
	{
		public float heightWithoutText;

		public float heightThumbnailImage;

		public float scaleY;

		public TextGenerationSettings textGenSetting_WithImage;

		public TextGenerationSettings textGenSetting_WithoutImage;
	}

	private enum MonologueState
	{
		None,
		Idle,
		NotKeyword,
		GetKeyword
	}

	private enum ContentState
	{
		Unknown,
		Enable,
		NomoreKeyword,
		LowMental
	}

	public enum SelPostType
	{
		Normal,
		LeftAnswer,
		RightAnswer
	}

	private enum CharSpecMotion
	{
		watch,
		watch_high,
		watch_low,
		watch_no
	}

	public GameObject m_RootObject;

	public Text m_TitleText;

	public GameObject m_ScreenShotBG;

	public GameObject m_BackGound;

	public CommonTabContainerPlus m_TabContainer;

	public GameObject m_MonologueRoot;

	public Text m_MonologueText;

	public Animator m_CharMotionAnimator;

	public GameObject m_TouchBlockPanel;

	public GameObject m_BackButtonObj;

	[Header("Canvaces")]
	public Canvas m_MainCanvas;

	private Canvas m_SelectionCanvas;

	[Header("Content Container")]
	public ScrollRect m_ScrollRect;

	public RectTransform m_ContentContanierRT;

	public float m_ContentInterval;

	public GameObject m_ChangingTabCover;

	public NextContentsLoadButtton m_NextPageButton;

	public int m_ContentPageSize = 20;

	public float m_BottomSpacing = 80f;

	public Animator m_aniDialogBlackBG;

	[Header("Scroll Members")]
	public Button m_ScrollButtonToFirst;

	public Button m_ScrollButtonRStick;

	public bool m_ScrollLoopEnable = true;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	public float m_ScrollRepeatTime = 0.2f;

	private float m_fScrollButtonPusingTime;

	[Header("Linked Objects")]
	public ShowImageOriginSize m_ShowImageOrigin;

	public UnityEngine.Object m_SelectionMenuPrefab;

	private CommonSelectionPlus m_SelectionMenu;

	private Canvas m_Canvas;

	private AudioManager m_AudioManager;

	private CommonButtonGuide m_ButtonGuide;

	private Mode m_curMode;

	private Xls.SequenceData m_curSequenceData;

	private int m_icurPhaseLevel;

	private bool m_isNeedContentAlign;

	private bool m_isInputBlock;

	private bool m_isMentalGageWaiting;

	private bool m_isPageScrolling;

	private bool m_isTutorialActivated;

	private Animator m_StateCheckAnimator;

	private GameDefine.EventProc m_fpClosedFP;

	private GameDefine.EventProc m_fpChangedSelectContent;

	private string m_MonologueIdleText = string.Empty;

	private LoadingSWatchIcon m_LoadingIcon;

	private SortedDictionary<string, List<Xls.SNSPostData>> m_dicSnsPostDatas = new SortedDictionary<string, List<Xls.SNSPostData>>();

	private SNSContentPlus m_OnCursorContent;

	private List<ContentInfo> m_curContentInfos = new List<ContentInfo>();

	private TextGenerator m_textGenerator;

	private int m_curBaseContentIdx = -1;

	private int m_onCursorContentIdx = -1;

	private int m_validContentIndexFirst = -1;

	private int m_validContentIndexLast = -1;

	private List<ContentInfo> m_ContainKeywordContentInfos = new List<ContentInfo>();

	private List<ContentInfo> m_ReplyContentInfos = new List<ContentInfo>();

	private List<SNSContentPlus> m_NormalContentSlots = new List<SNSContentPlus>();

	private List<SNSContentPlus> m_ReplyContentSlots = new List<SNSContentPlus>();

	private List<SNSContentPlus> m_SharedContentSlots = new List<SNSContentPlus>();

	private List<SNSContentPlus> m_activedContentSlots = new List<SNSContentPlus>();

	private bool m_isInitailizedSlots;

	private ContentSlotLayoutInfo m_NormalContentSlotLayoutInfo = default(ContentSlotLayoutInfo);

	private ContentSlotLayoutInfo m_ReplyContentSlotLayoutInfo = default(ContentSlotLayoutInfo);

	private ContentSlotLayoutInfo m_SharedContentSlotLayoutInfo = default(ContentSlotLayoutInfo);

	private ContentState m_curContentState;

	private bool m_isInitializedButtonGuidText;

	private string m_buttonGuide_SelContent;

	private string m_buttonGuide_ReadPost;

	private string m_buttonGuide_LoadNextPage;

	private string m_buttonGuide_ExitMenu;

	private string m_buttonGuide_ShowImage;

	private string m_buttonGuide_ScrollToTop;

	private string m_buttonGuide_ChangeTab;

	private string m_buttonGuide_SelectTab;

	private string m_buttonGuide_CancelTab;

	private bool m_isRunEvent;

	private GameDefine.eAnimChangeState m_eDialogAniState;

	private bool m_isDialogBlackDisappear;

	private const int c_defContentsDestroyOnce = 10;

	private bool m_isContentsClearing;

	private Animator m_DisapperContentChecker;

	private static SNSMenuPlus s_activedInstance;

	private const string c_assetBundleName = "prefabs/ingame/menu/ui_snsmenu";

	private static AssetBundleObjectHandler s_assetBundleHdr;

	private bool m_isCheckFinishedScroll_ForAddContent;

	private int m_addedContentIndex_backup = -1;

	private Animator m_insertedContentAni;

	private List<string> m_MontTexts_LowMental = new List<string>();

	private List<string> m_MontTexts_NomoreKeyword = new List<string>();

	private List<string> m_MontTexts_OldPost = new List<string>();

	private List<string> m_MontTexts_NoKeyword = new List<string>();

	private List<string> m_MontTexts_GetKeyword = new List<string>();

	private List<string> m_MontTexts_Reply = new List<string>();

	private bool m_isInitializedMonoTexts;

	private SNSContentPlus m_GetKeywordContent;

	private int m_oldMentalGageCanvasOrder;

	private SNSContentPlus m_curReplyContent;

	private Xls.SNSReplyData m_curXlsReplyData;

	private static bool s_isScrollingEvent;

	private static bool s_isAddingContent;

	private AssetBundleObjectHandler m_showingImageAssetObjHdr;

	public static SNSMenuPlus instance => s_activedInstance;

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
		m_ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Vertical, m_ScrollRect, m_ContentContanierRT, null, null, m_ScrollButtonToFirst);
		if (m_ScreenShotBG != null)
		{
			m_ScreenShotBG.SetActive(value: false);
		}
		if (m_NextPageButton != null)
		{
			m_NextPageButton.gameObject.SetActive(value: false);
		}
		if (m_MainCanvas != null && m_SelectionCanvas != null)
		{
			m_SelectionCanvas.sortingOrder = m_MainCanvas.sortingOrder + 1;
		}
		m_textGenerator = new TextGenerator();
		m_Canvas = base.gameObject.GetComponentInChildren<Canvas>();
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
		if (m_ScrollHandler != null)
		{
			m_ScrollHandler.ReleaseScroll();
		}
		m_SelectionCanvas = null;
		m_SelectionMenu = null;
		m_Canvas = null;
		m_AudioManager = null;
		m_ButtonGuide = null;
		m_curSequenceData = null;
		m_StateCheckAnimator = null;
		m_fpClosedFP = null;
		m_fpChangedSelectContent = null;
		if (m_LoadingIcon != null)
		{
			UnityEngine.Object.Destroy(m_LoadingIcon.gameObject);
		}
		m_LoadingIcon = null;
		if (m_dicSnsPostDatas != null)
		{
			m_dicSnsPostDatas.Clear();
		}
		m_OnCursorContent = null;
		if (m_curContentInfos != null)
		{
			m_curContentInfos.Clear();
		}
		m_textGenerator = null;
		if (m_ContainKeywordContentInfos != null)
		{
			m_ContainKeywordContentInfos.Clear();
		}
		if (m_ReplyContentInfos != null)
		{
			m_ReplyContentInfos.Clear();
		}
		ClearContentSlots();
		m_DisapperContentChecker = null;
		m_insertedContentAni = null;
		s_assetBundleHdr = null;
		m_insertedContentAni = null;
		s_activedInstance = null;
	}

	public void OnChangeScrollValue(Vector2 scrollPos)
	{
	}

	private void Update()
	{
		if (m_isDialogBlackDisappear && m_eDialogAniState != GameDefine.eAnimChangeState.none && GameGlobalUtil.CheckPlayEndUIAnimation(m_aniDialogBlackBG, GameDefine.UIAnimationState.disappear, ref m_eDialogAniState))
		{
			m_aniDialogBlackBG.gameObject.SetActive(value: false);
			m_isDialogBlackDisappear = false;
			m_eDialogAniState = GameDefine.eAnimChangeState.none;
		}
		if (m_isMentalGageWaiting)
		{
			if (MentalGageRenewal.s_Instance.IsCompleteChangeMentalGage())
			{
				m_isMentalGageWaiting = false;
				BeginGameOverSequence();
			}
			return;
		}
		if (m_DisapperContentChecker != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_DisapperContentChecker.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				CallClosedCallback();
				m_DisapperContentChecker = null;
			}
			return;
		}
		if (m_StateCheckAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo2 = m_StateCheckAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo2.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo2.normalizedTime >= 0.99f)
			{
				CallClosedCallback();
			}
			return;
		}
		if (m_ScrollHandler.IsScrolling)
		{
			m_ScrollHandler.Update();
			SetActivedContents_byScrollPos(m_ScrollHandler.scrollPos);
			bool flag = true;
			if (m_ScrollHandler.CurScrollEvent == UIUtil_ScrollHandler.ScrollEvent.None && m_onCursorContentIdx >= 0 && m_onCursorContentIdx < m_curContentInfos.Count)
			{
				ChangeOnCursorContent_byScrollPos();
				Vector2 zero = Vector2.zero;
				if (GamePadInput.GetLStickMove(out zero.x, out zero.y) || GamePadInput.GetLStickMove(out zero.x, out zero.y))
				{
					m_ScrollRect.velocity = Vector2.zero;
					flag = false;
				}
			}
			if (flag)
			{
				return;
			}
		}
		if (m_isTutorialActivated || m_isInputBlock || m_TabContainer.isChaningTab || ButtonPadInput.IsPlayingButPressAnim() || !(m_GetKeywordContent == null) || PopupDialoguePlus.IsAnyPopupActivated())
		{
			return;
		}
		float num = 0f;
		if (m_ScrollHandler.isScrollable)
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.TriangleButton, m_ScrollButtonToFirst);
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.TriangleButton))
			{
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetContentActivate(m_buttonGuide_ScrollToTop, isActivate: true);
				}
				OnClick_ScrollToTop();
			}
			else
			{
				bool flag2 = false;
				num = GamePadInput.GetAxisValue(PadInput.GameInput.RStickY);
				if (!GameGlobalUtil.IsAlmostSame(num, 0f))
				{
					if (m_ScrollHandler.scrollPos <= 0f && num < 0f && m_onCursorContentIdx != m_validContentIndexFirst)
					{
						SetOnCursorContentIndex(m_validContentIndexFirst);
						if (m_AudioManager != null)
						{
							m_AudioManager.PlayUISound("Menu_Select");
						}
					}
					else if (m_ScrollHandler.scrollPos >= m_ScrollHandler.scrollPos_Max && num > 0f && m_onCursorContentIdx != m_validContentIndexLast)
					{
						SetOnCursorContentIndex(m_validContentIndexLast);
						if (m_AudioManager != null)
						{
							m_AudioManager.PlayUISound("Menu_Select");
						}
					}
					else
					{
						m_ScrollHandler.ScrollByDirection(num > 0f, Mathf.Abs(num));
						SetActivedContents_byScrollPos(m_ScrollHandler.scrollPos);
						ChangeOnCursorContent_byScrollPos();
					}
				}
				else
				{
					if (!m_ScrollHandler.IsScrolling && m_isPageScrolling)
					{
						m_isPageScrolling = false;
					}
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
		num = GamePadInput.GetAxisValue(PadInput.GameInput.LStickY);
		if (Mathf.Abs(num) > 0.5f)
		{
			if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				ChangeOnCursorContentIndex(isUpSide: true);
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				ChangeOnCursorContentIndex(isUpSide: false);
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
				{
					ChangeOnCursorContentIndex(isUpSide: true);
					m_fScrollButtonPusingTime -= m_ScrollRepeatTime;
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing))
			{
				m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
				{
					ChangeOnCursorContentIndex(isUpSide: false);
					m_fScrollButtonPusingTime -= m_ScrollRepeatTime;
				}
			}
		}
		if (m_OnCursorContent != null)
		{
			Button curActivedButton = m_OnCursorContent.curActivedButton;
			if (curActivedButton != null)
			{
				ButtonPadInput.PushingInputButton(PadInput.GameInput.CircleButton, curActivedButton, null, m_OnCursorContent.m_SelectionIconButton);
				if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
				{
					if (m_ButtonGuide != null)
					{
						m_ButtonGuide.SetContentActivate(m_buttonGuide_ReadPost, isActivate: true);
					}
					m_OnCursorContent.OnProc_ButtonClickEvent();
				}
			}
			if (m_OnCursorContent.isExistImage)
			{
				ButtonPadInput.PressInputButton(PadInput.GameInput.SquareButton, m_OnCursorContent.m_ImageViewButton);
				if (GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton))
				{
					if (m_ButtonGuide != null)
					{
						m_ButtonGuide.SetContentActivate(m_buttonGuide_ShowImage, isActivate: true);
					}
					StartCoroutine(OnProc_ViewImageDetail(m_OnCursorContent));
				}
			}
		}
		if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
		{
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_ExitMenu, isActivate: true);
			}
			CloseSNSMenu();
		}
	}

	private void EnableInputBlock(bool isEnable)
	{
		m_isInputBlock = isEnable;
		if (m_TouchBlockPanel != null)
		{
			m_TouchBlockPanel.SetActive(isEnable);
		}
	}

	public static IEnumerator ShowSNSMenu_FormAssetBundle(Mode mode, GameDefine.EventProc fpClosed = null, GameDefine.EventProc fpChangedSelContent = null)
	{
		if (s_activedInstance == null)
		{
			s_activedInstance = (UnityEngine.Object.Instantiate(MainLoadThing.instance.m_prefabSNSMenu) as GameObject).GetComponent<SNSMenuPlus>();
			yield return null;
		}
		s_activedInstance.InitSNSMenu(mode, fpClosed, fpChangedSelContent);
	}

	public void InitSNSMenu(Mode mode, GameDefine.EventProc fpClosed = null, GameDefine.EventProc fpChangedSelContent = null)
	{
		if (mode != Mode.WatchMenu)
		{
			GameSwitch.GetInstance().SetIsCheckSNSKeywordBut(isSet: true);
		}
		m_AudioManager = GameGlobalUtil.GetAudioManager();
		m_curMode = mode;
		base.gameObject.SetActive(value: true);
		Text[] textComps = new Text[2] { m_TitleText, m_MonologueText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_MonologueRoot.SetActive(value: false);
		m_MonologueText.text = string.Empty;
		m_fpClosedFP = ((fpClosed == null) ? null : new GameDefine.EventProc(fpClosed.Invoke));
		m_fpChangedSelectContent = ((fpChangedSelContent == null) ? null : new GameDefine.EventProc(fpChangedSelContent.Invoke));
		if (m_TitleText != null)
		{
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText((m_curMode != Mode.WatchMenu) ? "TIMELINEMENU_TITLE" : "SNSMENU_TITLE");
		}
		m_NextPageButton.nextPageText = GameGlobalUtil.GetXlsProgramText("SNSMENU_LOAD_NEXT_PAGE");
		m_NextPageButton.loadingText = GameGlobalUtil.GetXlsProgramText("SNSMENU_LOADING_NEXT_PAGE");
		m_isRunEvent = false;
		m_isDialogBlackDisappear = false;
		m_isInitailizedSlots = false;
		m_isMentalGageWaiting = false;
		if (m_CharMotionAnimator != null)
		{
			m_CharMotionAnimator.gameObject.SetActive(m_curMode != Mode.WatchMenu);
			RectTransform component = m_CharMotionAnimator.gameObject.GetComponent<RectTransform>();
			if (component != null)
			{
				component.anchoredPosition = Vector2.zero;
			}
		}
		if (m_curMode != Mode.WatchMenu)
		{
			SetCharMotion();
		}
		if (m_ScreenShotBG != null)
		{
			m_ScreenShotBG.SetActive(m_curMode != Mode.WatchMenu);
		}
		if (m_aniDialogBlackBG != null)
		{
			m_aniDialogBlackBG.gameObject.SetActive(value: false);
		}
		Xls.TextData textData = null;
		string postTalkWindowString = GameSwitch.GetInstance().GetPostTalkWindowString();
		if (!string.IsNullOrEmpty(postTalkWindowString))
		{
			try
			{
				textData = Xls.TextData.GetData_byKey(postTalkWindowString);
			}
			catch (Exception)
			{
			}
		}
		m_MonologueIdleText = ((textData == null) ? "Notice Text!" : textData.m_strTxt);
		InitMonologueText();
		SetMonologueState(MonologueState.None);
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
		m_ScrollHandler.ResetScrollRange();
		BuildPostDataList_bySequence();
		if (m_TabContainer != null)
		{
			m_TabContainer.BuildTabButtonObjects();
			m_TabContainer.isInputBlock = false;
			Invoke("SetSNSTab_CurSequnce", 0.1f);
		}
		m_DisapperContentChecker = null;
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.appear.ToString());
		InitButtonGuide();
		EnableInputBlock(isEnable: true);
	}

	public void CloseSNSMenu(bool isEnableAnimation = true)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Cancel");
		}
		EventCameraEffect.Instance.Deactivate_SNSMenuBG();
		EnableInputBlock(isEnable: true);
		m_TabContainer.isInputBlock = true;
		if (!isEnableAnimation)
		{
			CallClosedCallback();
		}
		else
		{
			DisappearMenu();
		}
	}

	private void CallClosedCallback()
	{
		m_StateCheckAnimator = null;
		m_dicSnsPostDatas.Clear();
		m_TabContainer.ClearTabButtonObjects();
		ClearContentSlots();
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
		UnityEngine.Object.Destroy(base.gameObject);
		if (m_curMode != Mode.WatchMenu)
		{
			Resources.UnloadUnusedAssets();
		}
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
		m_buttonGuide_SelContent = GameGlobalUtil.GetXlsProgramText("SNSMENU_BTNGUIDE_SEL_CONTENTS");
		m_buttonGuide_ReadPost = GameGlobalUtil.GetXlsProgramText("SNSMENU_BTNGUIDE_READ_POST");
		m_buttonGuide_LoadNextPage = GameGlobalUtil.GetXlsProgramText("SNSMENU_BTNGUIDE_LOAD_NEXT_PAGE");
		m_buttonGuide_ExitMenu = GameGlobalUtil.GetXlsProgramText("SNSMENU_BTNGUIDE_EXIT_MENU");
		m_buttonGuide_ShowImage = GameGlobalUtil.GetXlsProgramText("SNSMENU_BTNGUIDE_SHOW_IMAGE");
		m_buttonGuide_ScrollToTop = GameGlobalUtil.GetXlsProgramText("SNSMENU_BTNGUIDE_SCROLL_TO_TOP");
		m_buttonGuide_ChangeTab = GameGlobalUtil.GetXlsProgramText("SNSMENU_BTNGUIDE_CHANGE_TAB");
		m_buttonGuide_SelectTab = GameGlobalUtil.GetXlsProgramText("SNSMENU_BTNGUIDE_SEL_TAB");
		m_buttonGuide_CancelTab = GameGlobalUtil.GetXlsProgramText("SNSMENU_BTNGUIDE_CANCEL_TAB");
		m_isInitializedButtonGuidText = true;
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuide_SelContent, PadInput.GameInput.LStickY);
		m_ButtonGuide.AddContent(m_buttonGuide_ReadPost, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_LoadNextPage, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ExitMenu, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ShowImage, PadInput.GameInput.SquareButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ScrollToTop, PadInput.GameInput.TriangleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_SelectTab, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_CancelTab, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.L1Button);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.R1Button);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Left);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ReadPost, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_LoadNextPage, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ShowImage, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_SelectTab, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_CancelTab, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ChangeTab, m_TabContainer.tabButtonInfos.Count > 1);
		m_ButtonGuide.SetShow(isShow: true);
	}

	private void BuildPostDataList_bySequence()
	{
		m_dicSnsPostDatas.Clear();
		List<Xls.SNSPostData> datas = Xls.SNSPostData.datas;
		int count = datas.Count;
		Xls.SNSPostData sNSPostData = null;
		List<Xls.SNSPostData> list = null;
		for (int i = 0; i < count; i++)
		{
			sNSPostData = datas[i];
			if (!string.IsNullOrEmpty(sNSPostData.m_strIDSeq))
			{
				if (m_dicSnsPostDatas.ContainsKey(sNSPostData.m_strIDSeq))
				{
					list = m_dicSnsPostDatas[sNSPostData.m_strIDSeq];
				}
				else
				{
					list = new List<Xls.SNSPostData>();
					m_dicSnsPostDatas.Add(sNSPostData.m_strIDSeq, list);
				}
				list.Add(sNSPostData);
			}
		}
	}

	private void SetSNSTab_CurSequnce()
	{
		bool flag = false;
		try
		{
			Xls.SequenceData data_bySwitchIdx = Xls.SequenceData.GetData_bySwitchIdx(GameSwitch.GetInstance().GetCurSequence());
			if (data_bySwitchIdx != null)
			{
				Xls.TextData data_byKey = Xls.TextData.GetData_byKey(data_bySwitchIdx.m_strIDName);
				if (data_byKey != null)
				{
					m_TabContainer.SetSelectedTab(data_byKey.m_strTxt);
					flag = true;
				}
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			if (!flag)
			{
				int count = m_TabContainer.tabButtonInfos.Count;
				if (count > 0)
				{
					m_TabContainer.SetSelectedTab_byIdx(0);
				}
			}
		}
	}

	private List<CommonTabContainerPlus.TabCreateInfo> SetTabCreateInfos()
	{
		List<CommonTabContainerPlus.TabCreateInfo> list = new List<CommonTabContainerPlus.TabCreateInfo>();
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		if (gameSwitch != null)
		{
			if (m_curMode == Mode.WatchMenu)
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
			else
			{
				Xls.SequenceData data_bySwitchIdx = Xls.SequenceData.GetData_bySwitchIdx(GameSwitch.GetInstance().GetCurSequence());
				Xls.TextData data_byKey = Xls.TextData.GetData_byKey(data_bySwitchIdx.m_strIDName);
				CommonTabContainerPlus.TabCreateInfo tabCreateInfo2 = new CommonTabContainerPlus.TabCreateInfo();
				list.Add(tabCreateInfo2);
				tabCreateInfo2.m_Text = ((data_byKey == null) ? string.Empty : data_byKey.m_strTxt);
				tabCreateInfo2.m_Tag = data_bySwitchIdx;
			}
		}
		return list;
	}

	private void OnChangedSelectTab(object sender, object arg)
	{
		m_ScrollRect.velocity = Vector2.zero;
		if (sender is CommonTabContainerPlus.TabButtonInfo { tag: not null, tag: Xls.SequenceData sequenceData })
		{
			m_curSequenceData = sequenceData;
			GameSwitch gameSwitch = GameSwitch.GetInstance();
			m_icurPhaseLevel = ((gameSwitch.GetCurSequence() != sequenceData.m_iIdx) ? 100 : gameSwitch.GetCurPhase());
			if (!m_isInitailizedSlots)
			{
				StartCoroutine(CreateContentSlots(sequenceData.m_strKey, m_icurPhaseLevel));
			}
			else
			{
				SetCurrentSequencePhase(sequenceData.m_strKey, m_icurPhaseLevel);
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
			m_ButtonGuide.SetContentEnable(m_buttonGuide_SelectTab, flag);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_CancelTab, flag);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_SelContent, !flag);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ReadPost, !flag && m_OnCursorContent != null && m_OnCursorContent.curActivedButtonGroup == m_OnCursorContent.m_KeywordButton);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ExitMenu, !flag);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, !flag);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_LoadNextPage, !flag && m_NextPageButton.curState == NextContentsLoadButtton.State.Selected);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ShowImage, !flag && m_OnCursorContent != null && m_OnCursorContent.isExistImage);
		}
	}

	private void OnProc_PressedTabButtons(object sender, object arg)
	{
		switch ((PadInput.GameInput)arg)
		{
		case PadInput.GameInput.CircleButton:
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_SelectTab, isActivate: true);
			}
			break;
		case PadInput.GameInput.CrossButton:
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_CancelTab, isActivate: true);
			}
			break;
		case PadInput.GameInput.L1Button:
		case PadInput.GameInput.R1Button:
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_ChangeTab, isActivate: true);
			}
			break;
		case PadInput.GameInput.TriangleButton:
		case PadInput.GameInput.SquareButton:
			break;
		}
	}

	private IEnumerator CreateContentSlots(string firstSequenceKey, int firstPhase)
	{
		EnableInputBlock(isEnable: true);
		m_TabContainer.isInputBlock = true;
		if (m_LoadingIcon == null)
		{
			m_LoadingIcon = LoadingSWatchIcon.Create(m_ScrollRect.gameObject);
		}
		m_LoadingIcon.gameObject.SetActive(value: true);
		ClearContentSlots();
		SNSContentPlus contentSlot = null;
		for (int i = 0; i < 7; i++)
		{
			contentSlot = SNSContentPlus.CreateSNSContentSlot(SNSContentPlus.SNSContentType.Normal);
			m_NormalContentSlots.Add(contentSlot);
			contentSlot.transform.SetParent(m_ContentContanierRT, worldPositionStays: false);
			contentSlot.foRectTransform.anchoredPosition = new Vector2(contentSlot.foRectTransform.anchoredPosition.x, 10000f);
			contentSlot.gameObject.SetActive(value: false);
			contentSlot.linkedSnsMenu = this;
			yield return null;
		}
		for (int j = 0; j < 7; j++)
		{
			contentSlot = SNSContentPlus.CreateSNSContentSlot(SNSContentPlus.SNSContentType.Reply);
			m_ReplyContentSlots.Add(contentSlot);
			contentSlot.transform.SetParent(m_ContentContanierRT, worldPositionStays: false);
			contentSlot.foRectTransform.anchoredPosition = new Vector2(contentSlot.foRectTransform.anchoredPosition.x, 10000f);
			contentSlot.gameObject.SetActive(value: false);
			contentSlot.linkedSnsMenu = this;
			yield return null;
		}
		for (int k = 0; k < 7; k++)
		{
			contentSlot = SNSContentPlus.CreateSNSContentSlot(SNSContentPlus.SNSContentType.Shared);
			m_SharedContentSlots.Add(contentSlot);
			contentSlot.transform.SetParent(m_ContentContanierRT, worldPositionStays: false);
			contentSlot.foRectTransform.anchoredPosition = new Vector2(contentSlot.foRectTransform.anchoredPosition.x, 10000f);
			contentSlot.gameObject.SetActive(value: false);
			contentSlot.linkedSnsMenu = this;
			yield return null;
		}
		SetContentSlotLayoutInfo(m_NormalContentSlots[0], ref m_NormalContentSlotLayoutInfo);
		SetContentSlotLayoutInfo(m_ReplyContentSlots[0], ref m_ReplyContentSlotLayoutInfo);
		SetContentSlotLayoutInfo(m_SharedContentSlots[0], ref m_SharedContentSlotLayoutInfo);
		SetCurrentSequencePhase(firstSequenceKey, firstPhase);
		m_isInitailizedSlots = true;
		EnableInputBlock(isEnable: false);
		m_TabContainer.isInputBlock = false;
		m_LoadingIcon.gameObject.SetActive(value: false);
		m_isTutorialActivated = true;
		if (m_TouchBlockPanel != null)
		{
			m_TouchBlockPanel.SetActive(value: true);
		}
		Invoke("ShowTutorialPopup", 0.2f);
	}

	private void ClearContentSlots()
	{
		SNSContentPlus sNSContentPlus = null;
		List<SNSContentPlus>.Enumerator enumerator = m_NormalContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			sNSContentPlus = enumerator.Current;
			if (!(sNSContentPlus == null))
			{
				UnityEngine.Object.Destroy(sNSContentPlus.gameObject);
			}
		}
		enumerator = m_ReplyContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			sNSContentPlus = enumerator.Current;
			if (!(sNSContentPlus == null))
			{
				UnityEngine.Object.Destroy(sNSContentPlus.gameObject);
			}
		}
		enumerator = m_SharedContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			sNSContentPlus = enumerator.Current;
			if (!(sNSContentPlus == null))
			{
				UnityEngine.Object.Destroy(sNSContentPlus.gameObject);
			}
		}
		m_NormalContentSlots.Clear();
		m_ReplyContentSlots.Clear();
		m_SharedContentSlots.Clear();
		m_activedContentSlots.Clear();
	}

	private void SetContentSlotLayoutInfo(SNSContentPlus contentSlot, ref ContentSlotLayoutInfo layoutInfo)
	{
		RectTransform foRectTransform = contentSlot.foRectTransform;
		RectTransform component = contentSlot.m_ImageRoot.m_ContentText.gameObject.GetComponent<RectTransform>();
		RectTransform component2 = contentSlot.m_ContentImageRoot.GetComponent<RectTransform>();
		layoutInfo.heightWithoutText = foRectTransform.rect.height - component.rect.height;
		layoutInfo.heightThumbnailImage = component2.rect.height;
		layoutInfo.scaleY = foRectTransform.localScale.y;
		layoutInfo.textGenSetting_WithImage = contentSlot.m_ImageRoot.m_ContentText.GetGenerationSettings(component.rect.size);
		layoutInfo.textGenSetting_WithImage.scaleFactor = 1f;
		component = contentSlot.m_NoImageRoot.m_ContentText.gameObject.GetComponent<RectTransform>();
		layoutInfo.textGenSetting_WithoutImage = contentSlot.m_NoImageRoot.m_ContentText.GetGenerationSettings(component.rect.size);
		layoutInfo.textGenSetting_WithoutImage.scaleFactor = 1f;
	}

	private void ClearActivedContentSlots()
	{
		SNSContentPlus sNSContentPlus = null;
		List<SNSContentPlus>.Enumerator enumerator = m_activedContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			sNSContentPlus = enumerator.Current;
			if (!(sNSContentPlus == null))
			{
				sNSContentPlus.gameObject.SetActive(value: false);
				sNSContentPlus.SetContentData(null);
			}
		}
		m_activedContentSlots.Clear();
		m_curBaseContentIdx = -1;
	}

	private void SetCurrentSequencePhase(string sequenceKey, int phase)
	{
		m_curContentInfos.Clear();
		m_validContentIndexFirst = -1;
		m_validContentIndexLast = -1;
		m_ContainKeywordContentInfos.Clear();
		m_ReplyContentInfos.Clear();
		ClearActivedContentSlots();
		if (!m_dicSnsPostDatas.ContainsKey(sequenceKey))
		{
			return;
		}
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		sbyte b = 0;
		Xls.SNSPostData sNSPostData = null;
		List<Xls.SNSPostData> list = m_dicSnsPostDatas[sequenceKey];
		float num = 0f;
		float scale = 1f;
		List<Xls.SNSPostData>.Enumerator enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			sNSPostData = enumerator.Current;
			if (sNSPostData == null || sNSPostData.m_iPhase > phase || (m_curMode != Mode.WatchMenu && sNSPostData.m_iPhase != phase))
			{
				continue;
			}
			b = gameSwitch.GetPostState(sNSPostData.m_iIdx);
			ContentInfo contentInfo = new ContentInfo();
			contentInfo.xlsData = sNSPostData;
			contentInfo.isValid = b == 1 || b == 2;
			contentInfo.isNew = b == 1;
			contentInfo.pos = num;
			contentInfo.height = CalculateContentHeight(sNSPostData, out scale);
			contentInfo.scaledHeight = contentInfo.height * scale;
			contentInfo.isHide = false;
			if (contentInfo.isValid)
			{
				int count = m_curContentInfos.Count;
				if (m_validContentIndexFirst < 0)
				{
					m_validContentIndexFirst = count;
				}
				if (m_validContentIndexLast < count)
				{
					m_validContentIndexLast = count;
				}
				if (m_curMode != Mode.WatchMenu)
				{
					AddToList_IfReplyOrGetKeywordContentInfo(contentInfo);
				}
			}
			m_curContentInfos.Add(contentInfo);
			num += ((!contentInfo.isValid) ? 0f : contentInfo.scaledHeight);
		}
		num += m_BottomSpacing;
		float size = Mathf.Max(num, m_ScrollRect.viewport.rect.height);
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
		m_ScrollHandler.ResetScrollRange();
		m_ScrollHandler.scrollPos = 0f;
		if (m_validContentIndexFirst >= 0)
		{
			if (m_curMode != Mode.WatchMenu && m_MonologueRoot != null)
			{
				m_MonologueRoot.SetActive(value: true);
			}
			SetCurrentBaseContentIndex(m_validContentIndexFirst, isIgnoreAni: false);
			SetOnCursorContentIndex(m_validContentIndexFirst);
		}
	}

	private void AddToList_IfReplyOrGetKeywordContentInfo(ContentInfo contentInfo)
	{
		if (contentInfo == null || contentInfo.xlsData == null)
		{
			return;
		}
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		Xls.SNSPostData xlsData = contentInfo.xlsData;
		if (!string.IsNullOrEmpty(xlsData.m_strIDReply) && xlsData.m_iIsSelPost == 0 && gameSwitch.GetPostReply(xlsData.m_iIdx) == 0)
		{
			if (xlsData.m_iReplyGroup < 0 || gameSwitch.GetReplyGroup(xlsData.m_iReplyGroup) == 0)
			{
				AddReplyContentInfo(contentInfo);
			}
			return;
		}
		Xls.SequenceData data_byKey = Xls.SequenceData.GetData_byKey(xlsData.m_strIDSeq);
		if (data_byKey != null && data_byKey.m_iIdx == gameSwitch.GetCurSequence() && xlsData.m_iPhase == gameSwitch.GetCurPhase() && !string.IsNullOrEmpty(xlsData.m_strIDKeyword) && gameSwitch.GetKeywordAllState(xlsData.m_strIDKeyword) == 0 && (xlsData.m_iGroupKeyword < 0 || gameSwitch.GetKeywordGroup(xlsData.m_iGroupKeyword) == 0))
		{
			AddContainKeywordContentInfo(contentInfo);
		}
	}

	private ContentInfo GetContentInfo_byPostData(Xls.SNSPostData postData)
	{
		if (postData == null)
		{
			return null;
		}
		ContentInfo contentInfo = null;
		List<ContentInfo>.Enumerator enumerator = m_curContentInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			contentInfo = enumerator.Current;
			if (contentInfo.xlsData == postData)
			{
				return contentInfo;
			}
		}
		return null;
	}

	private int GetContentIndex_byPostData(Xls.SNSPostData postData)
	{
		if (postData == null)
		{
			return -1;
		}
		ContentInfo contentInfo = null;
		int count = m_curContentInfos.Count;
		for (int i = 0; i < count; i++)
		{
			contentInfo = m_curContentInfos[i];
			if (contentInfo.xlsData == postData)
			{
				return i;
			}
		}
		return -1;
	}

	private float CalculateContentHeight(Xls.SNSPostData xlsData, out float scale)
	{
		scale = 1f;
		ContentSlotLayoutInfo contentSlotLayoutInfo;
		switch (xlsData.m_iPostType)
		{
		case 0:
			contentSlotLayoutInfo = m_NormalContentSlotLayoutInfo;
			break;
		case 1:
			contentSlotLayoutInfo = m_ReplyContentSlotLayoutInfo;
			break;
		case 2:
			contentSlotLayoutInfo = m_SharedContentSlotLayoutInfo;
			break;
		default:
			return 0f;
		}
		scale = contentSlotLayoutInfo.scaleY;
		Xls.TextData data_byKey = Xls.TextData.GetData_byKey(xlsData.m_strIDText);
		SNSContentPlus.ParseContentText((data_byKey == null) ? string.Empty : data_byKey.m_strTxt, xlsData.m_iPostType, out var outText, out var _);
		outText = TagText.TransTagTextToUnityText(outText, isIgnoreHideTag: false);
		bool flag = !string.IsNullOrEmpty(xlsData.m_strIDImg);
		float preferredHeight = m_textGenerator.GetPreferredHeight(outText, (!flag) ? contentSlotLayoutInfo.textGenSetting_WithoutImage : contentSlotLayoutInfo.textGenSetting_WithImage);
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

	private SNSContentPlus GetEmptyContentSlot(int contentType)
	{
		List<SNSContentPlus> list = null;
		SNSContentPlus.SNSContentType sNSContentType = SNSContentPlus.SNSContentType.Normal;
		switch (contentType)
		{
		case 0:
			list = m_NormalContentSlots;
			sNSContentType = SNSContentPlus.SNSContentType.Normal;
			break;
		case 1:
			list = m_ReplyContentSlots;
			sNSContentType = SNSContentPlus.SNSContentType.Reply;
			break;
		case 2:
			list = m_SharedContentSlots;
			sNSContentType = SNSContentPlus.SNSContentType.Shared;
			break;
		default:
			return null;
		}
		SNSContentPlus sNSContentPlus = null;
		List<SNSContentPlus>.Enumerator enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			sNSContentPlus = enumerator.Current;
			if (sNSContentPlus.xlsData == null)
			{
				return sNSContentPlus;
			}
		}
		sNSContentPlus = SNSContentPlus.CreateSNSContentSlot(sNSContentType);
		sNSContentPlus.transform.SetParent(m_ContentContanierRT, worldPositionStays: false);
		sNSContentPlus.gameObject.SetActive(value: false);
		sNSContentPlus.linkedSnsMenu = this;
		list.Add(sNSContentPlus);
		return sNSContentPlus;
	}

	private void ReleaseActivedContentSlot(SNSContentPlus activedSlot)
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
		if (m_curBaseContentIdx != idx)
		{
			int count = m_curContentInfos.Count;
			if (idx < 0 || idx >= count)
			{
				ClearActivedContentSlots();
				SetOnCursorContentIndex(-1);
				return;
			}
			float num = m_ScrollRect.viewport.rect.height * 1.5f;
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
						SNSContentPlus sNSContentPlus = m_activedContentSlots[num3];
						if (sNSContentPlus.indexInContentInfos >= idx)
						{
							num2 += m_curContentInfos[sNSContentPlus.indexInContentInfos].scaledHeight;
						}
						else
						{
							ReleaseActivedContentSlot(sNSContentPlus);
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
							SNSContentPlus sNSContentPlus2 = m_activedContentSlots[num5];
							if (sNSContentPlus2.indexInContentInfos > num4)
							{
								ReleaseActivedContentSlot(sNSContentPlus2);
							}
						}
						int num6 = indexInContentInfos;
						while (num6 > idx)
						{
							num6--;
							ContentInfo contentInfo2 = m_curContentInfos[num6];
							if (contentInfo2.isValid)
							{
								SNSContentPlus emptyContentSlot = GetEmptyContentSlot(contentInfo2.xlsData.m_iPostType);
								emptyContentSlot.SetContentData(contentInfo2.xlsData, 0f - contentInfo2.pos, contentInfo2.height, m_curMode);
								emptyContentSlot.indexInContentInfos = num6;
								emptyContentSlot.gameObject.SetActive(!contentInfo2.isHide);
								m_activedContentSlots.Insert(0, emptyContentSlot);
								if (isIgnoreAni)
								{
									GameGlobalUtil.PlayUIAnimation_WithChidren(emptyContentSlot.gameObject, strMot);
								}
							}
						}
						goto IL_039e;
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
					SNSContentPlus emptyContentSlot2 = GetEmptyContentSlot(contentInfo3.xlsData.m_iPostType);
					emptyContentSlot2.SetContentData(contentInfo3.xlsData, 0f - contentInfo3.pos, contentInfo3.height, m_curMode);
					emptyContentSlot2.indexInContentInfos = j;
					emptyContentSlot2.gameObject.SetActive(!contentInfo3.isHide);
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
		goto IL_039e;
		IL_039e:
		if (m_onCursorContentIdx >= 0)
		{
			SNSContentPlus contentSlot_byContentInfoIndex = GetContentSlot_byContentInfoIndex(m_onCursorContentIdx);
			if (contentSlot_byContentInfoIndex != null && contentSlot_byContentInfoIndex != m_OnCursorContent)
			{
				SetOnCursorContentIndex(m_onCursorContentIdx, isAdjustScrollPos: false);
			}
		}
		ResetAllContentState();
	}

	private void DisappearMenu()
	{
		m_StateCheckAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.disappear.ToString());
		if (m_StateCheckAnimator == null)
		{
			CallClosedCallback();
		}
	}

	private void AddContainKeywordContentInfo(ContentInfo contentInfo)
	{
		if (contentInfo != null && contentInfo.xlsData != null && !m_ContainKeywordContentInfos.Contains(contentInfo))
		{
			m_ContainKeywordContentInfos.Add(contentInfo);
		}
	}

	private void RemoveContainKeywordContentInfo(Xls.SNSPostData xlsPostData)
	{
		if (xlsPostData == null)
		{
			return;
		}
		int count = m_ContainKeywordContentInfos.Count;
		if (count <= 0)
		{
			return;
		}
		ContentInfo contentInfo = null;
		int num = count;
		while (num > 0)
		{
			num--;
			contentInfo = m_ContainKeywordContentInfos[num];
			if (contentInfo == null || contentInfo.xlsData == null)
			{
				m_ContainKeywordContentInfos.RemoveAt(num);
			}
			else if (string.Equals(contentInfo.xlsData.m_strIDKeyword, xlsPostData.m_strIDKeyword))
			{
				m_ContainKeywordContentInfos.RemoveAt(num);
			}
			else if (xlsPostData.m_iGroupKeyword >= 0 && contentInfo.xlsData.m_iGroupKeyword == xlsPostData.m_iGroupKeyword)
			{
				m_ContainKeywordContentInfos.RemoveAt(num);
			}
		}
	}

	private void AddReplyContentInfo(ContentInfo contentInfo)
	{
		if (contentInfo != null && contentInfo.xlsData != null && !m_ReplyContentInfos.Contains(contentInfo))
		{
			m_ReplyContentInfos.Add(contentInfo);
		}
	}

	private void RemoveReplyContentInfo(Xls.SNSPostData postData)
	{
		if (postData == null)
		{
			return;
		}
		int count = m_ReplyContentInfos.Count;
		if (count <= 0)
		{
			return;
		}
		ContentInfo contentInfo = null;
		int num = count;
		while (num > 0)
		{
			num--;
			contentInfo = m_ReplyContentInfos[num];
			if (contentInfo == null || contentInfo.xlsData == null)
			{
				m_ReplyContentInfos.RemoveAt(num);
			}
			else if (contentInfo.xlsData == postData)
			{
				m_ReplyContentInfos.RemoveAt(num);
			}
			else if (contentInfo.xlsData.m_iReplyGroup == postData.m_iReplyGroup)
			{
				m_ReplyContentInfos.RemoveAt(num);
			}
		}
	}

	private SNSContentPlus GetContentSlot_byContentInfoIndex(int idx)
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
		List<SNSContentPlus>.Enumerator enumerator = m_activedContentSlots.GetEnumerator();
		SNSContentPlus sNSContentPlus = null;
		while (enumerator.MoveNext())
		{
			sNSContentPlus = enumerator.Current;
			if (sNSContentPlus.indexInContentInfos == idx)
			{
				return sNSContentPlus;
			}
		}
		return null;
	}

	private void SetOnCursorContentIndex(int idx, bool isAdjustScrollPos = true, bool isSendNotice_ContentChanged = true)
	{
		int onCursorContentIdx = m_onCursorContentIdx;
		m_onCursorContentIdx = ((idx >= 0 && idx < m_curContentInfos.Count && m_curContentInfos[idx].isValid) ? idx : (-1));
		SNSContentPlus contentSlot_byContentInfoIndex = GetContentSlot_byContentInfoIndex(idx);
		SetOnCursorContentSlot(contentSlot_byContentInfoIndex);
		if (isAdjustScrollPos && m_onCursorContentIdx >= 0)
		{
			AdjustScrollPos_byOnCursorContent();
		}
		if (isSendNotice_ContentChanged && m_fpChangedSelectContent != null)
		{
			m_fpChangedSelectContent(this, m_onCursorContentIdx);
		}
	}

	public void TouchContent(SNSContentPlus contentSlot)
	{
		if (m_OnCursorContent != contentSlot && contentSlot != null && m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		int contentIndex_byPostData = GetContentIndex_byPostData(contentSlot.xlsData);
		SetOnCursorContentIndex(contentIndex_byPostData);
	}

	private void SetOnCursorContentSlot(SNSContentPlus contentSlot)
	{
		if (!(m_OnCursorContent == contentSlot))
		{
			if (m_OnCursorContent != null)
			{
				m_OnCursorContent.SetState(_isOnCursor: false, m_curContentState == ContentState.Enable, string.Empty);
			}
			if (contentSlot != null)
			{
				contentSlot.SetState(_isOnCursor: true, m_curContentState == ContentState.Enable, string.Empty);
			}
			m_OnCursorContent = contentSlot;
			if (m_curMode != Mode.WatchMenu)
			{
				SetMonologueState(MonologueState.Idle);
			}
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentEnable(m_buttonGuide_ReadPost, m_OnCursorContent != null && m_OnCursorContent.curActivedButtonGroup == m_OnCursorContent.m_KeywordButton);
				m_ButtonGuide.SetContentEnable(m_buttonGuide_ShowImage, m_OnCursorContent != null && m_OnCursorContent.isExistImage);
				m_ButtonGuide.SetContentEnable(m_buttonGuide_LoadNextPage, m_NextPageButton.curState == NextContentsLoadButtton.State.Selected);
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
				if (!m_ScrollLoopEnable)
				{
					return false;
				}
				num = count - 1;
			}
			else if (num >= count)
			{
				if (!m_ScrollLoopEnable)
				{
					return false;
				}
				num = 0;
			}
			if (m_curContentInfos[num].isValid)
			{
				SetOnCursorContentIndex(num);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Select");
				}
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetContentActivate(m_buttonGuide_SelContent, isActivate: true);
				}
				return true;
			}
		}
		while (num != m_onCursorContentIdx);
		return false;
	}

	private bool ChangeScrollPage(bool isUpSide)
	{
		if (!m_ScrollHandler.isScrollable || m_ScrollHandler.IsScrolling)
		{
			return false;
		}
		if (m_onCursorContentIdx < 0 || m_onCursorContentIdx >= m_curContentInfos.Count)
		{
			return false;
		}
		if (isUpSide && m_ScrollHandler.scrollPos <= 0f)
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
		if (!isUpSide && m_ScrollHandler.scrollPos >= m_ScrollHandler.scrollPos_Max)
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
		float num = m_ScrollRect.viewport.rect.height - m_BottomSpacing;
		float value = m_ScrollHandler.scrollPos + ((!isUpSide) ? num : (0f - num));
		value = Mathf.Clamp(value, 0f, m_ScrollHandler.scrollPos_Max);
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
		if (num2 == m_onCursorContentIdx && !isUpSide)
		{
			if (num2 < m_validContentIndexLast && GameGlobalUtil.IsAlmostSame(value, m_ScrollHandler.scrollPos_Max))
			{
				SetOnCursorContentIndex(m_validContentIndexLast);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Select");
				}
				return false;
			}
			num2 = Mathf.Min(m_onCursorContentIdx + 1, m_validContentIndexLast);
		}
		if (m_onCursorContentIdx != num2 && m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		SetOnCursorContentIndex(num2, isAdjustScrollPos: false);
		m_ScrollHandler.ScrollToTargetPos(0f - m_curContentInfos[num2].pos);
		return true;
	}

	private void ChangeOnCursorContent_byScrollPos()
	{
		if (m_onCursorContentIdx < 0 || m_onCursorContentIdx >= m_curContentInfos.Count)
		{
			return;
		}
		float height = m_ScrollRect.viewport.rect.height;
		float num = 0f - m_ContentContanierRT.offsetMax.y;
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
				if (!contentInfo.isValid)
				{
					continue;
				}
				num5 = 0f - (contentInfo.pos + contentInfo.scaledHeight);
				num5 -= m_BottomSpacing;
				if (Mathf.CeilToInt(num5) < num3)
				{
					continue;
				}
				SetOnCursorContentIndex(num6, isAdjustScrollPos: false);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Select");
				}
				break;
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
				if (!contentInfo.isValid)
				{
					continue;
				}
				num4 = 0f - contentInfo.pos;
				if (Mathf.FloorToInt(num4) <= num2)
				{
					SetOnCursorContentIndex(i, isAdjustScrollPos: false);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
					break;
				}
			}
		}
	}

	private void AdjustScrollPos_byOnCursorContent()
	{
		if (m_onCursorContentIdx >= 0 && m_onCursorContentIdx < m_curContentInfos.Count)
		{
			float height = m_ScrollRect.viewport.rect.height;
			float num = 0f - m_ContentContanierRT.offsetMax.y;
			float num2 = num - height;
			ContentInfo contentInfo = m_curContentInfos[m_onCursorContentIdx];
			float num3 = 0f - contentInfo.pos;
			float num4 = num3 - contentInfo.scaledHeight;
			num4 -= m_BottomSpacing;
			if (num4 < num2)
			{
				float fTargetPos = num4 + height;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos);
			}
			else if (num3 > num)
			{
				float fTargetPos2 = num3;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos2);
			}
		}
	}

	public Xls.SNSPostData GetPostDataNearBy(int baseContentIndex, int offset)
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
		Xls.SNSPostData result = null;
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
						result = contentInfo.xlsData;
						break;
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
						result = contentInfo2.xlsData;
						break;
					}
				}
			}
		}
		else
		{
			ContentInfo contentInfo3 = m_curContentInfos[baseContentIndex];
			if (contentInfo3.isValid)
			{
				result = contentInfo3.xlsData;
			}
		}
		return result;
	}

	public void OnClick_ScrollToTop()
	{
		if (m_ScrollHandler.isScrollable)
		{
			m_ScrollHandler.ScrollToTargetPos(0f);
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

	private void ResetAllContentState()
	{
		ContentState contentState = ContentState.Enable;
		if (m_curMode != Mode.WatchMenu)
		{
			if (m_ContainKeywordContentInfos.Count <= 0)
			{
				contentState = ContentState.NomoreKeyword;
			}
			else if (GameSwitch.GetInstance().GetMentalLowHigh() == ConstGameSwitch.eMental.LOW)
			{
				contentState = ContentState.LowMental;
			}
		}
		m_curContentState = contentState;
		bool isEnabled = true;
		string buttonStateText = string.Empty;
		switch (contentState)
		{
		case ContentState.NomoreKeyword:
			isEnabled = false;
			buttonStateText = GameGlobalUtil.GetXlsProgramText("SNSMENU_STATE_NOMORE_KEYWORD");
			break;
		case ContentState.LowMental:
			isEnabled = false;
			buttonStateText = GameGlobalUtil.GetXlsProgramText("SNSMENU_STATE_LOW_MENTAL");
			break;
		}
		SNSContentPlus sNSContentPlus = null;
		List<SNSContentPlus>.Enumerator enumerator = m_activedContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			sNSContentPlus = enumerator.Current;
			sNSContentPlus.CheckValid_KeywordButton();
			sNSContentPlus.CheckValid_ReplyButton();
			sNSContentPlus.SetState(m_OnCursorContent == sNSContentPlus, isEnabled, buttonStateText);
		}
	}

	private void InitMonologueText()
	{
		if (!m_isInitializedMonoTexts)
		{
			SetMonologueTextList(ref m_MontTexts_LowMental, "SNS_MONOTEXT_LOW_MENTAL", "SNS_MONOTEXT_LOW_MENTAL_RC");
			SetMonologueTextList(ref m_MontTexts_NomoreKeyword, "SNS_MONOTEXT_NOMORE_KEYWORD", "SNS_MONOTEXT_NOMORE_KEYWORD_RC");
			SetMonologueTextList(ref m_MontTexts_OldPost, "SNS_MONOTEXT_OLD_POST", "SNS_MONOTEXT_OLD_POST_RC");
			SetMonologueTextList(ref m_MontTexts_NoKeyword, "SNS_MONOTEXT_NO_KEYWORD", "SNS_MONOTEXT_NO_KEYWORD_RC");
			SetMonologueTextList(ref m_MontTexts_GetKeyword, "SNS_MONOTEXT_GET_KEYWORD", "SNS_MONOTEXT_GET_KEYWORD_RC");
			SetMonologueTextList(ref m_MontTexts_Reply, "SNS_MONOTEXT_REPLY", "SNS_MONOTEXT_REPLY_RC");
			m_isInitializedMonoTexts = true;
		}
	}

	private void SetMonologueTextList(ref List<string> textList, string xlsBaseDataName, string xlsRandomCountDataName)
	{
		textList.Clear();
		try
		{
			Xls.ProgramText data_byKey = Xls.ProgramText.GetData_byKey(xlsBaseDataName);
			if (data_byKey != null)
			{
				textList.Add(data_byKey.m_strTxt);
			}
			Xls.ProgramDefineStr data_byKey2 = Xls.ProgramDefineStr.GetData_byKey(xlsRandomCountDataName);
			if (data_byKey2 == null)
			{
				return;
			}
			int num = int.Parse(data_byKey2.m_strTxt);
			if (num <= 0)
			{
				return;
			}
			for (int i = 1; i <= num; i++)
			{
				data_byKey = Xls.ProgramText.GetData_byKey($"{xlsBaseDataName}_{i}");
				if (data_byKey != null)
				{
					textList.Add(data_byKey.m_strTxt);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void SetMonologueState(MonologueState monoState)
	{
		if (m_curMode == Mode.WatchMenu || monoState == MonologueState.None)
		{
			m_MonologueRoot.SetActive(value: false);
		}
		else
		{
			if (m_OnCursorContent == null)
			{
				return;
			}
			List<string> list = null;
			GameSwitch gameSwitch = GameSwitch.GetInstance();
			switch (monoState)
			{
			case MonologueState.Idle:
				if (gameSwitch.GetMentalLowHigh() == ConstGameSwitch.eMental.LOW)
				{
					list = m_MontTexts_LowMental;
					break;
				}
				if (m_OnCursorContent != null && !m_OnCursorContent.isReadable)
				{
					list = m_MontTexts_OldPost;
					break;
				}
				if (m_OnCursorContent != null && m_OnCursorContent.isReplyContained)
				{
					list = m_MontTexts_Reply;
					break;
				}
				if (m_OnCursorContent != null && m_OnCursorContent.isKeywordContained)
				{
					list = m_MontTexts_GetKeyword;
					break;
				}
				if (m_ContainKeywordContentInfos.Count <= 0 && m_ReplyContentInfos.Count <= 0)
				{
					list = m_MontTexts_NomoreKeyword;
					break;
				}
				m_MonologueText.text = m_MonologueIdleText;
				return;
			case MonologueState.NotKeyword:
				list = m_MontTexts_NoKeyword;
				break;
			}
			if (list == null || list.Count <= 0)
			{
				m_MonologueText.text = "No exist valid Text!!!";
				return;
			}
			int count = list.Count;
			if (count > 1)
			{
				m_MonologueText.text = list[UnityEngine.Random.Range(0, count)];
			}
			else
			{
				m_MonologueText.text = list[0];
			}
		}
	}

	private void SetCharMotion(bool isAutoByMental = true, CharSpecMotion motion = CharSpecMotion.watch)
	{
		if (m_CharMotionAnimator == null || !m_CharMotionAnimator.gameObject.activeSelf)
		{
			return;
		}
		if (isAutoByMental)
		{
			motion = CharSpecMotion.watch_no;
			float num = GameSwitch.GetInstance().GetMental();
			if (num > 0f)
			{
				switch (GameSwitch.GetInstance().GetMentalLowHigh())
				{
				case ConstGameSwitch.eMental.HIGH:
					motion = CharSpecMotion.watch_high;
					break;
				case ConstGameSwitch.eMental.NORMAL:
					motion = CharSpecMotion.watch;
					break;
				case ConstGameSwitch.eMental.LOW:
					motion = CharSpecMotion.watch_low;
					break;
				}
			}
		}
		m_CharMotionAnimator.Rebind();
		m_CharMotionAnimator.Play(motion.ToString());
	}

	public void OnProc_KeywordGetButton(SNSContentPlus snsContent)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Detail");
		}
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		if (snsContent == null)
		{
			FinishGetKeywordProc();
			return;
		}
		m_GetKeywordContent = snsContent;
		if (string.IsNullOrEmpty(snsContent.xlsData.m_strIDKeyword))
		{
			SetMonologueState(MonologueState.NotKeyword);
			ChangeMentalPoint();
		}
		else
		{
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetShow(isShow: false);
			}
			if (snsContent.xlsData.m_iGroupKeyword >= 0)
			{
				gameSwitch.SetKeywordGroup(snsContent.xlsData.m_iGroupKeyword, 1);
			}
			gameSwitch.SetKeywordAllState(snsContent.xlsData.m_strIDKeyword, 1, isSetPop: true, OnEvent_CloseKeywordGetPopup);
			snsContent.ResetContentText();
			RemoveContainKeywordContentInfo(snsContent.xlsData);
		}
		snsContent.HideButtons();
	}

	public void OnEvent_CloseKeywordGetPopup(object sender, object arg)
	{
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: true);
		}
		ChangeMentalPoint();
	}

	public void OnEvent_CloseActPointChangePopup(object sender, object arg)
	{
		ChangeMentalPoint();
	}

	private void ChangeMentalPoint()
	{
		MentalGageRenewal s_Instance = MentalGageRenewal.s_Instance;
		if (!(s_Instance == null) && !(m_GetKeywordContent == null) && m_GetKeywordContent.xlsData.m_iMentalDelta != 0)
		{
			GameSwitch gameSwitch = GameSwitch.GetInstance();
			if (gameSwitch != null)
			{
				int mental = gameSwitch.GetMental();
				if (gameSwitch.AddMental(m_GetKeywordContent.xlsData.m_iMentalDelta, isEnableEff: false))
				{
					EventEngine.GetInstance(isCreate: false)?.m_TalkChar.ChangeHanMotByMental();
					m_oldMentalGageCanvasOrder = s_Instance.canvasOrder;
					s_Instance.canvasOrder = m_MainCanvas.sortingOrder + 1;
					s_Instance.ShowChangedState_byMentalPoint(gameSwitch.GetMental(), mental, OnEvent_MentalGageTransComplete);
					if (gameSwitch.GetMental() <= 0)
					{
						return;
					}
				}
			}
		}
		FinishGetKeywordProc();
	}

	public void OnEvent_MentalGageTransComplete(object sender, object arg)
	{
		MentalGageRenewal s_Instance = MentalGageRenewal.s_Instance;
		if (s_Instance != null)
		{
			s_Instance.canvasOrder = m_oldMentalGageCanvasOrder;
		}
		if (GameSwitch.GetInstance().GetMental() <= ConstGameSwitch.MIN_MENTAL_POINT)
		{
			FinishGetKeywordProc();
		}
	}

	private void FinishGetKeywordProc()
	{
		m_GetKeywordContent = null;
		ResetAllContentState();
		SetCharMotion();
		SetMonologueState(MonologueState.Idle);
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		if (m_curMode != Mode.WatchMenu && gameSwitch.GetMental() <= ConstGameSwitch.MIN_MENTAL_POINT)
		{
			EnableInputBlock(isEnable: true);
			BeginGameOverSequence();
		}
	}

	private void Close_forInvoke()
	{
		CloseSNSMenu();
	}

	public void OnProc_ReplyButton(SNSContentPlus snsContent)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Detail");
		}
		if (m_SelectionMenuPrefab == null)
		{
			return;
		}
		if (m_SelectionMenu == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_SelectionMenuPrefab) as GameObject;
			if (gameObject == null)
			{
				return;
			}
			m_SelectionMenu = gameObject.GetComponent<CommonSelectionPlus>();
			if (m_SelectionMenu == null)
			{
				return;
			}
			m_SelectionCanvas = gameObject.GetComponentInChildren<Canvas>();
			if (m_SelectionCanvas != null && m_MainCanvas != null)
			{
				m_SelectionCanvas.sortingOrder = m_MainCanvas.sortingOrder + 1;
			}
			Transform component = m_SelectionMenu.GetComponent<Transform>();
			component.SetParent(base.gameObject.GetComponent<Transform>(), worldPositionStays: false);
			component.localPosition = Vector3.zero;
			component.localRotation = Quaternion.identity;
			component.localScale = Vector3.one;
		}
		Xls.SNSPostData xlsData = snsContent.xlsData;
		Xls.SNSReplyData data_byKey = Xls.SNSReplyData.GetData_byKey(xlsData.m_strIDReply);
		if (data_byKey != null)
		{
			m_curReplyContent = snsContent;
			m_curXlsReplyData = data_byKey;
			m_SelectionMenu.Show(m_curXlsReplyData, OnEvent_ResultSelection);
			EnableInputBlock(isEnable: true);
			m_TabContainer.isInputBlock = true;
			m_MonologueRoot.SetActive(value: false);
		}
	}

	public void OnEvent_ResultSelection(object sender, object arg)
	{
		CommonSelectionPlus.Buttons buttons = (CommonSelectionPlus.Buttons)arg;
		int num = -1;
		switch (buttons)
		{
		case CommonSelectionPlus.Buttons.Left:
			num = 1;
			break;
		case CommonSelectionPlus.Buttons.Right:
			num = 2;
			break;
		}
		if (num < 0)
		{
			EnableInputBlock(isEnable: false);
			m_TabContainer.isInputBlock = false;
			m_MonologueRoot.SetActive(value: true);
			InitButtonGuide();
			return;
		}
		if (m_curReplyContent.xlsData.m_iReplyGroup >= 0)
		{
			GameSwitch.GetInstance().SetReplyGroup(m_curReplyContent.xlsData.m_iReplyGroup, 1);
		}
		RemoveReplyContentInfo(m_curReplyContent.xlsData);
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		gameSwitch.SetPostReply(m_curReplyContent.xlsData.m_iIdx, 1);
		m_curReplyContent.HideButtons();
		int num2 = ((num != 1) ? m_curXlsReplyData.m_rightPostSwitch : m_curXlsReplyData.m_leftPostSwitch);
		if (num2 >= 0)
		{
			gameSwitch.SetEventSwitch(num2, 1);
		}
		StartCoroutine(InsertReplyContents(num));
	}

	private IEnumerator InsertReplyContents(int selPostType)
	{
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		int num = -1;
		int count = m_curContentInfos.Count;
		for (int i = 0; i < count; i++)
		{
			ContentInfo contentInfo = m_curContentInfos[i];
			Xls.SNSPostData xlsData = contentInfo.xlsData;
			if (xlsData.m_iIsSelPost == selPostType && xlsData.m_strIDReply.Equals(m_curXlsReplyData.m_key) && !contentInfo.isValid)
			{
				gameSwitch.SetPostState(xlsData.m_iIdx, 2);
				contentInfo.isNew = false;
				contentInfo.isValid = true;
				if (num < 0)
				{
					num = i;
				}
				if (m_validContentIndexFirst > i)
				{
					m_validContentIndexFirst = i;
				}
				if (m_validContentIndexLast < i)
				{
					m_validContentIndexLast = i;
				}
				AddToList_IfReplyOrGetKeywordContentInfo(contentInfo);
			}
		}
		if (num >= 0)
		{
			ResetContentPositionInfo(num);
			int curBaseContentIdx = m_curBaseContentIdx;
			int onCursorContentIdx = m_onCursorContentIdx;
			SetOnCursorContentSlot(null);
			ClearActivedContentSlots();
			SetCurrentBaseContentIndex(curBaseContentIdx);
			SetOnCursorContentIndex(onCursorContentIdx);
		}
		string text = ((selPostType != 1) ? m_curXlsReplyData.m_rightPostConti : m_curXlsReplyData.m_leftPostConti);
		if (!string.IsNullOrEmpty(text) && ContiDataHandler.IsExistConti(text))
		{
			gameSwitch.SetRecordFaterConti(text);
			m_isDialogBlackDisappear = false;
			m_eDialogAniState = GameDefine.eAnimChangeState.none;
			m_isRunEvent = true;
			EnableInputBlock(isEnable: true);
			EventEngine.GetInstance().EnableRecordFaterEventObj("RUN_RECORD", CheckInsertPostDatas);
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetShow(isShow: false);
			}
		}
		else
		{
			EnableInputBlock(isEnable: false);
			m_TabContainer.isInputBlock = false;
			m_MonologueRoot.SetActive(value: true);
			InitButtonGuide();
		}
		yield break;
	}

	private void CheckInsertPostDatas(object sender, object args)
	{
		FinishConti();
	}

	private void FinishConti()
	{
		m_isRunEvent = false;
		EnableInputBlock(isEnable: false);
		m_TabContainer.isInputBlock = false;
		GameSwitch.GetInstance().SetRecordFaterConti(null);
		m_MonologueRoot.SetActive(value: true);
		ResetAllContentState();
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		if (m_curMode != Mode.WatchMenu && gameSwitch.GetMental() <= ConstGameSwitch.MIN_MENTAL_POINT)
		{
			EnableInputBlock(isEnable: true);
			MentalGageRenewal s_Instance = MentalGageRenewal.s_Instance;
			if (s_Instance.IsCompleteChangeMentalGage())
			{
				BeginGameOverSequence();
			}
			else
			{
				m_isMentalGageWaiting = true;
			}
		}
		else
		{
			InitButtonGuide();
			GameGlobalUtil.PlayUIAnimation(m_aniDialogBlackBG, GameDefine.UIAnimationState.disappear, ref m_eDialogAniState);
			m_isDialogBlackDisappear = true;
		}
	}

	private void BeginGameOverSequence()
	{
		EnableInputBlock(isEnable: true);
		float time = 0f;
		Xls.ProgramDefineStr data_byKey = Xls.ProgramDefineStr.GetData_byKey("SNS_GAMEOVER_OUT_DELAY_TIME");
		if (data_byKey != null)
		{
			time = float.Parse(data_byKey.m_strTxt, CultureInfo.InvariantCulture);
		}
		Invoke("Close_forInvoke", time);
	}

	public static void ReplacePostData(string oldPostKey, string newPostKey)
	{
		if (!(s_activedInstance == null))
		{
			s_activedInstance._ReplacePostData(oldPostKey, newPostKey);
		}
	}

	private void _ReplacePostData(string oldPostKey, string newPostKey)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(oldPostKey);
		Xls.SNSPostData data_byKey2 = Xls.SNSPostData.GetData_byKey(newPostKey);
		if (data_byKey == null || data_byKey2 == null)
		{
			return;
		}
		ContentInfo contentInfo = null;
		foreach (ContentInfo curContentInfo in m_curContentInfos)
		{
			if (curContentInfo.xlsData != data_byKey)
			{
				continue;
			}
			curContentInfo.xlsData = data_byKey2;
			contentInfo = curContentInfo;
			break;
		}
		if (contentInfo == null)
		{
			return;
		}
		foreach (SNSContentPlus activedContentSlot in m_activedContentSlots)
		{
			if (activedContentSlot.xlsData != data_byKey)
			{
				continue;
			}
			activedContentSlot.SetContentData(data_byKey2, 0f - contentInfo.pos, contentInfo.height, m_curMode);
			break;
		}
	}

	public static bool IsFinishedMoveToAddedContent()
	{
		return !s_isScrollingEvent && !s_isAddingContent;
	}

	public static void AddInsertPostKey(string postKey, bool isMoveToContent)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(postKey);
		if (data_byKey != null)
		{
			AddInsertPostData(data_byKey, isMoveToContent);
		}
	}

	public static void AddInsertPostData(Xls.SNSPostData postData, bool isMoveToContent, float moveTime = 0.5f, int moveType = 3)
	{
		if (s_activedInstance == null)
		{
			return;
		}
		int contentIndex_byPostData = s_activedInstance.GetContentIndex_byPostData(postData);
		if (contentIndex_byPostData >= 0)
		{
			s_isScrollingEvent = false;
			s_isAddingContent = false;
			if (GameSwitch.GetInstance().SetPostState(postData.m_iIdx, 1))
			{
				s_activedInstance.StartCoroutine(s_activedInstance.InsertPostContents(contentIndex_byPostData, isMoveToContent, moveTime, moveType));
			}
			else if (isMoveToContent)
			{
				s_activedInstance.StartCoroutine(s_activedInstance.MoveAndSelect_byContentIndex(contentIndex_byPostData, moveTime, moveType));
			}
		}
	}

	private IEnumerator InsertPostContents(int addContentIdx, bool isMoveToLastAddedContent, float moveTime, int moveType)
	{
		s_isAddingContent = true;
		if (addContentIdx >= 0 && addContentIdx < m_curContentInfos.Count)
		{
			GameSwitch gameSwitch = GameSwitch.GetInstance();
			sbyte switchState = 0;
			if (isMoveToLastAddedContent)
			{
				yield return StartCoroutine(MoveAndSelect_byContentIndex(addContentIdx, moveTime, moveType));
				yield return new WaitForSecondsRealtime(0.5f);
			}
			ContentInfo contentInfo = m_curContentInfos[addContentIdx];
			if (contentInfo != null && !contentInfo.isValid)
			{
				switchState = gameSwitch.GetPostState(contentInfo.xlsData.m_iIdx);
				if (switchState == 1 || switchState == 2)
				{
					contentInfo.isNew = switchState == 1;
					contentInfo.isValid = true;
					contentInfo.isHide = true;
					if (m_validContentIndexFirst > addContentIdx)
					{
						m_validContentIndexFirst = addContentIdx;
					}
					if (m_validContentIndexLast < addContentIdx)
					{
						m_validContentIndexLast = addContentIdx;
					}
					AddToList_IfReplyOrGetKeywordContentInfo(contentInfo);
					float oldContentPos = ((m_onCursorContentIdx < 0) ? null : m_curContentInfos[m_onCursorContentIdx])?.pos ?? 0f;
					float oldScrollPos = m_ScrollHandler.scrollPos;
					ResetContentPositionInfo(addContentIdx);
					float newContentPos = ((m_onCursorContentIdx < 0) ? null : m_curContentInfos[m_onCursorContentIdx])?.pos ?? 0f;
					float newScrollPos = oldScrollPos + (newContentPos - oldContentPos);
					int oldOnCursorContentIndex = m_onCursorContentIdx;
					SetOnCursorContentSlot(null);
					ClearActivedContentSlots();
					SetActivedContents_byScrollPos(newScrollPos);
					SetOnCursorContentIndex(oldOnCursorContentIndex, isAdjustScrollPos: false);
					m_ScrollHandler.SetScrollPos(newScrollPos);
					yield return null;
					m_curContentInfos[addContentIdx].isHide = false;
					SNSContentPlus slot = GetContentSlot_byContentInfoIndex(addContentIdx);
					if (!(slot == null))
					{
						slot.gameObject.SetActive(value: true);
						Animator[] animators = slot.gameObject.GetComponentsInChildren<Animator>();
						if (animators != null && animators.Length > 0)
						{
							string aniStateName_Appear = GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.appear);
							Animator[] array = animators;
							foreach (Animator animator in array)
							{
								if (GameGlobalUtil.HasStateInAnimator(animator, GameDefine.UIAnimationState.appear))
								{
									animator.Play(aniStateName_Appear, -1, 0f);
								}
							}
							yield return null;
							string aniStateName_Idle = GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.idle);
							Animator[] array2 = animators;
							foreach (Animator animator2 in array2)
							{
								if (GameGlobalUtil.HasStateInAnimator(animator2, GameDefine.UIAnimationState.idle))
								{
									while (!animator2.GetCurrentAnimatorStateInfo(0).IsName(aniStateName_Idle))
									{
										yield return null;
									}
								}
							}
							SetOnCursorContentIndex(addContentIdx);
						}
					}
				}
			}
		}
		s_isAddingContent = false;
	}

	private IEnumerator MoveAndSelect_byContentIndex(int contentIndex, float moveTime, int moveType)
	{
		if (contentIndex < 0 || contentIndex >= m_curContentInfos.Count)
		{
			yield break;
		}
		ContentInfo contentInfo = m_curContentInfos[contentIndex];
		if (contentInfo != null)
		{
			s_isScrollingEvent = true;
			float targetPos = 0f - contentInfo.pos;
			m_ScrollHandler.ScrollToTargetPos(targetPos, moveTime, moveType);
			SetOnCursorContentIndex(contentIndex, isAdjustScrollPos: false);
			while (m_ScrollHandler.IsScrolling)
			{
				yield return null;
			}
			s_isScrollingEvent = false;
		}
	}

	private void ResetContentPositionInfo(int startContentIndex)
	{
		int count = m_curContentInfos.Count;
		if (startContentIndex >= 0 && startContentIndex < count)
		{
			ContentInfo contentInfo = m_curContentInfos[startContentIndex];
			float pos = contentInfo.pos;
			float num = pos + contentInfo.scaledHeight;
			for (int i = startContentIndex + 1; i < count; i++)
			{
				contentInfo = m_curContentInfos[i];
				contentInfo.pos = num;
				num += ((!contentInfo.isValid) ? 0f : contentInfo.scaledHeight);
			}
			num += m_BottomSpacing;
			float scrollPos = m_ScrollHandler.scrollPos;
			float num2 = Mathf.Max(num, m_ScrollRect.viewport.rect.height);
			m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num);
			m_ScrollHandler.ResetScrollRange();
			m_ScrollHandler.scrollPos = scrollPos;
		}
	}

	public void OnProc_ViewAdButton(SNSContentPlus snsContent)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Detail");
		}
	}

	public IEnumerator OnProc_ViewImageDetail(SNSContentPlus snsContent)
	{
		if (snsContent == null)
		{
			yield break;
		}
		Xls.CollImages xlsCollectionImage = Xls.CollImages.GetData_byKey(snsContent.xlsData.m_strIDImg);
		if (xlsCollectionImage == null)
		{
			yield break;
		}
		if (m_OnCursorContent != snsContent)
		{
			SetOnCursorContentIndex(snsContent.indexInContentInfos);
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
			EnableInputBlock(isEnable: true);
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
			EnableInputBlock(isEnable: true);
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
		EnableInputBlock(isEnable: false);
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

	private void ShowTutorialPopup()
	{
		string text = string.Empty;
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		int tutoMenuState = gameSwitch.GetTutoMenuState();
		if (tutoMenuState == 2 && m_curMode != Mode.WatchMenu)
		{
			text = "tuto_00026";
		}
		if (string.IsNullOrEmpty(text))
		{
			m_isTutorialActivated = false;
			if (m_TouchBlockPanel != null)
			{
				m_TouchBlockPanel.SetActive(value: false);
			}
			return;
		}
		m_isTutorialActivated = TutorialPopup.isShowAble(text);
		if (m_isTutorialActivated)
		{
			StartCoroutine(TutorialPopup.Show(text, OnClosed_TutorialPopup, m_MainCanvas));
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetShow(isShow: false);
			}
		}
		else
		{
			m_isTutorialActivated = false;
			if (m_TouchBlockPanel != null)
			{
				m_TouchBlockPanel.SetActive(value: false);
			}
		}
	}

	private void OnClosed_TutorialPopup(object sender, object arg)
	{
		m_isTutorialActivated = false;
		if (m_TouchBlockPanel != null)
		{
			m_TouchBlockPanel.SetActive(value: false);
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: true);
		}
	}

	public void TouchCloseSNSMenu()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			if (m_ShowImageOrigin != null && m_ShowImageOrigin.gameObject.activeInHierarchy)
			{
				m_ShowImageOrigin.OnClickScreen();
			}
			else
			{
				CloseSNSMenu();
			}
		}
	}
}
