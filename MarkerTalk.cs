using UnityEngine;
using UnityEngine.UI;

public class MarkerTalk : MonoBehaviour
{
	public Button m_butTalkBG;

	public Button m_butConvMark;

	public Button m_butO;

	public GameObject m_goNoKeywordObj_BG_0;

	public GameObject m_goNoKeywordObj_BG_1;

	public GameObject m_goNoKeywordObj_Icon;

	public Image m_imgCondition;

	public Text m_textName;

	[HideInInspector]
	public string m_strCharKey;

	private bool m_isTalkable = true;

	public void OnEnable()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_textName);
		string text = string.Empty;
		if (m_strCharKey != null)
		{
			Xls.CharData data_byKey = Xls.CharData.GetData_byKey(m_strCharKey);
			Xls.TextData textData = null;
			if (data_byKey != null)
			{
				textData = Xls.TextData.GetData_byKey(data_byKey.m_strNameKey);
				if (textData != null)
				{
					text = textData.m_strTxt;
				}
			}
		}
		m_textName.text = text;
	}

	public void OnDestroy()
	{
		m_strCharKey = null;
	}

	public void SetNoKeyword(bool isNoKeyword)
	{
		m_isTalkable = !isNoKeyword;
		m_goNoKeywordObj_BG_0.SetActive(isNoKeyword);
		m_goNoKeywordObj_BG_1.SetActive(isNoKeyword);
		m_goNoKeywordObj_Icon.SetActive(isNoKeyword);
	}

	public void SetConditionImg(Sprite sprCond)
	{
		if (sprCond != null)
		{
			m_imgCondition.sprite = sprCond;
		}
	}

	public bool IsTalkable()
	{
		return m_isTalkable;
	}
}
