using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class EFF_IntroSaveGuide : MonoBehaviour
{
	public Text m_textSaveGuide;

	private Animator m_animSaveGuide;

	private void OnEnable()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_textSaveGuide, isSetFontMaterial: false);
		if (m_textSaveGuide != null)
		{
			m_textSaveGuide.text = GameGlobalUtil.GetXlsProgramText("INTRO_SAVE_GUIDE_TEXT");
		}
		m_animSaveGuide = GetComponent<Animator>();
	}

	private void Update()
	{
		if (m_animSaveGuide != null)
		{
			m_animSaveGuide.speed = EventEngine.GetInstance().GetAnimatorSkipValue();
		}
	}
}
