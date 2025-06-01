using System;
using UnityEngine;
using UnityEngine.UI;

public class CollectionImageContent : MonoBehaviour
{
	[Serializable]
	public class SlotMemebers
	{
		public GameObject m_RootObj;

		public Image m_ThumbnailImage;

		public Text m_NameText;

		public GameObject m_NewSymbolObj;
	}

	public SlotMemebers m_NotSelected = new SlotMemebers();

	public SlotMemebers m_Selected = new SlotMemebers();

	private RectTransform m_rt;

	private bool m_isSelected;

	private Sprite m_Thumbnail;

	private string m_Name;

	private Xls.CollImages m_XlsData;

	private bool m_isValid = true;

	private bool m_isEnableNewTag;

	private bool m_isHide;

	private CollectionImageMenu m_ColImageMenu;

	private static string s_invalidateContentTitle = string.Empty;

	private static string s_categorizedTitleFormat = string.Empty;

	public RectTransform rectTransform
	{
		get
		{
			if (m_rt == null)
			{
				m_rt = base.gameObject.GetComponent<RectTransform>();
			}
			return m_rt;
		}
	}

	public bool select
	{
		get
		{
			return m_isSelected;
		}
		set
		{
			m_isSelected = value;
			if (!m_isHide)
			{
				m_NotSelected.m_RootObj.SetActive(!m_isSelected);
				m_Selected.m_RootObj.SetActive(m_isSelected);
			}
		}
	}

	public Sprite thumbnail
	{
		get
		{
			return m_Thumbnail;
		}
		set
		{
			m_Thumbnail = value;
			m_NotSelected.m_ThumbnailImage.sprite = m_Thumbnail;
			m_NotSelected.m_ThumbnailImage.enabled = false;
			m_NotSelected.m_ThumbnailImage.enabled = true;
			m_Selected.m_ThumbnailImage.sprite = m_Thumbnail;
			m_Selected.m_ThumbnailImage.enabled = false;
			m_Selected.m_ThumbnailImage.enabled = true;
		}
	}

	public string contentName
	{
		get
		{
			return m_Name;
		}
		set
		{
			m_Name = value;
			m_NotSelected.m_NameText.text = m_Name;
			m_Selected.m_NameText.text = m_Name;
		}
	}

	public Xls.CollImages xlsData => m_XlsData;

	public bool isValid => m_isValid;

	public bool enableNewTag
	{
		get
		{
			return m_isEnableNewTag;
		}
		set
		{
			m_isEnableNewTag = value;
			if (m_NotSelected.m_NewSymbolObj != null)
			{
				m_NotSelected.m_NewSymbolObj.SetActive(m_isEnableNewTag);
			}
			if (m_Selected.m_NewSymbolObj != null)
			{
				m_Selected.m_NewSymbolObj.SetActive(m_isEnableNewTag);
			}
		}
	}

	public bool hide
	{
		get
		{
			return m_isHide;
		}
		set
		{
			m_isHide = value;
			if (m_isHide)
			{
				m_NotSelected.m_RootObj.SetActive(value: false);
				m_Selected.m_RootObj.SetActive(value: false);
				return;
			}
			m_NotSelected.m_RootObj.SetActive(!m_isSelected);
			m_Selected.m_RootObj.SetActive(m_isSelected);
			string strMot = GameDefine.UIAnimationState.idle.ToString();
			if (m_NotSelected.m_RootObj.activeSelf)
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_NotSelected.m_RootObj, strMot);
			}
			if (m_Selected.m_RootObj.activeSelf)
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_Selected.m_RootObj, strMot);
			}
		}
	}

	private void OnDestroy()
	{
		m_rt = null;
		m_Thumbnail = null;
		m_Name = null;
		m_XlsData = null;
		m_ColImageMenu = null;
	}

	public void SetXlsData(Xls.CollImages _xlsData, CollectionImageMenu parentMenu, bool _isValid = true, string categoryName = null)
	{
		m_XlsData = _xlsData;
		m_ColImageMenu = parentMenu;
		m_isValid = _isValid;
		thumbnail = null;
		if (m_XlsData == null)
		{
			return;
		}
		FontManager.ResetTextFontByCurrentLanguage(m_Selected.m_NameText);
		FontManager.ResetTextFontByCurrentLanguage(m_NotSelected.m_NameText);
		if (_isValid)
		{
			Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(m_XlsData.m_strIDtext);
			if (data_byKey != null)
			{
				if (!string.IsNullOrEmpty(categoryName))
				{
					if (string.IsNullOrEmpty(s_categorizedTitleFormat))
					{
						s_categorizedTitleFormat = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_CONTENT_TITLE_FORMAT");
					}
					contentName = string.Format(s_categorizedTitleFormat, data_byKey.m_strTitle, categoryName);
				}
				else
				{
					contentName = data_byKey.m_strTitle;
				}
			}
			else
			{
				contentName = "Unknown";
			}
			string strIDImg = m_XlsData.m_strIDImg;
			Sprite sprite = null;
			Xls.ImageFile data_byKey2 = Xls.ImageFile.GetData_byKey(strIDImg);
			if (data_byKey2 != null)
			{
				sprite = MainLoadThing.instance.colImageThumbnailManager.GetThumbnailImageInCache(data_byKey2.m_strAssetPath_Thumbnail);
			}
			thumbnail = sprite;
			m_Selected.m_ThumbnailImage.color = ((!(sprite != null)) ? new Color(1f, 1f, 1f, 0f) : new Color(1f, 1f, 1f, 1f));
			return;
		}
		if (string.IsNullOrEmpty(s_invalidateContentTitle))
		{
			s_invalidateContentTitle = GameGlobalUtil.GetXlsProgramText("COLLECTION_IMAGE_MENU_INVALID_TITLE");
		}
		if (!string.IsNullOrEmpty(categoryName))
		{
			if (string.IsNullOrEmpty(s_categorizedTitleFormat))
			{
				s_categorizedTitleFormat = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_CONTENT_TITLE_FORMAT");
			}
			contentName = string.Format(s_categorizedTitleFormat, s_invalidateContentTitle, categoryName);
		}
		else
		{
			contentName = s_invalidateContentTitle;
		}
		m_Selected.m_ThumbnailImage.color = new Color(1f, 1f, 1f, 0f);
	}

	public void OnClickButton()
	{
		if (MainLoadThing.instance.IsTouchableState() && !(m_ColImageMenu == null))
		{
			m_ColImageMenu.OnClickButton_FromContent(this);
		}
	}
}
