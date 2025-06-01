using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class DialogEvent_Popup : MonoBehaviour
{
	public Text m_textRelationUpText;

	public Text m_textRelationBodyText;

	private Animator m_animPopup;

	private void OnEnable()
	{
		m_animPopup = GetComponent<Animator>();
		FontManager.ResetTextFontByCurrentLanguage(m_textRelationUpText);
		FontManager.ResetTextFontByCurrentLanguage(m_textRelationBodyText);
		m_textRelationUpText.text = GameGlobalUtil.GetXlsProgramText("RELATION_EVENT_POPUP_UP_TEXT");
		m_textRelationBodyText.text = GameGlobalUtil.GetXlsProgramText("RELATION_EVENT_POPUP_BODY_TEXT");
	}

	private void Update()
	{
		if (m_animPopup != null)
		{
			m_animPopup.speed = EventEngine.GetInstance().GetAnimatorSkipValue();
		}
	}
}
