using UnityEngine;
using UnityEngine.UI;

public class ButtonPadInput
{
	private class CheckButton
	{
		public PadInput.ButtonState m_buttonState = PadInput.ButtonState.Down;

		public PadInput.GameInput m_gameInput = PadInput.GameInput.None;

		public float m_fAnimTime;

		public Button[] m_butPress = new Button[4];

		public Sprite[] m_sprPressButNormal = new Sprite[4];

		public CallBackEventButton m_FuncCB;

		public bool m_isCallEndFunc = true;
	}

	public delegate void CallBackEventButton();

	private static float COLOR_MAX_TIME;

	private static float SPR_MAX_TIME;

	private static float MIN_TIME = 0.02f;

	private const int COUNT_BUT = 4;

	private static CheckButton m_clCheckButOne;

	private static bool s_isExcuteButtonEvent = true;

	public static void Init()
	{
		COLOR_MAX_TIME = GameGlobalUtil.GetXlsProgramDefineStrToFloat("BUTTON_PRESS_EFF_TIME");
		float num = COLOR_MAX_TIME - 0.3f;
		SPR_MAX_TIME = ((!(num <= MIN_TIME)) ? num : MIN_TIME);
	}

	public static void Update()
	{
		if (m_clCheckButOne == null)
		{
			return;
		}
		if (m_clCheckButOne.m_buttonState == PadInput.ButtonState.Down)
		{
			bool flag = false;
			Button button = m_clCheckButOne.m_butPress[0];
			m_clCheckButOne.m_fAnimTime += Time.deltaTime;
			if (button != null)
			{
				if (button.transition == Selectable.Transition.Animation && button.animator != null)
				{
					AnimatorStateInfo currentAnimatorStateInfo = button.animator.GetCurrentAnimatorStateInfo(0);
					if (currentAnimatorStateInfo.normalizedTime >= 0.99f)
					{
						if (currentAnimatorStateInfo.IsName("steam_push") || currentAnimatorStateInfo.IsName("push"))
						{
							flag = true;
						}
						else
						{
							m_clCheckButOne = null;
						}
					}
				}
				else
				{
					float num = ((button.transition != Selectable.Transition.ColorTint) ? SPR_MAX_TIME : COLOR_MAX_TIME);
					flag = m_clCheckButOne.m_fAnimTime >= num;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				if (m_clCheckButOne.m_isCallEndFunc)
				{
					ExcuteButton(button);
				}
				if (m_clCheckButOne.m_FuncCB != null)
				{
					m_clCheckButOne.m_FuncCB();
				}
				PlayEndAni();
			}
		}
		else
		{
			if (m_clCheckButOne.m_buttonState != PadInput.ButtonState.Pushing)
			{
				return;
			}
			if (m_clCheckButOne.m_gameInput == PadInput.GameInput.None)
			{
				PlayEndAni();
			}
			if (GamePadInput.IsButtonState(m_clCheckButOne.m_gameInput, PadInput.ButtonState.Pushing))
			{
				if (m_clCheckButOne.m_FuncCB != null)
				{
					m_clCheckButOne.m_FuncCB();
				}
			}
			else
			{
				PlayEndAni();
			}
		}
	}

	public static bool PushingInputButton(PadInput.GameInput gameInput, Button button, CallBackEventButton cbFunc = null, Button butSub_0 = null, Button butSub_1 = null, Button butSub_2 = null)
	{
		if (m_clCheckButOne != null)
		{
			return false;
		}
		if (!button.gameObject.activeInHierarchy)
		{
			return false;
		}
		bool result = false;
		if (GamePadInput.IsButtonState(gameInput, PadInput.ButtonState.Pushing) || GamePadInput.IsButtonState(gameInput, PadInput.ButtonState.Pushing, isAxisPositive: true))
		{
			AddAndShowPressAnim(button, PadInput.ButtonState.Pushing, butSub_0, butSub_1, butSub_2);
			m_clCheckButOne.m_gameInput = gameInput;
			m_clCheckButOne.m_FuncCB = cbFunc;
			result = true;
		}
		return result;
	}

	public static bool PressInputButton(PadInput.GameInput gameInput, Button button, Button butSub_0 = null, Button butSub_1 = null, Button butSub_2 = null, CallBackEventButton cbFunc = null, bool isShowAnim = true, bool isExcuteEvent = true)
	{
		if (m_clCheckButOne != null)
		{
			return false;
		}
		if (button == null)
		{
			return false;
		}
		if (!button.gameObject.activeInHierarchy)
		{
			return false;
		}
		bool result = false;
		if (GamePadInput.IsButtonState(gameInput, PadInput.ButtonState.Down) || GamePadInput.IsButtonState(gameInput, PadInput.ButtonState.Down, isAxisPositive: true))
		{
			s_isExcuteButtonEvent = isExcuteEvent;
			if (isShowAnim)
			{
				AddAndShowPressAnim(button, PadInput.ButtonState.Down, butSub_0, butSub_1, butSub_2);
				m_clCheckButOne.m_FuncCB = cbFunc;
			}
			else
			{
				ExcuteButton(button);
			}
			result = true;
		}
		return result;
	}

	public static void AddAndShowPressAnim(Button button, PadInput.ButtonState buttonState = PadInput.ButtonState.Down, Button butSub_0 = null, Button butSub_1 = null, Button butSub_2 = null, bool isCallEndFunc = true)
	{
		Button[] array = new Button[4] { button, butSub_0, butSub_1, butSub_2 };
		m_clCheckButOne = new CheckButton();
		m_clCheckButOne.m_fAnimTime = 0f;
		m_clCheckButOne.m_buttonState = buttonState;
		m_clCheckButOne.m_isCallEndFunc = isCallEndFunc;
		for (int i = 0; i < 4; i++)
		{
			button = (m_clCheckButOne.m_butPress[i] = array[i]);
			if (!(button == null) && button.transition == Selectable.Transition.SpriteSwap)
			{
				m_clCheckButOne.m_sprPressButNormal[i] = button.GetComponent<Image>().sprite;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			Button button2 = array[j];
			if (!(button2 == null))
			{
				if (button2.transition == Selectable.Transition.ColorTint)
				{
					CrossFadeColor(button2.targetGraphic, button2, button2.colors.pressedColor, instant: false);
				}
				else if (button2.transition == Selectable.Transition.SpriteSwap)
				{
					button2.GetComponent<Image>().sprite = button2.spriteState.pressedSprite;
				}
				else if (button2.transition == Selectable.Transition.Animation && button2.animator != null && button2.gameObject.activeInHierarchy)
				{
					button2.animator.Play("steam_push");
				}
			}
		}
	}

	private static void PlayEndAni()
	{
		for (int i = 0; i < 4; i++)
		{
			Button button = m_clCheckButOne.m_butPress[i];
			if (!(button == null))
			{
				if (button.transition == Selectable.Transition.ColorTint)
				{
					CrossFadeColor(button.targetGraphic, button, button.colors.normalColor, instant: false);
				}
				else if (button.transition == Selectable.Transition.SpriteSwap)
				{
					button.GetComponent<Image>().sprite = m_clCheckButOne.m_sprPressButNormal[i];
				}
				else if (button.transition == Selectable.Transition.Animation && button.animator != null && button.gameObject.activeInHierarchy)
				{
					button.animator.Play("steam_normal");
				}
			}
		}
		m_clCheckButOne = null;
	}

	public static bool IsPlayingButPressAnim(Button button = null)
	{
		bool flag = false;
		if (button == null)
		{
			return m_clCheckButOne != null;
		}
		return m_clCheckButOne.m_butPress[0] == button;
	}

	private static void ExcuteButton(Button button)
	{
		if (s_isExcuteButtonEvent && !(button == null))
		{
			button.onClick.Invoke();
		}
	}

	private static void CrossFadeColor(Graphic targetGraphic, Button button, Color targetColor, bool instant)
	{
		if (!(targetGraphic == null))
		{
			targetGraphic.CrossFadeColor(targetColor, (!instant) ? button.colors.fadeDuration : 0f, ignoreTimeScale: true, useAlpha: true);
		}
	}
}
