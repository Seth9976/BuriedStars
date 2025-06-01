using GameData;
using UnityEngine;
using UnityEngine.UI;

public class SWSub_RankMenu : MonoBehaviour
{
	private static int RANK_MAX_CNT = 5;

	public Text[] m_textName = new Text[RANK_MAX_CNT];

	public Text[] m_textNumber = new Text[RANK_MAX_CNT];

	public bool m_isSimpleView;

	public GameObject m_SimpleIconRoot;

	private void OnEnable()
	{
		if (m_isSimpleView)
		{
			Text[] textName = m_textName;
			foreach (Text text in textName)
			{
				if (!(text == null))
				{
					text.gameObject.SetActive(value: false);
				}
			}
			Text[] textNumber = m_textNumber;
			foreach (Text text2 in textNumber)
			{
				if (!(text2 == null))
				{
					text2.gameObject.SetActive(value: false);
				}
			}
			if (m_SimpleIconRoot != null)
			{
				m_SimpleIconRoot.SetActive(value: true);
			}
			return;
		}
		GameSwitch instance = GameSwitch.GetInstance();
		string text3 = null;
		string text4 = null;
		for (int k = 0; k < RANK_MAX_CNT; k++)
		{
			FontManager.ResetTextFontByCurrentLanguage(m_textName[k]);
			FontManager.ResetTextFontByCurrentLanguage(m_textNumber[k]);
			GameSwitch.VoteRank voteRank = instance.GetVoteRank(k);
			if (voteRank != null)
			{
				string xlsDataName = $"MINI_RANKMENU_RANK_{k + 1}";
				Xls.CharData data_bySwitchIdx = Xls.CharData.GetData_bySwitchIdx(voteRank.m_iCharIdx);
				if (data_bySwitchIdx != null)
				{
					text3 = GameGlobalUtil.GetXlsProgramText(xlsDataName);
					text4 = GameGlobalUtil.GetXlsTextData(data_bySwitchIdx.m_strNameKey);
					m_textName[k].text = text3 + " " + text4;
					m_textName[k].gameObject.SetActive(value: true);
				}
				m_textNumber[k].text = $"{voteRank.m_iVoteCnt:#,###0}";
				m_textNumber[k].gameObject.SetActive(value: true);
			}
		}
		if (m_SimpleIconRoot != null)
		{
			m_SimpleIconRoot.SetActive(value: false);
		}
	}
}
