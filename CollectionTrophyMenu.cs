using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class CollectionTrophyMenu : CommonBGChildBase
{
	public enum Category
	{
		All = -1,
		Bronze,
		Silver,
		Gold,
		Platinum
	}

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

	[Header("Scroll Members")]
	public GameObject m_ScrollbarRoot;

	public Button m_ScrollButtonToFirst;

	public Button m_ScrollButtonRStick;

	public bool m_ScrollLoopEnable = true;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	public float m_ScrollRepeatTime = 0.2f;

	private float m_fScrollButtonPusingTime;

	private Dictionary<int, CategoryInfo<Xls.Trophys>> m_CategoryInfos = new Dictionary<int, CategoryInfo<Xls.Trophys>>();

	private CategoryInfo<Xls.Trophys> m_curCategoryInfo;

	private List<ToDoSlotPlus> m_Contents = new List<ToDoSlotPlus>();

	private ToDoSlotPlus m_OnCursorContent;

	private int m_firstSelectCategory = -1;

	private bool m_isInputBlock;

	private bool m_isPageScrolling;

	private Animator m_CloseAnimator;

	private LoadingSWatchIcon m_LoadingIcon;

	private AudioManager m_AudioManager;

	private CommonButtonGuide m_ButtonGuide;

	private bool m_isInitializedButtonGuidText;

	private string m_buttonGuide_SelContent;

	private string m_buttonGuide_ExitMenu;

	private string m_buttonGuide_ScrollToTop;

	private string m_buttonGuide_ChangeTab;

	private string m_buttonGuide_SelectTab;

	private string m_buttonGuide_CancelTab;

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
		if (m_ContentSrcObject != null)
		{
			m_ContentSrcObject.SetActive(value: false);
		}
		m_ContentSrcObject.SetActive(value: false);
	}

	private void OnDisable()
	{
		if (m_LoadingIcon != null)
		{
			Object.Destroy(m_LoadingIcon.gameObject);
			m_LoadingIcon = null;
		}
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
		m_curCategoryInfo = null;
		if (m_Contents != null)
		{
			m_Contents.Clear();
		}
		m_OnCursorContent = null;
		m_CloseAnimator = null;
		m_LoadingIcon = null;
		m_AudioManager = null;
		m_ButtonGuide = null;
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
		if (!m_isInputBlock && !m_TabContainer.isChaningTab && !ButtonPadInput.IsPlayingButPressAnim() && !PopupDialoguePlus.IsAnyPopupActivated())
		{
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
			if (!m_ScrollHandler.IsScrolling)
			{
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
				if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
				{
					if (m_ButtonGuide != null)
					{
						m_ButtonGuide.SetContentActivate(m_buttonGuide_ExitMenu, isActivate: true);
					}
					Close();
				}
			}
		}
		m_ScrollHandler.Update();
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
			m_TabContainer.isInputBlock = false;
			m_firstSelectCategory = 0;
			Invoke("SetFirstSelectTab", 0.1f);
		}
		MatchCategoryInfoToTabButton();
		m_isInputBlock = false;
		m_CloseAnimator = null;
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.appear.ToString());
		InitButtonGuide();
	}

	public void InitTextMemebers()
	{
		Text[] textComps = new Text[5] { m_TitleText, m_TotalCompleteRateTitle, m_TotalCompleteRateValue, m_CompleteRateTitle, m_CompleteRateValue };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		ToDoSlotPlus.InitStaticTextMembers();
		if (m_TitleText != null)
		{
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_TITLE");
		}
		if (m_TotalCompleteRateTitle != null)
		{
			m_TotalCompleteRateTitle.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_COMPLETE_RATE");
		}
	}

	private void InitCategoryInfo()
	{
		m_CategoryInfos.Clear();
		CategoryInfo<Xls.Trophys> value = null;
		int[] array = new int[5] { -1, 0, 1, 2, 3 };
		int[] array2 = array;
		foreach (int num in array2)
		{
			if (!m_CategoryInfos.ContainsKey(num))
			{
				string categoryName = string.Empty;
				switch (num)
				{
				case -1:
					categoryName = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_TAB_ALL");
					break;
				case 0:
					categoryName = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_TAB1");
					break;
				case 1:
					categoryName = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_TAB2");
					break;
				case 2:
					categoryName = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_TAB3");
					break;
				case 3:
					categoryName = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_TAB4");
					break;
				}
				value = new CategoryInfo<Xls.Trophys>(num, categoryName);
				m_CategoryInfos.Add(num, value);
			}
		}
		if (m_CategoryInfos.Count <= 0)
		{
			return;
		}
		GameSwitch instance = GameSwitch.GetInstance();
		int num2 = 0;
		bool flag = false;
		bool flag2 = false;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		foreach (Xls.Trophys data in Xls.Trophys.datas)
		{
			value = null;
			if (m_CategoryInfos.TryGetValue(data.m_iCategory, out value) && value != null)
			{
				value.m_xlsDatas.Add(data);
				num2 = instance.GetTrophyCnt(data.m_iIndex);
				flag = instance.GetTrophyNew(data.m_iIndex) == 1;
				flag2 = instance.GetTrophyComplete(data.m_iIndex);
				value.m_ValidContentCount += (flag2 ? 1 : 0);
				value.m_NewContentCount += (flag ? 1 : 0);
				num3 += (flag ? 1 : 0);
				num4 += (flag2 ? 1 : 0);
				num5++;
			}
		}
		if (m_CategoryInfos.TryGetValue(-1, out value) && value != null)
		{
			value.m_xlsDatas = Xls.Trophys.datas;
			value.m_ValidContentCount = num4;
			value.m_NewContentCount = num3;
		}
		Dictionary<int, CategoryInfo<Xls.Trophys>>.Enumerator enumerator2 = m_CategoryInfos.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			value = enumerator2.Current.Value;
			if (value != null && value.m_xlsDatas != null && value.m_xlsDatas.Count > 0)
			{
				float num6 = value.m_xlsDatas.Count;
				float num7 = value.m_ValidContentCount;
				value.m_CompleteRate = num7 * 100f / num6;
				if (value.m_CompleteRate < 1f && num7 > 1f)
				{
					value.m_CompleteRate = 1f;
				}
			}
		}
		if (m_TotalCompleteRateValue != null)
		{
			float num8 = ((num4 >= num5) ? 100f : ((num5 <= 0) ? 0f : ((float)num4 * 100f / (float)num5)));
			if (num8 < 1f && num4 > 0)
			{
				num8 = 1f;
			}
			m_TotalCompleteRateValue.text = $"{(int)num8}%";
		}
	}

	private void MatchCategoryInfoToTabButton()
	{
		Dictionary<int, CategoryInfo<Xls.Trophys>>.Enumerator enumerator = m_CategoryInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CategoryInfo<Xls.Trophys> value = enumerator.Current.Value;
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
		SetOnCursorContent(null, isAdjustScrollPos: false);
		m_CloseAnimator = null;
		m_TabContainer.ClearTabButtonObjects();
		ClearContents();
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
		m_buttonGuide_SelContent = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_BTNGUIDE_SEL_CONTENT");
		m_buttonGuide_ExitMenu = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_BTNGUIDE_EXIT_MENU");
		m_buttonGuide_ScrollToTop = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_BTNGUIDE_SCROLL_TO_TOP");
		m_buttonGuide_ChangeTab = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_BTNGUIDE_CHANGE_TAB");
		m_buttonGuide_SelectTab = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_BTNGUIDE_SEL_TAB");
		m_buttonGuide_CancelTab = GameGlobalUtil.GetXlsProgramText("COLLECTION_TROPHY_MENU_BTNGUIDE_CANCEL_TAB");
		m_isInitializedButtonGuidText = true;
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuide_SelContent, PadInput.GameInput.LStickY);
		m_ButtonGuide.AddContent(m_buttonGuide_ExitMenu, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ScrollToTop, PadInput.GameInput.TriangleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_SelectTab, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_CancelTab, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.L1Button);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.R1Button);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Center);
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
			if (current != null && current.tag != null && current.tag is CategoryInfo<Xls.Trophys>)
			{
				CategoryInfo<Xls.Trophys> categoryInfo = current.tag as CategoryInfo<Xls.Trophys>;
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
		Dictionary<int, CategoryInfo<Xls.Trophys>>.Enumerator enumerator = m_CategoryInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CategoryInfo<Xls.Trophys> value = enumerator.Current.Value;
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
			StartCoroutine(InitContents(tabButtonInfo.tag as CategoryInfo<Xls.Trophys>));
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

	private IEnumerator InitContents(CategoryInfo<Xls.Trophys> categoryInfo)
	{
		ClearContents();
		m_curCategoryInfo = categoryInfo;
		float fContanierSize = 0f;
		float completeRate = 0f;
		Color colorCompleteRate = GameGlobalUtil.HexToColor(0);
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
		if (m_curCategoryInfo != null)
		{
			List<Xls.Trophys> xlsDatas = m_curCategoryInfo.m_xlsDatas;
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
				int getCount = 0;
				bool isCompleteContent = false;
				bool isEnableNewTag = false;
				string categoryName = null;
				Xls.Trophys xlsData = null;
				ToDoSlotPlus contentComp = null;
				GameSwitch gameSwitch = GameSwitch.GetInstance();
				int completeCount = 0;
				for (int i = 0; i < dataCount; i++)
				{
					yield return null;
					xlsData = xlsDatas[i];
					getCount = gameSwitch.GetTrophyCnt(xlsData.m_iIndex);
					isEnableNewTag = gameSwitch.GetTrophyNew(xlsData.m_iIndex) == 1;
					isCompleteContent = getCount >= xlsData.m_iMax;
					categoryName = null;
					if (categoryInfo.ID == -1)
					{
						CategoryInfo<Xls.Trophys> value = null;
						if (m_CategoryInfos.TryGetValue(xlsData.m_iCategory, out value) && value != null)
						{
							categoryName = value.Name;
						}
					}
					GameObject contentObject = Object.Instantiate(m_ContentSrcObject);
					contentComp = contentObject.GetComponent<ToDoSlotPlus>();
					contentComp.InitCollectionTrophyContent(xlsData, getCount, isEnableNewTag, categoryName, this);
					m_Contents.Add(contentComp);
					contentComp.rectTransform.SetParent(m_ContentContanierRT, worldPositionStays: false);
					contentObject.SetActive(value: false);
					completeCount += (isCompleteContent ? 1 : 0);
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
				if (dataCount > 0)
				{
					SetOnCursorContent(m_Contents[0]);
				}
				m_LoadingIcon.gameObject.SetActive(value: false);
				if (completeCount >= dataCount)
				{
					completeRate = 100f;
					colorCompleteRate = GameGlobalUtil.HexToColor(2078092);
				}
				else if (completeCount > 0)
				{
					completeRate = (float)(completeCount * 100) / (float)dataCount;
					if (completeRate < 1f)
					{
						completeRate = 1f;
					}
				}
				fContanierSize = Mathf.Max(m_ScrollRect.viewport.rect.height, fTotalHeight);
			}
		}
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fContanierSize);
		m_ScrollHandler.ResetScrollRange();
		m_ContentContanierRT.anchoredPosition = new Vector2(m_ContentContanierRT.anchoredPosition.x, 0f);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, m_ScrollHandler.isScrollable);
		if (m_CompleteRateTitle != null)
		{
			m_CompleteRateTitle.text = ((categoryInfo == null) ? string.Empty : categoryInfo.Name);
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
			Object.Destroy(m_Contents[i].gameObject);
		}
		m_Contents.Clear();
	}

	private void SetOnCursorContent(ToDoSlotPlus content, bool isAdjustScrollPos = true)
	{
		if (m_OnCursorContent == content)
		{
			return;
		}
		if (m_OnCursorContent != null)
		{
			m_OnCursorContent.SetSelect(isOn: false);
			if (m_OnCursorContent.VisibleNewSymbol)
			{
				m_OnCursorContent.VisibleNewSymbol = false;
				if (m_curCategoryInfo != null)
				{
					m_curCategoryInfo.ReflashLinkedTabButton();
				}
			}
		}
		if (content != null)
		{
			content.SetSelect(isOn: true);
		}
		m_OnCursorContent = content;
		if (m_OnCursorContent != null && m_OnCursorContent.VisibleNewSymbol)
		{
			GameSwitch.GetInstance().SetTrophyRead(m_OnCursorContent.xlsData.m_iIndex);
			if (m_curCategoryInfo != null)
			{
				m_curCategoryInfo.m_NewContentCount--;
			}
		}
		if (isAdjustScrollPos)
		{
			AdjustScrollPos_byOnCursonContent();
		}
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
					SetOnCursorContent(m_Contents[num5]);
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
					SetOnCursorContent(m_Contents[i]);
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

	public void TouchClose(bool isEnableAnimation)
	{
		if (MainLoadThing.instance.IsTouchableState())
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

	public void OnClick_Content(ToDoSlotPlus content)
	{
		if (m_OnCursorContent != content)
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Select");
			}
			SetOnCursorContent(content);
		}
	}
}
