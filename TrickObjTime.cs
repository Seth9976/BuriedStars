using GameData;
using UnityEngine;
using UnityEngine.UI;

public class TrickObjTime : MonoBehaviour
{
	public Text m_textAMPM;

	public Text m_textTimeMain;

	private int m_iAMPM = -1;

	private int m_iH = -1;

	private int m_iM = -1;

	private GameSwitch m_gameSwitch;

	private void OnEnable()
	{
		Text[] textComps = new Text[2] { m_textAMPM, m_textTimeMain };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_gameSwitch = GameSwitch.GetInstance();
		if (m_gameSwitch != null)
		{
			m_gameSwitch.GetTrickObjTime(ref m_iAMPM, ref m_iH, ref m_iM);
		}
		SetText();
	}

	private void OnDestroy()
	{
		m_gameSwitch = null;
	}

	private void Update()
	{
		if (m_gameSwitch != null)
		{
			int iAMPM = 0;
			int iH = 0;
			int iM = 0;
			m_gameSwitch.GetTrickObjTime(ref iAMPM, ref iH, ref iM);
			if (iAMPM != m_iAMPM || iH != m_iH || iM != m_iM)
			{
				m_iAMPM = iAMPM;
				m_iH = iH;
				m_iM = iM;
				SetText();
			}
		}
	}

	private void SetText()
	{
		string xlsDataName = ((m_iAMPM != 0) ? "PM" : "AM");
		string xlsProgramText = GameGlobalUtil.GetXlsProgramText(xlsDataName);
		m_textAMPM.text = xlsProgramText;
		string text = $"{m_iH:D2}:{m_iM:D2}";
		m_textTimeMain.text = text;
	}
}
