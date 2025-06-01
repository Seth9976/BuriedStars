using UnityEngine;
using UnityEngine.UI;

public class ConfigContent_Base : MonoBehaviour
{
	public enum ContentType
	{
		Unknown,
		Silde,
		OnOff,
		Button,
		ImageList,
		LanguageList,
		TextList
	}

	protected ContentType m_Type;

	[Header("Common Members")]
	public GameObject m_SelectCursorObj;

	public Color m_NormalColor = default(Color);

	public Color m_SelectionColor = default(Color);

	public Text m_LabelText;

	public Text m_NoticeText;

	private object m_Tag;

	private ConfigMenuPlus.ContentData m_ContentData;

	private bool m_isSelected;

	private bool m_isInitailized;

	private GameDefine.EventProc m_fpEventNoticeCB;

	private RectTransform m_RectTransform;

	private LayoutElement m_LayoutElement;

	private VerticalLayoutGroup m_vLayoutGroup;

	protected AudioManager m_AudioManager;

	protected SystemMenuBase m_ParentMenu;

	public ContentType contentType
	{
		get
		{
			return m_Type;
		}
		set
		{
			m_Type = value;
		}
	}

	public string Title
	{
		get
		{
			return (!(m_LabelText != null)) ? string.Empty : m_LabelText.text;
		}
		set
		{
			if (m_LabelText != null)
			{
				m_LabelText.text = value;
			}
		}
	}

	public string NoticeText
	{
		get
		{
			return (!(m_NoticeText != null)) ? string.Empty : m_NoticeText.text;
		}
		set
		{
			if (m_NoticeText != null)
			{
				m_NoticeText.text = value;
			}
		}
	}

	public object TagFlag
	{
		get
		{
			return m_Tag;
		}
		set
		{
			m_Tag = value;
		}
	}

	public ConfigMenuPlus.ContentData ContentData
	{
		get
		{
			return m_ContentData;
		}
		set
		{
			m_ContentData = value;
		}
	}

	public virtual bool Selected
	{
		get
		{
			return m_isSelected;
		}
		set
		{
			m_isSelected = value;
			if (m_SelectCursorObj != null)
			{
				m_SelectCursorObj.SetActive(m_isSelected);
			}
			if (m_LabelText != null)
			{
				m_LabelText.color = ((!m_isSelected) ? m_NormalColor : m_SelectionColor);
			}
		}
	}

	public bool Initailized => m_isInitailized;

	public GameDefine.EventProc OnEventNotice
	{
		get
		{
			return m_fpEventNoticeCB;
		}
		set
		{
			m_fpEventNoticeCB = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	public RectTransform RectTransform
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

	public LayoutElement LayoutElement
	{
		get
		{
			if (m_LayoutElement == null)
			{
				m_LayoutElement = base.gameObject.GetComponent<LayoutElement>();
			}
			return m_LayoutElement;
		}
	}

	public VerticalLayoutGroup VerticalLayoutGroup
	{
		get
		{
			if (m_vLayoutGroup == null)
			{
				m_vLayoutGroup = base.gameObject.GetComponent<VerticalLayoutGroup>();
			}
			return m_vLayoutGroup;
		}
	}

	public AudioManager AudioManager
	{
		set
		{
			m_AudioManager = value;
		}
	}

	public SystemMenuBase ParentMenu
	{
		set
		{
			m_ParentMenu = value;
		}
	}

	private void Start()
	{
		m_isInitailized = true;
	}

	private void OnDestroy()
	{
		m_ContentData = null;
		m_fpEventNoticeCB = null;
		m_RectTransform = null;
		m_LayoutElement = null;
		m_vLayoutGroup = null;
		m_AudioManager = null;
		m_ParentMenu = null;
	}

	public virtual void ResetFontByCurrentLanguage()
	{
		Text[] textComps = new Text[2] { m_LabelText, m_NoticeText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
	}

	public virtual void Closing()
	{
	}
}
