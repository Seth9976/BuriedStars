using UnityEngine;
using UnityEngine.UI;

public class SWSub_MemoMenu : MonoBehaviour
{
	public RectTransform m_rtfBG;

	public Text m_textTitle;

	public Text m_textContent;

	public Text m_textSubMemoTitle;

	public bool m_isSimpleView;

	public GameObject m_SimpleIconRoot;

	public void OnEnable()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_textTitle);
		FontManager.ResetTextFontByCurrentLanguage(m_textContent);
		FontManager.ResetTextFontByCurrentLanguage(m_textSubMemoTitle);
		if (m_textSubMemoTitle != null)
		{
			m_textSubMemoTitle.gameObject.SetActive(value: true);
			m_textSubMemoTitle.text = GameGlobalUtil.GetXlsProgramText("SMART_WATCH_BUTTON_MEMO");
		}
		if (m_rtfBG != null)
		{
			m_rtfBG.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_textTitle != null)
		{
			m_textTitle.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_textContent != null)
		{
			m_textContent.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_SimpleIconRoot != null)
		{
			m_SimpleIconRoot.SetActive(m_isSimpleView);
		}
	}

	public void SetKeywordKey(string strKeywordKey, bool isUsableSlot)
	{
		if (m_isSimpleView)
		{
			return;
		}
		bool flag;
		if (strKeywordKey == null)
		{
			flag = true;
		}
		else
		{
			flag = !isUsableSlot;
			Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
			if (!flag)
			{
				if (data_byKey == null)
				{
					flag = true;
				}
				else
				{
					Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(data_byKey.m_strTitleID);
					if (data_byKey2 == null)
					{
						flag = true;
					}
					else
					{
						m_textTitle.text = data_byKey2.m_strTitle;
						m_textContent.text = data_byKey2.m_strText;
					}
				}
			}
		}
		if (flag)
		{
			SetNone();
		}
		float preferredHeight = m_textContent.preferredHeight;
		float preferredHeight2 = m_textTitle.preferredHeight;
		float num = 54f;
		float y = preferredHeight + preferredHeight2 + num;
		Vector2 offsetMax = m_rtfBG.offsetMax;
		m_rtfBG.sizeDelta = new Vector2(m_rtfBG.rect.width, y);
		m_rtfBG.offsetMax = offsetMax;
	}

	public void SetNone()
	{
		if (!m_isSimpleView)
		{
			m_textTitle.text = GameGlobalUtil.GetXlsProgramText("NONE_KEYWORD_DETAIL_TITLE");
			m_textContent.text = GameGlobalUtil.GetXlsProgramText("NONE_KEYWORD_DETAIL_BODY");
		}
	}
}
