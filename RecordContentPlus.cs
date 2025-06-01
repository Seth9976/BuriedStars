using UnityEngine;
using UnityEngine.UI;

public class RecordContentPlus : MonoBehaviour
{
	public Text m_TitleText;

	public GameObject m_NewMarkObject;

	public GameObject m_SelectionCursor;

	public Text m_ContentText;

	public Button m_PlayButton;

	public Button m_StopButton;

	public Button m_PadIconButton;

	private Xls.CollSounds m_XlsData;

	private RecordMenuPlus m_RecordMenu;

	private CollectionSoundMenu m_CollectionSoungMenu;

	private bool m_isValidContent = true;

	private static string s_invalidateContentTitle = string.Empty;

	private static string s_invalidateContentText = string.Empty;

	private static string s_categorizedTitleFormat = string.Empty;

	private RectTransform m_RectTransform;

	public Xls.CollSounds xlsData => m_XlsData;

	public string title => m_TitleText.text;

	public string text => m_ContentText.text;

	public bool onCursor
	{
		get
		{
			return m_SelectionCursor.activeSelf;
		}
		set
		{
			m_SelectionCursor.SetActive(value);
			if (m_isValidContent)
			{
				m_PadIconButton.gameObject.SetActive(value);
			}
		}
	}

	public bool isPlaying
	{
		get
		{
			return m_StopButton.gameObject.activeSelf;
		}
		set
		{
			m_PlayButton.gameObject.SetActive(!value);
			m_StopButton.gameObject.SetActive(value);
		}
	}

	public bool activeNewMark
	{
		get
		{
			return m_NewMarkObject.gameObject.activeSelf;
		}
		set
		{
			m_NewMarkObject.gameObject.SetActive(value);
		}
	}

	public bool isValid => m_isValidContent;

	public RectTransform rectTransform
	{
		get
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	private void OnDestroy()
	{
		m_XlsData = null;
		m_RecordMenu = null;
		m_CollectionSoungMenu = null;
		m_RectTransform = null;
	}

	public static void InitStaticTextMembers()
	{
		s_invalidateContentTitle = string.Empty;
		s_invalidateContentText = string.Empty;
		s_categorizedTitleFormat = string.Empty;
	}

	public void InitRecordContent(int idx, Xls.CollSounds xlsColSoundData, RecordMenuPlus recordMenu, bool isEnableNewTag = false)
	{
		m_XlsData = xlsColSoundData;
		m_RecordMenu = recordMenu;
		m_isValidContent = true;
		Text[] textComps = new Text[2] { m_TitleText, m_ContentText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(m_XlsData.m_strIDtext);
		if (data_byKey != null)
		{
			m_TitleText.text = $"{idx + 1:D2}.{data_byKey.m_strTitle}";
			m_ContentText.text = data_byKey.m_strText;
		}
		else
		{
			m_TitleText.text = string.Empty;
			m_ContentText.text = string.Empty;
		}
		onCursor = false;
		isPlaying = false;
		activeNewMark = isEnableNewTag;
	}

	public void InitCollectionSoundContent(int idx, Xls.CollSounds xlsColSoundData, CollectionSoundMenu colSoundMenu, bool isValid = true, bool isEnableNewTag = false, string categoryName = null)
	{
		m_XlsData = xlsColSoundData;
		m_CollectionSoungMenu = colSoundMenu;
		m_isValidContent = isValid;
		Text[] textComps = new Text[2] { m_TitleText, m_ContentText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		if (isValid)
		{
			Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(m_XlsData.m_strIDtext);
			if (data_byKey != null)
			{
				string text = data_byKey.m_strTitle;
				if (!string.IsNullOrEmpty(categoryName))
				{
					if (string.IsNullOrEmpty(s_categorizedTitleFormat))
					{
						s_categorizedTitleFormat = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_CONTENT_TITLE_FORMAT");
					}
					text = string.Format(s_categorizedTitleFormat, data_byKey.m_strTitle, categoryName);
				}
				m_TitleText.text = text;
				m_ContentText.text = data_byKey.m_strText;
			}
			else
			{
				m_TitleText.text = string.Empty;
				m_ContentText.text = string.Empty;
			}
			isPlaying = false;
			activeNewMark = isEnableNewTag;
		}
		else
		{
			if (string.IsNullOrEmpty(s_invalidateContentTitle))
			{
				s_invalidateContentTitle = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_INVALID_TITLE");
			}
			if (string.IsNullOrEmpty(s_invalidateContentText))
			{
				s_invalidateContentText = GameGlobalUtil.GetXlsProgramText("COLLECTION_SOUND_MENU_INVALID_TEXT");
			}
			string text2 = s_invalidateContentTitle;
			if (!string.IsNullOrEmpty(categoryName))
			{
				if (string.IsNullOrEmpty(s_categorizedTitleFormat))
				{
					s_categorizedTitleFormat = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_CONTENT_TITLE_FORMAT");
				}
				text2 = string.Format(s_categorizedTitleFormat, s_invalidateContentTitle, categoryName);
			}
			m_TitleText.text = text2;
			m_ContentText.text = s_invalidateContentText;
			m_PlayButton.gameObject.SetActive(value: false);
			m_StopButton.gameObject.SetActive(value: false);
			m_PadIconButton.gameObject.SetActive(value: false);
			activeNewMark = false;
		}
		onCursor = false;
	}

	public void OnClick_PlayStopRecordButton(bool isPlay)
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			if (m_RecordMenu != null)
			{
				m_RecordMenu.OnProc_PlayStopRecord(this, isPlay);
				m_RecordMenu.TouchContent(this);
			}
			else if (m_CollectionSoungMenu != null)
			{
				m_CollectionSoungMenu.OnClick_Content(this);
				m_CollectionSoungMenu.OnProc_PlayStopRecord(this, isPlay);
			}
		}
	}

	public void OnClick_Content()
	{
		if ((bool)m_CollectionSoungMenu)
		{
			m_CollectionSoungMenu.OnClick_Content(this);
		}
	}
}
