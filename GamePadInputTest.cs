using System;
using UnityEngine;

public class GamePadInputTest : MonoBehaviour
{
	[Serializable]
	public class PS4GamePad
	{
		public SpriteRenderer thumbstick_left;

		public SpriteRenderer thumbstick_right;

		public SpriteRenderer cross;

		public SpriteRenderer circle;

		public SpriteRenderer triangle;

		public SpriteRenderer square;

		public SpriteRenderer dpad_down;

		public SpriteRenderer dpad_right;

		public SpriteRenderer dpad_up;

		public SpriteRenderer dpad_left;

		public SpriteRenderer L1;

		public SpriteRenderer L2;

		public SpriteRenderer R1;

		public SpriteRenderer R2;

		public SpriteRenderer lightbar;

		public SpriteRenderer options;

		public SpriteRenderer speaker;

		public SpriteRenderer touchpad;

		public Transform gyro;

		public TextMesh text;

		public Light light;
	}

	public PS4GamePad m_GamePadElements = new PS4GamePad();

	public Transform[] touches;

	public Color inputOn = Color.white;

	public Color inputOff = Color.grey;

	private void Start()
	{
		GamePadInput.Init();
	}

	private void Update()
	{
		GamePadInput.Update();
		UpdateThumbSticks();
		UpdateDPadButtons();
		UpdateFrontButtons();
		UpdateShoulderButtons();
		UpdateExtraButtons();
		float axisValue = GamePadInput.GetAxisValue(PadInput.InputType.LStickY);
		float axisValue2 = GamePadInput.GetAxisValue(PadInput.InputType.RStickY);
		if (axisValue != 0f || axisValue2 != 0f)
		{
			float curLargeMotorPower = GamePadInput.curLargeMotorPower;
			float curSmallMotorPower = GamePadInput.curSmallMotorPower;
			curLargeMotorPower -= axisValue * Time.deltaTime * 0.1f;
			curSmallMotorPower -= axisValue2 * Time.deltaTime * 0.1f;
			GamePadInput.SetVibration_On(Mathf.Clamp01(curLargeMotorPower), Mathf.Clamp01(curSmallMotorPower));
		}
		if (GamePadInput.IsButtonState(PadInput.InputType.Button3, PadInput.ButtonState.Down))
		{
			GamePadInput.SetVibration_Off();
		}
		if (GamePadInput.IsButtonState(PadInput.InputType.Button2, PadInput.ButtonState.Down))
		{
			GamePadInput.SetVibration_WithTime(3f, GamePadInput.VibrationType.Weak);
		}
		if (GamePadInput.IsButtonState(PadInput.InputType.Button1, PadInput.ButtonState.Down))
		{
			GamePadInput.SetVibration_WithTime(3f, GamePadInput.VibrationType.Middle);
		}
		if (GamePadInput.IsButtonState(PadInput.InputType.Button0, PadInput.ButtonState.Down))
		{
			GamePadInput.SetVibration_WithTime(3f, GamePadInput.VibrationType.Strong);
		}
		Color color = GamePadInput.curLightBarColor;
		if (GamePadInput.IsButtonState_Pushing(PadInput.InputType.ButtonL1))
		{
			color = Color.Lerp(color, Color.blue, Time.deltaTime * 4f);
		}
		if (GamePadInput.IsButtonState_Pushing(PadInput.InputType.ButtonL2))
		{
			color = Color.Lerp(color, Color.red, Time.deltaTime * 4f);
		}
		if (GamePadInput.IsButtonState_Pushing(PadInput.InputType.ButtonR1))
		{
			color = Color.Lerp(color, Color.magenta, Time.deltaTime * 4f);
		}
		if (GamePadInput.IsButtonState_Pushing(PadInput.InputType.ButtonR2))
		{
			color = Color.Lerp(color, Color.green, Time.deltaTime * 4f);
		}
		GamePadInput.SetLightbarColor(color);
	}

	private void UpdateThumbSticks()
	{
		Vector2 vector = new Vector2(GamePadInput.GetAxisValue(PadInput.InputType.LStickX), GamePadInput.GetAxisValue(PadInput.InputType.LStickY));
		Vector2 vector2 = new Vector2(GamePadInput.GetAxisValue(PadInput.InputType.RStickX), GamePadInput.GetAxisValue(PadInput.InputType.RStickY));
		m_GamePadElements.thumbstick_left.transform.localPosition = new Vector3(0.4f * vector.x, -0.4f * vector.y, 0f);
		m_GamePadElements.thumbstick_right.transform.localPosition = new Vector3(0.4f * vector2.x, -0.4f * vector2.y, 0f);
		m_GamePadElements.thumbstick_left.color = ((!GamePadInput.IsButtonState(PadInput.InputType.ButtonL3, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
		m_GamePadElements.thumbstick_right.color = ((!GamePadInput.IsButtonState(PadInput.InputType.ButtonR3, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
	}

	private void UpdateDPadButtons()
	{
		float axisValue = GamePadInput.GetAxisValue(PadInput.InputType.DPadX);
		float axisValue2 = GamePadInput.GetAxisValue(PadInput.InputType.DPadY);
		m_GamePadElements.dpad_left.color = ((!(axisValue < 0f)) ? inputOff : inputOn);
		m_GamePadElements.dpad_right.color = ((!(axisValue > 0f)) ? inputOff : inputOn);
		m_GamePadElements.dpad_up.color = ((!(axisValue2 < 0f)) ? inputOff : inputOn);
		m_GamePadElements.dpad_down.color = ((!(axisValue2 > 0f)) ? inputOff : inputOn);
	}

	private void UpdateFrontButtons()
	{
		m_GamePadElements.square.color = ((!GamePadInput.IsButtonState(PadInput.InputType.Button0, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
		m_GamePadElements.cross.color = ((!GamePadInput.IsButtonState(PadInput.InputType.Button1, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
		m_GamePadElements.circle.color = ((!GamePadInput.IsButtonState(PadInput.InputType.Button2, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
		m_GamePadElements.triangle.color = ((!GamePadInput.IsButtonState(PadInput.InputType.Button3, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
	}

	private void UpdateShoulderButtons()
	{
		m_GamePadElements.L1.color = ((!GamePadInput.IsButtonState(PadInput.InputType.ButtonL1, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
		m_GamePadElements.R1.color = ((!GamePadInput.IsButtonState(PadInput.InputType.ButtonR1, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
		m_GamePadElements.L2.color = ((!GamePadInput.IsButtonState(PadInput.InputType.ButtonL2, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
		m_GamePadElements.R2.color = ((!GamePadInput.IsButtonState(PadInput.InputType.ButtonR2, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
	}

	private void UpdateExtraButtons()
	{
		m_GamePadElements.touchpad.color = ((!GamePadInput.IsButtonState(PadInput.InputType.Button7, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
		m_GamePadElements.options.color = ((!GamePadInput.IsButtonState(PadInput.GameInput.OptionButton, PadInput.ButtonState.Pushing)) ? inputOff : inputOn);
	}
}
