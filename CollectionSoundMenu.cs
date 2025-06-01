using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class CollectionSoundMenu : CommonBGChildBase
{
	public GameObject m_RootObject;

	public Text m_TitleText;

	public Text m_TotalCompleteRateTitle;

	public Text m_TotalCompleteRateValue;

	public Text m_CompleteRateTitle;

	public Text m_CompleteRateValue;

	public GameObject m_CompleteRateValueBG;

	public CommonTabContainerPlus m_TabContainer;

	[Header("Contents Container")]
	public ScrollRect m_ScrollRect;

	public RectTransform m_ContentContanierRT;

	public GameObject m_ContentSrcObject;

	public float m_ContentInterval;

	public float m_BottomSpacing = 80f;

	public GameObject m_ChangingTabCover;

	public Animator m_aniDialogBlackBG;

	[Header("Scroll Members")]
	public GameObject m_ScrollbarRoot;

	public Button m_ScrollButtonToFirst;

	public Button m_ScrollButtonRStick;

	public bool m_ScrollLoopEnable = true;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	public float m_ScrollRepeatTime = 0.2f;

	private float m_fScrollButtonPusingTime;

	private Dictionary<int, CategoryInfo<Xls.CollSounds>> m_CategoryInfos = new Dictionary<int, CategoryInfo<Xls.CollSounds>>();

	private CategoryInfo<Xls.CollSounds> m_curCategoryInfo;

	private List<RecordContentPlus> m_Contents = new List<RecordContentPlus>();

	private RecordContentPlus m_OnCursorContent;

	private RecordContentPlus m_CurPlayingContent;

	private bool m_isPlayedAnySound;

	private bool m_isLoadingSoundAsset;

	private int m_firstSelectCategory = -1;

	private bool m_isInputBlock;

	private bool m_isPageScrolling;

	private Animator m_CloseAnimator;

	private LoadingSWatchIcon m_LoadingIcon;

	private const int c_AudioCannel = 0;

	private AudioManager m_AudioManager;

	private CommonButtonGuide m_ButtonGuide;

	private bool m_isInitializedButtonGuidText;

	private string m_buttonGuide_SelContent;

	private string m_buttonGuide_ExitMenu;

	private string m_buttonGuide_PlayStop;

	private string m_buttonGuide_ScrollToTop;

	private string m_buttonGuide_ChangeTab;

	private string m_buttonGuide_SelectTab;

	private string m_buttonGuide_CancelTab;

	private bool m_isCompleteEventInitialize;

	private bool m_isRunEvent;

	private GameDefine.eAnimChangeState m_eDialogAniState;

	private bool m_isDialogBlackDisappear;

	private EventEngine m_EventEngine;

	private bool m_isMainMenuScene;

	private bool isAppPaused;

	public bool isMainMenuScene
	{
		get
		{
			return m_isMainMenuScene;
		}
		set
		{
			m_isMainMenuScene = value;
		}
	}

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
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Vertical, m_ScrollRect, m_ContentContanierRT, null, null, m_ScrollButtonToFirst);
		m_ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
		m_ContentSrcObject.SetActive(value: false);
		m_EventEngine = EventEngine.GetInstance();
	}

	private void OnDestroy()
	{
		if (m_LoadingIcon != null)
		{
			UnityEngine.Object.Destroy(m_LoadingIcon.gameObject);
			m_LoadingIcon = null;
		}
		if (m_ScrollHandler != null)
		{
			m_ScrollHandler.ReleaseScroll();
		}
		if (m_CategoryInfos != null)
		{
			m_CategoryInfos.Clear();
		}
		m_curCategoryInfo = null;
		if (m_Contents != null)
		{
			m_Contents.Clear();
		}
		m_OnCursorContent = null;
		m_CurPlayingContent = null;
		m_CloseAnimator = null;
		m_AudioManager = null;
		m_ButtonGuide = null;
		m_EventEngine = null;
	}

	private void OnApplicationPause(bool pause)
	{
		isAppPaused = pause;
	}

	private void Update()
	{
		if (isAppPaused || !m_isCompleteEventInitialize)
		{
			return;
		}
		if (m_isDialogBlackDisappear)
		{
			if (m_eDialogAniState != GameDefine.eAnimChangeState.none && GameGlobalUtil.CheckPlayEndUIAnimation(m_aniDialogBlackBG, GameDefine.UIAnimationState.disappear, ref m_eDialogAniState))
			{
				m_aniDialogBlackBG.gameObject.SetActive(value: false);
				m_isDialogBlackDisappear = false;
				m_eDialogAniState = GameDefine.eAnimChangeState.none;
			}
			return;
		}
		if (m_CloseAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_CloseAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				CallClosedCallback();
			}
			return;
		}
		if (m_isMainMenuScene && m_isRunEvent)
		{
			m_EventEngine.PressSkipKey();
		}
		if (!m_isRunEvent && m_CurPlayingContent != null && !m_isLoadingSoundAsset && m_AudioManager != null && !m_AudioManager.IsPlayingChannel(0))
		{
			m_CurPlayingContent.isPlaying = false;
			m_CurPlayingContent = null;
		}
		if (m_ScrollHandler.IsScrolling)
		{
			m_ScrollHandler.Update();
			bool flag = true;
			if (m_ScrollHandler.CurScrollEvent == UIUtil_ScrollHandler.ScrollEvent.None && m_OnCursorContent != null)
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
		if (m_isInputBlock || m_TabContainer.isChaningTab || ButtonPadInput.IsPlayingButPressAnim() || PopupDialoguePlus.IsAnyPopupActivated())
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
				OnClick_ContentScrollToTop();
			}
			bool flag2 = false;
			num = GamePadInput.GetAxisValue(PadInput.GameInput.RStickY);
			if (!GameGlobalUtil.IsAlmostSame(num, 0f))
			{
				m_ScrollHandler.ScrollByDirection(num > 0f, Mathf.Abs(num));
				flag2 = true;
			}
			else
			{
				flag2 = !m_ScrollHandler.IsScrolling && m_isPageScrolling;
				if (flag2)
				{
					m_isPageScrolling = false;
				}
				if (!m_ScrollHandler.IsScrolling)
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
					if (m_isPageScrolling)
					{
						flag2 = false;
					}
				}
			}
			if (flag2)
			{
				ChangeOnCursorContent_byScrollPos();
			}
		}
		if (m_ScrollHandler.IsScrolling)
		{
			return;
		}
		num = GamePadInput.GetAxisValue(PadInput.GameInput.LStickY);
		if (Mathf.Abs(num) > 0.5f)
		{
			if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				ChangeOnCursorContent(isUpSide: true);
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				ChangeOnCursorContent(isUpSide: false);
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
				{
					m_fScrollButtonPusingTime = 0f;
					ChangeOnCursorContent(isUpSide: true);
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing))
			{
				m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
				{
					m_fScrollButtonPusingTime = 0f;
					ChangeOnCursorContent(isUpSide: false);
				}
			}
		}
		if (m_OnCursorContent != null && m_OnCursorContent.isValid)
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_OnCursorContent.m_PadIconButton, m_OnCursorContent.m_PlayButton, m_OnCursorContent.m_StopButton);
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
			{
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetContentActivate(m_buttonGuide_PlayStop, isActivate: true);
				}
				OnProc_PlayStopRecord(m_OnCursorContent, !m_OnCursorContent.isPlaying);
				return;
			}
		}
		if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
		{
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_ExitMenu, isActivate: true);
			}
			Close();
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		if (m_CompleteRateTitle != null)
		{
			m_CompleteRateTitle.gameObject.SetActive(value: false);
		}
		if (m_CompleteRateValue != null)
		{
			m_CompleteRateValue.gameObject.SetActive(value: false);
		}
		if (m_CompleteRateValueBG != null)
		{
			m_CompleteRateValueBG.gameObject.SetActive(value: false);
		}
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		if (m_AudioManager != null)
		{
			m_AudioManager.BackupCurGamePlaySound();
			m_AudioManager.Stop();
		}
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
		m_ScrollHandler.ResetScrollRange();
		m_isRunEvent = false;
		m_isDialogBlackDisappear = false;
		InitCategoryInfo();
		InitTextMemebers();
		if (m_TabContainer != null)
		{
			m_TabContainer.BuildTabButtonObjects();
			m_TabContainer.isInputBlock = false;
			m_firstSelectCategory = 0;
			Invoke("SetFirstSelectTab", 0.1f);
		}
		MatchCategoryInfoToTabButton();
		m_isInputBlock = false;
		m_isPageScrolling = false;
		m_CloseAnimator = null;
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.appear.ToString());
		InitButtonGuide();
		if (!m_isCompleteEventInitialize)
		{
			StartCoroutine(InitEventModules());
		}
	}

	private IEnumerator InitEventModules()
	{
		yield return StartCoroutine(ContiDataHandler.Init("ContiData"));
		m_isCompleteEventInitialize = true;
	}

	public void InitTextMemebers()
	{
		Text[] textComps = new Text[5] { m_TitleText, m_TotalCompleteRateTitle, m_TotalCompleteRateValue, m_CompleteRateTitle, m_CompleteRateValue };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		RecordContentPlus.InitStaticTextMembers();
		if (m_TitleText != null)
		{
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_TITLE");
		}
		if (m_TotalCompleteRateTitle != null)
		{
			m_TotalCompleteRateTitle.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_COMPLETE_RATE");
		}
	}

	private void InitCategoryInfo()
	{
		m_CategoryInfos.Clear();
		CategoryInfo<Xls.CollSounds> value = null;
		int[] array = (int[])Enum.GetValues(typeof(GameDefine.CollectionSoundType));
		int[] array2 = array;
		foreach (int num in array2)
		{
			if (!m_CategoryInfos.ContainsKey(num))
			{
				string empty = string.Empty;
				switch (num)
				{
				case -1:
					empty = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_TAB_ALL");
					break;
				case 0:
					empty = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_TAB1");
					break;
				case 1:
					empty = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_TAB2");
					break;
				case 2:
					empty = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_TAB3");
					break;
				case 3:
					empty = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_TAB4");
					break;
				default:
					continue;
				}
				value = new CategoryInfo<Xls.CollSounds>(num, empty);
				m_CategoryInfos.Add(num, value);
			}
		}
		if (m_CategoryInfos.Count <= 0)
		{
			return;
		}
		GameSwitch instance = GameSwitch.GetInstance();
		sbyte b = 0;
		bool flag = false;
		bool flag2 = false;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (Xls.CollSounds data in Xls.CollSounds.datas)
		{
			value = null;
			if (m_CategoryInfos.TryGetValue(data.m_iCategory, out value) && value != null)
			{
				value.m_xlsDatas.Add(data);
				b = instance.GetCollSound(data.m_iIdx);
				flag = b == 1;
				flag2 = flag || b == 2;
				value.m_ValidContentCount += (flag2 ? 1 : 0);
				value.m_NewContentCount += ((b == 1) ? 1 : 0);
				num2 += (flag ? 1 : 0);
				num3 += (flag2 ? 1 : 0);
				num4++;
			}
		}
		if (m_CategoryInfos.TryGetValue(-1, out value) && value != null)
		{
			value.m_xlsDatas = Xls.CollSounds.datas;
			value.m_ValidContentCount = num3;
			value.m_NewContentCount = num2;
		}
		Dictionary<int, CategoryInfo<Xls.CollSounds>>.Enumerator enumerator2 = m_CategoryInfos.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			value = enumerator2.Current.Value;
			if (value != null && value.m_xlsDatas != null && value.m_xlsDatas.Count > 0)
			{
				float num5 = value.m_xlsDatas.Count;
				float num6 = value.m_ValidContentCount;
				value.m_CompleteRate = num6 * 100f / num5;
				if (value.m_CompleteRate < 1f && num6 > 1f)
				{
					value.m_CompleteRate = 1f;
				}
			}
		}
		if (m_TotalCompleteRateValue != null)
		{
			float num7 = ((num3 >= num4) ? 100f : ((num4 <= 0) ? 0f : ((float)num3 * 100f / (float)num4)));
			if (num7 < 1f && num3 > 0)
			{
				num7 = 1f;
			}
			m_TotalCompleteRateValue.text = $"{(int)num7}%";
		}
	}

	private void MatchCategoryInfoToTabButton()
	{
		Dictionary<int, CategoryInfo<Xls.CollSounds>>.Enumerator enumerator = m_CategoryInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CategoryInfo<Xls.CollSounds> value = enumerator.Current.Value;
			if (value != null)
			{
				value.m_LinkedTabButton = m_TabContainer.GetTabButton(value);
				value.ReflashLinkedTabButton();
			}
		}
	}

	public void Close(bool isEnableAnimation = true)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Cancel");
		}
		m_isInputBlock = true;
		m_TabContainer.isInputBlock = true;
		if (m_CurPlayingContent != null)
		{
			OnProc_PlayStopRecord(m_CurPlayingContent, isPlay: false);
		}
		if (base.eventNoticeExit != null)
		{
			base.eventNoticeExit(this, false);
		}
		if (!isEnableAnimation)
		{
			CallClosedCallback();
			return;
		}
		m_CloseAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
		if (m_CloseAnimator == null)
		{
			CallClosedCallback();
		}
	}

	private void CallClosedCallback()
	{
		SetOnCursorContent(null, isAdjustScrollPos: false);
		m_CloseAnimator = null;
		m_TabContainer.ClearTabButtonObjects();
		ClearContents();
		base.gameObject.SetActive(value: false);
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
		if (m_AudioManager != null)
		{
			m_AudioManager.RestoreGamePlaySound();
		}
		if (base.eventCloseComplete != null)
		{
			base.eventCloseComplete(this, false);
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
		m_buttonGuide_SelContent = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_BTNGUIDE_SEL_CONTENT");
		m_buttonGuide_ExitMenu = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_BTNGUIDE_EXIT_MENU");
		m_buttonGuide_PlayStop = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_BTNGUIDE_PLAY_STOP");
		m_buttonGuide_ScrollToTop = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_BTNGUIDE_SCROLL_TO_TOP");
		m_buttonGuide_ChangeTab = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_BTNGUIDE_CHANGE_TAB");
		m_buttonGuide_SelectTab = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_BTNGUIDE_SEL_TAB");
		m_buttonGuide_CancelTab = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_BTNGUIDE_CANCEL_TAB");
		m_isInitializedButtonGuidText = true;
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuide_SelContent, PadInput.GameInput.LStickY);
		m_ButtonGuide.AddContent(m_buttonGuide_PlayStop, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ExitMenu, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ScrollToTop, PadInput.GameInput.TriangleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_SelectTab, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_CancelTab, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.L1Button);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.R1Button);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Center);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_PlayStop, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_SelectTab, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_CancelTab, isEnable: false);
		m_ButtonGuide.SetShow(isShow: true);
	}

	private void SetFirstSelectTab()
	{
		List<CommonTabContainerPlus.TabButtonInfo>.Enumerator enumerator = m_TabContainer.tabButtonInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CommonTabContainerPlus.TabButtonInfo current = enumerator.Current;
			if (current != null && current.tag != null && current.tag is CategoryInfo<Xls.CollSounds>)
			{
				CategoryInfo<Xls.CollSounds> categoryInfo = current.tag as CategoryInfo<Xls.CollSounds>;
				if (categoryInfo.ID == m_firstSelectCategory)
				{
					m_TabContainer.SetSelectedTab_byObject(current);
					return;
				}
			}
		}
		m_TabContainer.SetSelectedTab_byIdx(0);
	}

	private List<CommonTabContainerPlus.TabCreateInfo> SetTabCreateInfos()
	{
		List<CommonTabContainerPlus.TabCreateInfo> list = new List<CommonTabContainerPlus.TabCreateInfo>();
		Dictionary<int, CategoryInfo<Xls.CollSounds>>.Enumerator enumerator = m_CategoryInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CategoryInfo<Xls.CollSounds> value = enumerator.Current.Value;
			if (value != null && value.ID != -1)
			{
				CommonTabContainerPlus.TabCreateInfo tabCreateInfo = new CommonTabContainerPlus.TabCreateInfo();
				tabCreateInfo.m_Text = value.Name;
				tabCreateInfo.m_Tag = value;
				list.Add(tabCreateInfo);
			}
		}
		return list;
	}

	private void OnChangedSelectTab(object sender, object arg)
	{
		if (sender is CommonTabContainerPlus.TabButtonInfo tabButtonInfo)
		{
			m_isInputBlock = true;
			m_TabContainer.isInputBlock = true;
			m_ScrollRect.velocity = Vector2.zero;
			StartCoroutine(InitContents(tabButtonInfo.tag as CategoryInfo<Xls.CollSounds>));
			if (m_ChangingTabCover != null)
			{
				m_ChangingTabCover.SetActive(value: false);
			}
			if (m_CurPlayingContent != null)
			{
				OnProc_PlayStopRecord(m_CurPlayingContent, isPlay: false);
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
			m_ButtonGuide.SetContentEnable(m_buttonGuide_PlayStop, !flag && m_OnCursorContent != null && m_OnCursorContent.isValid);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ExitMenu, !flag);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, !flag && m_ScrollHandler.isScrollable);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ChangeTab, isEnable: true);
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

	private IEnumerator InitContents(CategoryInfo<Xls.CollSounds> categoryInfo)
	{
		SetOnCursorContent(null, isAdjustScrollPos: false);
		m_curCategoryInfo = categoryInfo;
		ClearContents();
		float containerSize = 0f;
		float completeRate = 0f;
		Color colorCompleteRate = GameGlobalUtil.HexToColor(0);
		string categoryName = string.Empty;
		if (m_CompleteRateTitle != null)
		{
			m_CompleteRateTitle.gameObject.SetActive(value: false);
		}
		if (m_CompleteRateValue != null)
		{
			m_CompleteRateValue.gameObject.SetActive(value: false);
		}
		if (m_CompleteRateValueBG != null)
		{
			m_CompleteRateValueBG.gameObject.SetActive(value: false);
		}
		if (categoryInfo != null)
		{
			List<Xls.CollSounds> xlsDatas = categoryInfo.m_xlsDatas;
			int dataCount = xlsDatas.Count;
			if (dataCount > 0)
			{
				if (m_LoadingIcon != null)
				{
					m_LoadingIcon.gameObject.SetActive(value: true);
				}
				else
				{
					m_LoadingIcon = LoadingSWatchIcon.Create(m_ScrollRect.gameObject);
				}
				sbyte switchState = 0;
				bool isValidContent = false;
				bool isEnableNewTag = false;
				Xls.CollSounds xlsData = null;
				RecordContentPlus contentComp = null;
				GameSwitch gameSwitch = GameSwitch.GetInstance();
				for (int i = 0; i < dataCount; i++)
				{
					yield return null;
					xlsData = xlsDatas[i];
					switchState = gameSwitch.GetCollSound(xlsData.m_iIdx);
					GameObject contentObject = UnityEngine.Object.Instantiate(m_ContentSrcObject);
					contentComp = contentObject.GetComponent<RecordContentPlus>();
					isEnableNewTag = switchState == 1;
					isValidContent = isEnableNewTag || switchState == 2;
					contentComp.InitCollectionSoundContent(i, xlsData, this, isValidContent, isEnableNewTag);
					m_Contents.Add(contentComp);
					contentComp.rectTransform.SetParent(m_ContentContanierRT, worldPositionStays: false);
					contentObject.SetActive(value: false);
				}
				yield return null;
				float fCurY = 0f;
				float fHeight = 0f;
				float fTotalHeight = 0f;
				dataCount = m_Contents.Count;
				for (int j = 0; j < dataCount; j++)
				{
					contentComp = m_Contents[j];
					contentComp.gameObject.SetActive(value: true);
					contentComp.rectTransform.anchoredPosition = new Vector2(contentComp.rectTransform.anchoredPosition.x, fCurY);
					fHeight = contentComp.rectTransform.rect.height * contentComp.rectTransform.localScale.y;
					fCurY -= fHeight + m_ContentInterval;
					fTotalHeight += fHeight;
				}
				if (dataCount > 1)
				{
					fTotalHeight += m_ContentInterval * (float)(dataCount - 1);
				}
				fTotalHeight += m_BottomSpacing;
				m_LoadingIcon.gameObject.SetActive(value: false);
				completeRate = categoryInfo.m_CompleteRate;
				if (GameGlobalUtil.IsAlmostSame(completeRate, 100f) || completeRate > 100f)
				{
					colorCompleteRate = GameGlobalUtil.HexToColor(2078092);
				}
				categoryName = categoryInfo.Name;
				containerSize = Mathf.Max(m_ScrollRect.viewport.rect.height, fTotalHeight);
			}
		}
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, containerSize);
		m_ScrollHandler.ResetScrollRange();
		m_ContentContanierRT.anchoredPosition = new Vector2(m_ContentContanierRT.anchoredPosition.x, 0f);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, m_ScrollHandler.isScrollable);
		SetOnCursorContent((m_Contents.Count <= 0) ? null : m_Contents[0]);
		if (m_CompleteRateTitle != null)
		{
			m_CompleteRateTitle.text = categoryName;
			m_CompleteRateTitle.gameObject.SetActive(value: true);
		}
		if (m_CompleteRateValue != null)
		{
			m_CompleteRateValue.gameObject.SetActive(value: true);
			m_CompleteRateValue.text = $"{((int)completeRate/*cast due to .constrained prefix*/).ToString()}%";
			m_CompleteRateValue.color = colorCompleteRate;
		}
		if (m_CompleteRateValueBG != null)
		{
			m_CompleteRateValueBG.SetActive(value: true);
		}
		m_isInputBlock = false;
		m_TabContainer.isInputBlock = false;
	}

	private void ClearContents()
	{
		int count = m_Contents.Count;
		for (int i = 0; i < count; i++)
		{
			UnityEngine.Object.Destroy(m_Contents[i].gameObject);
		}
		m_Contents.Clear();
	}

	private void SetOnCursorContent(RecordContentPlus content, bool isAdjustScrollPos = true)
	{
		if (m_OnCursorContent == content)
		{
			return;
		}
		if (m_OnCursorContent != null)
		{
			m_OnCursorContent.onCursor = false;
			if (m_OnCursorContent.activeNewMark)
			{
				m_OnCursorContent.activeNewMark = false;
				if (m_curCategoryInfo != null)
				{
					m_curCategoryInfo.ReflashLinkedTabButton();
				}
			}
		}
		if (content != null)
		{
			content.onCursor = true;
		}
		m_OnCursorContent = content;
		if (m_OnCursorContent != null && m_OnCursorContent.activeNewMark)
		{
			GameSwitch.GetInstance().SetCollSound(m_OnCursorContent.xlsData.m_iIdx, 2);
			if (m_curCategoryInfo != null)
			{
				m_curCategoryInfo.m_NewContentCount--;
			}
		}
		if (isAdjustScrollPos)
		{
			AdjustScrollPos_byOnCursonContent();
		}
		m_ButtonGuide.SetContentEnable(m_buttonGuide_PlayStop, m_OnCursorContent != null && m_OnCursorContent.isValid);
	}

	private bool ChangeOnCursorContent(bool isUpSide, bool isAdjustScrollPos = true)
	{
		int count = m_Contents.Count;
		if (m_OnCursorContent == null)
		{
			if (count > 0)
			{
				SetOnCursorContent(m_Contents[count - 1], isAdjustScrollPos: false);
			}
			return false;
		}
		int num = m_Contents.IndexOf(m_OnCursorContent);
		num = ((!isUpSide) ? (num + 1) : (num - 1));
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
		SetOnCursorContent(m_Contents[num], isAdjustScrollPos);
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

	private bool ChangeScrollPage(bool isUpSide)
	{
		if (!m_ScrollHandler.isScrollable || m_ScrollHandler.IsScrolling)
		{
			return false;
		}
		if ((isUpSide && m_ScrollHandler.scrollPos <= 0f) || (!isUpSide && m_ScrollHandler.scrollPos >= m_ScrollHandler.scrollPos_Max))
		{
			return false;
		}
		float num = m_ScrollRect.viewport.rect.height - m_BottomSpacing;
		float value = m_ScrollHandler.scrollPos + ((!isUpSide) ? num : (0f - num));
		value = Mathf.Clamp(value, 0f, m_ScrollHandler.scrollPos_Max);
		if (GameGlobalUtil.IsAlmostSame(value, m_ScrollHandler.scrollPos))
		{
			return false;
		}
		m_ScrollHandler.ScrollToTargetPos(0f - value);
		return true;
	}

	private void ChangeOnCursorContent_byScrollPos()
	{
		if (m_OnCursorContent == null)
		{
			return;
		}
		float height = m_ScrollRect.viewport.rect.height;
		float num = 0f - m_ContentContanierRT.offsetMax.y;
		float f = num - height;
		int num2 = Mathf.CeilToInt(num);
		int num3 = Mathf.FloorToInt(f);
		float y = m_OnCursorContent.rectTransform.offsetMax.y;
		float f2 = m_OnCursorContent.rectTransform.offsetMax.y - m_OnCursorContent.rectTransform.rect.height * m_OnCursorContent.rectTransform.localScale.y;
		int num4 = m_Contents.IndexOf(m_OnCursorContent);
		RectTransform rectTransform = null;
		if (Mathf.CeilToInt(f2) < num3)
		{
			int num5 = num4;
			while (num5 > 0)
			{
				num5--;
				rectTransform = m_Contents[num5].rectTransform;
				f2 = rectTransform.offsetMax.y - rectTransform.rect.height * rectTransform.localScale.y;
				if (Mathf.CeilToInt(f2) >= num3)
				{
					SetOnCursorContent(m_Contents[num5], isAdjustScrollPos: false);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
					break;
				}
			}
		}
		else
		{
			if (Mathf.FloorToInt(y) <= num2)
			{
				return;
			}
			int count = m_Contents.Count;
			for (int i = num4 + 1; i < count; i++)
			{
				rectTransform = m_Contents[i].rectTransform;
				y = rectTransform.offsetMax.y;
				if (Mathf.FloorToInt(y) <= num2)
				{
					SetOnCursorContent(m_Contents[i], isAdjustScrollPos: false);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
					break;
				}
			}
		}
	}

	private void AdjustScrollPos_byOnCursonContent()
	{
		if (!(m_OnCursorContent == null))
		{
			float height = m_ScrollRect.viewport.rect.height;
			float num = 0f - m_ContentContanierRT.offsetMax.y;
			float num2 = num - height;
			float y = m_OnCursorContent.rectTransform.offsetMax.y;
			float num3 = m_OnCursorContent.rectTransform.offsetMax.y - m_OnCursorContent.rectTransform.rect.height * m_OnCursorContent.rectTransform.localScale.y;
			num3 -= m_BottomSpacing;
			if (num3 < num2)
			{
				float fTargetPos = num3 + height;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos);
			}
			else if (y > num)
			{
				float fTargetPos2 = y;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos2);
			}
		}
	}

	public void OnClick_ContentScrollToTop()
	{
		if (m_ScrollHandler.isScrollable)
		{
			m_ScrollHandler.ScrollToTargetPos(0f);
			int count = m_Contents.Count;
			if (count > 0)
			{
				SetOnCursorContent(m_Contents[0], isAdjustScrollPos: false);
			}
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Scroll_Page");
			}
		}
	}

	public void OnProc_PlayStopRecord(RecordContentPlus recordContent, bool isPlay)
	{
		if (m_AudioManager == null)
		{
			return;
		}
		if (isPlay)
		{
			GameSwitch instance = GameSwitch.GetInstance();
			if (m_CurPlayingContent != null)
			{
				m_CurPlayingContent.isPlaying = false;
			}
			string strIDrecord = recordContent.xlsData.m_strIDrecord;
			strIDrecord = strIDrecord.Trim();
			if (strIDrecord != string.Empty)
			{
				m_AudioManager.Stop(0);
				bool flag = ContiDataHandler.IsExistConti(strIDrecord);
				if (flag)
				{
					m_CurPlayingContent = recordContent;
					instance.SetRecordFaterConti(strIDrecord);
				}
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetShow(isShow: false);
				}
				m_EventEngine.SetShowSkipBut(isShowSkipBut: true);
				m_EventEngine.SetEventBotGuide(isShow: true);
				m_aniDialogBlackBG.gameObject.SetActive(value: true);
				m_isDialogBlackDisappear = false;
				m_eDialogAniState = GameDefine.eAnimChangeState.none;
				m_isRunEvent = true;
				m_CurPlayingContent.isPlaying = flag;
				m_isInputBlock = true;
				m_TabContainer.isInputBlock = true;
				instance.IgnoreMentalRelationEvt(isSet: true);
				m_EventEngine.EnableRecordFaterEventObj("RUN_RECORD", FinishConti);
				BacklogDataManager.ClearBacklogDatas();
			}
			else
			{
				m_CurPlayingContent = recordContent;
				m_CurPlayingContent.isPlaying = true;
				StartCoroutine(PlaySound(recordContent.xlsData.m_strIDSnd));
			}
			m_isPlayedAnySound = true;
			if (!recordContent.activeNewMark)
			{
				return;
			}
			recordContent.activeNewMark = false;
			if (instance.GetCollSound(recordContent.xlsData.m_iIdx) == 1)
			{
				instance.SetCollSound(recordContent.xlsData.m_iIdx, 2);
				if (m_curCategoryInfo != null)
				{
					m_curCategoryInfo.m_NewContentCount--;
				}
			}
			if (m_curCategoryInfo != null)
			{
				m_curCategoryInfo.ReflashLinkedTabButton();
			}
		}
		else
		{
			if (m_CurPlayingContent != null)
			{
				m_CurPlayingContent.isPlaying = false;
			}
			m_AudioManager.Stop(0);
			m_CurPlayingContent = null;
		}
	}

	private IEnumerator PlaySound(string soundKey)
	{
		m_isLoadingSoundAsset = true;
		yield return StartCoroutine(m_AudioManager.PlayKeyReturnEnumerator(0, soundKey));
		m_isLoadingSoundAsset = false;
	}

	private void FinishConti(object sender, object arg)
	{
		m_EventEngine.SetEventBotGuide(isShow: false);
		m_CurPlayingContent.isPlaying = false;
		m_CurPlayingContent = null;
		m_isRunEvent = false;
		m_isInputBlock = false;
		m_TabContainer.isInputBlock = false;
		EventEngine.GetInstance(isCreate: false)?.SetSkip(isSkip: false);
		GameSwitch instance = GameSwitch.GetInstance();
		instance.SetRecordFaterConti(null);
		instance.IgnoreMentalRelationEvt(isSet: false);
		BacklogDataManager.ClearBacklogDatas();
		InitButtonGuide();
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentEnable(m_buttonGuide_PlayStop, m_OnCursorContent != null && m_OnCursorContent.isValid);
		}
		GameGlobalUtil.PlayUIAnimation(m_aniDialogBlackBG, GameDefine.UIAnimationState.disappear, ref m_eDialogAniState);
		m_isDialogBlackDisappear = true;
	}

	public void TouchClose(bool isEnableAnimation)
	{
		if (!m_isInputBlock && MainLoadThing.instance.IsTouchableState())
		{
			if (m_ChangingTabCover != null && m_TabContainer.isChaningTab)
			{
				m_TabContainer.CancelChangingState();
			}
			else
			{
				Close(isEnableAnimation);
			}
		}
	}

	public void OnClick_Content(RecordContentPlus content)
	{
		if (!m_isInputBlock && m_OnCursorContent != content)
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Select");
			}
			SetOnCursorContent(content);
		}
	}
}
