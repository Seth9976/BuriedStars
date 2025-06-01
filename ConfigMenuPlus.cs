using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class ConfigMenuPlus : SystemMenuBase
{
	public enum ContentType
	{
		TextLanguage,
		TypingSpeed,
		AutoDelay,
		VSync,
		ConfigToDefault,
		SaveDataReset,
		SubmitCancelButtonSwap,
		ScreenMode,
		ScreenResolution,
		ControlerType
	}

	[Serializable]
	public class ContentData
	{
		[NonSerialized]
		public ContentType m_ContentType;

		[NonSerialized]
		public ConfigContent_Base.ContentType m_ContentObjType;

		public string m_LabelXlsKey = string.Empty;

		public int m_ExtraInterval;

		public string m_NoticeXlsKey = string.Empty;
	}

	[Serializable]
	public class ContentDataToggle : ContentData
	{
		public string m_SubTextXlsKey = string.Empty;

		public string m_OnBtnTextXlsKey = string.Empty;

		public string m_OffBtnTextXlsKey = string.Empty;
	}

	[Serializable]
	public class ContentDataSlider : ContentData
	{
		public float m_ValueMax = 3f;

		public float m_ValueDelta = 1f;
	}

	[Serializable]
	public class ContentDataButton : ContentData
	{
		public string m_ButtonTextXlsKey = string.Empty;
	}

	[Serializable]
	public class ContentDataLanguageList : ContentData
	{
		public ConfigContent_ImageList.LanguageMemberData[] m_LanguageMemberDatas;
	}

	[Serializable]
	public class ContentDataTextList : ContentData
	{
		public ConfigContent_ImageList.TextMemberData[] m_TextMemberDatas;
	}

	public Text m_TitleText;

	public Text m_NoticeText;

	[Header("Content Container")]
	public RectTransform m_ContentContainerRT;

	public ConfigContent_Button m_SrcContent_Button;

	public ConfigContent_Toggle m_SrcContent_Toggle;

	public ConfigContent_Slide m_SrcContent_Slide;

	public ConfigContent_ImageList m_SrcContent_ImageList;

	public int m_ContentInterval;

	public bool m_CursorLoopEnable = true;

	[Header("Scroll Members")]
	public ScrollRect m_ScrollRect;

	public GameObject m_ScrollbarRoot;

	public float m_BottomSpacing = 80f;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	[Header("Content Datas")]
	public ContentDataLanguageList m_LanguageListContentData = new ContentDataLanguageList();

	public ContentDataSlider m_TypingSpeedContentData = new ContentDataSlider();

	public ContentDataSlider m_AutoDelayContentData = new ContentDataSlider();

	public ContentDataToggle m_VSyncContentData = new ContentDataToggle();

	public ContentDataTextList m_SubmitCancelButtonSwapData = new ContentDataTextList();

	public ContentDataButton m_ConfigToDefaultContentData = new ContentDataButton();

	public ContentDataButton m_SaveDataResetContentData = new ContentDataButton();

	public ContentDataTextList m_ScreenModeContentData = new ContentDataTextList();

	public ContentDataTextList m_ScreenResolutionContentData = new ContentDataTextList();

	public ContentDataTextList m_ControlerTypeContentData = new ContentDataTextList();

	public ContentType[] m_ContentsOrder;

	private ContentData[] m_ContentDatas;

	private List<ConfigContent_Base> m_Contents = new List<ConfigContent_Base>();

	private ConfigContent_Base m_OnCursorContent;

	private bool m_isInitailized;

	private bool m_isIgnoreValueChangedOnce;

	private SystemLanguage m_prevLanguage = SystemLanguage.Unknown;

	private float m_prevTypingSpeed;

	private float m_prevAutoDelay;

	private bool m_prevVSync;

	private GameSwitch.eButType m_prevOButtonUsage;

	private GameSwitch.eScreenMode m_prevScreenMode;

	private GameSwitch.eResolution m_prevResolution;

	private GameSwitch.eControllerType m_prevControllerType;

	private bool m_isNeedAskSaveData = true;

	private bool m_isForceSaveData;

	private bool m_isResetedSaveData;

	private bool m_isEnableCloseDirection;

	private bool m_isChangedLanguage;

	private bool m_isPageScrolling;

	public bool IsChangedLanguage => m_isChangedLanguage;

	protected override void Awake()
	{
		base.Awake();
		if (m_SrcContent_Button != null)
		{
			m_SrcContent_Button.gameObject.SetActive(value: false);
		}
		if (m_SrcContent_Toggle != null)
		{
			m_SrcContent_Toggle.gameObject.SetActive(value: false);
		}
		if (m_SrcContent_Slide != null)
		{
			m_SrcContent_Slide.gameObject.SetActive(value: false);
		}
		if (m_SrcContent_ImageList != null)
		{
			m_SrcContent_ImageList.gameObject.SetActive(value: false);
		}
		m_LanguageListContentData.m_ContentObjType = ConfigContent_Base.ContentType.LanguageList;
		m_TypingSpeedContentData.m_ContentObjType = ConfigContent_Base.ContentType.Silde;
		m_TypingSpeedContentData.m_ValueMax = 4f;
		m_AutoDelayContentData.m_ContentObjType = ConfigContent_Base.ContentType.Silde;
		m_AutoDelayContentData.m_ValueMax = 4f;
		m_VSyncContentData.m_ContentObjType = ConfigContent_Base.ContentType.OnOff;
		m_SubmitCancelButtonSwapData.m_ContentObjType = ConfigContent_Base.ContentType.TextList;
		m_ConfigToDefaultContentData.m_ContentObjType = ConfigContent_Base.ContentType.Button;
		m_SaveDataResetContentData.m_ContentObjType = ConfigContent_Base.ContentType.Button;
		m_ScreenModeContentData.m_ContentObjType = ConfigContent_Base.ContentType.TextList;
		m_ScreenResolutionContentData.m_ContentObjType = ConfigContent_Base.ContentType.TextList;
		m_ControlerTypeContentData.m_ContentObjType = ConfigContent_Base.ContentType.TextList;
		if (m_ContentsOrder == null || m_ContentsOrder.Length <= 0)
		{
			return;
		}
		List<ContentData> list = new List<ContentData>();
		ContentData contentData = null;
		for (int i = 0; i < m_ContentsOrder.Length; i++)
		{
			switch (m_ContentsOrder[i])
			{
			case ContentType.TextLanguage:
				contentData = m_LanguageListContentData;
				break;
			case ContentType.TypingSpeed:
				contentData = m_TypingSpeedContentData;
				break;
			case ContentType.AutoDelay:
				contentData = m_AutoDelayContentData;
				break;
			case ContentType.VSync:
				contentData = m_VSyncContentData;
				break;
			case ContentType.ConfigToDefault:
				contentData = m_ConfigToDefaultContentData;
				break;
			case ContentType.SaveDataReset:
				contentData = m_SaveDataResetContentData;
				break;
			case ContentType.SubmitCancelButtonSwap:
				contentData = m_SubmitCancelButtonSwapData;
				break;
			case ContentType.ScreenMode:
				contentData = m_ScreenModeContentData;
				break;
			case ContentType.ScreenResolution:
				contentData = m_ScreenResolutionContentData;
				break;
			case ContentType.ControlerType:
				contentData = m_ControlerTypeContentData;
				break;
			}
			if (contentData != null)
			{
				contentData.m_ContentType = m_ContentsOrder[i];
				list.Add(contentData);
			}
		}
		m_ContentDatas = list.ToArray();
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Vertical, m_ScrollRect, m_ContentContainerRT);
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (m_ScrollHandler != null)
		{
			m_ScrollHandler.ReleaseScroll();
		}
		m_ContentDatas = null;
		m_Contents = null;
		m_OnCursorContent = null;
	}

	protected override void Update()
	{
		base.Update();
		if (m_DisappearCheckAnimator != null)
		{
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
		if (m_isInputBlock || PopupDialoguePlus.IsAnyPopupActivated())
		{
			return;
		}
		Vector2 lStickMove = GamePadInput.GetLStickMove();
		lStickMove.Normalize();
		if (Mathf.Abs(lStickMove.y) >= 0.5f)
		{
			if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				ChangeOnCursorContent(isUpSide: true);
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				ChangeOnCursorContent(isUpSide: false);
			}
		}
		else
		{
			float axisValue = GamePadInput.GetAxisValue(PadInput.GameInput.RStickY);
			bool flag2 = false;
			if (Mathf.Abs(axisValue) >= 0.5f)
			{
				if (m_ScrollHandler.scrollPos <= 0f && axisValue < 0f && m_OnCursorContent != m_Contents[0])
				{
					SetOnCursorContent(m_Contents[0]);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
				}
				else if (m_ScrollHandler.scrollPos >= m_ScrollHandler.scrollPos_Max && axisValue > 0f && m_OnCursorContent != m_Contents[m_Contents.Count - 1])
				{
					SetOnCursorContent(m_Contents[m_Contents.Count - 1]);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
				}
				else
				{
					m_ScrollHandler.ScrollByDirection(axisValue > 0f, Mathf.Abs(axisValue));
					flag2 = true;
				}
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
					axisValue = 0f - GamePadInput.GetMouseWheelScrollDelta();
					if (axisValue > 0.9f)
					{
						m_isPageScrolling = ChangeScrollPage(isUpSide: false);
					}
					else if (axisValue < -0.9f)
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
		if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
		{
			if (m_OutterExitButton != null)
			{
				ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_OutterExitButton, null, null, null, null, isShowAnim: true, isExcuteEvent: false);
			}
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Cancel");
			}
			Close();
		}
	}

	public override void Show(bool isEnableAnimation = true, GameDefine.EventProc fpClosedCB = null, bool isNeedSetCloseCB = true, Button outterExitButton = null)
	{
		m_isInitailized = false;
		base.Show(isEnableAnimation, fpClosedCB, isNeedSetCloseCB, outterExitButton);
		InitContents();
		ShowContents(isEnableAnimation: true, isPrevBackup: true);
		ResetXlsTexts();
		m_isInputBlock = true;
		m_isChangedLanguage = false;
		m_isNeedAskSaveData = true;
		m_isForceSaveData = false;
		m_isResetedSaveData = false;
		m_isEnableCloseDirection = false;
		int count = m_Contents.Count;
		if (count > 0)
		{
			SetOnCursorContent(m_Contents[0]);
		}
		for (int i = 0; i < count; i++)
		{
			m_Contents[i].AudioManager = m_AudioManager;
		}
		StartCoroutine(AlignContents(!m_isInFromMainMenu));
	}

	public override void ResetXlsTexts()
	{
		Text[] textComps = new Text[2] { m_TitleText, m_NoticeText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		if (m_TitleText != null)
		{
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText("GAME_CONFIG_MENU_TITLE");
		}
		if (m_NoticeText != null)
		{
			m_NoticeText.text = GameGlobalUtil.GetXlsProgramText("GAME_CONFIG_MENU_NOTICE");
		}
		int count = m_Contents.Count;
		for (int i = 0; i < count; i++)
		{
			SetContentText(i);
		}
	}

	public override void Close(bool isEnableAnimation = true)
	{
		m_isInputBlock = true;
		m_isEnableCloseDirection = isEnableAnimation;
		if (m_isForceSaveData)
		{
			SaveLoad.GetInstance().SaveLoadWhat(SaveLoad.eSaveWhat.eSaveOptConfig, 0, OnComplete_SaveAndClose);
			return;
		}
		GameSwitch instance = GameSwitch.GetInstance();
		if (m_prevLanguage != instance.GetCurSubtitleLanguage() || Mathf.RoundToInt(m_prevTypingSpeed) != instance.GetTypingEff() || !GameGlobalUtil.IsAlmostSame(m_prevAutoDelay, instance.GetAutoDelayTime()) || m_prevVSync != instance.GetVSyncOn() || m_prevOButtonUsage != (GameSwitch.eButType)instance.GetOXType() || m_prevScreenMode != instance.GetScreenMode() || m_prevResolution != instance.GetResolution() || m_prevControllerType != instance.GetControllerType())
		{
			if (m_isNeedAskSaveData)
			{
				PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("GAME_CONFIG_POPUP_ENABLE_CHANGED"), PopupResult_EnableChanged);
			}
			else
			{
				SaveLoad.GetInstance().SaveLoadWhat(SaveLoad.eSaveWhat.eSaveOptConfig, 0, OnComplete_SaveAndClose);
			}
		}
		else
		{
			base.Close(isEnableAnimation);
		}
	}

	protected override void Closed()
	{
		base.gameObject.SetActive(value: false);
		if (m_fpClosedCB != null)
		{
			m_fpClosedCB(this, m_isResetedSaveData);
		}
	}

	private void PopupResult_EnableChanged(PopupDialoguePlus.Result result)
	{
		if (result == PopupDialoguePlus.Result.Yes)
		{
			SaveLoad.GetInstance().SaveLoadWhat(SaveLoad.eSaveWhat.eSaveOptConfig, 0, OnComplete_SaveAndClose);
			return;
		}
		GameSwitch instance = GameSwitch.GetInstance();
		if (m_prevLanguage != instance.GetCurSubtitleLanguage())
		{
			instance.SetOptLang(m_prevLanguage);
		}
		instance.SetTypingEff((byte)Mathf.RoundToInt(m_prevTypingSpeed));
		instance.SetAutoDelayTime(Mathf.RoundToInt(m_prevAutoDelay));
		instance.SetVSyncOn(m_prevVSync);
		if (instance.GetOXType() != (int)m_prevOButtonUsage)
		{
			instance.SetOXType(m_prevOButtonUsage);
			ReflashOXChangeButtons();
		}
		if (instance.GetScreenMode() != m_prevScreenMode)
		{
			instance.SetScreenMode(m_prevScreenMode);
		}
		if (instance.GetResolution() != m_prevResolution)
		{
			instance.SetResolution(m_prevResolution);
		}
		if (instance.GetControllerType() != m_prevControllerType)
		{
			instance.SetControllerType(m_prevControllerType);
		}
		base.Close();
	}

	private void OnComplete_SaveAndClose(bool isExistErr)
	{
		SystemLanguage curSubtitleLanguage = GameSwitch.GetInstance().GetCurSubtitleLanguage();
		if (m_prevLanguage != curSubtitleLanguage)
		{
			LoadingScreen.Show();
			MainLoadThing.instance.StartCoroutine(XlsDataHandler.SetCurrentLanguage(curSubtitleLanguage, clearPrevLanguageText: false, OnComplete_LoadLanguagePack));
		}
		else
		{
			base.Close(m_isEnableCloseDirection);
		}
	}

	private void OnComplete_LoadLanguagePack(object sender, object args)
	{
		m_isChangedLanguage = true;
		LoadingScreen.Close();
		base.Close(m_isEnableCloseDirection);
	}

	private void InitContents()
	{
		ClearContents();
		if (m_ContentContainerRT == null || m_SrcContent_Slide == null || m_SrcContent_Toggle == null || m_SrcContent_Button == null || m_SrcContent_ImageList == null)
		{
			return;
		}
		int num = m_ContentDatas.Length;
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		ConfigContent_Base configContent_Base = null;
		ContentData contentData = null;
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			contentData = m_ContentDatas[i];
			switch (contentData.m_ContentObjType)
			{
			case ConfigContent_Base.ContentType.Silde:
				gameObject = m_SrcContent_Slide.gameObject;
				break;
			case ConfigContent_Base.ContentType.OnOff:
				gameObject = m_SrcContent_Toggle.gameObject;
				break;
			case ConfigContent_Base.ContentType.Button:
				gameObject = m_SrcContent_Button.gameObject;
				break;
			case ConfigContent_Base.ContentType.ImageList:
			case ConfigContent_Base.ContentType.LanguageList:
			case ConfigContent_Base.ContentType.TextList:
				gameObject = m_SrcContent_ImageList.gameObject;
				break;
			default:
				gameObject = null;
				break;
			}
			gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			gameObject2.name = $"Content_{i}";
			configContent_Base = gameObject2.GetComponent<ConfigContent_Base>();
			m_Contents.Add(configContent_Base);
			configContent_Base.contentType = contentData.m_ContentObjType;
			configContent_Base.RectTransform.SetParent(m_ContentContainerRT, worldPositionStays: false);
			configContent_Base.RectTransform.anchoredPosition = new Vector2(configContent_Base.RectTransform.anchoredPosition.x, num2);
			num2 -= configContent_Base.RectTransform.rect.height + (float)m_ContentInterval + (float)contentData.m_ExtraInterval;
			if (configContent_Base is ConfigContent_Slide)
			{
				ConfigContent_Slide configContent_Slide = configContent_Base as ConfigContent_Slide;
				ContentDataSlider contentDataSlider = contentData as ContentDataSlider;
				configContent_Slide.CurValue = 0f;
				configContent_Slide.MinValue = 0f;
				configContent_Slide.MaxValue = contentDataSlider.m_ValueMax;
				configContent_Slide.DeltaValue = contentDataSlider.m_ValueDelta;
			}
			else if (configContent_Base is ConfigContent_ImageList)
			{
				ConfigContent_ImageList configContent_ImageList = configContent_Base as ConfigContent_ImageList;
				if (contentData.m_ContentObjType == ConfigContent_Base.ContentType.LanguageList)
				{
					ContentDataLanguageList contentDataLanguageList = contentData as ContentDataLanguageList;
					int num3 = ((contentDataLanguageList.m_LanguageMemberDatas != null) ? contentDataLanguageList.m_LanguageMemberDatas.Length : 0);
					if (num3 > 0)
					{
						ConfigContent_ImageList.ItemData[] array = new ConfigContent_ImageList.ItemData[num3];
						for (int j = 0; j < num3; j++)
						{
							ConfigContent_ImageList.ItemData itemData = (array[j] = new ConfigContent_ImageList.ItemData());
							itemData.m_Tag = contentDataLanguageList.m_LanguageMemberDatas[j].m_Language;
							itemData.m_SpriteAsset = contentDataLanguageList.m_LanguageMemberDatas[j].m_SpriteAsset;
						}
						configContent_ImageList.ItemDatas = array;
					}
				}
				else if (contentData.m_ContentObjType == ConfigContent_Base.ContentType.TextList)
				{
					ContentDataTextList contentDataTextList = contentData as ContentDataTextList;
					if (contentData.m_ContentType == ContentType.SubmitCancelButtonSwap)
					{
						ConfigContent_ImageList.ItemData[] array2 = new ConfigContent_ImageList.ItemData[2]
						{
							new ConfigContent_ImageList.ItemData(),
							null
						};
						array2[0].m_Tag = GameSwitch.eButType.OEnter;
						array2[1] = new ConfigContent_ImageList.ItemData();
						array2[1].m_Tag = GameSwitch.eButType.XEnter;
						configContent_ImageList.ItemDatas = array2;
					}
					else
					{
						int num4 = ((contentDataTextList.m_TextMemberDatas != null) ? contentDataTextList.m_TextMemberDatas.Length : 0);
						if (num4 > 0)
						{
							ConfigContent_ImageList.ItemData[] array3 = new ConfigContent_ImageList.ItemData[num4];
							for (int k = 0; k < num4; k++)
							{
								ConfigContent_ImageList.TextMemberData textMemberData = contentDataTextList.m_TextMemberDatas[k];
								(array3[k] = new ConfigContent_ImageList.ItemData()).m_Tag = textMemberData.m_Value;
							}
							configContent_ImageList.ItemDatas = array3;
						}
					}
				}
			}
			configContent_Base.TagFlag = contentData.m_ContentType;
			configContent_Base.ContentData = contentData;
			configContent_Base.OnEventNotice = OnEventNotice_Content;
			configContent_Base.ParentMenu = this;
			gameObject2.SetActive(value: false);
		}
	}

	private void ShowContents(bool isEnableAnimation = true, bool isPrevBackup = false)
	{
		if (m_Contents == null)
		{
			return;
		}
		GameSwitch instance = GameSwitch.GetInstance();
		int count = m_Contents.Count;
		ConfigContent_Base configContent_Base = null;
		for (int i = 0; i < count; i++)
		{
			configContent_Base = m_Contents[i];
			if (configContent_Base == null)
			{
				continue;
			}
			configContent_Base.gameObject.SetActive(value: true);
			switch ((ContentType)configContent_Base.TagFlag)
			{
			case ContentType.TextLanguage:
				if (configContent_Base is ConfigContent_ImageList)
				{
					ConfigContent_ImageList configContent_ImageList4 = configContent_Base as ConfigContent_ImageList;
					configContent_ImageList4.SetCurrentItemByTag(instance.GetCurSubtitleLanguage());
					if (isPrevBackup)
					{
						m_prevLanguage = instance.GetCurSubtitleLanguage();
					}
				}
				break;
			case ContentType.TypingSpeed:
				if (configContent_Base is ConfigContent_Slide)
				{
					ConfigContent_Slide configContent_Slide = configContent_Base as ConfigContent_Slide;
					configContent_Slide.m_ValueTextOffset = 1;
					configContent_Slide.CurValue = (int)instance.GetTypingEff();
					if (isPrevBackup)
					{
						m_prevTypingSpeed = configContent_Slide.CurValue;
					}
				}
				break;
			case ContentType.AutoDelay:
				if (configContent_Base is ConfigContent_Slide)
				{
					ConfigContent_Slide configContent_Slide2 = configContent_Base as ConfigContent_Slide;
					configContent_Slide2.m_ValueTextOffset = 1;
					configContent_Slide2.CurValue = instance.GetAutoDelayTime();
					if (isPrevBackup)
					{
						m_prevAutoDelay = configContent_Slide2.CurValue;
					}
				}
				break;
			case ContentType.VSync:
				if (configContent_Base is ConfigContent_Toggle)
				{
					ConfigContent_Toggle configContent_Toggle = configContent_Base as ConfigContent_Toggle;
					configContent_Toggle.ToggleOn = instance.GetVSyncOn();
					if (isPrevBackup)
					{
						m_prevVSync = configContent_Toggle.ToggleOn;
					}
				}
				break;
			case ContentType.SubmitCancelButtonSwap:
				if (configContent_Base is ConfigContent_ImageList)
				{
					ConfigContent_ImageList configContent_ImageList2 = configContent_Base as ConfigContent_ImageList;
					GameSwitch.eButType oXType = (GameSwitch.eButType)GameSwitch.GetInstance().GetOXType();
					configContent_ImageList2.SetCurrentItemByTag(oXType);
					if (isPrevBackup)
					{
						m_prevOButtonUsage = oXType;
					}
				}
				break;
			case ContentType.ScreenMode:
				if (configContent_Base is ConfigContent_ImageList)
				{
					ConfigContent_ImageList configContent_ImageList5 = configContent_Base as ConfigContent_ImageList;
					GameSwitch.eScreenMode screenMode = GameSwitch.GetInstance().GetScreenMode();
					configContent_ImageList5.SetCurrentItemByTag(screenMode.ToString());
					if (isPrevBackup)
					{
						m_prevScreenMode = screenMode;
					}
				}
				break;
			case ContentType.ScreenResolution:
				if (configContent_Base is ConfigContent_ImageList)
				{
					ConfigContent_ImageList configContent_ImageList3 = configContent_Base as ConfigContent_ImageList;
					GameSwitch.eResolution resolution = GameSwitch.GetInstance().GetResolution();
					configContent_ImageList3.SetCurrentItemByTag(resolution.ToString());
					if (isPrevBackup)
					{
						m_prevResolution = resolution;
					}
				}
				break;
			case ContentType.ControlerType:
				if (configContent_Base is ConfigContent_ImageList)
				{
					ConfigContent_ImageList configContent_ImageList = configContent_Base as ConfigContent_ImageList;
					GameSwitch.eControllerType controllerType = GameSwitch.GetInstance().GetControllerType();
					configContent_ImageList.SetCurrentItemByTag(controllerType.ToString());
					if (isPrevBackup)
					{
						m_prevControllerType = controllerType;
					}
				}
				break;
			}
		}
	}

	private IEnumerator AlignContents(bool isDisableSaveResetContent)
	{
		int count = m_Contents.Count;
		ConfigContent_Base content = null;
		ConfigContent_Base contentSaveReset = ((!isDisableSaveResetContent) ? null : GetContent(ContentType.SaveDataReset));
		float curPos = 0f;
		for (int i = 0; i < count; i++)
		{
			content = m_Contents[i];
			if (content == null || contentSaveReset == content)
			{
				continue;
			}
			if (!content.Initailized)
			{
				if (!content.gameObject.activeSelf)
				{
					content.gameObject.SetActive(value: true);
				}
				yield return null;
			}
			content.RectTransform.anchoredPosition = new Vector2(content.RectTransform.anchoredPosition.x, curPos);
			curPos -= content.RectTransform.rect.height + (float)m_ContentInterval;
		}
		float totalContentHeight = 0f - (curPos - (float)m_ContentInterval) + m_BottomSpacing;
		m_ContentContainerRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalContentHeight);
		m_ScrollHandler.ResetScrollRange();
		if (contentSaveReset != null)
		{
			contentSaveReset.gameObject.SetActive(value: false);
		}
		m_isInputBlock = false;
		m_isInitailized = true;
	}

	private ConfigContent_Base GetContent(ContentType type)
	{
		foreach (ConfigContent_Base content in m_Contents)
		{
			if (!(content == null))
			{
				ContentType contentType = (ContentType)content.TagFlag;
				if (contentType == type)
				{
					return content;
				}
			}
		}
		return null;
	}

	private void ClearContents()
	{
		if (m_Contents == null)
		{
			return;
		}
		int count = m_Contents.Count;
		ConfigContent_Base configContent_Base = null;
		for (int i = 0; i < count; i++)
		{
			configContent_Base = m_Contents[i];
			if (!(configContent_Base == null))
			{
				UnityEngine.Object.Destroy(configContent_Base.gameObject);
				m_Contents[i] = null;
			}
		}
		m_Contents.Clear();
	}

	private void SetContentText(int contentIdx)
	{
		int a = ((m_ContentDatas != null) ? m_ContentDatas.Length : 0);
		int b = ((m_Contents != null) ? m_Contents.Count : 0);
		int num = Mathf.Min(a, b);
		if (contentIdx < 0 || contentIdx >= num)
		{
			return;
		}
		ContentData contentData = m_ContentDatas[contentIdx];
		ConfigContent_Base configContent_Base = m_Contents[contentIdx];
		configContent_Base.ResetFontByCurrentLanguage();
		configContent_Base.Title = GameGlobalUtil.GetXlsProgramText(contentData.m_LabelXlsKey);
		configContent_Base.NoticeText = (string.IsNullOrEmpty(contentData.m_NoticeXlsKey) ? string.Empty : GameGlobalUtil.GetXlsProgramText(contentData.m_NoticeXlsKey));
		switch (contentData.m_ContentObjType)
		{
		case ConfigContent_Base.ContentType.Silde:
			break;
		case ConfigContent_Base.ContentType.OnOff:
		{
			ContentDataToggle contentDataToggle = contentData as ContentDataToggle;
			ConfigContent_Toggle configContent_Toggle = configContent_Base as ConfigContent_Toggle;
			configContent_Toggle.LeftButtonText = GameGlobalUtil.GetXlsProgramText(contentDataToggle.m_OnBtnTextXlsKey);
			configContent_Toggle.RightButtonText = GameGlobalUtil.GetXlsProgramText(contentDataToggle.m_OffBtnTextXlsKey);
			break;
		}
		case ConfigContent_Base.ContentType.Button:
		{
			ContentDataButton contentDataButton = contentData as ContentDataButton;
			ConfigContent_Button configContent_Button = configContent_Base as ConfigContent_Button;
			configContent_Button.ButtonText = GameGlobalUtil.GetXlsProgramText(contentDataButton.m_ButtonTextXlsKey);
			break;
		}
		case ConfigContent_Base.ContentType.ImageList:
			break;
		case ConfigContent_Base.ContentType.TextList:
		{
			ContentDataTextList contentDataTextList = contentData as ContentDataTextList;
			ConfigContent_ImageList configContent_ImageList = configContent_Base as ConfigContent_ImageList;
			if ((ContentType)configContent_Base.TagFlag == ContentType.SubmitCancelButtonSwap)
			{
				if (configContent_ImageList.ItemDatas == null || configContent_ImageList.ItemDatas.Length != 2)
				{
					configContent_ImageList.ItemDatas = new ConfigContent_ImageList.ItemData[2];
				}
				bool flag = GamePadInput.CurrentGamePadType != PadInput_Steam.GamePadType.PlayStation;
				configContent_ImageList.ItemDatas[0].m_Text = GameGlobalUtil.GetXlsProgramText((!flag) ? "GAME_CONFIG_PARAM_SUBMIT" : "GAME_CONFIG_PARAM_SUBMIT_XBOX");
				configContent_ImageList.ItemDatas[1].m_Text = GameGlobalUtil.GetXlsProgramText((!flag) ? "GAME_CONFIG_PARAM_CANCEL" : "GAME_CONFIG_PARAM_CANCEL_XBOX");
				configContent_ImageList.SetCurrentItemIndex(configContent_ImageList.SelectedIndex);
				break;
			}
			int a2 = ((contentDataTextList.m_TextMemberDatas != null) ? contentDataTextList.m_TextMemberDatas.Length : 0);
			int b2 = ((configContent_ImageList.ItemDatas != null) ? configContent_ImageList.ItemDatas.Length : 0);
			int num2 = Mathf.Min(a2, b2);
			for (int i = 0; i < num2; i++)
			{
				ConfigContent_ImageList.TextMemberData textMemberData = contentDataTextList.m_TextMemberDatas[i];
				ConfigContent_ImageList.ItemData itemData = configContent_ImageList.ItemDatas[i];
				itemData.m_Text = GameGlobalUtil.GetXlsProgramText(textMemberData.m_ItemTextKey);
			}
			configContent_ImageList.SetCurrentItemIndex(configContent_ImageList.SelectedIndex);
			break;
		}
		case ConfigContent_Base.ContentType.LanguageList:
			break;
		}
	}

	public void OnEventNotice_Content(object sender, object arg)
	{
		if (!m_isInitailized)
		{
			return;
		}
		if (m_isIgnoreValueChangedOnce)
		{
			m_isIgnoreValueChangedOnce = false;
			return;
		}
		ConfigContent_Base configContent_Base = sender as ConfigContent_Base;
		if (configContent_Base == null)
		{
			return;
		}
		if (m_OnCursorContent != configContent_Base)
		{
			SetOnCursorContent(configContent_Base);
		}
		GameSwitch instance = GameSwitch.GetInstance();
		switch ((ContentType)configContent_Base.TagFlag)
		{
		case ContentType.TextLanguage:
			if (configContent_Base is ConfigContent_ImageList)
			{
				ConfigContent_ImageList configContent_ImageList4 = configContent_Base as ConfigContent_ImageList;
				SystemLanguage sysLang = (SystemLanguage)configContent_ImageList4.GetCurrentItemTag();
				instance.SetOptLang(sysLang);
			}
			break;
		case ContentType.TypingSpeed:
			if (configContent_Base is ConfigContent_Slide)
			{
				ConfigContent_Slide configContent_Slide = configContent_Base as ConfigContent_Slide;
				instance.SetTypingEff((byte)Mathf.RoundToInt(configContent_Slide.CurValue));
			}
			break;
		case ContentType.AutoDelay:
			if (configContent_Base is ConfigContent_Slide)
			{
				ConfigContent_Slide configContent_Slide2 = configContent_Base as ConfigContent_Slide;
				instance.SetAutoDelayTime(Mathf.RoundToInt(configContent_Slide2.CurValue));
			}
			break;
		case ContentType.VSync:
			if (configContent_Base is ConfigContent_Toggle)
			{
				ConfigContent_Toggle configContent_Toggle = configContent_Base as ConfigContent_Toggle;
				if (configContent_Toggle.ToggleOn)
				{
					m_isInputBlock = true;
					PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("GAME_CONFIG_POPUP_VSYNC_ON"), OnPopupResult_VSyncOn);
				}
				else
				{
					instance.SetVSyncOn(isOn: false);
				}
			}
			break;
		case ContentType.ConfigToDefault:
			m_isInputBlock = true;
			PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("GAME_CONFIG_POPUP_CONFIG_TO_DEFAULT"), OnPopupResult_ConfigToDefault);
			break;
		case ContentType.SaveDataReset:
			m_isInputBlock = true;
			PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("GAME_CONFIG_POPUP_RESET_SAVE_DATA"), OnPopupResult_ResetSaveData);
			break;
		case ContentType.SubmitCancelButtonSwap:
			if (configContent_Base is ConfigContent_ImageList)
			{
				ConfigContent_ImageList configContent_ImageList5 = configContent_Base as ConfigContent_ImageList;
				instance.SetOXType((GameSwitch.eButType)configContent_ImageList5.GetCurrentItemTag());
				ReflashOXChangeButtons();
			}
			break;
		case ContentType.ScreenMode:
		{
			ConfigContent_ImageList configContent_ImageList3 = configContent_Base as ConfigContent_ImageList;
			string value3 = configContent_ImageList3.GetCurrentItemTag() as string;
			if (!string.IsNullOrEmpty(value3))
			{
				GameSwitch.eScreenMode eScreenMode = (GameSwitch.eScreenMode)Enum.Parse(typeof(GameSwitch.eScreenMode), value3);
				if (eScreenMode != instance.GetScreenMode())
				{
					instance.SetScreenMode(eScreenMode);
				}
			}
			break;
		}
		case ContentType.ScreenResolution:
		{
			ConfigContent_ImageList configContent_ImageList2 = configContent_Base as ConfigContent_ImageList;
			string value2 = configContent_ImageList2.GetCurrentItemTag() as string;
			if (!string.IsNullOrEmpty(value2))
			{
				GameSwitch.eResolution eResolution = (GameSwitch.eResolution)Enum.Parse(typeof(GameSwitch.eResolution), value2);
				if (eResolution != instance.GetResolution())
				{
					instance.SetResolution(eResolution);
				}
			}
			break;
		}
		case ContentType.ControlerType:
		{
			ConfigContent_ImageList configContent_ImageList = configContent_Base as ConfigContent_ImageList;
			string value = configContent_ImageList.GetCurrentItemTag() as string;
			if (!string.IsNullOrEmpty(value))
			{
				GameSwitch.eControllerType eControllerType = (GameSwitch.eControllerType)Enum.Parse(typeof(GameSwitch.eControllerType), value);
				if (eControllerType != instance.GetControllerType())
				{
					instance.SetControllerType(eControllerType);
				}
			}
			break;
		}
		}
	}

	private void OnPopupResult_VSyncOn(PopupDialoguePlus.Result result)
	{
		m_isInputBlock = false;
		if (result == PopupDialoguePlus.Result.Yes)
		{
			GameSwitch.GetInstance().SetVSyncOn(isOn: true);
			return;
		}
		ConfigContent_Base content = GetContent(ContentType.VSync);
		ConfigContent_Toggle configContent_Toggle = content as ConfigContent_Toggle;
		configContent_Toggle.ToggleOn = false;
	}

	private void ReflashOXChangeButtons()
	{
		if (m_OutterExitButton != null && m_OutterExitButton.gameObject.activeSelf)
		{
			OXChangeButton componentInChildren = m_OutterExitButton.gameObject.GetComponentInChildren<OXChangeButton>();
			if (componentInChildren != null)
			{
				componentInChildren.enabled = false;
				componentInChildren.enabled = true;
			}
			else
			{
				PadIconHandler componentInChildren2 = m_OutterExitButton.gameObject.GetComponentInChildren<PadIconHandler>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.Reflash();
				}
			}
		}
		OXChangeButton[] componentsInChildren = base.gameObject.GetComponentsInChildren<OXChangeButton>();
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			OXChangeButton[] array = componentsInChildren;
			foreach (OXChangeButton oXChangeButton in array)
			{
				oXChangeButton.enabled = false;
				oXChangeButton.enabled = true;
			}
		}
		PadIconHandler[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<PadIconHandler>();
		if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
		{
			PadIconHandler[] array2 = componentsInChildren2;
			foreach (PadIconHandler padIconHandler in array2)
			{
				padIconHandler.Reflash();
			}
		}
	}

	public void TouchCursorContent(ConfigContent_Base content)
	{
		ConfigContent_Base onCursorContent = m_OnCursorContent;
		SetOnCursorContent(content);
		if (onCursorContent != content && m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
	}

	private void SetOnCursorContent(ConfigContent_Base content, bool isAdjustScrollPos = true)
	{
		if (!(m_OnCursorContent == content))
		{
			if (m_OnCursorContent != null)
			{
				m_OnCursorContent.Selected = false;
			}
			if (content != null)
			{
				content.Selected = true;
			}
			m_OnCursorContent = content;
			if (isAdjustScrollPos)
			{
				AdjustScrollPos_byOnCursonContent();
			}
		}
	}

	private bool ChangeOnCursorContent(bool isUpSide)
	{
		int count = m_Contents.Count;
		if (m_OnCursorContent == null)
		{
			if (count > 0)
			{
				SetOnCursorContent(m_Contents[0]);
			}
			return false;
		}
		int num = m_Contents.IndexOf(m_OnCursorContent);
		int num2 = num;
		ConfigContent_Base configContent_Base = null;
		do
		{
			num = ((!isUpSide) ? (num + 1) : (num - 1));
			if (num < 0)
			{
				if (!m_CursorLoopEnable)
				{
					return false;
				}
				num = count - 1;
			}
			else if (num >= count)
			{
				if (!m_CursorLoopEnable)
				{
					return false;
				}
				num = 0;
			}
			if (num == num2)
			{
				return false;
			}
			configContent_Base = m_Contents[num];
		}
		while (!(configContent_Base != null) || !configContent_Base.gameObject.activeSelf);
		SetOnCursorContent(m_Contents[num]);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
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
		float num = 0f - m_ContentContainerRT.offsetMax.y;
		float f = num - height;
		int num2 = Mathf.CeilToInt(num);
		int num3 = Mathf.FloorToInt(f);
		float y = m_OnCursorContent.RectTransform.offsetMax.y;
		float f2 = m_OnCursorContent.RectTransform.offsetMax.y - m_OnCursorContent.RectTransform.rect.height * m_OnCursorContent.RectTransform.localScale.y;
		int num4 = m_Contents.IndexOf(m_OnCursorContent);
		RectTransform rectTransform = null;
		if (Mathf.CeilToInt(f2) < num3)
		{
			int num5 = num4;
			while (num5 > 0)
			{
				num5--;
				rectTransform = m_Contents[num5].RectTransform;
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
				rectTransform = m_Contents[i].RectTransform;
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
			float num = 0f - m_ContentContainerRT.offsetMax.y;
			float num2 = num - height;
			float y = m_OnCursorContent.RectTransform.offsetMax.y;
			float num3 = m_OnCursorContent.RectTransform.offsetMax.y - m_OnCursorContent.RectTransform.rect.height * m_OnCursorContent.RectTransform.localScale.y;
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

	private void OnPopupResult_ConfigToDefault(PopupDialoguePlus.Result result)
	{
		m_isInputBlock = false;
		if (result == PopupDialoguePlus.Result.Yes)
		{
			float scrollPos = m_ScrollHandler.scrollPos;
			ConfigContent_Base onCursorContent = m_OnCursorContent;
			m_isForceSaveData = true;
			m_AudioManager.InitOptSound();
			GameSwitch instance = GameSwitch.GetInstance();
			GameSwitch.eUIButType uIButType = instance.GetUIButType();
			instance.InitOption();
			instance.SetUIButType(uIButType);
			ShowContents(isEnableAnimation: false);
			StartCoroutine(AlignContents(!m_isInFromMainMenu));
			m_ScrollHandler.SetScrollPos(scrollPos);
			SetOnCursorContent(onCursorContent, isAdjustScrollPos: false);
		}
	}

	private void OnPopupResult_ResetSaveData(PopupDialoguePlus.Result result)
	{
		if (result != PopupDialoguePlus.Result.Yes)
		{
			m_isInputBlock = false;
		}
		else
		{
			SaveLoad.GetInstance().DeleteAll(OnComplete_DeleteSaveData);
		}
	}

	private void OnComplete_DeleteSaveData(bool isExistErr)
	{
		SaveLoad.GetInstance().SaveLoadWhat(SaveLoad.eSaveWhat.eLoadEntireBuf, 0, OnComplete_ResetedSaveData);
	}

	private void OnComplete_ResetedSaveData(bool isExistErr)
	{
		PopupDialoguePlus.ShowPopup_OK(GameGlobalUtil.GetXlsProgramText("GAME_CONFIG_POPUP_RESET_SAVE_DATA_COMP"), OnClosedPopup_NoticeResetSaveData);
	}

	private void OnClosedPopup_NoticeResetSaveData(PopupDialoguePlus.Result result)
	{
		m_isInputBlock = false;
		m_isNeedAskSaveData = false;
		m_isForceSaveData = false;
		m_isResetedSaveData = true;
		Close();
	}

	public override bool CheckExistChangeValue(GameDefine.EventProc fpCB)
	{
		bool flag = false;
		GameSwitch instance = GameSwitch.GetInstance();
		if (m_prevLanguage != instance.GetCurSubtitleLanguage() || Mathf.RoundToInt(m_prevTypingSpeed) != instance.GetTypingEff() || !GameGlobalUtil.IsAlmostSame(m_prevAutoDelay, instance.GetAutoDelayTime()) || m_prevVSync != instance.GetVSyncOn() || m_prevOButtonUsage != (GameSwitch.eButType)instance.GetOXType() || m_prevScreenMode != instance.GetScreenMode() || m_prevResolution != instance.GetResolution() || m_prevControllerType != instance.GetControllerType())
		{
			flag = true;
		}
		if (flag)
		{
			m_fpCBChangeValueClose = fpCB;
			PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("GAME_CONFIG_POPUP_ENABLE_CHANGED"), PopupResult_EnableChanged);
		}
		return flag;
	}
}
