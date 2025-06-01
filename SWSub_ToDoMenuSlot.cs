using UnityEngine;
using UnityEngine.UI;

public class SWSub_ToDoMenuSlot : MonoBehaviour
{
	public Text m_textTitle;

	public GameObject m_goInComplete;

	public Text m_textInComplete;

	public GameObject m_goComplete;

	public Text m_textComplete;

	public GameObject m_goInCompleteIcon;

	public GameObject m_goCompleteIcon;

	private void OnEnable()
	{
		m_textTitle.text = string.Empty;
		m_textInComplete.text = string.Empty;
		m_textComplete.text = string.Empty;
	}

	public void InitSlot()
	{
		Text[] textComps = new Text[3] { m_textTitle, m_textInComplete, m_textComplete };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_goInComplete.SetActive(value: false);
		m_goComplete.SetActive(value: false);
		m_goInCompleteIcon.SetActive(value: false);
		m_goCompleteIcon.SetActive(value: false);
		m_textComplete.text = GameGlobalUtil.GetXlsProgramText("TODO_COMPLETE");
	}

	public void SetTitle(string strTitle)
	{
		m_textTitle.text = strTitle;
	}

	public void SetTodoCount(int iGetCnt, int iMaxCnt)
	{
		bool flag = iGetCnt >= iMaxCnt;
		m_goInComplete.SetActive(!flag);
		m_goInCompleteIcon.SetActive(!flag);
		m_goComplete.SetActive(flag);
		m_goCompleteIcon.SetActive(flag);
		if (!flag)
		{
			m_textInComplete.text = iGetCnt + " / " + iMaxCnt;
		}
	}
}
