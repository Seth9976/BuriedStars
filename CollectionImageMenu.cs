using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class CollectionImageMenu : CommonBGChildBase
{
	public GameObject m_RootObject;

	public Text m_TitleText;

	public Text m_TotalCompleteRateTitle;

	public Text m_TotalCompleteRateValue;

	public Text m_CompleteRateTitle;

	public Text m_CompleteRateValue;

	public GameObject m_CompleteRateValueBG;

	public CommonTabContainerPlus m_TabContainer;

	public GameObject m_BackButtonObj;

	public ShowImageOriginSize m_ImageViewer;

	[Header("Contents Container")]
	public ScrollRect m_ScrollRect;

	public RectTransform m_ContentContanierRT;

	public GridLayoutGroup m_ContentGrid;

	public int m_ContentSlotRawCount = 4;

	public float m_BottomSpacing = 80f;

	public GameObject m_ContentSrcObject;

	public GameObject m_ChangingTabCover;

	[Header("Scroll Members")]
	public GameObject m_ScrollbarRoot;

	public Button m_ScrollButtonToFirst;

	public Button m_ScrollButtonRStick;

	public bool m_ScrollLoopEnable = true;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	public float m_ScrollRepeatTime = 0.2f;

	private float m_fScrollButtonPusingTime;

	private Canvas m_Canvas;

	private Dictionary<int, CategoryInfo<Xls.CollImages>> m_CategoryInfos = new Dictionary<int, CategoryInfo<Xls.CollImages>>();

	private CategoryInfo<Xls.CollImages> m_curCategoryInfo;

	private int m_ContentSlotTotalCount;

	private List<CollectionImageContent> m_ContentSlots = new List<CollectionImageContent>();

	private int m_ContentRawCount;

	private List<Xls.CollImages> m_curContentDatas;

	private int m_CurrentBaseContentRaw;

	private int m_OnCursorContentDataIdx = -1;

	private CollectionImageContent m_OnCursorContentSlot;

	private float m_ContentSlotRawHeight;

	private int m_firstSelectCategory = -1;

	private bool m_isInputBlock;

	private Animator m_CloseAnimator;

	private LoadingSWatchIcon m_LoadingIcon;

	private const int c_AudioCannel = 5;

	private AudioManager m_AudioManager;

	private CommonButtonGuide m_ButtonGuide;

	private bool m_isInitializedButtonGuidText;

	private string m_buttonGuide_SelContent;

	private string m_buttonGuide_ExitMenu;

	private string m_buttonGuide_ImageView;

	private string m_buttonGuide_ScrollToTop;

	private string m_buttonGuide_ChangeTab;

	private string m_buttonGuide_SelectTab;

	private string m_buttonGuide_CancelTab;

	private AssetBundleObjectHandler m_showingImageAssetObjHdr;

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
		m_ContentSlotTotalCount = m_ContentGrid.constraintCount * m_ContentSlotRawCount;
		m_ContentSrcObject.SetActive(value: false);
		m_Canvas = base.gameObject.GetComponentInChildren<Canvas>();
	}

	private void OnDestroy()
	{
		if (m_ScrollHandler != null)
		{
			m_ScrollHandler.ReleaseScroll();
		}
		if (m_CategoryInfos != null)
		{
			m_CategoryInfos.Clear();
		}
		if (m_ContentSlots != null)
		{
			m_ContentSlots.Clear();
		}
		if (m_curContentDatas != null)
		{
			m_curContentDatas.Clear();
		}
		m_ButtonGuide = null;
		m_AudioManager = null;
		m_LoadingIcon = null;
		m_CloseAnimator = null;
		m_OnCursorContentSlot = null;
		m_curCategoryInfo = null;
		m_Canvas = null;
	}

	private void Update()
	{
		if (m_CloseAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_CloseAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				CallClosedCallback();
			}
			return;
		}
		if (m_ScrollHandler.IsScrolling)
		{
			m_ScrollHandler.Update();
			AlignContentSlot_byScrollPos(m_ScrollHandler.scrollPos);
			bool flag = true;
			if (m_ScrollHandler.CurScrollEvent == UIUtil_ScrollHandler.ScrollEvent.None && m_OnCursorContentDataIdx >= 0 && m_OnCursorContentDataIdx < m_curContentDatas.Count)
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
				return;
			}
			num = GamePadInput.GetAxisValue(PadInput.GameInput.RStickY);
			if (!GameGlobalUtil.IsAlmostSame(num, 0f))
			{
				m_ScrollHandler.ScrollByDirection(num > 0f, Mathf.Abs(num));
				AlignContentSlot_byScrollPos(m_ScrollHandler.scrollPos);
				ChangeOnCursorContent_byScrollPos();
				return;
			}
			num = 0f - GamePadInput.GetMouseWheelScrollDelta();
			if (num > 0.9f)
			{
				ChangeScrollPage(isUpSide: false);
			}
			else if (num < -0.9f)
			{
				ChangeScrollPage(isUpSide: true);
			}
		}
		float fAxisX = 0f;
		float fAxisY = 0f;
		if (GamePadInput.GetLStickMove(out fAxisX, out fAxisY))
		{
			int deltaX = 0;
			int deltaY = 0;
			if (Mathf.Abs(fAxisX) >= Mathf.Abs(fAxisY))
			{
				if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
				{
					deltaX = -1;
					m_fScrollButtonPusingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
				{
					deltaX = 1;
					m_fScrollButtonPusingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Pushing))
				{
					if (IsOverScrollButtonPushingTime())
					{
						deltaX = -1;
					}
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
				{
					deltaX = 1;
				}
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				deltaY = -1;
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				deltaY = 1;
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				if (IsOverScrollButtonPushingTime())
				{
					deltaY = -1;
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
			{
				deltaY = 1;
			}
			if (ChangeOnCursorContent(deltaX, deltaY))
			{
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Select");
				}
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetContentActivate(m_buttonGuide_SelContent, isActivate: true);
				}
			}
		}
		if (m_OnCursorContentSlot != null && GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
		{
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_ImageView, isActivate: true);
			}
			StartCoroutine(ShowImage(m_OnCursorContentSlot));
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
		InitCategoryInfo();
		InitTextMemebers();
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
		m_ScrollHandler.ResetScrollRange();
		if (m_TabContainer != null)
		{
			m_TabContainer.BuildTabButtonObjects();
			m_TabContainer.isInputBlock = true;
			m_firstSelectCategory = 0;
			Invoke("SetFirstSelectTab", 0.1f);
		}
		MatchCategoryInfoToTabButton();
		m_isInputBlock = true;
		m_CloseAnimator = null;
		m_curContentDatas = null;
		m_CurrentBaseContentRaw = 0;
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.appear.ToString());
		InitButtonGuide();
	}

	public void InitTextMemebers()
	{
		Text[] textComps = new Text[5] { m_TitleText, m_TotalCompleteRateTitle, m_TotalCompleteRateValue, m_CompleteRateTitle, m_CompleteRateValue };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		if (m_TitleText != null)
		{
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_TITLE");
		}
		if (m_TotalCompleteRateTitle != null)
		{
			m_TotalCompleteRateTitle.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_COMPLETE_RATE");
		}
	}

	private void InitCategoryInfo()
	{
		m_CategoryInfos.Clear();
		CategoryInfo<Xls.CollImages> value = null;
		int[] array = new int[4] { 0, 1, 3, 4 };
		int[] array2 = array;
		foreach (int num in array2)
		{
			if (!m_CategoryInfos.ContainsKey(num))
			{
				string categoryName = string.Empty;
				switch (num)
				{
				case 0:
					categoryName = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_TAB1");
					break;
				case 1:
					categoryName = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_TAB2");
					break;
				case 3:
					categoryName = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_TAB4");
					break;
				case 4:
					categoryName = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_TAB5");
					break;
				}
				value = new CategoryInfo<Xls.CollImages>(num, categoryName);
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
		foreach (Xls.CollImages data in Xls.CollImages.datas)
		{
			value = null;
			if (m_CategoryInfos.TryGetValue(data.m_iCategory, out value) && value != null)
			{
				value.m_xlsDatas.Add(data);
				b = instance.GetCollImage(data.m_iIdx);
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
			value.m_xlsDatas = Xls.CollImages.datas;
			value.m_ValidContentCount = num3;
			value.m_NewContentCount = num2;
		}
		Dictionary<int, CategoryInfo<Xls.CollImages>>.Enumerator enumerator2 = m_CategoryInfos.GetEnumerator();
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
		Dictionary<int, CategoryInfo<Xls.CollImages>>.Enumerator enumerator = m_CategoryInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CategoryInfo<Xls.CollImages> value = enumerator.Current.Value;
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
		SetOnCursorContentDataIdx(-1);
		m_CloseAnimator = null;
		m_TabContainer.ClearTabButtonObjects();
		ClearContentSlots();
		base.gameObject.SetActive(value: false);
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
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
		m_buttonGuide_SelContent = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_BTNGUIDE_SEL_CONTENT");
		m_buttonGuide_ExitMenu = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_BTNGUIDE_EXIT_MENU");
		m_buttonGuide_ImageView = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_BTNGUIDE_VIEW");
		m_buttonGuide_ScrollToTop = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_BTNGUIDE_SCROLL_TO_TOP");
		m_buttonGuide_ChangeTab = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_BTNGUIDE_CHANGE_TAB");
		m_buttonGuide_SelectTab = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_BTNGUIDE_SEL_TAB");
		m_buttonGuide_CancelTab = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_BTNGUIDE_CANCEL_TAB");
		m_isInitializedButtonGuidText = true;
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuide_SelContent, PadInput.GameInput.LStickY, isIngoreAxis: true);
		m_ButtonGuide.AddContent(m_buttonGuide_ImageView, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ExitMenu, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ScrollToTop, PadInput.GameInput.TriangleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_SelectTab, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_CancelTab, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.L1Button);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.R1Button);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Center);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ImageView, isEnable: false, isNeedAlign: false);
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
			if (current != null && current.tag != null && current.tag is CategoryInfo<Xls.CollImages>)
			{
				CategoryInfo<Xls.CollImages> categoryInfo = current.tag as CategoryInfo<Xls.CollImages>;
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
		Dictionary<int, CategoryInfo<Xls.CollImages>>.Enumerator enumerator = m_CategoryInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CategoryInfo<Xls.CollImages> value = enumerator.Current.Value;
			if (value != null)
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
			CategoryInfo<Xls.CollImages> categoryInfo = tabButtonInfo.tag as CategoryInfo<Xls.CollImages>;
			m_isInputBlock = true;
			m_TabContainer.isInputBlock = true;
			m_ScrollRect.velocity = Vector2.zero;
			if (m_ContentSlots == null || m_ContentSlots.Count != m_ContentSlotTotalCount)
			{
				StartCoroutine(CreateContentSlots(categoryInfo));
			}
			else
			{
				StartCoroutine(SetCurrentCategory(categoryInfo));
			}
			if (m_ChangingTabCover != null)
			{
				m_ChangingTabCover.SetActive(value: false);
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
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ImageView, !flag && m_OnCursorContentSlot != null && m_OnCursorContentSlot.isValid);
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

	private IEnumerator CreateContentSlots(CategoryInfo<Xls.CollImages> categoryInfo)
	{
		ClearContentSlots();
		if (m_ContentSlotTotalCount > 0 && !(m_ContentSrcObject == null))
		{
			if (m_LoadingIcon != null)
			{
				m_LoadingIcon.gameObject.SetActive(value: true);
			}
			else
			{
				m_LoadingIcon = LoadingSWatchIcon.Create(m_RootObject);
			}
			CollectionImageContent contentSlot = null;
			for (int i = 0; i < m_ContentSlotTotalCount; i++)
			{
				GameObject slotObject = Object.Instantiate(m_ContentSrcObject);
				contentSlot = slotObject.GetComponent<CollectionImageContent>();
				contentSlot.rectTransform.SetParent(m_ContentContanierRT, worldPositionStays: false);
				m_ContentSlots.Add(contentSlot);
				slotObject.SetActive(value: false);
				yield return null;
			}
			List<CollectionImageContent>.Enumerator enumerator = m_ContentSlots.GetEnumerator();
			while (enumerator.MoveNext())
			{
				contentSlot = enumerator.Current;
				if (!(contentSlot == null))
				{
					contentSlot.hide = true;
					contentSlot.gameObject.SetActive(value: true);
				}
			}
			yield return null;
			m_ContentGrid.enabled = false;
			m_ContentGrid.enabled = true;
			yield return null;
			CollectionImageContent slotBase = m_ContentSlots[0];
			CollectionImageContent slotNextRaw = m_ContentSlots[m_ContentGrid.constraintCount];
			m_ContentSlotRawHeight = slotBase.rectTransform.offsetMin.y - slotNextRaw.rectTransform.offsetMin.y;
		}
		yield return StartCoroutine(SetCurrentCategory(categoryInfo, isIgnoreSlotAni: false));
		if (m_LoadingIcon != null)
		{
			Object.Destroy(m_LoadingIcon.gameObject);
			m_LoadingIcon = null;
		}
	}

	private void ClearContentSlots()
	{
		CollectionImageContent collectionImageContent = null;
		List<CollectionImageContent>.Enumerator enumerator = m_ContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			collectionImageContent = enumerator.Current;
			if (!(collectionImageContent == null))
			{
				Object.Destroy(collectionImageContent.gameObject);
			}
		}
		m_ContentSlots.Clear();
	}

	private IEnumerator SetCurrentCategory(CategoryInfo<Xls.CollImages> categoryInfo, bool isIgnoreSlotAni = true)
	{
		SetOnCursorContentDataIdx(-1);
		m_curCategoryInfo = categoryInfo;
		if (m_LoadingIcon != null && !m_LoadingIcon.gameObject.activeSelf)
		{
			m_LoadingIcon.gameObject.SetActive(value: true);
		}
		else if (m_LoadingIcon == null)
		{
			m_LoadingIcon = LoadingSWatchIcon.Create(m_RootObject);
		}
		m_ContentRawCount = 0;
		m_curContentDatas = null;
		int num = 0;
		if (categoryInfo != null)
		{
			m_curContentDatas = categoryInfo.m_xlsDatas;
			num = m_curContentDatas.Count;
			if (num > 0)
			{
				m_ContentRawCount = num / m_ContentGrid.constraintCount + ((num % m_ContentGrid.constraintCount > 0) ? 1 : 0);
			}
		}
		float a = (float)m_ContentRawCount * m_ContentSlotRawHeight + m_BottomSpacing;
		a = Mathf.Max(a, m_ScrollRect.viewport.rect.height);
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, a);
		m_ScrollHandler.ResetScrollRange();
		m_ScrollHandler.scrollPos = 0f;
		m_ContentGrid.padding.top = 0;
		m_ContentGrid.enabled = false;
		m_ContentGrid.enabled = true;
		SetCurrentBaseContentRaw(0);
		SetOnCursorContentDataIdx(0, isAdjustScrollPos: false, isIgnoreSlotAni);
		int num2 = 0;
		float num3 = 0f;
		Color color = GameGlobalUtil.HexToColor(0);
		if (num > 0)
		{
			GameSwitch instance = GameSwitch.GetInstance();
			sbyte b = 1;
			sbyte b2 = 2;
			List<Xls.CollImages>.Enumerator enumerator = m_curContentDatas.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Xls.CollImages current = enumerator.Current;
				sbyte collImage = instance.GetCollImage(current.m_iIdx);
				num2 += ((collImage == b || collImage == b2) ? 1 : 0);
			}
		}
		if (num2 >= num)
		{
			num3 = 100f;
			color = GameGlobalUtil.HexToColor(2078092);
		}
		else if (num2 > 0)
		{
			num3 = (float)(num2 * 100) / (float)num;
			if (num3 < 1f)
			{
				num3 = 1f;
			}
		}
		if (m_CompleteRateTitle != null)
		{
			m_CompleteRateTitle.text = ((categoryInfo == null) ? string.Empty : categoryInfo.Name);
			m_CompleteRateTitle.gameObject.SetActive(value: true);
		}
		if (m_CompleteRateValue != null)
		{
			m_CompleteRateValue.text = $"{((int)num3/*cast due to .constrained prefix*/).ToString()}%";
			m_CompleteRateValue.color = color;
			m_CompleteRateValue.gameObject.SetActive(value: true);
		}
		if (m_CompleteRateValueBG != null)
		{
			m_CompleteRateValueBG.gameObject.SetActive(value: true);
		}
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, m_ScrollHandler.isScrollable);
		m_isInputBlock = false;
		m_TabContainer.isInputBlock = false;
		if (m_LoadingIcon != null)
		{
			Object.Destroy(m_LoadingIcon.gameObject);
			m_LoadingIcon = null;
		}
		yield break;
	}

	private void SetCurrentBaseContentRaw(int rawIdx)
	{
		if (m_curContentDatas == null || m_curContentDatas.Count <= 0)
		{
			return;
		}
		int num = rawIdx * m_ContentGrid.constraintCount;
		int count = m_curContentDatas.Count;
		int i = 0;
		CollectionImageContent collectionImageContent = null;
		sbyte b = 0;
		sbyte b2 = 1;
		sbyte b3 = 2;
		GameSwitch instance = GameSwitch.GetInstance();
		bool flag = false;
		bool flag2 = false;
		Xls.CollImages collImages = null;
		while (i < m_ContentSlotTotalCount && num < count)
		{
			collectionImageContent = m_ContentSlots[i];
			if (!(collectionImageContent == null))
			{
				collImages = m_curContentDatas[num];
				b = instance.GetCollImage(collImages.m_iIdx);
				flag2 = b == b2;
				flag = flag2 || b == b3;
				collectionImageContent.SetXlsData(collImages, this, flag);
				collectionImageContent.enableNewTag = flag2;
				collectionImageContent.hide = false;
			}
			i++;
			num++;
		}
		for (; i < m_ContentSlotTotalCount; i++)
		{
			collectionImageContent = m_ContentSlots[i];
			if (!(collectionImageContent == null))
			{
				collectionImageContent.SetXlsData(null, null, _isValid: false);
				collectionImageContent.enableNewTag = false;
				collectionImageContent.hide = true;
			}
		}
		m_CurrentBaseContentRaw = rawIdx;
	}

	private void AlignContentSlot_byScrollPos(float scrollPos)
	{
		int num = (int)(scrollPos / m_ContentSlotRawHeight);
		if (m_CurrentBaseContentRaw != num)
		{
			SetCurrentBaseContentRaw(num);
		}
		m_ContentGrid.padding.top = num * (int)m_ContentSlotRawHeight;
		m_ContentGrid.enabled = false;
		m_ContentGrid.enabled = true;
		CollectionImageContent contentSlot_byDataIdx = GetContentSlot_byDataIdx(m_OnCursorContentDataIdx);
		if (contentSlot_byDataIdx != m_OnCursorContentSlot)
		{
			SetOnCursorContentSlot(contentSlot_byDataIdx);
		}
	}

	private CollectionImageContent GetContentSlot_byXlsData(Xls.CollImages xlsData)
	{
		if (xlsData == null)
		{
			return null;
		}
		CollectionImageContent collectionImageContent = null;
		List<CollectionImageContent>.Enumerator enumerator = m_ContentSlots.GetEnumerator();
		while (enumerator.MoveNext())
		{
			collectionImageContent = enumerator.Current;
			if (collectionImageContent == null || collectionImageContent.hide || collectionImageContent.xlsData != xlsData)
			{
				continue;
			}
			return collectionImageContent;
		}
		return null;
	}

	private CollectionImageContent GetContentSlot_byDataIdx(int idx)
	{
		if (m_curContentDatas == null || idx < 0 || idx >= m_curContentDatas.Count)
		{
			return null;
		}
		return GetContentSlot_byXlsData(m_curContentDatas[idx]);
	}

	private void SetOnCursorContentDataIdx(int idx, bool isAdjustScrollPos = true, bool isIngoreAnim = true)
	{
		int onCursorContentDataIdx = m_OnCursorContentDataIdx;
		Xls.CollImages collImages = null;
		if (idx < 0 || idx >= m_curContentDatas.Count)
		{
			m_OnCursorContentDataIdx = -1;
		}
		else
		{
			m_OnCursorContentDataIdx = idx;
			collImages = m_curContentDatas[idx];
		}
		if (onCursorContentDataIdx != m_OnCursorContentDataIdx)
		{
			Xls.CollImages collImages2 = ((onCursorContentDataIdx < 0 || onCursorContentDataIdx >= m_curContentDatas.Count) ? null : m_curContentDatas[onCursorContentDataIdx]);
			if (collImages2 != null)
			{
				GameSwitch instance = GameSwitch.GetInstance();
				if (instance.GetCollImage(collImages2.m_iIdx) == 1)
				{
					instance.SetCollImage(collImages2.m_iIdx, 2);
					if (m_curCategoryInfo != null)
					{
						m_curCategoryInfo.m_NewContentCount--;
						m_curCategoryInfo.ReflashLinkedTabButton();
					}
					CollectionImageContent contentSlot_byXlsData = GetContentSlot_byXlsData(collImages2);
					if (contentSlot_byXlsData != null)
					{
						contentSlot_byXlsData.enableNewTag = false;
					}
				}
			}
		}
		CollectionImageContent content = ((collImages == null) ? null : GetContentSlot_byXlsData(collImages));
		SetOnCursorContentSlot(content, isIngoreAnim);
		if (isAdjustScrollPos)
		{
			AdjustScrollPos_byOnCursonContent();
		}
	}

	private void SetOnCursorContentSlot(CollectionImageContent content, bool isIngoreAnim = true)
	{
		if (m_OnCursorContentSlot == content)
		{
			return;
		}
		string strMot = GameDefine.UIAnimationState.idle.ToString();
		if (m_OnCursorContentSlot != null)
		{
			m_OnCursorContentSlot.select = false;
			if (isIngoreAnim)
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_OnCursorContentSlot.gameObject, strMot);
			}
		}
		if (content != null)
		{
			content.select = true;
			if (isIngoreAnim)
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(content.gameObject, strMot);
			}
		}
		m_OnCursorContentSlot = content;
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ImageView, m_OnCursorContentSlot != null && m_OnCursorContentSlot.isValid);
	}

	private bool ChangeOnCursorContent(int deltaX, int deltaY, bool isAdjustScrollPos = true)
	{
		if (deltaX == 0 && deltaY == 0)
		{
			return false;
		}
		int count = m_curContentDatas.Count;
		if (m_OnCursorContentDataIdx < 0)
		{
			if (count > 0)
			{
				SetOnCursorContentDataIdx(0, isAdjustScrollPos: false);
			}
			return false;
		}
		int constraintCount = m_ContentGrid.constraintCount;
		int num = count / constraintCount + ((count % constraintCount > 0) ? 1 : 0);
		int onCursorContentDataIdx = m_OnCursorContentDataIdx;
		int num2 = onCursorContentDataIdx;
		int num3 = onCursorContentDataIdx % constraintCount;
		int num4 = onCursorContentDataIdx / constraintCount;
		if (deltaX != 0)
		{
			int num5 = num4 * constraintCount;
			int a = num5 + constraintCount;
			a = Mathf.Min(a, count);
			num2 = onCursorContentDataIdx + deltaX;
			if (num2 < num5)
			{
				num2 = a - 1;
			}
			else if (num2 >= a)
			{
				num2 = num5;
			}
		}
		else if (deltaY != 0)
		{
			int num6 = (num - 1) * constraintCount + num3;
			int num7 = ((num6 >= count) ? (num - 1) : num);
			num4 += deltaY;
			if (num4 < 0)
			{
				num4 = num7 - 1;
			}
			else if (num4 >= num7)
			{
				num4 = 0;
			}
			num2 = num4 * constraintCount + num3;
		}
		num2 = Mathf.Clamp(num2, 0, count - 1);
		if (num2 == onCursorContentDataIdx)
		{
			return false;
		}
		SetOnCursorContentDataIdx(num2, isAdjustScrollPos);
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
		if (m_OnCursorContentDataIdx < 0)
		{
			return;
		}
		float height = m_ScrollRect.viewport.rect.height;
		float num = 0f - m_ContentContanierRT.offsetMax.y;
		float f = num - height;
		int num2 = Mathf.CeilToInt(num);
		int num3 = Mathf.FloorToInt(f);
		int num4 = m_OnCursorContentDataIdx / m_ContentGrid.constraintCount;
		float num5 = 0f - (float)num4 * m_ContentSlotRawHeight;
		float f2 = num5 - m_ContentSlotRawHeight;
		if (Mathf.CeilToInt(num5) < num3)
		{
			int num6 = m_OnCursorContentDataIdx;
			float num7 = 0f;
			int num8 = num4;
			while (num8 > 0)
			{
				num8--;
				num6 = Mathf.Max(num6 - m_ContentGrid.constraintCount, 0);
				num7 = 0f - (float)num8 * m_ContentSlotRawHeight;
				if (Mathf.CeilToInt(num7) >= num3)
				{
					SetOnCursorContentDataIdx(num6, isAdjustScrollPos: false);
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
			if (Mathf.FloorToInt(f2) <= num2)
			{
				return;
			}
			int num9 = m_OnCursorContentDataIdx;
			float num10 = 0f;
			for (int i = num4 + 1; i < m_ContentRawCount; i++)
			{
				num9 = Mathf.Min(num9 + m_ContentGrid.constraintCount, m_curContentDatas.Count - 1);
				num10 = 0f - (float)i * m_ContentSlotRawHeight - m_ContentSlotRawHeight;
				if (Mathf.FloorToInt(num10) <= num2)
				{
					SetOnCursorContentDataIdx(num9, isAdjustScrollPos: false);
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
		if (m_OnCursorContentDataIdx >= 0)
		{
			float height = m_ScrollRect.viewport.rect.height;
			float num = 0f - m_ContentContanierRT.offsetMax.y;
			float num2 = num - height;
			int num3 = m_OnCursorContentDataIdx / m_ContentGrid.constraintCount;
			float num4 = 0f - (float)num3 * m_ContentSlotRawHeight;
			float num5 = num4 - m_ContentSlotRawHeight;
			num5 -= m_BottomSpacing;
			if (num5 < num2)
			{
				float fTargetPos = num5 + height;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos);
			}
			else if (num4 > num)
			{
				float fTargetPos2 = num4;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos2);
			}
		}
	}

	public void OnClick_ContentScrollToTop()
	{
		if (m_ScrollHandler.isScrollable)
		{
			m_ScrollHandler.ScrollToTargetPos(0f);
			SetOnCursorContentDataIdx(0, isAdjustScrollPos: false);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Scroll_Page");
			}
		}
	}

	public void OnClickButton_FromContent(CollectionImageContent content)
	{
		if (!(content == null) && content.xlsData != null)
		{
			int num = m_curContentDatas.IndexOf(content.xlsData);
			if (num >= 0)
			{
				SetOnCursorContentDataIdx(num);
				StartCoroutine(ShowImage(content));
			}
		}
	}

	public IEnumerator ShowImage(CollectionImageContent content)
	{
		if (content == null)
		{
			yield break;
		}
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound((!content.isValid) ? "Menu_Select" : "Menu_OK");
		}
		if (!string.IsNullOrEmpty(content.xlsData.m_strIDColImageDest))
		{
			ImageDetailViewer imageDetailViewer = MainLoadThing.instance.imageDetailViewer;
			if (imageDetailViewer == null || !content.isValid)
			{
				yield break;
			}
			m_isInputBlock = true;
			m_TabContainer.isInputBlock = true;
			imageDetailViewer.SetCanvasOrder((!(m_Canvas != null)) ? 100000 : (m_Canvas.sortingOrder + 1));
			yield return StartCoroutine(imageDetailViewer.ShowImage(content.xlsData, ClosedImageViewer));
		}
		else
		{
			if (m_ImageViewer == null || !content.isValid)
			{
				yield break;
			}
			Xls.ImageFile xlsImageFile = Xls.ImageFile.GetData_byKey(content.xlsData.m_strIDImg);
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
				ClosedImageViewer(null, null);
				yield break;
			}
			m_ImageViewer.gameObject.SetActive(value: true);
			m_ImageViewer.ShowImage(isShow: true, spr, ClosedImageViewer);
			if (m_BackButtonObj != null)
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackButtonObj, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear));
			}
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
		if (!content.enableNewTag)
		{
			yield break;
		}
		content.enableNewTag = false;
		GameSwitch instance = GameSwitch.GetInstance();
		if (instance.GetCollImage(content.xlsData.m_iIdx) == 1)
		{
			instance.SetCollImage(content.xlsData.m_iIdx, 2);
			if (m_curCategoryInfo != null)
			{
				m_curCategoryInfo.m_NewContentCount--;
				m_curCategoryInfo.ReflashLinkedTabButton();
			}
		}
	}

	private void ClosedImageViewer(object sender, object arg)
	{
		m_isInputBlock = false;
		m_TabContainer.isInputBlock = false;
		if (m_ImageViewer != null)
		{
			m_ImageViewer.gameObject.SetActive(value: false);
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: true);
		}
		if (m_showingImageAssetObjHdr != null)
		{
			m_showingImageAssetObjHdr.UnloadAssetBundle();
			m_showingImageAssetObjHdr = null;
		}
		if (sender != null && sender is ShowImageOriginSize && m_BackButtonObj != null)
		{
			m_BackButtonObj.SetActive(value: true);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackButtonObj, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.appear));
		}
	}

	public void TouchClose()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			if (m_ImageViewer.gameObject.activeInHierarchy)
			{
				m_ImageViewer.OnClickScreen();
			}
			else
			{
				Close();
			}
		}
	}
}
