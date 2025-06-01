using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveDataSlot : MonoBehaviour
{
	[Serializable]
	public class SlotData
	{
		public GameObject m_RootObject;

		public GameObject m_ImageRootObj;

		public Image m_ImageSlot;

		public Text m_LocationText;

		public Text m_DateText;

		public GameObject m_NoDataObject;

		public Text m_SlotNumberText;

		public Text m_SlotAutoText;
	}

	public SlotData m_SelSlotData = new SlotData();

	public SlotData m_NotSelSlotData = new SlotData();

	public Button m_SelectButtonIcon;

	private int m_SlotIndex = -1;

	private bool m_isOnCursor;

	private GameDefine.EventProc m_fpClicked;

	private static string s_AutoSlotTag = string.Empty;

	private RectTransform m_RectTransform;

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

	public int slotIndex
	{
		get
		{
			return m_SlotIndex;
		}
		set
		{
			m_SlotIndex = value;
		}
	}

	public string slotNumberText
	{
		set
		{
			if (m_SelSlotData.m_SlotNumberText != null)
			{
				m_SelSlotData.m_SlotNumberText.text = value;
			}
			if (m_NotSelSlotData.m_SlotNumberText != null)
			{
				m_NotSelSlotData.m_SlotNumberText.text = value;
			}
		}
	}

	public bool enableSlotNumberText
	{
		set
		{
			if (m_SelSlotData.m_SlotNumberText != null)
			{
				m_SelSlotData.m_SlotNumberText.gameObject.SetActive(value);
			}
			if (m_NotSelSlotData.m_SlotNumberText != null)
			{
				m_NotSelSlotData.m_SlotNumberText.gameObject.SetActive(value);
			}
		}
	}

	public string slotAutoText
	{
		set
		{
			if (m_SelSlotData.m_SlotAutoText != null)
			{
				m_SelSlotData.m_SlotAutoText.text = value;
			}
			if (m_NotSelSlotData.m_SlotAutoText != null)
			{
				m_NotSelSlotData.m_SlotAutoText.text = value;
			}
		}
	}

	public bool enableSlotAutoText
	{
		set
		{
			if (m_SelSlotData.m_SlotAutoText != null)
			{
				m_SelSlotData.m_SlotAutoText.gameObject.SetActive(value);
			}
			if (m_NotSelSlotData.m_SlotAutoText != null)
			{
				m_NotSelSlotData.m_SlotAutoText.gameObject.SetActive(value);
			}
		}
	}

	public string locationText
	{
		get
		{
			return m_SelSlotData.m_LocationText.text;
		}
		set
		{
			m_SelSlotData.m_LocationText.text = value;
			m_NotSelSlotData.m_LocationText.text = value;
		}
	}

	public string dateText
	{
		get
		{
			return m_SelSlotData.m_DateText.text;
		}
		set
		{
			m_SelSlotData.m_DateText.text = value;
			m_NotSelSlotData.m_DateText.text = value;
		}
	}

	public bool isOnCursor
	{
		get
		{
			return m_isOnCursor;
		}
		set
		{
			m_isOnCursor = value;
			m_SelSlotData.m_RootObject.SetActive(value);
			m_NotSelSlotData.m_RootObject.SetActive(!value);
			if (m_SelectButtonIcon != null)
			{
				m_SelectButtonIcon.gameObject.SetActive(value && !noDataSlot);
			}
			GameGlobalUtil.PlayUIAnimation_WithChidren((!value) ? m_NotSelSlotData.m_RootObject : m_SelSlotData.m_RootObject, GameDefine.UIAnimationState.idle.ToString());
		}
	}

	public Sprite image
	{
		get
		{
			return m_SelSlotData.m_ImageSlot.sprite;
		}
		set
		{
			m_SelSlotData.m_ImageSlot.sprite = value;
			m_NotSelSlotData.m_ImageSlot.sprite = value;
		}
	}

	public bool noDataSlot
	{
		get
		{
			return m_SelSlotData.m_NoDataObject != null && m_SelSlotData.m_NoDataObject.activeSelf;
		}
		set
		{
			if (m_SelSlotData.m_NoDataObject != null)
			{
				m_SelSlotData.m_NoDataObject.SetActive(value);
			}
			if (m_SelSlotData.m_ImageRootObj != null)
			{
				m_SelSlotData.m_ImageRootObj.SetActive(!value);
			}
			if (m_SelSlotData.m_LocationText != null)
			{
				m_SelSlotData.m_LocationText.gameObject.SetActive(!value);
			}
			if (m_SelSlotData.m_DateText != null)
			{
				m_SelSlotData.m_DateText.gameObject.SetActive(!value);
			}
			if (m_NotSelSlotData.m_NoDataObject != null)
			{
				m_NotSelSlotData.m_NoDataObject.SetActive(value);
			}
			if (m_NotSelSlotData.m_ImageRootObj != null)
			{
				m_NotSelSlotData.m_ImageRootObj.SetActive(!value);
			}
			if (m_NotSelSlotData.m_LocationText != null)
			{
				m_NotSelSlotData.m_LocationText.gameObject.SetActive(!value);
			}
			if (m_NotSelSlotData.m_DateText != null)
			{
				m_NotSelSlotData.m_DateText.gameObject.SetActive(!value);
			}
		}
	}

	private void Start()
	{
		Text[] textComps = new Text[8] { m_SelSlotData.m_SlotAutoText, m_SelSlotData.m_SlotNumberText, m_SelSlotData.m_LocationText, m_SelSlotData.m_DateText, m_NotSelSlotData.m_SlotAutoText, m_NotSelSlotData.m_SlotNumberText, m_NotSelSlotData.m_LocationText, m_NotSelSlotData.m_DateText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		Text component = m_SelSlotData.m_NoDataObject.GetComponent<Text>();
		Text component2 = m_NotSelSlotData.m_NoDataObject.GetComponent<Text>();
		string xlsProgramText = GameGlobalUtil.GetXlsProgramText("SAVESLOT_NO_DATA");
		if (component != null)
		{
			FontManager.ResetTextFontByCurrentLanguage(component);
			component.text = xlsProgramText;
		}
		if (component2 != null)
		{
			FontManager.ResetTextFontByCurrentLanguage(component2);
			component2.text = xlsProgramText;
		}
	}

	private void OnDestroy()
	{
		m_fpClicked = null;
		m_RectTransform = null;
	}
}
