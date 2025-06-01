using UnityEngine;
using UnityEngine.UI;

public class TrueStaffroll : MonoBehaviour
{
	[Header("Character Key")]
	public string m_strCharKey;

	[Header("Text Object")]
	public Text[] m_textName = new Text[5];

	public Text[] m_textCVName = new Text[3];

	private void OnEnable()
	{
		if (string.IsNullOrEmpty(m_strCharKey))
		{
			return;
		}
		FontManager.ResetTextFontByCurrentLanguage(m_textName);
		FontManager.ResetTextFontByCurrentLanguage(m_textCVName);
		Xls.CharData data_byKey = Xls.CharData.GetData_byKey(m_strCharKey);
		if (data_byKey == null)
		{
			return;
		}
		int num = m_textName.Length;
		string xlsTextData = GameGlobalUtil.GetXlsTextData(data_byKey.m_strNameKey);
		for (int i = 0; i < num; i++)
		{
			if (m_textName[i] != null)
			{
				m_textName[i].text = xlsTextData;
			}
		}
		num = m_textCVName.Length;
		string xlsProgramText = GameGlobalUtil.GetXlsProgramText("TRUE_STAFF_CV");
		string xlsTextData2 = GameGlobalUtil.GetXlsTextData(data_byKey.m_strCVName);
		for (int j = 0; j < num; j++)
		{
			if (m_textCVName[j] != null)
			{
				m_textCVName[j].text = xlsProgramText + xlsTextData2;
			}
		}
	}
}
