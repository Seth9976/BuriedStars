using UnityEngine;
using UnityEngine.UI;

public class ToDoSlotPlus : MonoBehaviour
{
	private enum Category
	{
		Bronze,
		Silver,
		Gold,
		Platinum
	}

	public GameObject m_goSelectOutline;

	public GameObject m_goCompleteText;

	public GameObject m_goGaugeRoot;

	public Slider m_sliderGetCount;

	public GameObject m_goCompIcon;

	public GameObject m_goNonCompIcon;

	public GameObject m_goNew;

	public Text m_textTitle;

	public Text m_textContent;

	public Text m_textGetCount;

	public Text m_textComplete;

	public Text m_textNew;

	private string m_strTrophyKey;

	private int m_iMinCnt;

	private int m_iGetCnt;

	private int m_iMaxCnt;

	private bool m_isComplete;

	private CollectionTrophyMenu m_colTrophyMenu;

	private RectTransform m_rt;

	private Xls.Trophys m_xlsData;

	private static string s_HidedContentTitle = string.Empty;

	private static string s_HidedContentText = string.Empty;

	private static string s_categorizedTitleFormat = string.Empty;

	public bool VisibleNewSymbol
	{
		get
		{
			return m_goNew != null && m_goNew.activeSelf;
		}
		set
		{
			if (m_goNew != null)
			{
				m_goNew.SetActive(value);
			}
		}
	}

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

	public Xls.Trophys xlsData => m_xlsData;

	private void OnDestroy()
	{
		m_rt = null;
		m_xlsData = null;
	}

	public void InitSlotState()
	{
		Text[] textComps = new Text[5] { m_textTitle, m_textContent, m_textGetCount, m_textComplete, m_textNew };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_goSelectOutline.SetActive(value: false);
		m_goCompleteText.SetActive(value: false);
		m_goGaugeRoot.SetActive(value: false);
		m_goCompIcon.SetActive(value: false);
		m_goNonCompIcon.SetActive(value: false);
		m_goNew.SetActive(value: false);
		m_textNew.text = GameGlobalUtil.GetXlsProgramText("TODO_NEW");
		m_textComplete.text = GameGlobalUtil.GetXlsProgramText("TODO_COMPLETE");
		m_textTitle.text = string.Empty;
		m_textContent.text = string.Empty;
		SetTodoCount(0, isSetText: true, 0);
		SetComplete(isComp: false, isForceSet: true);
	}

	public void SetTrophyKey(string strKey)
	{
		m_strTrophyKey = strKey;
	}

	public string GetTrophyKey()
	{
		return m_strTrophyKey;
	}

	public void SetTodoCount(int iGetCnt, bool isSetText = true, int iMaxCnt = -1)
	{
		m_iGetCnt = iGetCnt;
		if (iMaxCnt != -1)
		{
			m_iMaxCnt = iMaxCnt;
		}
		if (m_iMaxCnt <= iGetCnt)
		{
			SetComplete(isComp: true);
			return;
		}
		SetComplete(isComp: false);
		m_sliderGetCount.minValue = m_iMinCnt;
		m_sliderGetCount.maxValue = m_iMaxCnt;
		m_sliderGetCount.value = iGetCnt;
		if (isSetText)
		{
			m_textGetCount.text = iGetCnt + " / " + m_iMaxCnt;
		}
	}

	private void SetComplete(bool isComp, bool isForceSet = false)
	{
		if (isForceSet || isComp != m_isComplete)
		{
			m_goCompIcon.SetActive(isComp);
			m_goNonCompIcon.SetActive(!isComp);
			m_textComplete.gameObject.SetActive(isComp);
			m_goGaugeRoot.SetActive(!isComp && m_iMaxCnt > 1);
			m_isComplete = isComp;
		}
	}

	public void SetContentText(string strTitle, string strContent)
	{
		m_textTitle.text = strTitle;
		m_textContent.text = strContent;
	}

	public void SetSelect(bool isOn)
	{
		m_goSelectOutline.SetActive(isOn);
	}

	public void SetNew(bool isNew)
	{
		m_goNew.SetActive(isNew);
	}

	public static void InitStaticTextMembers()
	{
		s_HidedContentTitle = string.Empty;
		s_HidedContentText = string.Empty;
		s_categorizedTitleFormat = string.Empty;
	}

	public void InitCollectionTrophyContent(Xls.Trophys _xlsData, int _getCount, bool isEnableNewTag, string categoryName = null, CollectionTrophyMenu parentMenu = null)
	{
		m_xlsData = _xlsData;
		m_colTrophyMenu = parentMenu;
		InitSlotState();
		SetTodoCount(_getCount, isSetText: true, m_xlsData.m_iMax);
		string text = string.Empty;
		string strContent = string.Empty;
		if (_getCount <= 0 && m_xlsData.m_iCategory != 0)
		{
			if (string.IsNullOrEmpty(s_HidedContentTitle))
			{
				s_HidedContentTitle = GameGlobalUtil.GetXlsProgramText("TODO_TITLE_HIDE");
			}
			if (string.IsNullOrEmpty(s_HidedContentText))
			{
				s_HidedContentText = GameGlobalUtil.GetXlsProgramText("TODO_CONTENT_HIDE");
			}
			text = s_HidedContentTitle;
			strContent = s_HidedContentText;
		}
		else
		{
			Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(m_xlsData.m_strName);
			if (data_byKey != null)
			{
				text = data_byKey.m_strTitle;
				strContent = data_byKey.m_strText;
			}
		}
		if (!string.IsNullOrEmpty(categoryName))
		{
			if (string.IsNullOrEmpty(s_categorizedTitleFormat))
			{
				s_categorizedTitleFormat = GameGlobalUtil.GetXlsProgramText("COLLECTION_COMMON_CONTENT_TITLE_FORMAT");
			}
			text = string.Format(s_categorizedTitleFormat, text, categoryName);
		}
		SetContentText(text, strContent);
		SetNew(isEnableNewTag);
		SetSelect(isOn: false);
	}

	public void OnClick_Content()
	{
		if (m_colTrophyMenu != null)
		{
			m_colTrophyMenu.OnClick_Content(this);
		}
	}
}
