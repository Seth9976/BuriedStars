using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class EFF_PuzzlePhaseOutro : MonoBehaviour
{
	public Text[] m_textMain = new Text[3];

	public Text m_textSub;

	private Animator m_animPuzzle;

	private void OnEnable()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_textMain);
		FontManager.ResetTextFontByCurrentLanguage(m_textSub);
		Text[] textMain = m_textMain;
		foreach (Text text in textMain)
		{
			if (text != null)
			{
				text.text = GameGlobalUtil.GetXlsProgramText("EFF_PUZZLEPHASE_OUTRO_MAIN");
			}
		}
		if (m_textSub != null)
		{
			m_textSub.text = GameGlobalUtil.GetXlsProgramText("EFF_PUZZLEPHASE_OUTRO_SUB");
		}
		m_animPuzzle = GetComponent<Animator>();
	}

	private void Update()
	{
		if (m_animPuzzle != null)
		{
			m_animPuzzle.speed = EventEngine.GetInstance().GetAnimatorSkipValue();
		}
	}
}
