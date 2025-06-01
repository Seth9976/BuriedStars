using UnityEngine;
using UnityEngine.UI;

public class Intro_Phrase : MonoBehaviour
{
	public Text m_textIntroPhraseAText;

	public Text m_textIntroPhraseB1Text;

	public Text m_textIntroPhraseB2Text;

	private void OnEnable()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_textIntroPhraseAText);
		FontManager.ResetTextFontByCurrentLanguage(m_textIntroPhraseB1Text);
		FontManager.ResetTextFontByCurrentLanguage(m_textIntroPhraseB2Text);
		m_textIntroPhraseAText.text = GameGlobalUtil.GetXlsProgramText("INTRO_PHRASE_A");
		m_textIntroPhraseB1Text.text = GameGlobalUtil.GetXlsProgramText("INTRO_PHRASE_B1");
		m_textIntroPhraseB2Text.text = GameGlobalUtil.GetXlsProgramText("INTRO_PHRASE_B2");
	}
}
