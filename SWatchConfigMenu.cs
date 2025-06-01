using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class SWatchConfigMenu : MonoBehaviour
{
	[Serializable]
	public class ContentMembers
	{
		public GameObject m_ContentRootObj;

		public Text m_ContentNumber;

		public GameObject m_NewMarkObj;

		public Text m_ContentName;

		public GameObject m_CheckedIconObj;

		public Text m_SelectedContentName;

		public Button m_ScrollLeftButton;

		public Button m_ScrollRightButton;

		public RectTransform m_ScrollNodeContainer;

		public GameObject m_ScrollNodeSrcObj;

		public Sprite m_ScrollNodeSpriteSelect;

		public Sprite m_ScrollNodeSpriteNotSelect;

		public Button m_ContentSelectButton;

		public Text m_ContentSelectButtonText;

		public Button m_ContentSelectPadIconButton;

		public GameObject m_ContentNotSelectButton;

		public Text m_ContentNotSelectButtonText;

		[NonSerialized]
		public List<Image> m_ScrollNodeImages = new List<Image>();

		[NonSerialized]
		public int m_SelectedIdx = -1;

		[NonSerialized]
		public int m_SelectionIdx = -1;

		[NonSerialized]
		public int m_newContentCount;

		[NonSerialized]
		public CommonTabButtonPlus m_linkedTabButton;

		public void ReflashLinkedTabButton()
		{
			if (m_linkedTabButton != null)
			{
				m_linkedTabButton.SetVisibleNewSymbol(m_newContentCount > 0);
			}
		}
	}

	[Serializable]
	public class ContentMembers_RingSound : ContentMembers
	{
		public Button m_SoundPlayButton;

		public Button m_SoundStopButton;

		public Button m_SoundPadIconButton;

		[NonSerialized]
		public List<Xls.CollSounds> m_xlsDatas = new List<Xls.CollSounds>();

		[NonSerialized]
		public int m_PlayingIdx = -1;
	}

	[Serializable]
	public class ContentMembers_WallPaper : ContentMembers
	{
		public Image m_WallPaperImage;

		[NonSerialized]
		public List<Xls.CollImages> m_xlsDatas = new List<Xls.CollImages>();
	}

	public enum Category
	{
		Unknown = -1,
		RingSound,
		WallPaper,
		Count
	}

	private delegate void SetSelectionIdx(int idx, bool isEnableCallBack);

	private delegate IEnumerator ienuSetSelectionIdx(int idx, bool isEnableCallBack);

	public GameObject m_RootObject;

	public Text m_TitleText;

	public CommonTabContainerPlus m_TabContainer;

	public GameObject m_ChangingTabCover;

	[Header("Ring Sound Content")]
	public ContentMembers_RingSound m_RingSoundContent = new ContentMembers_RingSound();

	[Header("Wallpaper Content")]
	public ContentMembers_WallPaper m_WallPaperContent = new ContentMembers_WallPaper();

	private AudioManager m_AudioManager;

	private CommonButtonGuide m_ButtonGuide;

	private bool m_isInputBlock;

	private bool m_isTutorialActivated;

	private Animator m_CloseCheckAnimator;

	private GameDefine.EventProc m_fpClosedFP;

	private GameDefine.EventProc m_fpChangedSelectContent;

	private GameDefine.ienuEventProc m_fpChangedWallpaper;

	private Category m_CurCategory = Category.Unknown;

	private string m_ContentNumberFormat = string.Empty;

	private string m_NameTag_RingSoundKor = string.Empty;

	private string m_NameTag_RingSoundJpn = string.Empty;

	private bool m_isInitializedButtonGuidText;

	private string m_buttonGuide_SelContent;

	private string m_buttonGuide_SubmitContent;

	private string m_buttonGuide_ExitMenu;

	private string m_buttonGuide_PlayStopRing;

	private string m_buttonGuide_ChangeTab;

	private string m_buttonGuide_SelectTab;

	private string m_buttonGuide_CancelTab;

	private static SWatchConfigMenu s_activedInstance;

	private const string c_assetBundleName = "prefabs/ingame/menu/ui_swatchconfigmenu";

	private static AssetBundleObjectHandler s_assetBundleHdr;

	public static SWatchConfigMenu instance => s_activedInstance;

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
	}

	private void OnDestroy()
	{
		m_AudioManager = null;
		m_ButtonGuide = null;
		m_CloseCheckAnimator = null;
		m_fpClosedFP = null;
		m_fpChangedSelectContent = null;
		m_fpChangedWallpaper = null;
		s_activedInstance = null;
		s_assetBundleHdr = null;
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
		}
		else if (!m_isTutorialActivated && !m_isInputBlock && !m_TabContainer.isChaningTab && !ButtonPadInput.IsPlayingButPressAnim() && !PopupDialoguePlus.IsAnyPopupActivated())
		{
			switch (m_CurCategory)
			{
			case Category.RingSound:
				PadInputUpdate_RingSound();
				break;
			case Category.WallPaper:
				PadInputUpdate_WallPaper();
				break;
			}
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				CloseSWatchConfigMenu();
			}
		}
	}

	private void PadInputUpdate_RingSound()
	{
		float y = Input.mouseScrollDelta.y;
		if (!GameGlobalUtil.IsAlmostSame(y, 0f))
		{
			OnClick_ScrollButton(y < 0f);
			return;
		}
		if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
		{
			OnClick_ScrollButton(isRight: false);
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_SelContent, isActivate: true);
			}
		}
		else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
		{
			OnClick_ScrollButton(isRight: true);
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_SelContent, isActivate: true);
			}
		}
		if (m_RingSoundContent.m_SelectionIdx < 0 || m_RingSoundContent.m_SelectionIdx >= m_RingSoundContent.m_ScrollNodeImages.Count)
		{
			return;
		}
		Button button = (m_RingSoundContent.m_SoundPlayButton.gameObject.activeSelf ? m_RingSoundContent.m_SoundPlayButton : ((!m_RingSoundContent.m_SoundStopButton.gameObject.activeSelf) ? null : m_RingSoundContent.m_SoundStopButton));
		if (GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton) && button != null)
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.SquareButton, m_RingSoundContent.m_SoundPadIconButton, button);
			OnClick_RingPlayStopButton(button == m_RingSoundContent.m_SoundPlayButton);
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_PlayStopRing, isActivate: true);
			}
		}
		if (m_RingSoundContent.m_ContentSelectButton.gameObject.activeSelf && GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) && m_RingSoundContent.m_SelectionIdx != m_RingSoundContent.m_SelectedIdx)
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_RingSoundContent.m_ContentSelectPadIconButton, m_RingSoundContent.m_ContentSelectButton);
			OnClick_SetContentToSwitch();
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_SubmitContent, isActivate: true);
			}
		}
	}

	private void PadInputUpdate_WallPaper()
	{
		float y = Input.mouseScrollDelta.y;
		if (!GameGlobalUtil.IsAlmostSame(y, 0f))
		{
			OnClick_ScrollButton(y < 0f);
			return;
		}
		if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
		{
			OnClick_ScrollButton(isRight: false);
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_SelContent, isActivate: true);
			}
		}
		else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
		{
			OnClick_ScrollButton(isRight: true);
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_SelContent, isActivate: true);
			}
		}
		if (m_WallPaperContent.m_ContentSelectButton.gameObject.activeSelf && m_WallPaperContent.m_SelectionIdx >= 0 && m_WallPaperContent.m_SelectionIdx < m_WallPaperContent.m_ScrollNodeImages.Count && GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_WallPaperContent.m_ContentSelectPadIconButton, m_WallPaperContent.m_ContentSelectButton);
			OnClick_SetContentToSwitch();
			if (m_fpChangedWallpaper != null)
			{
				MainLoadThing.instance.StartCoroutine(m_fpChangedWallpaper(this, null));
			}
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_SubmitContent, isActivate: true);
			}
		}
	}

	public static IEnumerator ShowSWConfigMenu_FormAssetBundle(GameDefine.EventProc fpClosed = null, GameDefine.EventProc fpChangedSelContent = null, GameDefine.ienuEventProc fpChangedWallpaper = null)
	{
		if (s_activedInstance == null)
		{
			s_activedInstance = (UnityEngine.Object.Instantiate(MainLoadThing.instance.m_prefabSWatchConfigMenu) as GameObject).GetComponent<SWatchConfigMenu>();
			yield return null;
		}
		s_activedInstance.ShowSWatchConfigMenu(fpClosed, fpChangedSelContent, fpChangedWallpaper);
	}

	public void ShowSWatchConfigMenu(GameDefine.EventProc fpClosed = null, GameDefine.EventProc fpChangedSelContent = null, GameDefine.ienuEventProc fpChangedWallpaper = null)
	{
		m_fpClosedFP = ((fpClosed == null) ? null : new GameDefine.EventProc(fpClosed.Invoke));
		m_fpChangedSelectContent = ((fpChangedSelContent == null) ? null : new GameDefine.EventProc(fpChangedSelContent.Invoke));
		m_fpChangedWallpaper = ((fpChangedWallpaper == null) ? null : fpChangedWallpaper);
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		Text[] textComps = new Text[11]
		{
			m_TitleText, m_RingSoundContent.m_ContentNumber, m_RingSoundContent.m_ContentName, m_RingSoundContent.m_SelectedContentName, m_RingSoundContent.m_ContentNotSelectButtonText, m_RingSoundContent.m_ContentSelectButtonText, m_WallPaperContent.m_ContentNumber, m_WallPaperContent.m_ContentName, m_WallPaperContent.m_SelectedContentName, m_WallPaperContent.m_ContentNotSelectButtonText,
			m_WallPaperContent.m_ContentSelectButtonText
		};
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_TitleText.text = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_TITLE");
		m_ContentNumberFormat = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_CONTENT_NUM_FORMAT");
		m_NameTag_RingSoundKor = GameGlobalUtil.GetXlsProgramDefineStr("RINGSOUND_TAG_KOR");
		m_NameTag_RingSoundJpn = GameGlobalUtil.GetXlsProgramDefineStr("RINGSOUND_TAG_JPN");
		InitButtonGuide();
		InitContent_RingSound();
		InitContent_WallPaper();
		if (m_TabContainer != null)
		{
			m_TabContainer.BuildTabButtonObjects();
			m_TabContainer.isInputBlock = false;
			List<CommonTabContainerPlus.TabButtonInfo>.Enumerator enumerator = m_TabContainer.tabButtonInfos.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CommonTabContainerPlus.TabButtonInfo current = enumerator.Current;
				if (current != null)
				{
					switch ((int)current.tag)
					{
					case 0:
						m_RingSoundContent.m_linkedTabButton = current.tabButtonComp;
						break;
					case 1:
						m_WallPaperContent.m_linkedTabButton = current.tabButtonComp;
						break;
					}
				}
			}
			m_RingSoundContent.ReflashLinkedTabButton();
			m_WallPaperContent.ReflashLinkedTabButton();
		}
		m_isInputBlock = false;
		if (m_TabContainer.tabButtonInfos.Count > 0)
		{
			m_TabContainer.SetSelectedTab_byIdx(0);
		}
	}

	public void CloseSWatchConfigMenu()
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Cancel");
		}
		m_isInputBlock = true;
		m_TabContainer.isInputBlock = true;
		m_CloseCheckAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
		if (m_CloseCheckAnimator == null)
		{
			CallClosedCallback();
		}
	}

	private void CallClosedCallback()
	{
		m_CloseCheckAnimator = null;
		m_TabContainer.ClearTabButtonObjects();
		if (m_RingSoundContent.m_PlayingIdx >= 0)
		{
			OnClick_RingPlayStopButton(isPlay: false);
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
		if (m_AudioManager != null && m_CurCategory == Category.RingSound)
		{
			m_AudioManager.RestoreGamePlaySound();
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
		m_buttonGuide_SelContent = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_BTNGUIDE_SEL_CONTENTS");
		m_buttonGuide_SubmitContent = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_BTNGUIDE_SUBMIT_CONTENT");
		m_buttonGuide_ExitMenu = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_BTNGUIDE_EXIT_MENU");
		m_buttonGuide_PlayStopRing = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_BTNGUIDE_PLAY_RING");
		m_buttonGuide_ChangeTab = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_BTNGUIDE_CHANGE_TAB");
		m_buttonGuide_SelectTab = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_BTNGUIDE_SUBMIT_TAB");
		m_buttonGuide_CancelTab = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_BTNGUIDE_CANCEL_TAB");
		m_isInitializedButtonGuidText = true;
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuide_SelContent, PadInput.GameInput.LStickX);
		m_ButtonGuide.AddContent(m_buttonGuide_SubmitContent, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ExitMenu, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_PlayStopRing, PadInput.GameInput.SquareButton);
		m_ButtonGuide.AddContent(m_buttonGuide_SelectTab, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_CancelTab, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.L1Button);
		m_ButtonGuide.AddContent(m_buttonGuide_ChangeTab, PadInput.GameInput.R1Button);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Left);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_PlayStopRing, isEnable: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_SelectTab, isEnable: false);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_CancelTab, isEnable: false);
		m_ButtonGuide.SetShow(isShow: true);
	}

	private List<CommonTabContainerPlus.TabCreateInfo> SetTabCreateInfos()
	{
		List<CommonTabContainerPlus.TabCreateInfo> list = new List<CommonTabContainerPlus.TabCreateInfo>();
		int num = 2;
		string[] array = new string[num];
		array[0] = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_TAB_RINGSOUND");
		array[1] = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_TAB_WALLPAPER");
		CommonTabContainerPlus.TabCreateInfo tabCreateInfo = null;
		for (int i = 0; i < num; i++)
		{
			tabCreateInfo = new CommonTabContainerPlus.TabCreateInfo();
			list.Add(tabCreateInfo);
			tabCreateInfo.m_Text = array[i];
			tabCreateInfo.m_Tag = i;
		}
		return list;
	}

	private void OnChangedSelectTab(object sender, object arg)
	{
		if (!(sender is CommonTabContainerPlus.TabButtonInfo tabButtonInfo))
		{
			return;
		}
		Category category = (Category)tabButtonInfo.tag;
		if (m_CurCategory != category && m_CurCategory == Category.RingSound && m_AudioManager != null)
		{
			m_AudioManager.RestoreGamePlaySound();
		}
		m_CurCategory = category;
		switch (m_CurCategory)
		{
		case Category.RingSound:
			m_RingSoundContent.m_ContentRootObj.SetActive(value: true);
			m_WallPaperContent.m_ContentRootObj.SetActive(value: false);
			SetSelectionContent_RingSound(m_RingSoundContent.m_SelectionIdx);
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentEnable(m_buttonGuide_PlayStopRing, isEnable: true);
			}
			if (m_AudioManager != null)
			{
				m_AudioManager.BackupCurGamePlaySound();
				m_AudioManager.SetBgVolHalfAndStopTheOtherSnds();
			}
			break;
		case Category.WallPaper:
			m_RingSoundContent.m_ContentRootObj.SetActive(value: false);
			m_WallPaperContent.m_ContentRootObj.SetActive(value: true);
			MainLoadThing.instance.StartCoroutine(SetSelectionContent_WallPaper(m_WallPaperContent.m_SelectionIdx));
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentEnable(m_buttonGuide_PlayStopRing, isEnable: false);
			}
			break;
		}
		if (m_fpChangedSelectContent != null)
		{
			m_fpChangedSelectContent(this, m_CurCategory);
		}
	}

	private void OnChangingSelectTab(object sender, object arg)
	{
		OnClick_RingPlayStopButton(isPlay: false);
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
			m_ButtonGuide.SetContentEnable(m_buttonGuide_SubmitContent, !flag);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ExitMenu, !flag);
			m_ButtonGuide.SetContentEnable(m_buttonGuide_PlayStopRing, !flag && m_CurCategory == Category.RingSound);
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

	private void InitContent_RingSound()
	{
		m_RingSoundContent.m_ContentSelectButtonText.text = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_SETRING_BUTTON");
		m_RingSoundContent.m_ContentNotSelectButtonText.text = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_CONTENT_SELECTED");
		m_RingSoundContent.m_xlsDatas.Clear();
		m_RingSoundContent.m_newContentCount = 0;
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		sbyte b = 0;
		bool flag = false;
		bool flag2 = false;
		Xls.CollSounds collSounds = null;
		List<Xls.CollSounds>.Enumerator enumerator = Xls.CollSounds.datas.GetEnumerator();
		while (enumerator.MoveNext())
		{
			collSounds = enumerator.Current;
			if (collSounds != null && (collSounds.m_iCategory == 3 || (collSounds.m_iCategory == 5 && collSounds.m_iDataType == 2)))
			{
				b = gameSwitch.GetSoundSwitch(collSounds.m_iIdx);
				flag = b == 1;
				if (flag || b == 2)
				{
					m_RingSoundContent.m_xlsDatas.Add(collSounds);
					m_RingSoundContent.m_newContentCount += (flag ? 1 : 0);
				}
			}
		}
		InitContent_Common(m_RingSoundContent, m_RingSoundContent.m_xlsDatas.Count);
		if (m_RingSoundContent.m_xlsDatas.Count <= 0)
		{
			return;
		}
		m_RingSoundContent.m_SelectedIdx = -1;
		m_RingSoundContent.m_SelectionIdx = -1;
		int idx = 0;
		int swRingtone = gameSwitch.GetSwRingtone();
		if (swRingtone >= 0)
		{
			collSounds = Xls.CollSounds.GetData_byIdx(swRingtone);
			if (collSounds != null)
			{
				idx = m_RingSoundContent.m_xlsDatas.IndexOf(collSounds);
			}
		}
		SetSelectedContentIdx(m_RingSoundContent, idx, isSetSwitch: false);
		SetSelectionContent_RingSound(m_RingSoundContent.m_SelectedIdx, isEnableCallBack: false);
	}

	private void InitContent_WallPaper()
	{
		m_WallPaperContent.m_ContentSelectButtonText.text = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_SETWALL_BUTTON");
		m_WallPaperContent.m_ContentNotSelectButtonText.text = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_CONTENT_SELECTED");
		m_WallPaperContent.m_SelectedContentName.text = string.Empty;
		m_WallPaperContent.m_ContentName.text = string.Empty;
		m_WallPaperContent.m_xlsDatas.Clear();
		m_WallPaperContent.m_newContentCount = 0;
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		sbyte b = 0;
		bool flag = false;
		bool flag2 = false;
		Xls.CollImages collImages = null;
		List<Xls.CollImages>.Enumerator enumerator = Xls.CollImages.datas.GetEnumerator();
		while (enumerator.MoveNext())
		{
			collImages = enumerator.Current;
			if (collImages != null && collImages.m_iCategory == 4)
			{
				b = gameSwitch.GetImageSwitch(collImages.m_iIdx);
				flag = b == 1;
				if (flag || b == 2)
				{
					m_WallPaperContent.m_xlsDatas.Add(collImages);
					m_WallPaperContent.m_newContentCount += (flag ? 1 : 0);
				}
			}
		}
		InitContent_Common(m_WallPaperContent, m_WallPaperContent.m_xlsDatas.Count);
		if (m_WallPaperContent.m_xlsDatas.Count <= 0)
		{
			return;
		}
		m_WallPaperContent.m_SelectedIdx = -1;
		m_WallPaperContent.m_SelectionIdx = -1;
		int idx = 0;
		int sWBackImage = gameSwitch.GetSWBackImage();
		if (sWBackImage >= 0)
		{
			collImages = Xls.CollImages.GetData_bySwitchIdx(sWBackImage);
			if (collImages != null)
			{
				idx = m_WallPaperContent.m_xlsDatas.IndexOf(collImages);
			}
		}
		SetSelectedContentIdx(m_WallPaperContent, idx, isSetSwitch: false);
		MainLoadThing.instance.StartCoroutine(SetSelectionContent_WallPaper(m_WallPaperContent.m_SelectedIdx, isEnableCallBack: false));
	}

	private void InitContent_Common(ContentMembers contentMembers, int contentCount)
	{
		int count = contentMembers.m_ScrollNodeImages.Count;
		for (int i = 0; i < count; i++)
		{
			UnityEngine.Object.Destroy(contentMembers.m_ScrollNodeImages[i].gameObject);
		}
		contentMembers.m_ScrollNodeImages.Clear();
		if (contentCount > 0)
		{
			contentMembers.m_ContentNumber.gameObject.SetActive(value: true);
			contentMembers.m_NewMarkObj.SetActive(value: true);
			contentMembers.m_ContentName.gameObject.SetActive(value: true);
			contentMembers.m_CheckedIconObj.SetActive(value: false);
			contentMembers.m_SelectedContentName.gameObject.SetActive(value: true);
			contentMembers.m_ScrollLeftButton.gameObject.SetActive(contentCount > 1);
			contentMembers.m_ScrollRightButton.gameObject.SetActive(contentCount > 1);
			contentMembers.m_ScrollNodeContainer.gameObject.SetActive(value: true);
			contentMembers.m_ScrollNodeSrcObj.SetActive(value: false);
			if (contentMembers.m_ContentSelectButton != null)
			{
				contentMembers.m_ContentSelectButton.gameObject.SetActive(value: true);
			}
			if (contentMembers.m_ContentNotSelectButton != null)
			{
				contentMembers.m_ContentNotSelectButton.gameObject.SetActive(value: false);
			}
			GameObject gameObject = null;
			Image image = null;
			RectTransform rectTransform = null;
			for (int j = 0; j < contentCount; j++)
			{
				gameObject = UnityEngine.Object.Instantiate(contentMembers.m_ScrollNodeSrcObj);
				image = gameObject.GetComponent<Image>();
				contentMembers.m_ScrollNodeImages.Add(image);
				image.sprite = contentMembers.m_ScrollNodeSpriteNotSelect;
				rectTransform = gameObject.GetComponent<RectTransform>();
				rectTransform.SetParent(contentMembers.m_ScrollNodeContainer);
				rectTransform.localScale = Vector3.one;
				gameObject.SetActive(value: true);
			}
		}
		else
		{
			contentMembers.m_ContentNumber.gameObject.SetActive(value: false);
			contentMembers.m_NewMarkObj.SetActive(value: false);
			contentMembers.m_ContentName.gameObject.SetActive(value: false);
			contentMembers.m_CheckedIconObj.SetActive(value: false);
			contentMembers.m_SelectedContentName.gameObject.SetActive(value: false);
			contentMembers.m_ScrollLeftButton.gameObject.SetActive(value: false);
			contentMembers.m_ScrollRightButton.gameObject.SetActive(value: false);
			contentMembers.m_ScrollNodeContainer.gameObject.SetActive(value: false);
			contentMembers.m_ContentSelectButton.gameObject.SetActive(value: false);
			contentMembers.m_ContentNotSelectButton.gameObject.SetActive(value: false);
		}
	}

	private void SetSelectionContent_RingSound(int idx, bool isEnableCallBack = true)
	{
		if (m_RingSoundContent.m_SelectionIdx == idx)
		{
			return;
		}
		if (m_RingSoundContent.m_NewMarkObj.activeSelf)
		{
			m_RingSoundContent.m_NewMarkObj.SetActive(value: false);
			m_RingSoundContent.ReflashLinkedTabButton();
		}
		Xls.CollSounds collSounds = ((idx < 0 || idx >= m_RingSoundContent.m_xlsDatas.Count) ? null : m_RingSoundContent.m_xlsDatas[idx]);
		if (collSounds != null)
		{
			m_RingSoundContent.m_ContentNumber.text = string.Format(m_ContentNumberFormat, idx + 1);
			GameSwitch gameSwitch = GameSwitch.GetInstance();
			bool flag = gameSwitch.GetSoundSwitch(collSounds.m_iIdx) == 1;
			m_RingSoundContent.m_NewMarkObj.SetActive(flag);
			if (flag)
			{
				gameSwitch.SetSoundSwitch(collSounds.m_iIdx, 2);
				m_RingSoundContent.m_newContentCount--;
			}
			Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(collSounds.m_strIDtext);
			m_RingSoundContent.m_ContentName.text = ((data_byKey == null) ? string.Empty : data_byKey.m_strTitle);
			m_RingSoundContent.m_ContentName.gameObject.SetActive(value: true);
			m_RingSoundContent.m_SoundPlayButton.gameObject.SetActive(m_RingSoundContent.m_PlayingIdx != idx);
			m_RingSoundContent.m_SoundStopButton.gameObject.SetActive(m_RingSoundContent.m_PlayingIdx == idx);
			m_RingSoundContent.m_SoundPadIconButton.gameObject.SetActive(value: true);
		}
		else
		{
			m_RingSoundContent.m_ContentNumber.gameObject.SetActive(value: false);
			m_RingSoundContent.m_NewMarkObj.SetActive(value: false);
			m_RingSoundContent.m_ContentName.gameObject.SetActive(value: false);
			m_RingSoundContent.m_SoundPlayButton.gameObject.SetActive(value: false);
			m_RingSoundContent.m_SoundStopButton.gameObject.SetActive(value: false);
			m_RingSoundContent.m_SoundPadIconButton.gameObject.SetActive(value: false);
		}
		SetSelectionContentIdx(m_RingSoundContent, idx, isEnableCallBack);
	}

	private IEnumerator SetSelectionContent_WallPaper(int idx, bool isEnableCallBack = true)
	{
		if (m_WallPaperContent.m_SelectionIdx == idx)
		{
			yield break;
		}
		if (m_WallPaperContent.m_NewMarkObj.activeSelf)
		{
			m_WallPaperContent.m_NewMarkObj.SetActive(value: false);
			m_WallPaperContent.ReflashLinkedTabButton();
		}
		Xls.CollImages xlsData = ((idx < 0 || idx >= m_WallPaperContent.m_xlsDatas.Count) ? null : m_WallPaperContent.m_xlsDatas[idx]);
		if (xlsData != null)
		{
			m_WallPaperContent.m_ContentNumber.text = string.Format(m_ContentNumberFormat, idx + 1);
			GameSwitch gameSwitch = GameSwitch.GetInstance();
			bool isNewMarkVisible = gameSwitch.GetImageSwitch(xlsData.m_iIdx) == 1;
			m_WallPaperContent.m_NewMarkObj.SetActive(isNewMarkVisible);
			if (isNewMarkVisible)
			{
				gameSwitch.SetImageSwitch(xlsData.m_iIdx, 2);
				m_WallPaperContent.m_newContentCount--;
			}
			Xls.TextListData xlsTextListData = Xls.TextListData.GetData_byKey(xlsData.m_strIDtext);
			m_WallPaperContent.m_ContentName.text = ((xlsTextListData == null) ? string.Empty : xlsTextListData.m_strTitle);
			m_WallPaperContent.m_ContentName.gameObject.SetActive(value: true);
			Xls.ImageFile xlsImageFileData = Xls.ImageFile.GetData_byKey(xlsData.m_strIDImg);
			Sprite sprTemp = null;
			string strPath = null;
			if (xlsImageFileData != null)
			{
				strPath = xlsImageFileData.m_strAssetPath;
				yield return MainLoadThing.instance.StartCoroutine(GameGlobalUtil.GetSprRequestFromImgPath(strPath));
				sprTemp = GameGlobalUtil.m_sprLoadFromImgXls;
				m_WallPaperContent.m_WallPaperImage.sprite = sprTemp;
			}
			m_WallPaperContent.m_WallPaperImage.gameObject.SetActive(value: true);
		}
		else
		{
			m_WallPaperContent.m_ContentNumber.gameObject.SetActive(value: false);
			m_WallPaperContent.m_NewMarkObj.SetActive(value: false);
			m_WallPaperContent.m_ContentName.gameObject.SetActive(value: false);
			m_WallPaperContent.m_WallPaperImage.gameObject.SetActive(value: false);
		}
		SetSelectionContentIdx(m_WallPaperContent, idx, isEnableCallBack);
		yield return null;
	}

	private void SetSelectionContentIdx(ContentMembers contentMembers, int idx, bool isEnableCallBack = true)
	{
		if (contentMembers.m_SelectionIdx != idx)
		{
			int count = contentMembers.m_ScrollNodeImages.Count;
			if (contentMembers.m_SelectionIdx >= 0 && contentMembers.m_SelectionIdx < count)
			{
				contentMembers.m_ScrollNodeImages[contentMembers.m_SelectionIdx].sprite = contentMembers.m_ScrollNodeSpriteNotSelect;
			}
			if (idx >= 0 && idx < count)
			{
				contentMembers.m_ScrollNodeImages[idx].sprite = contentMembers.m_ScrollNodeSpriteSelect;
			}
			else
			{
				idx = -1;
			}
			contentMembers.m_SelectionIdx = idx;
			contentMembers.m_CheckedIconObj.SetActive(contentMembers.m_SelectionIdx == contentMembers.m_SelectedIdx);
			contentMembers.m_ContentSelectButton.gameObject.SetActive(contentMembers.m_SelectionIdx != contentMembers.m_SelectedIdx);
			contentMembers.m_ContentNotSelectButton.gameObject.SetActive(contentMembers.m_SelectionIdx == contentMembers.m_SelectedIdx);
			if (isEnableCallBack && m_fpChangedSelectContent != null)
			{
				m_fpChangedSelectContent(this, m_CurCategory);
			}
		}
	}

	private void SetSelectedContentIdx(ContentMembers contentMembers, int idx, bool isSetSwitch = true)
	{
		if (contentMembers.m_SelectedIdx == idx)
		{
			return;
		}
		string text = string.Empty;
		string text2 = string.Empty;
		if (contentMembers == m_RingSoundContent)
		{
			text = "SWATCHCONFIG_MENU_CURRING_FORMAT";
			Xls.CollSounds collSounds = ((idx < 0 || idx >= m_RingSoundContent.m_xlsDatas.Count) ? null : m_RingSoundContent.m_xlsDatas[idx]);
			if (collSounds != null)
			{
				Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(collSounds.m_strIDtext);
				if (data_byKey != null)
				{
					text2 = data_byKey.m_strTitle;
				}
			}
		}
		else if (contentMembers == m_WallPaperContent)
		{
			text = "SWATCHCONFIG_MENU_CURWALL_FORMAT";
			Xls.CollImages collImages = ((idx < 0 || idx >= m_WallPaperContent.m_xlsDatas.Count) ? null : m_WallPaperContent.m_xlsDatas[idx]);
			if (collImages != null)
			{
				Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(collImages.m_strIDtext);
				if (data_byKey2 != null)
				{
					text2 = data_byKey2.m_strTitle;
				}
			}
		}
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
		{
			string xlsProgramText = GameGlobalUtil.GetXlsProgramText(text);
			contentMembers.m_SelectedContentName.text = string.Format(xlsProgramText, text2);
			contentMembers.m_SelectedContentName.gameObject.SetActive(value: true);
			contentMembers.m_CheckedIconObj.SetActive(value: true);
			if (isSetSwitch)
			{
				GameSwitch gameSwitch = GameSwitch.GetInstance();
				if (contentMembers == m_RingSoundContent)
				{
					gameSwitch.SetSWRingtone(m_RingSoundContent.m_xlsDatas[idx].m_strKey, isFromMenu: true);
				}
				else if (contentMembers == m_WallPaperContent)
				{
					gameSwitch.SetSWBackImage(m_WallPaperContent.m_xlsDatas[idx].m_strKey, isFromMenu: true);
				}
			}
		}
		else
		{
			contentMembers.m_SelectedContentName.gameObject.SetActive(value: false);
			contentMembers.m_CheckedIconObj.SetActive(value: false);
		}
		contentMembers.m_SelectedIdx = idx;
		contentMembers.m_ContentSelectButton.gameObject.SetActive(value: false);
		contentMembers.m_ContentNotSelectButton.gameObject.SetActive(value: true);
	}

	public void OnClick_ScrollButton(bool isRight)
	{
		ContentMembers contentMembers = null;
		SetSelectionIdx setSelectionIdx = null;
		ienuSetSelectionIdx ienuSetSelectionIdx = null;
		switch (m_CurCategory)
		{
		case Category.RingSound:
			contentMembers = m_RingSoundContent;
			setSelectionIdx = SetSelectionContent_RingSound;
			break;
		case Category.WallPaper:
			contentMembers = m_WallPaperContent;
			ienuSetSelectionIdx = SetSelectionContent_WallPaper;
			break;
		}
		int count = contentMembers.m_ScrollNodeImages.Count;
		if (count <= 1)
		{
			return;
		}
		if (m_CurCategory == Category.RingSound)
		{
			OnClick_RingPlayStopButton(isPlay: false);
		}
		int selectionIdx = contentMembers.m_SelectionIdx;
		if (!isRight)
		{
			selectionIdx--;
			if (selectionIdx < 0)
			{
				selectionIdx = count - 1;
			}
		}
		else
		{
			selectionIdx++;
			if (selectionIdx >= count)
			{
				selectionIdx = 0;
			}
		}
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		setSelectionIdx?.Invoke(selectionIdx, isEnableCallBack: true);
		if (ienuSetSelectionIdx != null)
		{
			MainLoadThing.instance.StartCoroutine(ienuSetSelectionIdx(selectionIdx, isEnableCallBack: true));
		}
	}

	public void OnClick_RingPlayStopButton(bool isPlay)
	{
		if (!(m_AudioManager == null) && m_RingSoundContent.m_SelectionIdx >= 0 && m_RingSoundContent.m_SelectionIdx < m_RingSoundContent.m_xlsDatas.Count)
		{
			int iChannel = 1;
			if (isPlay)
			{
				Xls.CollSounds xlsColSoundData = m_RingSoundContent.m_xlsDatas[m_RingSoundContent.m_SelectionIdx];
				PlayRingSound(xlsColSoundData, string.Empty);
				m_RingSoundContent.m_SoundPlayButton.gameObject.SetActive(value: false);
				m_RingSoundContent.m_SoundStopButton.gameObject.SetActive(value: true);
				m_RingSoundContent.m_PlayingIdx = m_RingSoundContent.m_SelectionIdx;
			}
			else
			{
				m_AudioManager.Stop(iChannel);
				m_RingSoundContent.m_SoundPlayButton.gameObject.SetActive(value: true);
				m_RingSoundContent.m_SoundStopButton.gameObject.SetActive(value: false);
				m_RingSoundContent.m_PlayingIdx = -1;
			}
		}
	}

	public void OnClick_SetContentToSwitch()
	{
		ContentMembers contentMembers = null;
		switch (m_CurCategory)
		{
		case Category.RingSound:
			contentMembers = m_RingSoundContent;
			break;
		case Category.WallPaper:
			contentMembers = m_WallPaperContent;
			break;
		}
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_OK");
		}
		SetSelectedContentIdx(contentMembers, contentMembers.m_SelectionIdx);
		if (m_fpChangedSelectContent != null)
		{
			m_fpChangedSelectContent(this, m_CurCategory);
		}
		GameSwitch.GetInstance().AddTrophyCnt("trp_00028");
	}

	public void GetCurrentSelectingTexts(out string tabText, out string selectedContentText, out string selectionContentText)
	{
		tabText = m_TabContainer.selectedTabInfo.tabButtonComp.text;
		ContentMembers contentMembers = null;
		switch (m_CurCategory)
		{
		case Category.RingSound:
			contentMembers = m_RingSoundContent;
			break;
		case Category.WallPaper:
			contentMembers = m_WallPaperContent;
			break;
		default:
			selectedContentText = string.Empty;
			selectionContentText = string.Empty;
			break;
		}
		selectedContentText = contentMembers.m_SelectedContentName.text;
		selectionContentText = contentMembers.m_ContentName.text;
	}

	private void ShowTutorialPopup()
	{
		string strTutorialKey = "tuto_00023";
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

	public void OnClickCloseSWatchConfigMenu()
	{
		if (MainLoadThing.instance.IsTouchableState() && !m_isTutorialActivated && !m_isInputBlock && !m_TabContainer.isChaningTab && !ButtonPadInput.IsPlayingButPressAnim() && !PopupDialoguePlus.IsAnyPopupActivated())
		{
			CloseSWatchConfigMenu();
		}
	}

	public void OnClickSelectRingToneMenu()
	{
		if (MainLoadThing.instance.IsTouchableState() && m_RingSoundContent.m_SelectionIdx >= 0 && m_RingSoundContent.m_SelectionIdx < m_RingSoundContent.m_ScrollNodeImages.Count)
		{
			OnClick_SetContentToSwitch();
		}
	}

	public void OnClickSelectWallPaper()
	{
		if (MainLoadThing.instance.IsTouchableState() && m_WallPaperContent.m_ContentSelectButton.gameObject.activeSelf && m_WallPaperContent.m_SelectionIdx >= 0 && m_WallPaperContent.m_SelectionIdx < m_WallPaperContent.m_ScrollNodeImages.Count)
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_WallPaperContent.m_ContentSelectPadIconButton, m_WallPaperContent.m_ContentSelectButton);
			OnClick_SetContentToSwitch();
			if (m_fpChangedWallpaper != null)
			{
				MainLoadThing.instance.StartCoroutine(m_fpChangedWallpaper(this, null));
			}
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_SubmitContent, isActivate: true);
			}
		}
	}

	public void OnClickPlayRignTone(bool isPlay)
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClick_RingPlayStopButton(isPlay);
		}
	}

	public void OnClickLeftRight(bool isLeft)
	{
		OnClick_ScrollButton(!isLeft);
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentActivate(m_buttonGuide_SelContent, isActivate: true);
		}
	}

	public static bool PlayRingSound(string collectionSoundKey, string channelName = "", float time = 0f, bool setVolumn = false, float volumn = 0f, bool loop = true)
	{
		if (string.IsNullOrEmpty(collectionSoundKey))
		{
			return false;
		}
		return PlayRingSound(Xls.CollSounds.GetData_byKey(collectionSoundKey), channelName, time, setVolumn, volumn, loop);
	}

	public static bool PlayRingSound(Xls.CollSounds xlsColSoundData, string channelName = "", float time = 0f, bool setVolumn = false, float volumn = 0f, bool loop = true)
	{
		if (xlsColSoundData == null)
		{
			return false;
		}
		AudioManager audioManager = GameGlobalUtil.GetAudioManager();
		if (audioManager == null)
		{
			return false;
		}
		int iChannel = (string.IsNullOrEmpty(channelName) ? 1 : audioManager.GetChannelIdx(channelName));
		string text = xlsColSoundData.m_strIDSnd;
		if (xlsColSoundData.m_iDataType == 2)
		{
			string text2 = string.Empty;
			switch (GameSwitch.GetInstance().GetVoiceLang())
			{
			case ConstGameSwitch.eVoiceLang.KOR:
				text2 = "RINGSOUND_TAG_KOR";
				break;
			case ConstGameSwitch.eVoiceLang.JPN:
				text2 = "RINGSOUND_TAG_JPN";
				break;
			}
			string text3 = string.Empty;
			if (!string.IsNullOrEmpty(text2))
			{
				text3 = GameGlobalUtil.GetXlsProgramDefineStr(text2);
			}
			text += text3;
		}
		audioManager.PlayKey(iChannel, text, setVolumn, time, volumn, loop);
		return true;
	}
}
