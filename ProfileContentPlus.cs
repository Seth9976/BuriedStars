using GameData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProfileContentPlus : MonoBehaviour
{
	[Header("Content Area")]
	public Text m_ContentTitle;

	public Text m_ContentText;

	public Text m_ContentNewMark;

	public GameObject m_NewSymbolObject;

	public GameObject m_SelectionCursor;

	[Header("Image Thumbnail")]
	public GameObject m_ContentImageRoot;

	public Image m_ContentImage;

	public GameObject m_ImageViewMark;

	public Button m_ImageViewButton;

	private ProfileMenuPlus m_profileMenu;

	private CollectionProfileMenu m_colProfileMenu;

	private Xls.Profiles m_XlsData;

	private static string s_invalidateContentTitle = string.Empty;

	private static string s_invalidateContentText = string.Empty;

	private static string s_categorizedTitleFormat = string.Empty;

	private bool m_isInitailized;

	private bool m_isOnCursor;

	private bool m_isExistImage;

	private RectTransform m_RectTransform;

	public Xls.Profiles xlsData => m_XlsData;

	public bool isInitailized => m_isInitailized;

	public bool onCursor
	{
		get
		{
			return m_isOnCursor;
		}
		set
		{
			m_isOnCursor = value;
			if (m_SelectionCursor != null)
			{
				m_SelectionCursor.SetActive(m_isOnCursor);
			}
			if (m_isExistImage)
			{
				if (m_ImageViewMark != null)
				{
					m_ImageViewMark.SetActive(m_isOnCursor);
				}
				if (m_ImageViewButton != null)
				{
					m_ImageViewButton.gameObject.SetActive(m_isOnCursor);
				}
			}
		}
	}

	public string text
	{
		get
		{
			return m_ContentText.text;
		}
		set
		{
			m_ContentText.text = TagText.TransTagTextToUnityText(value, isIgnoreHideTag: true);
			m_ContentText.CalculateLayoutInputVertical();
		}
	}

	public string title
	{
		get
		{
			return m_ContentTitle.text;
		}
		set
		{
			m_ContentTitle.text = value;
		}
	}

	public bool visibleNewTag
	{
		get
		{
			return m_NewSymbolObject != null && m_NewSymbolObject.activeSelf;
		}
		set
		{
			if (m_NewSymbolObject != null)
			{
				m_NewSymbolObject.SetActive(value);
			}
		}
	}

	public bool isExistImage => m_isExistImage;

	public Sprite imageThumbnail
	{
		get
		{
			return m_ContentImage.sprite;
		}
		set
		{
			m_ContentImage.sprite = value;
			if (m_ContentImageRoot != null)
			{
				m_ContentImageRoot.SetActive(m_isExistImage);
				if (value != null)
				{
					m_ContentImage.color = Color.white;
					GameGlobalUtil.AddEventTrigger(m_ContentImageRoot, EventTriggerType.PointerClick, OnClick_ThumbnailImage);
				}
			}
		}
	}

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

	public static void InitStaticTextMembers()
	{
		s_invalidateContentTitle = string.Empty;
		s_invalidateContentText = string.Empty;
		s_categorizedTitleFormat = string.Empty;
	}

	private void Start()
	{
		RectTransform component = m_ContentText.gameObject.GetComponent<RectTransform>();
		float b = 0f;
		if (m_isExistImage)
		{
			RectTransform component2 = m_ContentImageRoot.GetComponent<RectTransform>();
			b = component2.rect.height;
		}
		float height = component.rect.height;
		float num = Mathf.Max(m_ContentText.preferredHeight, b);
		if (!GameGlobalUtil.IsAlmostSame(height, num))
		{
			float num2 = num - height;
			RectTransform component3 = base.gameObject.GetComponent<RectTransform>();
			component3.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, component3.rect.height + num2);
		}
		m_isInitailized = true;
	}

	private void OnDestroy()
	{
		m_profileMenu = null;
		m_colProfileMenu = null;
		m_XlsData = null;
		m_RectTransform = null;
	}

	private void OnClick_ThumbnailImage(BaseEventData evtData)
	{
		if (!(m_profileMenu == null))
		{
			m_profileMenu.StartCoroutine(m_profileMenu.OnProc_ViewImageDetail(this));
		}
	}

	public void InitProfileContent(Xls.Profiles xlsProfileData, ProfileMenuPlus profileMenu)
	{
		m_XlsData = xlsProfileData;
		m_profileMenu = profileMenu;
		Text[] textComps = new Text[3] { m_ContentTitle, m_ContentText, m_ContentNewMark };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_ContentNewMark.text = GameGlobalUtil.GetXlsProgramText("PROFILE_MENU_NEW");
		Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(m_XlsData.m_strName);
		if (data_byKey != null)
		{
			m_ContentTitle.text = data_byKey.m_strTitle;
			m_ContentText.text = data_byKey.m_strText;
		}
		else
		{
			m_ContentTitle.text = string.Empty;
			m_ContentText.text = string.Empty;
		}
		m_isExistImage = !string.IsNullOrEmpty(m_XlsData.m_strIDImg);
		Sprite sprite = null;
		if (m_isExistImage)
		{
			Xls.CollImages data_byKey2 = Xls.CollImages.GetData_byKey(xlsData.m_strIDImg);
			if (data_byKey2 != null)
			{
				Xls.ImageFile data_byKey3 = Xls.ImageFile.GetData_byKey(data_byKey2.m_strIDImg);
				if (data_byKey3 != null)
				{
					sprite = MainLoadThing.instance.colImageThumbnailManager.GetThumbnailImageInCache(data_byKey3.m_strAssetPath_Thumbnail);
				}
			}
			imageThumbnail = sprite;
		}
		imageThumbnail = sprite;
	}

	public void SetCollectionProfileData(Xls.Profiles xlsData, CollectionProfileMenu colProfileMenu, bool isValid = true, bool isEnableNewTag = false, bool isEnableCategoryName = false)
	{
		m_XlsData = xlsData;
		m_colProfileMenu = colProfileMenu;
		Text[] textComps = new Text[3] { m_ContentTitle, m_ContentText, m_ContentNewMark };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		if (isEnableCategoryName && string.IsNullOrEmpty(s_categorizedTitleFormat))
		{
			s_categorizedTitleFormat = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_CONTENT_TITLE_FORMAT");
		}
		if (!isValid)
		{
			if (string.IsNullOrEmpty(s_invalidateContentTitle))
			{
				s_invalidateContentTitle = GameGlobalUtil.GetXlsProgramText("COLLECTION_PROFILE_MENU_INVALID_TITLE");
			}
			if (string.IsNullOrEmpty(s_invalidateContentText))
			{
				s_invalidateContentText = GameGlobalUtil.GetXlsProgramText("COLLECTION_PROFILE_MENU_INVALID_TEXT");
			}
			if (isEnableCategoryName)
			{
				Xls.CharData_ForProfile data_bySwitchIdx = Xls.CharData_ForProfile.GetData_bySwitchIdx(xlsData.m_iCtgIdx);
				Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey((GameSwitch.GetInstance().GetTrophyCnt(data_bySwitchIdx.m_strUpdateThropyKey) <= 0) ? data_bySwitchIdx.m_strProfileText : data_bySwitchIdx.m_strProfileText_Update);
				string arg = ((data_byKey == null) ? "Unknown" : data_byKey.m_strTitle);
				m_ContentTitle.text = string.Format(s_categorizedTitleFormat, s_invalidateContentTitle, arg);
			}
			else
			{
				m_ContentTitle.text = s_invalidateContentTitle;
			}
			m_ContentText.text = s_invalidateContentText;
			m_isExistImage = false;
			imageThumbnail = null;
			visibleNewTag = false;
			return;
		}
		Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(m_XlsData.m_strName);
		if (data_byKey2 != null)
		{
			string empty = string.Empty;
			if (isEnableCategoryName)
			{
				Xls.CharData_ForProfile data_bySwitchIdx2 = Xls.CharData_ForProfile.GetData_bySwitchIdx(xlsData.m_iCtgIdx);
				Xls.TextListData data_byKey3 = Xls.TextListData.GetData_byKey((GameSwitch.GetInstance().GetTrophyCnt(data_bySwitchIdx2.m_strUpdateThropyKey) <= 0) ? data_bySwitchIdx2.m_strProfileText : data_bySwitchIdx2.m_strProfileText_Update);
				string arg2 = ((data_byKey3 == null) ? "Unknown" : data_byKey3.m_strTitle);
				empty = string.Format(s_categorizedTitleFormat, data_byKey2.m_strTitle, arg2);
			}
			else
			{
				empty = data_byKey2.m_strTitle;
			}
			m_ContentTitle.text = empty;
			m_ContentText.text = data_byKey2.m_strText;
		}
		else
		{
			m_ContentTitle.text = string.Empty;
			m_ContentText.text = string.Empty;
		}
		m_isExistImage = !string.IsNullOrEmpty(m_XlsData.m_strIDImg);
		Sprite sprite = null;
		if (m_isExistImage)
		{
			Xls.CollImages data_byKey4 = Xls.CollImages.GetData_byKey(xlsData.m_strIDImg);
			if (data_byKey4 != null)
			{
				Xls.ImageFile data_byKey5 = Xls.ImageFile.GetData_byKey(data_byKey4.m_strIDImg);
				if (data_byKey5 != null)
				{
					sprite = MainLoadThing.instance.colImageThumbnailManager.GetThumbnailImageInCache(data_byKey5.m_strAssetPath_Thumbnail);
				}
			}
		}
		imageThumbnail = sprite;
		visibleNewTag = isEnableNewTag;
	}

	public void OnClick_Content()
	{
		if (m_profileMenu != null)
		{
			m_profileMenu.OnClick_Content(this);
		}
		else if (m_colProfileMenu != null)
		{
			m_colProfileMenu.OnClick_Content(this);
		}
	}
}
