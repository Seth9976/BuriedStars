using GameData;
using UnityEngine;
using UnityEngine.UI;

public class SWSub_AutoWrite : MonoBehaviour
{
	[Header("Text")]
	public Text m_textTitle;

	public Text m_textUserName;

	public Text m_textContent;

	public Text m_textUploadTime;

	public Text m_textTimeCount;

	[Header("Simple Mode")]
	public bool m_isSimpleView;

	public GameObject m_SimpleIconRoot;

	private void OnEnable()
	{
		Text[] textComps = new Text[5] { m_textTitle, m_textUserName, m_textContent, m_textUploadTime, m_textTimeCount };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		if (m_textTitle != null)
		{
			m_textTitle.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_textUserName != null)
		{
			m_textUserName.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_textContent != null)
		{
			m_textContent.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_textUploadTime != null)
		{
			m_textUploadTime.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_textTimeCount != null)
		{
			m_textTimeCount.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_SimpleIconRoot != null)
		{
			m_SimpleIconRoot.SetActive(m_isSimpleView);
		}
		if (m_isSimpleView)
		{
			return;
		}
		m_textTitle.text = GameGlobalUtil.GetXlsProgramText("AUTO_WRITE_TITLE");
		m_textUploadTime.text = GameGlobalUtil.GetXlsProgramText("AUTO_WRITE_UPLOAD_TIME");
		m_textTimeCount.text = GameGlobalUtil.GetXlsProgramText("AUTO_WRITE_TIME_COUNT");
		string key = "acc_00000";
		Xls.AccountData data_byKey = Xls.AccountData.GetData_byKey(key);
		if (data_byKey != null)
		{
			Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(data_byKey.m_nicknameID);
			if (data_byKey2 != null)
			{
				m_textUserName.text = data_byKey2.m_strTitle;
			}
		}
		SetAutoText();
	}

	public void SetAutoText()
	{
		if (!m_isSimpleView)
		{
			int autoSNSText = GameSwitch.GetInstance().GetAutoSNSText();
			Xls.WatchFaterAutoText data_bySwitchIdx = Xls.WatchFaterAutoText.GetData_bySwitchIdx(autoSNSText);
			if (data_bySwitchIdx != null)
			{
				Xls.TextData data_byKey = Xls.TextData.GetData_byKey(data_bySwitchIdx.m_strTextID);
				m_textContent.text = data_byKey.m_strTxt;
			}
		}
	}
}
