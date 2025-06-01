using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class ProfileMenuPlus : MonoBehaviour
{
	private class CategoryInfo : CategoryInfo<Xls.Profiles>
	{
		public Xls.CharData_ForProfile m_XlsProfileTab;

		public CategoryInfo(int categoryID, string categoryName)
			: base(categoryID, categoryName)
		{
		}
	}

	public GameObject m_RootObject;

	public Text m_TitleText;

	public CommonTabContainerPlus m_TabContainer;

	public GameObject m_CharInfoRoot;

	public Text m_CharNameText;

	public Text m_CharProfileText;

	public Image m_CharImage;

	public GameObject m_BackButtonObj;

	[Header("Content Container")]
	public ScrollRect m_ScrollRect;

	public RectTransform m_ContentContanierRT;

	public GameObject m_ContentSrcObject;

	public float m_ContentInterval;

	public float m_BottomSpacing = 80f;

	public GameObject m_ChangingTabCover;

	[Header("Scroll Members")]
	public Button m_ScrollButtonToFirst;

	public Button m_ScrollButtonRStick;

	public bool m_ScrollLoopEnable = true;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	public float m_ScrollRepeatTime = 0.2f;

	private float m_fScrollButtonPusingTime;

	[Header("Linked Objects")]
	public ShowImageOriginSize m_ShowImageOrigin;

	private Canvas m_Canvas;

	private AudioManager m_AudioManager;

	private CommonButtonGuide m_ButtonGuide;

	private LoadingSWatchIcon m_LoadingIcon;

	private bool m_isNeedContentAlign;

	private bool m_isInputBlock;

	private bool m_isPageScrolling;

	private bool m_isTutorialActivated;

	private Animator m_CloseCheckAnimator;

	private GameDefine.EventProc m_fpClosedFP;

	private GameDefine.EventProc m_fpChangedSelectContent;

	private Dictionary<int, CategoryInfo> m_CategoryInfos = new Dictionary<int, CategoryInfo>();

	private CategoryInfo m_curCategoryInfo;

	private List<ProfileContentPlus> m_Contents = new List<ProfileContentPlus>();

	private ProfileContentPlus m_OnCursorContent;

	private bool m_isInitializedButtonGuidText;

	private string m_buttonGuide_SelContent;

	private string m_buttonGuide_ExitMenu;

	private string m_buttonGuide_ShowImage;

	private string m_buttonGuide_ScrollToTop;

	private string m_buttonGuide_ChangeTab;

	private string m_buttonGuide_SelectTab;

	private string m_buttonGuide_CancelTab;

	private static ProfileMenuPlus s_activedInstance;

	private const string c_assetBundleName = "prefabs/ingame/menu/ui_profilemenu";

	private static AssetBundleObjectHandler s_assetBundleHdr;

	private AssetBundleObjectHandler m_showingImageAssetObjHdr;

	public static ProfileMenuPlus instance => s_activedInstance;

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
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Vertical, m_ScrollRect, m_ContentContanierRT, null, null, m_ScrollButtonToFirst);
		m_ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
		if (m_CharInfoRoot != null)
		{
			m_CharInfoRoot.gameObject.SetActive(value: false);
		}
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
		m_Canvas = null;
		m_AudioManager = null;
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.ClearContents();
		}
		m_ButtonGuide = null;
		if (m_LoadingIcon != null)
		{
			UnityEngine.Object.Destroy(m_LoadingIcon.gameObject);
		}
		m_LoadingIcon = null;
		m_CloseCheckAnimator = null;
		m_fpClosedFP = null;
		m_fpChangedSelectContent = null;
		if (m_CategoryInfos != null)
		{
			m_CategoryInfos.Clear();
		}
		m_curCategoryInfo = null;
		if (m_Contents != null)
		{
			m_Contents.Clear();
		}
		m_Contents = null;
		m_OnCursorContent = null;
		s_assetBundleHdr = null;
		s_activedInstance = null;
	}

	private void Update()
	{
		if (m_CloseCheckAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_CloseCheckAnimator.GetCurrentAnimatorStateInfo(0);
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
		if (m_isTutorialActivated || m_isInputBlock || m_TabContainer.isChaningTab || ButtonPadInput.IsPlayingButPressAnim() || PopupDialoguePlus.IsAnyPopupActivated())
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
		if (m_OnCursorContent != null && m_OnCursorContent.isExistImage)
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
		if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
		{
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_ExitMenu, isActivate: true);
			}
			CloseProfileMenu();
		}
	}

	private void LateUpdate()
	{
		AlignContents();
	}

	public static IEnumerator ShowProfileMenu_FormAssetBundle(GameDefine.EventProc fpClosed = null, GameDefine.EventProc fpChangedSelContent = null)
	{
		if (s_activedInstance == null)
		{
			s_activedInstance = (UnityEngine.Object.Instantiate(MainLoadThing.instance.m_prefabProfileMenu) as GameObject).GetComponent<ProfileMenuPlus>();
			yield return null;
		}
		s_activedInstance.ShowProfileMenu(fpClosed, fpChangedSelContent);
	}

	public void ShowProfileMenu(GameDefine.EventProc fpClosed = null, GameDefine.EventProc fpChangedSelContent = null)
	{
		m_fpClosedFP = ((fpClosed == null) ? null : new GameDefine.EventProc(fpClosed.Invoke));
		m_fpChangedSelectContent = ((fpChangedSelContent == null) ? null : new GameDefine.EventProc(fpChangedSelContent.Invoke));
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		Text[] textComps = new Text[3] { m_TitleText, m_CharNameText, m_CharProfileText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_TitleText.text = GameGlobalUtil.GetXlsProgramText("PROFILE_MENU_TITLE");
		ProfileContentPlus.InitStaticTextMembers();
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
		m_ScrollHandler.ResetScrollRange();
		InitCategoryInfo();
		if (m_TabContainer != null)
		{
			m_TabContainer.BuildTabButtonObjects();
			m_TabContainer.isInputBlock = false;
			m_TabContainer.SetSelectedTab_byIdx(0);
		}
		MatchCategoryInfoToTabButton();
		InitButtonGuide();
		m_isInputBlock = false;
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("View_KeyTalk_Title");
		}
	}

	public void CloseProfileMenu(bool isEnableAnimation = true)
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
		m_CloseCheckAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
		if (m_CloseCheckAnimator == null)
		{
			CallClosedCallback();
		}
	}

	private void CallClosedCallback()
	{
		SetOnCursorContent(null, isAdjustScrollPos: false);
		m_CloseCheckAnimator = null;
		m_TabContainer.ClearTabButtonObjects();
		ClearContents();
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
		UnityEngine.Object.Destroy(base.gameObject);
		s_activedInstance = null;
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
		m_buttonGuide_SelContent = GameGlobalUtil.GetXlsProgramText("PROFILE_MENU_BTNGUIDE_SEL_CONTENTS");
		m_buttonGuide_ExitMenu = GameGlobalUtil.GetXlsProgramText("PROFILE_MENU_BTNGUIDE_EXIT_MENU");
		m_buttonGuide_ShowImage = GameGlobalUtil.GetXlsProgramText("PROFILE_MENU_BTNGUIDE_SHOW_IMAGE");
		m_buttonGuide_ScrollToTop = GameGlobalUtil.GetXlsProgramText("PROFILE_MENU_BTNGUIDE_SCROLL_TO_TOP");
		m_buttonGuide_ChangeTab = GameGlobalUtil.GetXlsProgramText("PROFILE_MENU_BTNGUIDE_CHANGE_TAB");
		m_buttonGuide_SelectTab = GameGlobalUtil.GetXlsProgramText("PROFILE_MENU_BTNGUIDE_SEL_TAB");
		m_buttonGuide_CancelTab = GameGlobalUtil.GetXlsProgramText("PROFILE_MENU_BTNGUIDE_CANCEL_TAB");
		m_isInitializedButtonGuidText = true;
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuide_SelContent, PadInput.GameInput.LStickY);
		m_ButtonGuide.AddContent(m_buttonGuide_ExitMenu, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ShowImage, PadInput.GameInput.SquareButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ScrollToTop, PadInput.GameInput.TriangleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_SelectTab, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_CancelTab, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.L1Button);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.R1Button);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Left);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ShowImage, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_SelectTab, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_CancelTab, isEnable: false);
		m_ButtonGuide.SetShow(isShow: true);
	}

	private void InitCategoryInfo()
	{
		m_CategoryInfos.Clear();
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		sbyte b = 0;
		CategoryInfo categoryInfo = null;
		Xls.TextListData textListData = null;
		Xls.CharData_ForProfile charData_ForProfile = null;
		List<Xls.CharData_ForProfile>.Enumerator enumerator = Xls.CharData_ForProfile.datas.GetEnumerator();
		while (enumerator.MoveNext())
		{
			charData_ForProfile = enumerator.Current;
			if (charData_ForProfile != null)
			{
				b = gameSwitch.GetEventSwitch(charData_ForProfile.m_iUpdateThropySwitch);
				textListData = Xls.TextListData.GetData_byKey((b != 1 && b != 2) ? charData_ForProfile.m_strProfileText : charData_ForProfile.m_strProfileText_Update);
				if (textListData != null)
				{
					categoryInfo = new CategoryInfo(charData_ForProfile.m_iCategoryIdx, textListData.m_strTitle);
					categoryInfo.m_XlsProfileTab = charData_ForProfile;
					m_CategoryInfos.Add(charData_ForProfile.m_iCategoryIdx, categoryInfo);
				}
			}
		}
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		int num2 = 0;
		Xls.Profiles profiles = null;
		List<Xls.Profiles>.Enumerator enumerator2 = Xls.Profiles.datas.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			profiles = enumerator2.Current;
			categoryInfo = null;
			if (m_CategoryInfos.TryGetValue(profiles.m_iCtgIdx, out categoryInfo) && categoryInfo != null)
			{
				b = gameSwitch.GetCharProfile(profiles.m_iIdx);
				flag = b == 1;
				flag2 = flag || b == 2;
				if (flag2)
				{
					categoryInfo.m_xlsDatas.Add(profiles);
					categoryInfo.m_ValidContentCount += (flag2 ? 1 : 0);
					categoryInfo.m_NewContentCount += ((b == 1) ? 1 : 0);
					num += (flag ? 1 : 0);
					num2 += (flag2 ? 1 : 0);
				}
			}
		}
	}

	private void MatchCategoryInfoToTabButton()
	{
		Dictionary<int, CategoryInfo>.Enumerator enumerator = m_CategoryInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CategoryInfo value = enumerator.Current.Value;
			if (value != null)
			{
				value.m_LinkedTabButton = m_TabContainer.GetTabButton(value);
				value.ReflashLinkedTabButton();
			}
		}
	}

	private List<CommonTabContainerPlus.TabCreateInfo> SetTabCreateInfos()
	{
		List<CommonTabContainerPlus.TabCreateInfo> list = new List<CommonTabContainerPlus.TabCreateInfo>();
		Dictionary<int, CategoryInfo>.Enumerator enumerator = m_CategoryInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CategoryInfo value = enumerator.Current.Value;
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
		if (sender is CommonTabContainerPlus.TabButtonInfo { tag: not null } tabButtonInfo)
		{
			SetOnCursorContent(null);
			CategoryInfo categoryInfo = tabButtonInfo.tag as CategoryInfo;
			StartCoroutine(InitContents(categoryInfo));
			StartCoroutine(InitLeftContents(categoryInfo.m_XlsProfileTab));
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

	private IEnumerator InitContents(CategoryInfo categoryInfo)
	{
		ClearContents();
		if (m_ContentSrcObject == null)
		{
			yield break;
		}
		m_curCategoryInfo = categoryInfo;
		if (categoryInfo == null)
		{
			yield break;
		}
		List<Xls.Profiles> xlsProfileDatas = categoryInfo.m_xlsDatas;
		if (xlsProfileDatas != null && xlsProfileDatas.Count > 0)
		{
			m_isInputBlock = true;
			m_TabContainer.isInputBlock = true;
			int count = xlsProfileDatas.Count;
			Xls.Profiles xlsProfileData = null;
			GameObject contentObject = null;
			ProfileContentPlus profileContent = null;
			if (m_LoadingIcon != null)
			{
				m_LoadingIcon.gameObject.SetActive(value: true);
			}
			else
			{
				m_LoadingIcon = LoadingSWatchIcon.Create(m_ScrollRect.gameObject);
			}
			GameSwitch gameSwitch = GameSwitch.GetInstance();
			sbyte switchState = 0;
			sbyte switchStateOn = 1;
			for (int i = 0; i < count; i++)
			{
				yield return null;
				xlsProfileData = xlsProfileDatas[i];
				switchState = gameSwitch.GetCharProfile(xlsProfileData.m_strKey);
				contentObject = UnityEngine.Object.Instantiate(m_ContentSrcObject);
				profileContent = contentObject.GetComponent<ProfileContentPlus>();
				m_Contents.Add(profileContent);
				profileContent.InitProfileContent(xlsProfileData, this);
				profileContent.visibleNewTag = switchState == switchStateOn;
				profileContent.rectTransform.SetParent(m_ContentContanierRT, worldPositionStays: false);
				profileContent.gameObject.SetActive(value: false);
			}
			count = m_Contents.Count;
			for (int j = 0; j < count; j++)
			{
				m_Contents[j].gameObject.SetActive(value: true);
			}
			m_isNeedContentAlign = true;
			m_LoadingIcon.gameObject.SetActive(value: false);
		}
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

	private void AlignContents()
	{
		if (!m_isNeedContentAlign)
		{
			return;
		}
		float size = 0f;
		int count = m_Contents.Count;
		if (count > 0)
		{
			ProfileContentPlus profileContentPlus = m_Contents[count - 1];
			if (!profileContentPlus.isInitailized)
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < count; i++)
			{
				profileContentPlus = m_Contents[i];
				profileContentPlus.rectTransform.anchoredPosition = new Vector2(profileContentPlus.rectTransform.anchoredPosition.x, num2);
				num3 = profileContentPlus.rectTransform.rect.height * profileContentPlus.rectTransform.localScale.y;
				num2 -= num3 + m_ContentInterval;
				num += num3;
			}
			if (count > 1)
			{
				num += m_ContentInterval * (float)(count - 1);
			}
			num += m_BottomSpacing;
			size = Mathf.Max(m_ScrollRect.viewport.rect.height, num);
		}
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
		m_ScrollHandler.ResetScrollRange();
		m_ContentContanierRT.anchoredPosition = new Vector2(m_ContentContanierRT.anchoredPosition.x, 0f);
		if (m_Contents.Count > 0)
		{
			SetOnCursorContent(m_Contents[0]);
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, m_ScrollHandler.isScrollable);
		}
		m_isInputBlock = false;
		m_TabContainer.isInputBlock = false;
		m_isNeedContentAlign = false;
	}

	private IEnumerator InitLeftContents(Xls.CharData_ForProfile xlsCharForProfile)
	{
		if (m_CharInfoRoot != null)
		{
			m_CharInfoRoot.SetActive(value: true);
		}
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		sbyte switchState = gameSwitch.GetEventSwitch(xlsCharForProfile.m_iUpdateThropySwitch);
		bool isUpdateSwitchOn = switchState == 1 || switchState == 2;
		Xls.TextListData xlsTextGroupData = null;
		Xls.ImageFile xlsImageFileData = null;
		try
		{
			if (isUpdateSwitchOn)
			{
				xlsTextGroupData = Xls.TextListData.GetData_byKey(xlsCharForProfile.m_strProfileText_Update);
				xlsImageFileData = Xls.ImageFile.GetData_byKey(xlsCharForProfile.m_strImageKey_Update);
			}
			else
			{
				xlsTextGroupData = Xls.TextListData.GetData_byKey(xlsCharForProfile.m_strProfileText);
				xlsImageFileData = Xls.ImageFile.GetData_byKey(xlsCharForProfile.m_strImageKey);
			}
		}
		catch (Exception)
		{
		}
		if (xlsTextGroupData != null)
		{
			m_CharNameText.text = xlsTextGroupData.m_strTitle;
			m_CharProfileText.text = xlsTextGroupData.m_strText;
		}
		else
		{
			m_CharNameText.text = string.Empty;
			m_CharProfileText.text = string.Empty;
		}
		if (xlsImageFileData != null)
		{
			m_CharImage.gameObject.SetActive(value: true);
			string strPath = xlsImageFileData.m_strAssetPath;
			yield return MainLoadThing.instance.StartCoroutine(GameGlobalUtil.GetSprRequestFromImgPath(strPath));
			m_CharImage.sprite = GameGlobalUtil.m_sprLoadFromImgXls;
			m_CharImage.color = Color.white;
		}
		else
		{
			m_CharImage.gameObject.SetActive(value: false);
			m_CharImage.sprite = null;
		}
		yield return null;
	}

	private void SetOnCursorContent(ProfileContentPlus content, bool isAdjustScrollPos = true)
	{
		if (m_OnCursorContent == content)
		{
			return;
		}
		if (m_OnCursorContent != null)
		{
			m_OnCursorContent.onCursor = false;
			if (m_OnCursorContent.visibleNewTag)
			{
				m_OnCursorContent.visibleNewTag = false;
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
		if (isAdjustScrollPos)
		{
			AdjustScrollPos_byOnCursonContent();
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ShowImage, m_OnCursorContent != null && m_OnCursorContent.isExistImage);
		}
		if (m_fpChangedSelectContent != null)
		{
			m_fpChangedSelectContent(this, m_OnCursorContent);
		}
		if (m_OnCursorContent != null && m_OnCursorContent.visibleNewTag)
		{
			GameSwitch.GetInstance().SetCharProfile(m_OnCursorContent.xlsData.m_strKey, 2);
			if (m_curCategoryInfo != null)
			{
				m_curCategoryInfo.m_NewContentCount--;
			}
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

	public IEnumerator OnProc_ViewImageDetail(ProfileContentPlus profileContent)
	{
		if (profileContent == null)
		{
			yield break;
		}
		Xls.CollImages xlsCollectionImage = Xls.CollImages.GetData_byKey(profileContent.xlsData.m_strIDImg);
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
			m_ShowImageOrigin.ShowImage(isShow: true, spr, OnProc_ClosedViewImageDetail);
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

	public ProfileContentPlus GetContentNearBy(ProfileContentPlus baseContent, int idxOffset)
	{
		int count = m_Contents.Count;
		if (count <= 0)
		{
			return null;
		}
		if (baseContent == null)
		{
			baseContent = ((!(m_OnCursorContent != null)) ? m_Contents[0] : m_OnCursorContent);
		}
		if (idxOffset == 0)
		{
			return baseContent;
		}
		int num = m_Contents.IndexOf(baseContent);
		int num2 = num + idxOffset;
		return (num2 < 0 || num2 >= count) ? null : m_Contents[num2];
	}

	public string GetCurrentTabText()
	{
		if (m_TabContainer == null || m_TabContainer.selectedTabInfo == null)
		{
			return string.Empty;
		}
		return m_TabContainer.selectedTabInfo.tabButtonComp.text;
	}

	private void ShowTutorialPopup()
	{
		string strTutorialKey = "tuto_00021";
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

	public void TouchProfileMenu(bool isEnableAnimation = true)
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			CloseProfileMenu(isEnableAnimation);
		}
	}

	public void OnClick_Content(ProfileContentPlus content)
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
