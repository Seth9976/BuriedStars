using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenuPlus : SystemMenuBase
{
	public enum ContentType
	{
		Unknown = -1,
		BGM,
		FX,
		Voice,
		VoiceLanguage,
		Count
	}

	public class ContentData
	{
		[NonSerialized]
		public ContentType m_Type = ContentType.Unknown;

		[NonSerialized]
		public ConfigContent_Base.ContentType m_WidgetType;
	}

	[Serializable]
	public class ContentDataVolumn : ContentData
	{
		public float m_ValueMax = 10f;

		public float m_ValueDelta = 1f;
	}

	[Serializable]
	public class VoiceLangMemberData
	{
		public ConstGameSwitch.eVoiceLang m_Language;

		public Sprite m_SpriteAsset;
	}

	[Serializable]
	public class ContentDataLangList : ContentData
	{
		public VoiceLangMemberData[] m_VoiceLangDatas;
	}

	public Text m_TitleText;

	public Text m_NoticeText;

	[Header("Content Container")]
	public RectTransform m_ContentContainerRT;

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
	public ContentDataVolumn m_BGMContentData = new ContentDataVolumn();

	public ContentDataVolumn m_FXContentData = new ContentDataVolumn();

	public ContentDataVolumn m_VoiceContentData = new ContentDataVolumn();

	public ContentDataLangList m_VoiceLangContentData = new ContentDataLangList();

	private ContentData[] m_ContentDatas;

	private List<ConfigContent_Base> m_Contents;

	private ConfigContent_Base m_OnCursorContent;

	private GameSwitch m_gameSwitch;

	private float m_prevBGMVolume;

	private float m_prevFxVolume;

	private float m_prevVoiceVolume;

	private ConstGameSwitch.eVoiceLang m_prevVoiceLang;

	protected override void Awake()
	{
		m_BGMContentData.m_Type = ContentType.BGM;
		m_BGMContentData.m_WidgetType = ConfigContent_Base.ContentType.Silde;
		m_BGMContentData.m_ValueMax = 10f;
		m_FXContentData.m_Type = ContentType.FX;
		m_FXContentData.m_WidgetType = ConfigContent_Base.ContentType.Silde;
		m_FXContentData.m_ValueMax = 10f;
		m_VoiceContentData.m_Type = ContentType.Voice;
		m_VoiceContentData.m_WidgetType = ConfigContent_Base.ContentType.Silde;
		m_VoiceContentData.m_ValueMax = 10f;
		m_VoiceLangContentData.m_Type = ContentType.VoiceLanguage;
		m_VoiceLangContentData.m_WidgetType = ConfigContent_Base.ContentType.ImageList;
		int num = 4;
		m_ContentDatas = new ContentData[num];
		m_ContentDatas[0] = m_BGMContentData;
		m_ContentDatas[1] = m_FXContentData;
		m_ContentDatas[2] = m_VoiceContentData;
		m_ContentDatas[3] = m_VoiceLangContentData;
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Vertical, m_ScrollRect, m_ContentContainerRT);
		m_ScrollHandler.SetScrollPos(0f);
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
		m_gameSwitch = null;
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
		m_ScrollHandler.Update();
		if (m_DisappearCheckAnimator != null || m_isInputBlock || PopupDialoguePlus.IsAnyPopupActivated() || m_ScrollHandler.IsScrolling)
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
		base.Show(isEnableAnimation, fpClosedCB, isNeedSetCloseCB, outterExitButton);
		m_isInputBlock = true;
		m_gameSwitch = GameSwitch.GetInstance();
		InitContents();
		ShowContents();
		ResetXlsTexts();
		m_prevBGMVolume = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.BGM);
		m_prevFxVolume = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.EFF);
		m_prevVoiceVolume = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.VOICE);
		m_prevVoiceLang = m_gameSwitch.GetVoiceLang();
		int count = m_Contents.Count;
		if (count > 0)
		{
			SetOnCursorContent(m_Contents[0]);
		}
		foreach (ConfigContent_Base content in m_Contents)
		{
			if (!(content == null))
			{
				content.AudioManager = m_AudioManager;
			}
		}
		StartCoroutine(AlignContents());
	}

	public override void ResetXlsTexts()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_TitleText);
		FontManager.ResetTextFontByCurrentLanguage(m_NoticeText);
		if (m_TitleText != null)
		{
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText("SOUND_CONFIG_MENU_TITLE");
		}
		if (m_NoticeText != null)
		{
			m_NoticeText.text = GameGlobalUtil.GetXlsProgramText("SOUND_CONFIG_MENU_NOTICE");
		}
		int count = m_Contents.Count;
		for (int i = 0; i < count; i++)
		{
			SetContentText(i);
		}
	}

	public override void Close(bool isEnableAnimation = true)
	{
		foreach (ConfigContent_Base content in m_Contents)
		{
			if (!(content == null))
			{
				content.Closing();
			}
		}
		m_isInputBlock = true;
		float optVolume = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.BGM);
		float optVolume2 = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.EFF);
		float optVolume3 = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.VOICE);
		ConstGameSwitch.eVoiceLang voiceLang = m_gameSwitch.GetVoiceLang();
		if (!GameGlobalUtil.IsAlmostSame(m_prevBGMVolume, optVolume) || !GameGlobalUtil.IsAlmostSame(m_prevFxVolume, optVolume2) || !GameGlobalUtil.IsAlmostSame(m_prevVoiceVolume, optVolume3) || m_prevVoiceLang != voiceLang)
		{
			PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("GAME_CONFIG_POPUP_ENABLE_CHANGED"), PopupResult_EnableChanged);
		}
		else
		{
			base.Close(isEnableAnimation);
		}
	}

	private void PopupResult_EnableChanged(PopupDialoguePlus.Result result)
	{
		if (result == PopupDialoguePlus.Result.Yes)
		{
			SaveLoad.GetInstance().SaveLoadWhat(SaveLoad.eSaveWhat.eSaveOptConfig);
			base.Close();
			return;
		}
		m_AudioManager.SetOptVolume(AudioManager.eSND_VOLUME.BGM, m_prevBGMVolume);
		m_AudioManager.SetOptVolume(AudioManager.eSND_VOLUME.EFF, m_prevFxVolume);
		m_AudioManager.SetOptVolume(AudioManager.eSND_VOLUME.VOICE, m_prevVoiceVolume);
		m_gameSwitch.SetVoiceLang(m_prevVoiceLang);
		base.Close();
	}

	private void InitContents()
	{
		ClearContents();
		if (m_ContentContainerRT == null || m_SrcContent_Slide == null || m_SrcContent_ImageList == null)
		{
			return;
		}
		int num = m_ContentDatas.Length;
		if (m_Contents == null)
		{
			m_Contents = new List<ConfigContent_Base>();
		}
		GameObject gameObject = null;
		ConfigContent_Base configContent_Base = null;
		ContentData contentData = null;
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			contentData = m_ContentDatas[i];
			if (contentData == null)
			{
				continue;
			}
			switch (contentData.m_WidgetType)
			{
			default:
				return;
			case ConfigContent_Base.ContentType.Silde:
				gameObject = UnityEngine.Object.Instantiate(m_SrcContent_Slide.gameObject);
				break;
			case ConfigContent_Base.ContentType.ImageList:
				gameObject = UnityEngine.Object.Instantiate(m_SrcContent_ImageList.gameObject);
				break;
			}
			gameObject.name = $"Content_{i}";
			configContent_Base = gameObject.GetComponent<ConfigContent_Base>();
			m_Contents.Add(configContent_Base);
			configContent_Base.RectTransform.SetParent(m_ContentContainerRT, worldPositionStays: false);
			configContent_Base.RectTransform.anchoredPosition = new Vector2(configContent_Base.RectTransform.anchoredPosition.x, num2);
			num2 -= configContent_Base.RectTransform.rect.height + (float)m_ContentInterval;
			configContent_Base.TagFlag = contentData.m_Type;
			configContent_Base.OnEventNotice = OnChangedValue_Content;
			configContent_Base.ParentMenu = this;
			if (contentData is ContentDataVolumn)
			{
				ContentDataVolumn contentDataVolumn = contentData as ContentDataVolumn;
				ConfigContent_Slide configContent_Slide = configContent_Base as ConfigContent_Slide;
				configContent_Slide.CurValue = 0f;
				configContent_Slide.MinValue = 0f;
				configContent_Slide.MaxValue = contentDataVolumn.m_ValueMax;
				configContent_Slide.DeltaValue = contentDataVolumn.m_ValueDelta;
			}
			else if (contentData is ContentDataLangList)
			{
				ContentDataLangList contentDataLangList = contentData as ContentDataLangList;
				ConfigContent_ImageList configContent_ImageList = configContent_Base as ConfigContent_ImageList;
				if (contentDataLangList.m_VoiceLangDatas != null && contentDataLangList.m_VoiceLangDatas.Length > 0)
				{
					int num3 = contentDataLangList.m_VoiceLangDatas.Length;
					ConfigContent_ImageList.ItemData[] array = new ConfigContent_ImageList.ItemData[num3];
					ConfigContent_ImageList.ItemData itemData = null;
					VoiceLangMemberData voiceLangMemberData = null;
					for (int j = 0; j < num3; j++)
					{
						itemData = new ConfigContent_ImageList.ItemData();
						voiceLangMemberData = contentDataLangList.m_VoiceLangDatas[j];
						itemData.m_SpriteAsset = voiceLangMemberData.m_SpriteAsset;
						itemData.m_Tag = voiceLangMemberData.m_Language;
						array[j] = itemData;
					}
					configContent_ImageList.ItemDatas = array;
				}
			}
			gameObject.SetActive(value: false);
		}
	}

	private void ShowContents(bool isEnableAnimation = true)
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
			if (configContent_Base == null)
			{
				continue;
			}
			configContent_Base.gameObject.SetActive(value: true);
			switch ((ContentType)configContent_Base.TagFlag)
			{
			case ContentType.BGM:
				if (m_AudioManager != null && configContent_Base is ConfigContent_Slide)
				{
					float optVolume = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.BGM);
					ConfigContent_Slide configContent_Slide = configContent_Base as ConfigContent_Slide;
					configContent_Slide.CurValue = Mathf.RoundToInt(configContent_Slide.MaxValue * optVolume);
				}
				break;
			case ContentType.FX:
				if (m_AudioManager != null && configContent_Base is ConfigContent_Slide)
				{
					float optVolume3 = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.EFF);
					ConfigContent_Slide configContent_Slide3 = configContent_Base as ConfigContent_Slide;
					configContent_Slide3.CurValue = Mathf.RoundToInt(configContent_Slide3.MaxValue * optVolume3);
				}
				break;
			case ContentType.Voice:
				if (m_AudioManager != null && configContent_Base is ConfigContent_Slide)
				{
					float optVolume2 = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.VOICE);
					ConfigContent_Slide configContent_Slide2 = configContent_Base as ConfigContent_Slide;
					configContent_Slide2.CurValue = Mathf.RoundToInt(configContent_Slide2.MaxValue * optVolume2);
				}
				break;
			case ContentType.VoiceLanguage:
				if (configContent_Base is ConfigContent_ImageList)
				{
					ConstGameSwitch.eVoiceLang voiceLang = m_gameSwitch.GetVoiceLang();
					ConfigContent_ImageList configContent_ImageList = configContent_Base as ConfigContent_ImageList;
					configContent_ImageList.SetCurrentItemByTag(voiceLang);
				}
				break;
			}
		}
	}

	private IEnumerator AlignContents()
	{
		int count = m_Contents.Count;
		ConfigContent_Base content = null;
		float curPos = 0f;
		for (int i = 0; i < count; i++)
		{
			content = m_Contents[i];
			if (content == null)
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
		m_isInputBlock = false;
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
		if (contentData != null && !(configContent_Base == null))
		{
			configContent_Base.ResetFontByCurrentLanguage();
			string xlsDataName = string.Empty;
			string xlsDataName2 = string.Empty;
			switch (contentData.m_Type)
			{
			case ContentType.BGM:
				xlsDataName = "SOUND_CONFIG_MENU_ITEM_BGM";
				xlsDataName2 = "SOUND_CONFIG_MENU_ITEM_BGM_NOTICE";
				break;
			case ContentType.FX:
				xlsDataName = "SOUND_CONFIG_MENU_ITEM_FX";
				xlsDataName2 = "SOUND_CONFIG_MENU_ITEM_FX_NOTICE";
				break;
			case ContentType.Voice:
				xlsDataName = "SOUND_CONFIG_MENU_ITEM_VOICE";
				xlsDataName2 = "SOUND_CONFIG_MENU_ITEM_VOICE_NOTICE";
				break;
			case ContentType.VoiceLanguage:
				xlsDataName = "SOUND_CONFIG_MENU_ITEM_VOICELANG";
				xlsDataName2 = "SOUND_CONFIG_MENU_ITEM_VOICELANG_NOTICE";
				break;
			}
			configContent_Base.Title = GameGlobalUtil.GetXlsProgramText(xlsDataName);
			configContent_Base.NoticeText = GameGlobalUtil.GetXlsProgramText(xlsDataName2);
		}
	}

	public void OnChangedValue_Content(object sender, object arg)
	{
		if (m_isInputBlock)
		{
			return;
		}
		ConfigContent_Base configContent_Base = sender as ConfigContent_Base;
		if (configContent_Base == null)
		{
			return;
		}
		if (configContent_Base != m_OnCursorContent)
		{
			SetOnCursorContent(configContent_Base);
		}
		switch ((ContentType)configContent_Base.TagFlag)
		{
		case ContentType.BGM:
			if (m_AudioManager != null && configContent_Base is ConfigContent_Slide)
			{
				ConfigContent_Slide configContent_Slide = configContent_Base as ConfigContent_Slide;
				float value = configContent_Slide.CurValue / configContent_Slide.MaxValue;
				value = Mathf.Clamp01(value);
				m_AudioManager.SetOptVolume(AudioManager.eSND_VOLUME.BGM, value);
			}
			break;
		case ContentType.FX:
			if (m_AudioManager != null && configContent_Base is ConfigContent_Slide)
			{
				ConfigContent_Slide configContent_Slide3 = configContent_Base as ConfigContent_Slide;
				float value3 = configContent_Slide3.CurValue / configContent_Slide3.MaxValue;
				value3 = Mathf.Clamp01(value3);
				m_AudioManager.SetOptVolume(AudioManager.eSND_VOLUME.EFF, value3);
			}
			break;
		case ContentType.Voice:
			if (m_AudioManager != null && configContent_Base is ConfigContent_Slide)
			{
				ConfigContent_Slide configContent_Slide2 = configContent_Base as ConfigContent_Slide;
				float value2 = configContent_Slide2.CurValue / configContent_Slide2.MaxValue;
				value2 = Mathf.Clamp01(value2);
				m_AudioManager.SetOptVolume(AudioManager.eSND_VOLUME.VOICE, value2);
			}
			break;
		case ContentType.VoiceLanguage:
			if (configContent_Base is ConfigContent_ImageList)
			{
				ConfigContent_ImageList configContent_ImageList = configContent_Base as ConfigContent_ImageList;
				ConstGameSwitch.eVoiceLang voiceLang = (ConstGameSwitch.eVoiceLang)configContent_ImageList.GetCurrentItemTag();
				m_gameSwitch.SetVoiceLang(voiceLang);
			}
			break;
		}
	}

	private void SetOnCursorContent(ConfigContent_Base content)
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
			AdjustScrollPos_byOnCursonContent();
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
		SetOnCursorContent(m_Contents[num]);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		return true;
	}

	public void TouchOnCursorContent(ConfigContent_Base onCursorContent)
	{
		ConfigContent_Base onCursorContent2 = m_OnCursorContent;
		SetOnCursorContent(onCursorContent);
		if (onCursorContent2 != onCursorContent && m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
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

	public override bool CheckExistChangeValue(GameDefine.EventProc fpCB)
	{
		bool flag = false;
		float optVolume = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.BGM);
		float optVolume2 = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.EFF);
		float optVolume3 = m_AudioManager.GetOptVolume(AudioManager.eSND_VOLUME.VOICE);
		ConstGameSwitch.eVoiceLang voiceLang = m_gameSwitch.GetVoiceLang();
		if (!GameGlobalUtil.IsAlmostSame(m_prevBGMVolume, optVolume) || !GameGlobalUtil.IsAlmostSame(m_prevFxVolume, optVolume2) || !GameGlobalUtil.IsAlmostSame(m_prevVoiceVolume, optVolume3) || m_prevVoiceLang != voiceLang)
		{
			flag = true;
		}
		if (flag)
		{
			m_fpCBChangeValueClose = fpCB;
			PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("GAME_CONFIG_POPUP_ENABLE_CHANGED"), PopupResult_EnableChanged);
		}
		foreach (ConfigContent_Base content in m_Contents)
		{
			content.Closing();
		}
		return flag;
	}
}
