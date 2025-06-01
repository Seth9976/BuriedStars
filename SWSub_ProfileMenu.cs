using UnityEngine;
using UnityEngine.UI;

public class SWSub_ProfileMenu : MonoBehaviour
{
	public Text m_TitleText;

	[Header("Content Memeber")]
	public Text m_ContentTitle;

	public Text m_ContentText;

	[Header("Simple Mode")]
	public bool m_isSimpleView;

	public GameObject m_SimpleIconRoot;

	private string m_strTitle = string.Empty;

	private void OnEnable()
	{
		m_TitleText.text = string.Empty;
		m_ContentTitle.text = string.Empty;
		m_ContentText.text = string.Empty;
		FontManager.ResetTextFontByCurrentLanguage(m_ContentTitle);
		FontManager.ResetTextFontByCurrentLanguage(m_ContentText);
		if (m_TitleText != null)
		{
			m_TitleText.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_ContentTitle != null)
		{
			m_ContentTitle.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_ContentText != null)
		{
			m_ContentText.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_SimpleIconRoot != null)
		{
			m_SimpleIconRoot.SetActive(m_isSimpleView);
		}
		m_strTitle = GameGlobalUtil.GetXlsProgramText("PROFILE_MENU_TITLE");
	}

	public void OnProc_ChnagedSelectContent(object sender, object arg)
	{
		if (m_isSimpleView)
		{
			return;
		}
		ProfileMenuPlus profileMenuPlus = sender as ProfileMenuPlus;
		if (!(profileMenuPlus == null))
		{
			ProfileContentPlus profileContentPlus = arg as ProfileContentPlus;
			if (!(profileContentPlus == null))
			{
				m_ContentTitle.text = profileContentPlus.title;
				m_ContentText.text = profileContentPlus.text;
				string currentTabText = profileMenuPlus.GetCurrentTabText();
				if (!string.IsNullOrEmpty(currentTabText))
				{
					m_TitleText.text = $"{m_strTitle} / {currentTabText}";
				}
				else
				{
					m_TitleText.text = m_strTitle;
				}
				return;
			}
		}
		m_TitleText.text = m_strTitle;
		m_ContentTitle.text = string.Empty;
		m_ContentText.text = string.Empty;
	}
}
