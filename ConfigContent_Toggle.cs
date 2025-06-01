using UnityEngine;
using UnityEngine.UI;

public class ConfigContent_Toggle : ConfigContent_Base
{
	[Header("On/Off Type Members")]
	public Toggle m_ToggleButton;

	public Text m_LeftButtonText1;

	public Text m_LeftButtonText2;

	public Text m_RightButtonText1;

	public Text m_RightButtonText2;

	public Button m_SelectButtonIcon;

	public bool ToggleOn
	{
		get
		{
			return m_ToggleButton != null && m_ToggleButton.isOn;
		}
		set
		{
			if (m_ToggleButton != null)
			{
				m_ToggleButton.isOn = value;
			}
		}
	}

	public string LeftButtonText
	{
		get
		{
			return (!(m_LeftButtonText1 != null)) ? string.Empty : m_LeftButtonText1.text;
		}
		set
		{
			if (m_LeftButtonText1 != null)
			{
				m_LeftButtonText1.text = value;
			}
			if (m_LeftButtonText2 != null)
			{
				m_LeftButtonText2.text = value;
			}
		}
	}

	public string RightButtonText
	{
		get
		{
			return (!(m_RightButtonText1 != null)) ? string.Empty : m_RightButtonText1.text;
		}
		set
		{
			if (m_RightButtonText1 != null)
			{
				m_RightButtonText1.text = value;
			}
			if (m_RightButtonText2 != null)
			{
				m_RightButtonText2.text = value;
			}
		}
	}

	public ConfigContent_Toggle()
	{
		m_Type = ContentType.OnOff;
	}

	private void Update()
	{
		if (!m_ParentMenu.isInputBlock && Selected && !(m_ToggleButton == null) && (GamePadInput.IsButtonState_Down(PadInput.GameInput.LStickX) || GamePadInput.IsButtonState_Down(PadInput.GameInput.LStickX, isAxisPositive: true)))
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_OK");
			}
			ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_SelectButtonIcon);
			m_ToggleButton.isOn = !m_ToggleButton.isOn;
		}
	}

	public void OnClick_ToggleButton(bool isOn)
	{
		if (base.OnEventNotice != null)
		{
			base.OnEventNotice(this, isOn);
		}
	}

	public override void ResetFontByCurrentLanguage()
	{
		base.ResetFontByCurrentLanguage();
		Text[] textComps = new Text[4] { m_LeftButtonText1, m_LeftButtonText2, m_RightButtonText1, m_RightButtonText2 };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
	}

	public void TouchToggleButton(bool isOn)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_OK");
		}
		OnClick_ToggleButton(isOn);
	}
}
