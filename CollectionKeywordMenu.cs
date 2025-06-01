using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class CollectionKeywordMenu : CommonBGChildBase
{
	private enum SlotPage
	{
		Unknown = -1,
		Prev,
		Current,
		Next
	}

	[Serializable]
	public class KeywordInfoMembers
	{
		public GameObject m_RootObject;

		public Text m_KeywordName;

		public GameObject m_NoImageContentRoot;

		public Text m_NoImageContentText;

		public GameObject m_ImagedContentRoot;

		public Text m_ImagedContentText;

		public Image m_ImagedContentImage;

		public Button m_ImageViewPadIconButton;
	}

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

	public Text m_textLogIcon;

	[Header("Content Container")]
	public ScrollRect m_ScrollRect;

	public RectTransform m_ContentContanierRT;

	public GridLayoutGroup m_ContentGrid;

	public GameObject m_ContentSrcObject;

	public GameObject m_ChangingTabCover;

	public float m_InputRepeatTimeBound = 0.2f;

	[Header("Scroll Members")]
	public GameObject m_ScrollRoot;

	public Button m_ScrollButttonLeft;

	public Button m_ScrollButttonRight;

	public RectTransform m_ScrollNodeRT;

	public GameObject m_ScrollNodeSrcObj;

	public Sprite m_SelectNodeImage;

	public Sprite m_NotSelectNodeImage;

	public float m_ScrollSpeed = 1000f;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	[Header("Keyword Detail Infos")]
	public KeywordInfoMembers m_KeywordDetailInfo = new KeywordInfoMembers();

	public Button m_LogViewerPadIconButton;

	[Header("Keyword Log Viewer")]
	public GameObject m_LogViewerRoot;

	public KeywordInfoMembers m_LogViewerDetailInfo = new KeywordInfoMembers();

	public Image m_LogViewerKeywordCard;

	public GameObject m_LogViewerScrollRoot;

	public Button m_LogViewerToPrevButton;

	public Button m_LogViewerToNextButton;

	public Text m_LogViewerCloseText;

	public Text m_LogViewerDirButtonText;

	public Button m_LogViewerCloseOButton;

	public Button m_LogViewerCloseXButton;

	public Button m_LogViewerDirButton;

	public Text m_LogViewerRelationshipText;

	public Text m_LogViewerGetKeywordText;

	private const int c_LogViewResultSlotCount = 5;

	public Text m_LogViewResultEmpty;

	public KeywordUseResultSlot[] m_LogViewResultSlots = new KeywordUseResultSlot[5];

	private Sprite[] m_CharFaceImages = new Sprite[5];

	private AudioManager m_AudioManager;

	private CommonButtonGuide m_ButtonGuide;

	private LoadingSWatchIcon m_LoadingIcon;

	private Dictionary<int, CategoryInfo<Xls.CollKeyword>> m_CategoryInfos = new Dictionary<int, CategoryInfo<Xls.CollKeyword>>();

	private CategoryInfo<Xls.CollKeyword> m_curCategoryInfo;

	private KeywordContainer m_keywordContainer = new KeywordContainer();

	private List<Xls.CollKeyword> m_curXlsDatas;

	private List<Xls.CollKeyword> m_validXlsDatas = new List<Xls.CollKeyword>();

	private KeywordSlotPlus m_OnCursorContent;

	private float m_PadStickPushingTime;

	private List<Image> m_PageNodeImages = new List<Image>();

	private int m_PageCount;

	private int m_curPage;

	private const int c_pageSlotRawCount = 2;

	private const int c_pageSlotColCount = 4;

	private const int c_pageSlotTotalCount = 8;

	private const int c_maxSlotObjectCount = 24;

	private int m_idxScrollNextPage;

	private float m_pageWidth;

	private float m_slotWidth;

	private SlotPage m_scrollDirTo = SlotPage.Current;

	private float m_scrollableWidth;

	private float m_checkScrolledWidth;

	private float m_remainedScrollFactor;

	private int m_resetSlotColumnIdx;

	private bool m_isResetOnSlotCursor;

	private const float c_scrollDuration = 0.3f;

	private float m_scrollSpeed;

	private int[] m_logViewEnableCategories = new int[1] { 1 };

	private string m_categorizedTitleFormat = string.Empty;

	private string m_invalidateContentTitle = string.Empty;

	private string m_invalidateContentText = string.Empty;

	private int m_firstSelectCategory = -1;

	private bool m_isInputBlock;

	private Animator m_CloseAnimator;

	private Animator m_LogViewerCloseAnimator;

	private bool m_isInitializedButtonGuidText;

	private string m_buttonGuide_SelContent;

	private string m_buttonGuide_ExitMenu;

	private string m_buttonGuide_ShowImage;

	private string m_buttonGuide_LogViewer;

	private string m_buttonGuide_ChangeTab;

	private string m_buttonGuide_SelectTab;

	private string m_buttonGuide_CancelTab;

	private const string c_characterIconBundleName = "image/keyword_ui";

	private ContentThumbnailManager m_characterIconManager = new ContentThumbnailManager("image/keyword_ui");

	private Canvas m_Canvas;

	private AssetBundleObjectHandler m_showingImageAssetObjHdr;

	private void OnDestroy()
	{
		if (m_ScrollHandler != null)
		{
			m_ScrollHandler.ReleaseScroll();
		}
		m_AudioManager = null;
		m_ButtonGuide = null;
		m_LoadingIcon = null;
		if (m_CategoryInfos != null)
		{
			m_CategoryInfos.Clear();
		}
		m_curCategoryInfo = null;
		if (m_curXlsDatas != null)
		{
			m_curXlsDatas.Clear();
		}
		m_curXlsDatas = null;
		if (m_validXlsDatas != null)
		{
			m_validXlsDatas.Clear();
		}
		if (m_PageNodeImages != null)
		{
			m_PageNodeImages.Clear();
		}
		m_OnCursorContent = null;
		m_CloseAnimator = null;
		m_LogViewerCloseAnimator = null;
		m_Canvas = null;
		m_keywordContainer.Release();
		ClearPageNodes();
		if (m_CharFaceImages != null)
		{
			m_CharFaceImages = null;
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
		if (m_ContentSrcObject != null)
		{
			m_ContentSrcObject.SetActive(value: false);
		}
		m_keywordContainer.InitContainer(m_ContentGrid, m_ContentSrcObject);
		m_keywordContainer.OnCursorChanged = OnProc_OnCursorChanged;
		m_keywordContainer.ChangedCurrentPage = OnProc_ChangedCurrentPage;
		m_keywordContainer.KeyInputedScrollPage = OnProc_KeyInputedScrollPage;
		m_keywordContainer.KeyInputedMoveCursor = OnProc_KeyInputedMoveCursor;
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Horizontal, m_ScrollRect, m_ContentContanierRT, m_ScrollButttonLeft, m_ScrollButttonRight);
		m_Canvas = base.gameObject.GetComponentInChildren<Canvas>();
		if (m_LogViewerDirButtonText != null)
		{
			m_LogViewerDirButtonText.gameObject.SetActive(value: false);
		}
		if (m_LogViewerDirButton != null)
		{
			m_LogViewerDirButton.gameObject.SetActive(value: false);
		}
		if (m_LogViewerPadIconButton != null)
		{
			m_LogViewerPadIconButton.gameObject.SetActive(value: false);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_keywordContainer.IsPageScrolling())
		{
			m_keywordContainer.UpdatePageScroll();
		}
		else if (m_LogViewerRoot.activeSelf)
		{
			Update_LogViewer();
		}
		else if (m_CloseAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_CloseAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				CallClosedCallback();
			}
		}
		else
		{
			if (m_isInputBlock || m_TabContainer.isChaningTab || ButtonPadInput.IsPlayingButPressAnim() || PopupDialoguePlus.IsAnyPopupActivated())
			{
				return;
			}
			m_keywordContainer.ProcKeyInput();
			if (m_OnCursorContent != null)
			{
				if (m_KeywordDetailInfo.m_ImagedContentRoot.activeSelf)
				{
					ButtonPadInput.PressInputButton(PadInput.GameInput.SquareButton, m_KeywordDetailInfo.m_ImageViewPadIconButton);
					if (GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton))
					{
						if (m_ButtonGuide != null)
						{
							m_ButtonGuide.SetContentActivate(m_buttonGuide_ShowImage, isActivate: true);
						}
						StartCoroutine(OnProc_ViewImageDetail(m_OnCursorContent));
					}
				}
				if (m_LogViewerPadIconButton != null && m_LogViewerPadIconButton.gameObject.activeSelf)
				{
					ButtonPadInput.PressInputButton(PadInput.GameInput.TouchPadButton, m_LogViewerPadIconButton);
					if (GamePadInput.IsButtonState_Down(PadInput.GameInput.TouchPadButton))
					{
						if (m_ButtonGuide != null)
						{
							m_ButtonGuide.SetContentActivate(m_buttonGuide_LogViewer, isActivate: true);
						}
						OnClickLogButton();
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

	private void OnProc_KeyInputedScrollPage(object sender, object args)
	{
	}

	private void OnProc_KeyInputedMoveCursor(object sender, object args)
	{
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentActivate(m_buttonGuide_SelContent, isActivate: true);
		}
	}

	private void Update_LogViewer()
	{
		if (m_LogViewerCloseAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_LogViewerCloseAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				m_LogViewerRoot.SetActive(value: false);
				m_TabContainer.isInputBlock = false;
				m_LogViewerCloseAnimator = null;
			}
		}
		else
		{
			if (m_isInputBlock || m_keywordContainer.IsPageScrolling() || PopupDialoguePlus.IsAnyPopupActivated())
			{
				return;
			}
			if (m_OnCursorContent != null && m_KeywordDetailInfo.m_ImagedContentRoot.activeSelf)
			{
				ButtonPadInput.PressInputButton(PadInput.GameInput.SquareButton, m_KeywordDetailInfo.m_ImageViewPadIconButton);
				if (GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton))
				{
					if (m_ButtonGuide != null)
					{
						m_ButtonGuide.SetContentActivate(m_buttonGuide_ShowImage, isActivate: true);
					}
					StartCoroutine(OnProc_ViewImageDetail(m_OnCursorContent));
				}
			}
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.TouchPadButton))
			{
				ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_LogViewerCloseOButton);
				ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_LogViewerCloseXButton);
				CloseLogViewer();
			}
		}
	}

	public void PrevNextInLogViewer(int deltaIndex)
	{
		if (deltaIndex == 0 || m_OnCursorContent == null || m_OnCursorContent.xlsData == null || m_validXlsDatas == null || m_validXlsDatas.Count <= 0)
		{
			return;
		}
		ButtonPadInput.PressInputButton(PadInput.GameInput.LStickX, m_LogViewerDirButton);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		int num = m_validXlsDatas.IndexOf(m_OnCursorContent.xlsData);
		if (num < 0)
		{
			return;
		}
		int num2 = num + deltaIndex;
		if (num2 >= m_validXlsDatas.Count)
		{
			num2 = 0;
		}
		else if (num2 < 0)
		{
			num2 = m_validXlsDatas.Count - 1;
		}
		if (num2 == num)
		{
			return;
		}
		int num3 = m_keywordContainer.KeywordDatas.IndexOf(m_validXlsDatas[num2]);
		if (num3 < 0)
		{
			return;
		}
		int num4 = num3 / m_keywordContainer.SlotCountPerPage;
		if (m_keywordContainer.CurrentPageIndex != num4)
		{
			if (Mathf.Abs(m_keywordContainer.CurrentPageIndex - num4) == 1)
			{
				m_keywordContainer.StartPageScroll(num4, (deltaIndex > 0) ? KeywordContainer.SlotPage.Next : KeywordContainer.SlotPage.Prev);
			}
			else
			{
				m_keywordContainer.SetCurrentPage(num4);
			}
		}
		m_keywordContainer.SetOnCursorKeywordData(num3);
	}

	private bool IsOverScrollButtonPushingTime()
	{
		m_PadStickPushingTime += Time.deltaTime;
		if (m_PadStickPushingTime >= m_InputRepeatTimeBound)
		{
			m_PadStickPushingTime = 0f;
			return true;
		}
		return false;
	}

	public IEnumerator Show()
	{
		m_isInputBlock = true;
		m_TabContainer.isInputBlock = true;
		base.gameObject.SetActive(value: false);
		yield return MainLoadThing.instance.StartCoroutine(m_characterIconManager.LoadAssetsAll());
		yield return null;
		int useIdx = 0;
		Xls.ImageFile xlsImageFile = null;
		foreach (Xls.CharData data in Xls.CharData.datas)
		{
			useIdx = data.m_iUseIdx;
			if (useIdx >= 0 && useIdx < 5 && !(m_CharFaceImages[useIdx] != null))
			{
				xlsImageFile = Xls.ImageFile.GetData_byKey(data.m_strDotIconImage);
				if (xlsImageFile != null)
				{
					m_CharFaceImages[useIdx] = m_characterIconManager.GetThumbnailImageInCache(xlsImageFile.m_strAssetPath);
				}
			}
		}
		m_PageCount = 0;
		m_ScrollRoot.SetActive(value: false);
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
		SetKeywordDetailInfos(null);
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		InitCategoryInfo();
		InitTextMemebers();
		if (m_TabContainer != null)
		{
			m_TabContainer.BuildTabButtonObjects();
			m_TabContainer.isInputBlock = false;
		}
		MatchCategoryInfoToTabButton();
		m_firstSelectCategory = 1;
		yield return null;
		SetFirstSelectTab();
		m_CloseAnimator = null;
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.appear.ToString());
		InitButtonGuide();
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
	}

	public void InitTextMemebers()
	{
		Text[] textComps = new Text[17]
		{
			m_TitleText, m_TotalCompleteRateTitle, m_TotalCompleteRateValue, m_CompleteRateTitle, m_CompleteRateValue, m_textLogIcon, m_LogViewerCloseText, m_LogViewerDirButtonText, m_LogViewResultEmpty, m_LogViewerRelationshipText,
			m_LogViewerGetKeywordText, m_KeywordDetailInfo.m_KeywordName, m_KeywordDetailInfo.m_ImagedContentText, m_KeywordDetailInfo.m_NoImageContentText, m_LogViewerDetailInfo.m_KeywordName, m_LogViewerDetailInfo.m_ImagedContentText, m_LogViewerDetailInfo.m_NoImageContentText
		};
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_TitleText.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_KEYWORD_MENU_TITLE");
		if (m_TotalCompleteRateTitle != null)
		{
			m_TotalCompleteRateTitle.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_COMPLETE_RATE");
		}
		if (m_LogViewerCloseText != null)
		{
			m_LogViewerCloseText.text = GameGlobalUtil.GetXlsProgramText("KEYWORD_HELP_WINDOW_CLOSE");
		}
		if (m_LogViewerDirButtonText != null)
		{
			m_LogViewerDirButtonText.text = GameGlobalUtil.GetXlsProgramText("KEYWORD_HELP_WINDOW_CHANGE_KEYWORD");
		}
		m_LogViewResultEmpty.text = GameGlobalUtil.GetXlsProgramText("KEYWORD_USE_RECORD_NOT_EXIST");
		m_LogViewerRelationshipText.text = GameGlobalUtil.GetXlsProgramText("KEYWORDMENU_HELP_WINDOW_RELATION");
		m_LogViewerGetKeywordText.text = GameGlobalUtil.GetXlsProgramText("KEYWORDMENU_HELP_WINDOW_GET_KEYWORD");
		if (m_textLogIcon != null)
		{
			m_textLogIcon.text = GameGlobalUtil.GetXlsProgramText("KEYWORD_LOG");
		}
		m_categorizedTitleFormat = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_CONTENT_TITLE_FORMAT");
		m_invalidateContentTitle = GameGlobalUtil.GetXlsProgramText("COLLECTION_KEYWORD_MENU_INVALID_TITLE");
		m_invalidateContentText = GameGlobalUtil.GetXlsProgramText("COLLECTION_KEYWORD_MENU_INVALID_TEXT");
	}

	private void InitCategoryInfo()
	{
		m_CategoryInfos.Clear();
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("COLLECTION_KEYWORD_ORDER");
		if (string.IsNullOrEmpty(xlsProgramDefineStr))
		{
			return;
		}
		string[] array = xlsProgramDefineStr.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		List<int> list = new List<int>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrEmpty(text))
			{
				int result = -1;
				if (int.TryParse(text, out result) && !list.Contains(result))
				{
					list.Add(result);
				}
			}
		}
		CategoryInfo<Xls.CollKeyword> categoryInfo = null;
		foreach (int item in list)
		{
			string xlsDataName = $"KEYWORD_COLLECTION_CTG_{item:D2}";
			string xlsProgramText = GameGlobalUtil.GetXlsProgramText(xlsDataName);
			categoryInfo = new CategoryInfo<Xls.CollKeyword>(item, xlsProgramText);
			m_CategoryInfos.Add(item, categoryInfo);
		}
		string xlsProgramText2 = GameGlobalUtil.GetXlsProgramText("KEYWORD_COLLECTION_CTG_HIDDEN");
		GameSwitch instance = GameSwitch.GetInstance();
		sbyte b = 0;
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (Xls.CollKeyword data in Xls.CollKeyword.datas)
		{
			categoryInfo = null;
			if (m_CategoryInfos.TryGetValue(data.m_Sequence, out categoryInfo) && categoryInfo != null)
			{
				categoryInfo.m_xlsDatas.Add(data);
				b = instance.GetCollKeyword(data.m_iIndex);
				flag = b == 1;
				flag2 = flag || b == 2;
				categoryInfo.m_ValidContentCount += (flag2 ? 1 : 0);
				categoryInfo.m_NewContentCount += ((b == 1) ? 1 : 0);
				num2 += (flag ? 1 : 0);
				num3 += (flag2 ? 1 : 0);
				num++;
			}
		}
		Dictionary<int, CategoryInfo<Xls.CollKeyword>>.Enumerator enumerator3 = m_CategoryInfos.GetEnumerator();
		while (enumerator3.MoveNext())
		{
			categoryInfo = enumerator3.Current.Value;
			if (categoryInfo == null)
			{
				continue;
			}
			if (categoryInfo.m_xlsDatas == null || categoryInfo.m_xlsDatas.Count <= 0 || categoryInfo.m_ValidContentCount <= 0)
			{
				categoryInfo.Name = xlsProgramText2;
				categoryInfo.m_CompleteRate = 0f;
				continue;
			}
			float num4 = categoryInfo.m_xlsDatas.Count;
			float num5 = categoryInfo.m_ValidContentCount;
			categoryInfo.m_CompleteRate = ((!(num5 < num4)) ? 100f : (num5 * 100f / num4));
			if (categoryInfo.m_CompleteRate < 1f)
			{
				categoryInfo.m_CompleteRate = 1f;
			}
		}
		if (m_TotalCompleteRateValue != null)
		{
			float num6 = ((num3 >= num) ? 100f : ((num <= 0) ? 0f : ((float)num3 * 100f / (float)num)));
			if (num6 < 1f && num3 > 0)
			{
				num6 = 1f;
			}
			m_TotalCompleteRateValue.text = $"{(int)num6}%";
		}
	}

	private void MatchCategoryInfoToTabButton()
	{
		Dictionary<int, CategoryInfo<Xls.CollKeyword>>.Enumerator enumerator = m_CategoryInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CategoryInfo<Xls.CollKeyword> value = enumerator.Current.Value;
			if (value != null)
			{
				value.m_LinkedTabButton = m_TabContainer.GetTabButton(value);
				value.ReflashLinkedTabButton();
			}
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
		m_buttonGuide_SelContent = GameGlobalUtil.GetXlsProgramText("KEYWORD_BOT_MENU_CURSOR");
		m_buttonGuide_ExitMenu = GameGlobalUtil.GetXlsProgramText("KEYWORD_BOT_MENU_EXIT");
		m_buttonGuide_ShowImage = GameGlobalUtil.GetXlsProgramText("KEYWORD_BOT_MENU_SHOW_IMAGE");
		m_buttonGuide_LogViewer = GameGlobalUtil.GetXlsProgramText("KEYWORD_BOT_MENU_LOG");
		m_buttonGuide_ChangeTab = GameGlobalUtil.GetXlsProgramText("KEYWORD_BOT_MENU_TAB");
		m_buttonGuide_SelectTab = GameGlobalUtil.GetXlsProgramText("KEYWORD_BOT_MENU_TAB_SEL");
		m_buttonGuide_CancelTab = GameGlobalUtil.GetXlsProgramText("KEYWORD_BOT_MENU_TAB_CANCEL");
		m_isInitializedButtonGuidText = true;
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuide_SelContent, PadInput.GameInput.LStickX, isIngoreAxis: true);
		m_ButtonGuide.AddContent(m_buttonGuide_ExitMenu, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ShowImage, PadInput.GameInput.SquareButton);
		m_ButtonGuide.AddContent(m_buttonGuide_SelectTab, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_CancelTab, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.L1Button);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.R1Button);
		m_ButtonGuide.AddContent(m_buttonGuide_LogViewer, PadInput.GameInput.TouchPadButton);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Center);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ShowImage, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_SelectTab, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_CancelTab, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_LogViewer, isEnable: false);
		m_ButtonGuide.SetShow(isShow: true);
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
		m_keywordContainer.SetOnCursorKeywordData(-1);
		m_CloseAnimator = null;
		m_TabContainer.ClearTabButtonObjects();
		m_keywordContainer.ClearSlotObjects();
		ClearPageNodes();
		if (m_characterIconManager != null)
		{
			m_characterIconManager.ClearThumbnailCaches();
		}
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

	private void SetFirstSelectTab()
	{
		List<CommonTabContainerPlus.TabButtonInfo>.Enumerator enumerator = m_TabContainer.tabButtonInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CommonTabContainerPlus.TabButtonInfo current = enumerator.Current;
			if (current != null && current.tag != null && current.tag is CategoryInfo<Xls.CollKeyword>)
			{
				CategoryInfo<Xls.CollKeyword> categoryInfo = current.tag as CategoryInfo<Xls.CollKeyword>;
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
		Dictionary<int, CategoryInfo<Xls.CollKeyword>>.Enumerator enumerator = m_CategoryInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			CategoryInfo<Xls.CollKeyword> value = enumerator.Current.Value;
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
			CategoryInfo<Xls.CollKeyword> categoryInfo = tabButtonInfo.tag as CategoryInfo<Xls.CollKeyword>;
			int num = categoryInfo?.ID ?? (-1);
			m_isInputBlock = true;
			m_TabContainer.isInputBlock = true;
			if (m_keywordContainer.KeywordSlots.Count <= 0)
			{
				MainLoadThing.instance.StartCoroutine(CreateSlotObjects(categoryInfo));
			}
			else
			{
				SetCurrentCategory(categoryInfo);
				m_isInputBlock = false;
				m_TabContainer.isInputBlock = false;
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
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ShowImage, !flag && m_OnCursorContent != null && m_KeywordDetailInfo.m_ImagedContentRoot.activeInHierarchy);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ExitMenu, !flag);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_LogViewer, isEnable: false);
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

	private IEnumerator CreateSlotObjects(CategoryInfo<Xls.CollKeyword> categoryInfo)
	{
		yield return MainLoadThing.instance.StartCoroutine(m_keywordContainer.CreateSlotObjects());
		yield return null;
		SetCurrentCategory(categoryInfo);
		m_isInputBlock = false;
		m_TabContainer.isInputBlock = false;
	}

	private void SetCurrentCategory(CategoryInfo<Xls.CollKeyword> categoryInfo)
	{
		m_keywordContainer.SetOnCursorKeywordData(-1);
		m_curCategoryInfo = categoryInfo;
		m_keywordContainer.SetKeywordDatas(m_curCategoryInfo.m_xlsDatas);
		SetPageCount(m_keywordContainer.PageCount);
		m_keywordContainer.SetCurrentPage(0);
		m_keywordContainer.SetOnCursorKeywordData(0);
		m_validXlsDatas.Clear();
		Xls.CollKeyword collKeyword = null;
		GameSwitch instance = GameSwitch.GetInstance();
		sbyte b = 0;
		sbyte b2 = 1;
		sbyte b3 = 2;
		List<Xls.CollKeyword>.Enumerator enumerator = m_curCategoryInfo.m_xlsDatas.GetEnumerator();
		while (enumerator.MoveNext())
		{
			collKeyword = enumerator.Current;
			if (collKeyword != null)
			{
				b = instance.GetCollKeyword(collKeyword.m_iIndex);
				if (b == b2 || b == b3)
				{
					m_validXlsDatas.Add(collKeyword);
				}
			}
		}
		Color color = GameGlobalUtil.HexToColor(0);
		if (categoryInfo.m_CompleteRate >= 100f)
		{
			color = GameGlobalUtil.HexToColor(2078092);
		}
		if (m_CompleteRateTitle != null)
		{
			m_CompleteRateTitle.text = ((categoryInfo == null) ? string.Empty : categoryInfo.Name);
			m_CompleteRateTitle.gameObject.SetActive(value: true);
		}
		if (m_CompleteRateValue != null)
		{
			m_CompleteRateValue.text = $"{((int)categoryInfo.m_CompleteRate/*cast due to .constrained prefix*/).ToString()}%";
			m_CompleteRateValue.color = color;
			m_CompleteRateValue.gameObject.SetActive(value: true);
		}
		if (m_CompleteRateValueBG != null)
		{
			m_CompleteRateValueBG.gameObject.SetActive(value: true);
		}
	}

	private void SetPageCount(int pageCount)
	{
		if (m_PageCount == pageCount)
		{
			return;
		}
		m_PageCount = pageCount;
		int num = m_PageCount - m_PageNodeImages.Count;
		if (num > 0)
		{
			GameObject gameObject = null;
			for (int i = 0; i < num; i++)
			{
				gameObject = UnityEngine.Object.Instantiate(m_ScrollNodeSrcObj);
				gameObject.GetComponent<RectTransform>().SetParent(m_ScrollNodeRT, worldPositionStays: false);
				gameObject.SetActive(value: false);
				m_PageNodeImages.Add(gameObject.GetComponent<Image>());
			}
		}
		int count = m_PageNodeImages.Count;
		int j = 0;
		Image image = null;
		for (; j < m_PageCount; j++)
		{
			image = m_PageNodeImages[j];
			image.sprite = m_NotSelectNodeImage;
			image.gameObject.SetActive(value: true);
		}
		for (; j < count; j++)
		{
			image = m_PageNodeImages[j];
			image.sprite = m_NotSelectNodeImage;
			image.gameObject.SetActive(value: false);
		}
		m_ScrollRoot.SetActive(m_PageCount > 1);
	}

	private void OnProc_ChangedCurrentPage(object sender, object args)
	{
		if (args is KeywordContainer.EventArg_ChangedCurrentPage eventArg_ChangedCurrentPage)
		{
			int num = ((m_PageNodeImages != null) ? m_PageNodeImages.Count : 0);
			if (eventArg_ChangedCurrentPage.m_prevPageIndex >= 0 && eventArg_ChangedCurrentPage.m_prevPageIndex < num)
			{
				m_PageNodeImages[eventArg_ChangedCurrentPage.m_prevPageIndex].sprite = m_NotSelectNodeImage;
			}
			if (eventArg_ChangedCurrentPage.m_currentPageIndex >= 0 && eventArg_ChangedCurrentPage.m_currentPageIndex < num)
			{
				m_PageNodeImages[eventArg_ChangedCurrentPage.m_currentPageIndex].sprite = m_SelectNodeImage;
			}
		}
	}

	private void ClearPageNodes()
	{
		int count = m_PageNodeImages.Count;
		GameObject gameObject = null;
		for (int i = 0; i < count; i++)
		{
			gameObject = m_PageNodeImages[i].gameObject;
			UnityEngine.Object.Destroy(gameObject);
		}
		m_PageNodeImages.Clear();
	}

	private void OnProc_OnCursorChanged(object sender, object args)
	{
		if (!(args is KeywordContainer.EventArg_OnCursorChanged eventArg_OnCursorChanged))
		{
			return;
		}
		bool flag = false;
		if (eventArg_OnCursorChanged.m_prevOnCursorSlot != null)
		{
			if (eventArg_OnCursorChanged.m_prevOnCursorSlot.isEnableNewMark)
			{
				eventArg_OnCursorChanged.m_prevOnCursorSlot.isEnableNewMark = false;
			}
			if (m_curCategoryInfo != null)
			{
				m_curCategoryInfo.ReflashLinkedTabButton();
			}
		}
		if (eventArg_OnCursorChanged.m_curOnCursorData != null)
		{
			Xls.CollKeyword curOnCursorData = eventArg_OnCursorChanged.m_curOnCursorData;
			if (curOnCursorData != null)
			{
				GameSwitch instance = GameSwitch.GetInstance();
				sbyte collKeyword = instance.GetCollKeyword(curOnCursorData.m_strKey);
				if (collKeyword == 1)
				{
					instance.SetCollKeyword(curOnCursorData.m_strKey, 2);
					if (m_curCategoryInfo != null)
					{
						m_curCategoryInfo.m_NewContentCount--;
					}
				}
				if (collKeyword == 1 || collKeyword == 2)
				{
					flag = Array.IndexOf(m_logViewEnableCategories, curOnCursorData.m_iCtg) >= 0;
				}
			}
		}
		m_OnCursorContent = eventArg_OnCursorChanged.m_curOnCursorSlot;
		SetKeywordDetailInfos(eventArg_OnCursorChanged.m_curOnCursorData);
		if (m_LogViewerRoot.activeSelf)
		{
			SetLogViewInfos(eventArg_OnCursorChanged.m_curOnCursorData);
		}
		if (m_LogViewerPadIconButton != null)
		{
			m_LogViewerPadIconButton.gameObject.SetActive(flag);
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentEnable(m_buttonGuide_LogViewer, flag);
		}
	}

	private void SetKeywordDetailInfos(Xls.CollKeyword xlsData)
	{
		m_KeywordDetailInfo.m_RootObject.SetActive(value: true);
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		if (xlsData != null)
		{
			GameSwitch instance = GameSwitch.GetInstance();
			sbyte b = 0;
			b = instance.GetCollKeyword(xlsData.m_iIndex);
			if (b != 1 && b != 2)
			{
				text = m_invalidateContentTitle;
				text2 = m_invalidateContentText;
			}
			else
			{
				Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(xlsData.m_strTitleID);
				if (data_byKey != null)
				{
					text = data_byKey.m_strTitle;
					text2 = data_byKey.m_strText;
				}
				text3 = xlsData.m_strImgID;
			}
		}
		m_KeywordDetailInfo.m_KeywordName.text = text;
		if (string.IsNullOrEmpty(text3))
		{
			m_KeywordDetailInfo.m_NoImageContentText.text = text2;
			m_KeywordDetailInfo.m_NoImageContentRoot.SetActive(value: true);
			m_KeywordDetailInfo.m_ImagedContentRoot.SetActive(value: false);
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentEnable(m_buttonGuide_ShowImage, isEnable: false);
			}
			return;
		}
		m_KeywordDetailInfo.m_ImagedContentText.text = text2;
		Sprite sprite = null;
		Xls.CollImages data_byKey2 = Xls.CollImages.GetData_byKey(text3);
		if (data_byKey2 != null)
		{
			Xls.ImageFile data_byKey3 = Xls.ImageFile.GetData_byKey(data_byKey2.m_strIDImg);
			if (data_byKey3 != null)
			{
				sprite = MainLoadThing.instance.colImageThumbnailManager.GetThumbnailImageInCache(data_byKey3.m_strAssetPath_Thumbnail);
			}
		}
		m_KeywordDetailInfo.m_ImagedContentImage.sprite = sprite;
		m_KeywordDetailInfo.m_ImageViewPadIconButton.gameObject.SetActive(sprite != null);
		m_KeywordDetailInfo.m_NoImageContentRoot.SetActive(value: false);
		m_KeywordDetailInfo.m_ImagedContentRoot.SetActive(value: true);
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ShowImage, isEnable: true);
		}
	}

	public void OnClickSlot(KeywordSlotPlus slot)
	{
		int keywordDataIndex = ((!(slot != null) || slot.xlsData == null) ? (-1) : m_keywordContainer.KeywordDatas.IndexOf(slot.xlsData));
		m_keywordContainer.SetOnCursorKeywordData(keywordDataIndex);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
	}

	public void OnClickLogButton()
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Detail");
		}
		SetLogViewInfos(m_keywordContainer.OnCursorData);
	}

	public void OnClickMovePage(bool isRight)
	{
		if (!m_isInputBlock && !m_TabContainer.isChaningTab && !ButtonPadInput.IsPlayingButPressAnim() && !PopupDialoguePlus.IsAnyPopupActivated() && !m_keywordContainer.IsPageScrolling())
		{
			m_keywordContainer.StartPageScroll(isRight ? KeywordContainer.SlotPage.Next : KeywordContainer.SlotPage.Prev, isOnCursorAtFirstSlot: true);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Scroll_Page");
			}
		}
	}

	private void SetLogViewInfos(Xls.CollKeyword xlsData)
	{
		m_LogViewerCloseAnimator = null;
		if (xlsData == null)
		{
			m_LogViewerRoot.SetActive(value: false);
			m_TabContainer.isInputBlock = false;
			m_ButtonGuide.SetShow(isShow: true);
			return;
		}
		m_LogViewerDetailInfo.m_RootObject.SetActive(value: true);
		m_LogViewerKeywordCard.sprite = m_OnCursorContent.m_imgSelSlot.sprite;
		m_LogViewerDetailInfo.m_KeywordName.text = m_KeywordDetailInfo.m_KeywordName.text;
		m_LogViewerDetailInfo.m_NoImageContentText.text = m_KeywordDetailInfo.m_NoImageContentText.text;
		m_LogViewerDetailInfo.m_ImagedContentText.text = m_KeywordDetailInfo.m_ImagedContentText.text;
		m_LogViewerDetailInfo.m_ImagedContentImage.sprite = m_KeywordDetailInfo.m_ImagedContentImage.sprite;
		m_LogViewerDetailInfo.m_NoImageContentRoot.SetActive(m_KeywordDetailInfo.m_NoImageContentRoot.activeSelf);
		m_LogViewerDetailInfo.m_ImagedContentRoot.SetActive(m_KeywordDetailInfo.m_ImagedContentRoot.activeSelf);
		m_LogViewerScrollRoot.SetActive(value: false);
		string[] array = new string[5] { xlsData.m_strReactionMin, xlsData.m_strReactionSeo, xlsData.m_strReactionOh, xlsData.m_strReactionLee, xlsData.m_strReactionChang };
		GameSwitch instance = GameSwitch.GetInstance();
		KeywordUseResultSlot keywordUseResultSlot = null;
		byte b = 0;
		int num = 0;
		int num2 = Mathf.Min(m_LogViewResultSlots.Length, array.Length);
		for (int i = 0; i < num2; i++)
		{
			keywordUseResultSlot = m_LogViewResultSlots[i];
			keywordUseResultSlot = m_LogViewResultSlots[num];
			keywordUseResultSlot.SetFaceImage(m_CharFaceImages[i]);
			if (instance.GetCollSelKeywod(i, xlsData.m_strKey) == 1)
			{
				keywordUseResultSlot.SetMentalImage(4);
			}
			else
			{
				b = instance.GetCollKeywordUse(i, xlsData.m_strKey);
				if (b == 0)
				{
					continue;
				}
				keywordUseResultSlot.SetMentalImage(b);
			}
			keywordUseResultSlot.gameObject.SetActive(value: true);
			if (keywordUseResultSlot.m_goCutKeyword != null)
			{
				keywordUseResultSlot.m_goCutKeyword.SetActive(instance.GetCollCutKeyword(i, xlsData.m_strKey) != 0);
			}
			num++;
		}
		num2 = m_LogViewResultSlots.Length;
		for (int j = num; j < num2; j++)
		{
			m_LogViewResultSlots[j].gameObject.SetActive(value: false);
		}
		m_LogViewResultEmpty.gameObject.SetActive(num == 0);
		m_LogViewerRoot.SetActive(value: true);
		m_ButtonGuide.SetShow(isShow: false);
		m_TabContainer.isInputBlock = true;
	}

	public void CloseLogViewer()
	{
		if (!(m_LogViewerCloseAnimator != null))
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Cancel");
			}
			m_LogViewerCloseAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_LogViewerRoot, GameDefine.UIAnimationState.disappear.ToString());
			m_ButtonGuide.SetShow(isShow: true);
		}
	}

	public IEnumerator OnProc_ViewImageDetail(KeywordSlotPlus content)
	{
		if (content == null)
		{
			yield break;
		}
		Xls.CollImages xlsCollectionImage = Xls.CollImages.GetData_byKey(content.xlsData.m_strImgID);
		if (xlsCollectionImage == null)
		{
			yield break;
		}
		if (string.IsNullOrEmpty(xlsCollectionImage.m_strIDColImageDest))
		{
			if (m_ImageViewer == null)
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
			m_ImageViewer.gameObject.SetActive(value: true);
			m_ImageViewer.ShowImage(isShow: true, spr, OnProc_ClosedViewImageDetail);
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
		m_isInputBlock = false;
		m_TabContainer.isInputBlock = m_LogViewerRoot.activeSelf;
		if (m_ImageViewer != null)
		{
			m_ImageViewer.gameObject.SetActive(value: false);
		}
		if (m_ButtonGuide != null && !m_LogViewerRoot.activeSelf)
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
			if (m_ChangingTabCover != null && m_TabContainer.isChaningTab)
			{
				m_TabContainer.CancelChangingState();
			}
			else if (m_ImageViewer.gameObject.activeInHierarchy)
			{
				m_ImageViewer.OnClickScreen();
			}
			else if (m_LogViewerRoot.activeSelf)
			{
				CloseLogViewer();
			}
			else
			{
				Close();
			}
		}
	}

	public void TouchScrollButton(bool isRightDir)
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClickMovePage(isRightDir);
		}
	}

	public void TouchLogButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClickLogButton();
		}
	}

	public void TouchCloseButton_LogViewer()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			CloseLogViewer();
		}
	}

	public void TouchImageViewButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			StartCoroutine(OnProc_ViewImageDetail(m_OnCursorContent));
		}
	}
}
