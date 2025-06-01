using UnityEngine;
using UnityEngine.UI;

public class ConfigContent_Slide : ConfigContent_Base
{
	[Header("Slide Type Members")]
	public Text m_ValueText;

	public int m_ValueTextOffset;

	public Slider m_Slider;

	public Image m_SliderHandleImage;

	public Sprite m_SliderHandleNormal;

	public Sprite m_SliderHandleSelect;

	public Button m_LeftButton;

	public Button m_RightButton;

	public float m_PadInputRepeatTime = 0.2f;

	private float m_PadInputPusingTime;

	private float m_DeltaValue = 1f;

	public float CurValue
	{
		get
		{
			return (!(m_Slider != null)) ? 0f : m_Slider.value;
		}
		set
		{
			if (m_Slider != null)
			{
				m_Slider.value = value;
			}
			if (m_ValueText != null)
			{
				m_ValueText.text = (Mathf.RoundToInt(value) + m_ValueTextOffset).ToString();
			}
		}
	}

	public float MinValue
	{
		get
		{
			return (!(m_Slider != null)) ? 0f : m_Slider.minValue;
		}
		set
		{
			if (m_Slider != null)
			{
				m_Slider.minValue = value;
			}
		}
	}

	public float MaxValue
	{
		get
		{
			return (!(m_Slider != null)) ? 0f : m_Slider.maxValue;
		}
		set
		{
			if (m_Slider != null)
			{
				m_Slider.maxValue = value;
			}
		}
	}

	public float DeltaValue
	{
		get
		{
			return m_DeltaValue;
		}
		set
		{
			m_DeltaValue = value;
		}
	}

	public override bool Selected
	{
		get
		{
			return base.Selected;
		}
		set
		{
			base.Selected = value;
			if (m_SliderHandleImage != null)
			{
				m_SliderHandleImage.sprite = ((!base.Selected) ? m_SliderHandleNormal : m_SliderHandleSelect);
			}
			StopVoice();
		}
	}

	public ConfigContent_Slide()
	{
		m_Type = ContentType.Silde;
	}

	private void Update()
	{
		if (m_ParentMenu.isInputBlock || !Selected)
		{
			return;
		}
		Vector2 lStickMove = GamePadInput.GetLStickMove();
		lStickMove.Normalize();
		if (!(Mathf.Abs(lStickMove.x) >= 0.5f))
		{
			return;
		}
		if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
		{
			OnClick_ChangeValueButton(isIncrease: false);
			m_PadInputPusingTime = 0f;
		}
		else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
		{
			OnClick_ChangeValueButton(isIncrease: true);
			m_PadInputPusingTime = 0f;
		}
		else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Pushing))
		{
			m_PadInputPusingTime += Time.deltaTime;
			if (m_PadInputPusingTime >= m_PadInputRepeatTime)
			{
				OnClick_ChangeValueButton(isIncrease: false);
				m_PadInputPusingTime = 0f;
			}
		}
		else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Pushing))
		{
			m_PadInputPusingTime += Time.deltaTime;
			if (m_PadInputPusingTime >= m_PadInputRepeatTime)
			{
				OnClick_ChangeValueButton(isIncrease: true);
				m_PadInputPusingTime = 0f;
			}
		}
	}

	public void OnClick_ChangeValueButton(bool isIncrease)
	{
		if (m_Slider == null)
		{
			return;
		}
		m_Slider.value += ((!isIncrease) ? (0f - m_DeltaValue) : m_DeltaValue);
		if (m_AudioManager != null)
		{
			if (base.TagFlag != null && base.TagFlag is SoundMenuPlus.ContentType && (SoundMenuPlus.ContentType)base.TagFlag == SoundMenuPlus.ContentType.Voice)
			{
				m_AudioManager.Stop(5);
				m_AudioManager.PlayVoice("volume_setting");
			}
			else
			{
				m_AudioManager.PlayUISound("Menu_Select");
			}
		}
	}

	public void StopVoice()
	{
		if (base.TagFlag != null && base.TagFlag is SoundMenuPlus.ContentType && (SoundMenuPlus.ContentType)base.TagFlag == SoundMenuPlus.ContentType.Voice)
		{
			m_AudioManager.StopVoice();
		}
	}

	public void OnChangedValue_Slide(float value)
	{
		int num = Mathf.RoundToInt(value) + m_ValueTextOffset;
		m_ValueText.text = num.ToString();
		if (base.OnEventNotice != null)
		{
			base.OnEventNotice(this, value);
		}
	}

	public override void ResetFontByCurrentLanguage()
	{
		base.ResetFontByCurrentLanguage();
		FontManager.ResetTextFontByCurrentLanguage(m_ValueText);
	}

	public override void Closing()
	{
		StopVoice();
	}

	public void TouchChangeValueButton(bool isIncrease)
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClick_ChangeValueButton(isIncrease);
		}
	}
}
