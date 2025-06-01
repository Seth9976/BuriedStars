using UnityEngine;
using UnityEngine.UI;

public class Intro_Broadcast : MonoBehaviour
{
	public Text m_textIntroBroadcastMainText;

	public Text m_textIntroBroadcastSub00Text;

	public Text m_textIntroBroadcastSub01Text;

	private void OnEnable()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_textIntroBroadcastMainText);
		FontManager.ResetTextFontByCurrentLanguage(m_textIntroBroadcastSub00Text);
		FontManager.ResetTextFontByCurrentLanguage(m_textIntroBroadcastSub01Text);
		m_textIntroBroadcastMainText.text = GameGlobalUtil.GetXlsProgramText("INTRO_BROADCAST_MAIN");
		m_textIntroBroadcastSub00Text.text = GameGlobalUtil.GetXlsProgramText("INTRO_BROADCAST_SUB00");
		m_textIntroBroadcastSub01Text.text = GameGlobalUtil.GetXlsProgramText("INTRO_BROADCAST_SUB01");
	}
}
