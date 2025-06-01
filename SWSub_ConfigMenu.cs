using UnityEngine;
using UnityEngine.UI;

public class SWSub_ConfigMenu : MonoBehaviour
{
	public Text m_TitleText;

	public Text m_SelectedContentText;

	public Text m_SelectionContentText;

	public bool m_isSimpleView;

	public GameObject m_SimpleIconRoot;

	private string m_strTitle = string.Empty;

	private bool m_isEnabled;

	private object m_backuped_Sender;

	private object m_backuped_Arg;

	private void OnEnable()
	{
		Text[] textComps = new Text[3] { m_TitleText, m_SelectedContentText, m_SelectionContentText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		if (m_TitleText != null)
		{
			m_TitleText.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_SelectedContentText != null)
		{
			m_SelectedContentText.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_SelectionContentText != null)
		{
			m_SelectionContentText.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_SimpleIconRoot != null)
		{
			m_SimpleIconRoot.SetActive(m_isSimpleView);
		}
		m_strTitle = GameGlobalUtil.GetXlsProgramText("SWATCHCONFIG_MENU_TITLE");
		m_TitleText.text = string.Empty;
		m_SelectedContentText.text = string.Empty;
		m_SelectionContentText.text = string.Empty;
		m_isEnabled = true;
		if (m_backuped_Sender != null)
		{
			OnProc_ChangedSelectContent(m_backuped_Sender, m_backuped_Arg);
		}
	}

	private void OnDisable()
	{
		m_isEnabled = false;
		m_backuped_Sender = null;
		m_backuped_Arg = null;
	}

	public void OnProc_ChangedSelectContent(object sender, object arg)
	{
		if (m_isSimpleView)
		{
			return;
		}
		if (!m_isEnabled)
		{
			m_backuped_Sender = sender;
			m_backuped_Arg = arg;
			return;
		}
		SWatchConfigMenu sWatchConfigMenu = sender as SWatchConfigMenu;
		if (!(sWatchConfigMenu == null))
		{
			string tabText = string.Empty;
			string selectedContentText = string.Empty;
			string selectionContentText = string.Empty;
			sWatchConfigMenu.GetCurrentSelectingTexts(out tabText, out selectedContentText, out selectionContentText);
			m_TitleText.text = $"{m_strTitle} / {tabText}";
			m_SelectedContentText.text = selectedContentText;
			m_SelectionContentText.text = selectionContentText;
		}
	}
}
