using UnityEngine;
using UnityEngine.UI;

public class ConfigContent_Button : ConfigContent_Base
{
	[Header("Button Type Members")]
	public Button m_ButtonObject;

	public Text m_ButtonText;

	public Button m_SelectButtonIcon;

	public string ButtonText
	{
		get
		{
			return (!(m_ButtonText != null)) ? string.Empty : m_ButtonText.text;
		}
		set
		{
			if (m_ButtonText != null)
			{
				m_ButtonText.text = value;
			}
		}
	}

	public ConfigContent_Button()
	{
		m_Type = ContentType.Button;
	}

	private void Update()
	{
		if (!m_ParentMenu.isInputBlock && Selected && !(m_ButtonObject == null) && GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_SelectButtonIcon, m_ButtonObject);
			OnClick_Button();
		}
	}

	public void OnClick_Button()
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_OK");
		}
		if (base.OnEventNotice != null)
		{
			base.OnEventNotice(this, null);
		}
	}

	public override void ResetFontByCurrentLanguage()
	{
		base.ResetFontByCurrentLanguage();
		FontManager.ResetTextFontByCurrentLanguage(m_ButtonText);
	}

	public void TouchButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClick_Button();
		}
	}
}
