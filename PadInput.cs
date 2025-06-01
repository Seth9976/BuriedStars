using System;
using System.Collections.Generic;
using UnityEngine;

public class PadInput
{
	public enum InputType
	{
		LStickX = 0,
		LStickY = 1,
		RStickX = 2,
		RStickY = 3,
		DPadX = 4,
		DPadY = 5,
		Button0 = 6,
		Button1 = 7,
		Button2 = 8,
		Button3 = 9,
		Button4 = 10,
		Button5 = 11,
		Button6 = 12,
		Button7 = 13,
		ButtonL1 = 14,
		ButtonR1 = 15,
		ButtonL2 = 16,
		ButtonR2 = 17,
		ButtonL3 = 18,
		ButtonR3 = 19,
		Keyboard_Bound = 20,
		Keyboard_DirX = 20,
		Keyboard_DirY = 21,
		Keyboard_PageUpDown = 22,
		Keyboard_Key0 = 23,
		Keyboard_Key1 = 24,
		Keyboard_Key2 = 25,
		Keyboard_Key3 = 26,
		Keyboard_Key4 = 27,
		Keyboard_Key5 = 28,
		Keyboard_Key6 = 29,
		Keyboard_Key7 = 30,
		Keyboard_KeySkip = 31
	}

	public enum GameInput
	{
		None = -1,
		LStickX,
		LStickY,
		RStickX,
		RStickY,
		CircleButton,
		CrossButton,
		TriangleButton,
		SquareButton,
		L1Button,
		R1Button,
		OptionButton,
		TouchPadButton,
		SkipButton,
		Count
	}

	public enum ProcType
	{
		Axis,
		Button,
		Trigger
	}

	public enum ButtonState
	{
		None = 0,
		Down = 2,
		Up = 4,
		Pushing = 1
	}

	protected class InputState
	{
		private InputType m_inputType;

		private ProcType m_procType;

		private string m_AxisName = string.Empty;

		private KeyCode m_keyCode;

		public GameInput m_MatchedGameInput = GameInput.None;

		public float m_AxisValue;

		public ButtonState m_ButtonState;

		public ButtonState m_ButtonState2;

		public float m_Time;

		public InputType inputType => m_inputType;

		public ProcType procType => m_procType;

		public string axisName => m_AxisName;

		public KeyCode keyCode => m_keyCode;

		public InputState(InputType _type, ProcType _procType, string _axisName = "", KeyCode _keyCode = KeyCode.None)
		{
			m_inputType = _type;
			m_procType = _procType;
			m_AxisName = _axisName;
			m_keyCode = _keyCode;
		}
	}

	protected class GameInputData
	{
		public List<InputType> m_matchedTypes = new List<InputType>();

		public bool m_isSetted;

		public float m_AxisValue;

		public ButtonState m_ButtonState;

		public ButtonState m_ButtonState2;

		public float m_Time;
	}

	public enum InputDeviceType
	{
		Unknown,
		Keyboard,
		Joypad
	}

	public enum ButtonIconType
	{
		None,
		Keyboard,
		PS4,
		XBox,
		Switch
	}

	protected InputState[] m_padInputStates;

	protected GameInputData[] m_gameInputDatas;

	protected KeyCode[] m_keyboardInputCheckCodes;

	protected static InputDeviceType s_lastInputDeviceType;

	protected static ButtonIconType s_curButtonIconType;

	public static InputDeviceType LastInputDeviceType => s_lastInputDeviceType;

	public static ButtonIconType CurButtonIconType => s_curButtonIconType;

	public virtual int loggedInUserID => 0;

	protected PadInput()
	{
		string[] names = Enum.GetNames(typeof(InputType));
		m_padInputStates = new InputState[names.Length];
		m_gameInputDatas = new GameInputData[13];
		for (int i = 0; i < m_gameInputDatas.Length; i++)
		{
			m_gameInputDatas[i] = new GameInputData();
		}
	}

	public virtual void Init()
	{
	}

	public virtual void Destroy()
	{
	}

	public virtual void Update()
	{
		int num = m_gameInputDatas.Length;
		GameInputData gameInputData = null;
		for (int i = 0; i < num; i++)
		{
			gameInputData = m_gameInputDatas[i];
			if (gameInputData != null)
			{
				gameInputData.m_isSetted = false;
				gameInputData.m_AxisValue = 0f;
				gameInputData.m_ButtonState = ButtonState.None;
				gameInputData.m_ButtonState2 = ButtonState.None;
				gameInputData.m_Time = 0f;
			}
		}
		num = m_padInputStates.Length;
		InputState inputState = null;
		for (int j = 0; j < num; j++)
		{
			inputState = m_padInputStates[j];
			if (inputState != null)
			{
				switch (inputState.procType)
				{
				case ProcType.Axis:
					CheckInputState_Axis(inputState);
					break;
				case ProcType.Button:
					CheckInputState_Button(inputState);
					break;
				case ProcType.Trigger:
					CheckInputState_Trigger(inputState);
					break;
				}
			}
		}
	}

	public virtual void Reset()
	{
		int num = m_padInputStates.Length;
		InputState inputState = null;
		for (int i = 0; i < num; i++)
		{
			inputState = m_padInputStates[i];
			if (inputState != null)
			{
				inputState.m_AxisValue = 0f;
				inputState.m_ButtonState = ButtonState.None;
				inputState.m_ButtonState2 = ButtonState.None;
				inputState.m_Time = 0f;
			}
		}
		num = m_gameInputDatas.Length;
		GameInputData gameInputData = null;
		for (int j = 0; j < num; j++)
		{
			gameInputData = m_gameInputDatas[j];
			if (gameInputData != null)
			{
				gameInputData.m_isSetted = false;
				gameInputData.m_AxisValue = 0f;
				gameInputData.m_ButtonState = ButtonState.None;
				gameInputData.m_ButtonState2 = ButtonState.None;
				gameInputData.m_Time = 0f;
			}
		}
	}

	public virtual void AddGameInput2InputType(GameInput gameInput, InputType inputType, bool isOldClear = true)
	{
		if (gameInput >= GameInput.LStickX && (int)gameInput < m_gameInputDatas.Length)
		{
			InputState inputState = m_padInputStates[(int)inputType];
			if (inputState.m_MatchedGameInput != GameInput.None)
			{
				RemoveGameInput2InputType(inputState.m_MatchedGameInput, inputType);
			}
			GameInputData gameInputData = m_gameInputDatas[(int)gameInput];
			if (isOldClear)
			{
				gameInputData.m_matchedTypes.Clear();
			}
			gameInputData.m_matchedTypes.Add(inputType);
			inputState.m_MatchedGameInput = gameInput;
		}
	}

	public virtual void RemoveGameInput2InputType(GameInput gameInput, InputType inputType)
	{
		if (gameInput >= GameInput.LStickX && (int)gameInput < m_gameInputDatas.Length)
		{
			RemoveGameInput2InputType(m_gameInputDatas[(int)gameInput], inputType);
		}
	}

	protected virtual void RemoveGameInput2InputType(GameInputData gameInputData, InputType inputType)
	{
		gameInputData.m_matchedTypes.Remove(inputType);
		m_padInputStates[(int)inputType].m_MatchedGameInput = GameInput.None;
	}

	protected virtual bool CheckInputState_Axis(InputState inputState)
	{
		float axisValue = inputState.m_AxisValue;
		inputState.m_AxisValue = Input.GetAxis(inputState.axisName);
		bool result = true;
		bool isPushed = inputState.m_AxisValue < 0f;
		bool isPushed2 = inputState.m_AxisValue > 0f;
		if (GameGlobalUtil.IsAlmostSame(inputState.m_AxisValue, 0f))
		{
			inputState.m_AxisValue = 0f;
			inputState.m_Time = 0f;
			isPushed = false;
			isPushed2 = false;
			result = false;
		}
		else if ((inputState.m_AxisValue > 0f && axisValue >= 0f) || (inputState.m_AxisValue < 0f && axisValue <= 0f))
		{
			inputState.m_Time += Time.deltaTime;
		}
		else
		{
			inputState.m_Time = Time.deltaTime;
		}
		float fTime = 0f;
		SetButtonInputState(ref inputState.m_ButtonState, ref fTime, isPushed);
		SetButtonInputState(ref inputState.m_ButtonState2, ref fTime, isPushed2);
		return result;
	}

	protected virtual bool CheckInputState_Button(InputState inputState)
	{
		bool button = Input.GetButton(inputState.axisName);
		SetButtonInputState(ref inputState.m_ButtonState, ref inputState.m_Time, button);
		return button;
	}

	protected virtual bool CheckInputState_Trigger(InputState inputState)
	{
		inputState.m_AxisValue = Input.GetAxis(inputState.axisName);
		bool flag = false;
		if (!GameGlobalUtil.IsAlmostSame(inputState.m_AxisValue, 0f))
		{
			flag = true;
			inputState.m_AxisValue = Mathf.Abs(inputState.m_AxisValue);
		}
		else
		{
			inputState.m_AxisValue = 0f;
		}
		SetButtonInputState(ref inputState.m_ButtonState, ref inputState.m_Time, flag);
		return flag;
	}

	protected virtual void SetButtonInputState(ref ButtonState buttonState, ref float fTime, bool isPushed)
	{
		if ((buttonState & ButtonState.Pushing) == ButtonState.Pushing)
		{
			buttonState = (isPushed ? ButtonState.Pushing : ButtonState.Up);
		}
		else
		{
			buttonState = (isPushed ? ((ButtonState)3) : ButtonState.None);
		}
		fTime = ((!isPushed) ? 0f : (fTime + Time.deltaTime));
	}

	protected InputState GetInputState(InputType inputType)
	{
		return (inputType < InputType.LStickX || (int)inputType >= m_padInputStates.Length) ? null : m_padInputStates[(int)inputType];
	}

	protected GameInputData GetGameInputData(GameInput gameInput)
	{
		return (gameInput < GameInput.LStickX || (int)gameInput >= m_gameInputDatas.Length) ? null : m_gameInputDatas[(int)gameInput];
	}

	public virtual float GetAxisValue(InputType inputType)
	{
		return GetInputState(inputType)?.m_AxisValue ?? 0f;
	}

	public virtual float GetAxisValue(GameInput gameInput)
	{
		GameInputData gameInputData = GetGameInputData(gameInput);
		if (gameInputData == null || gameInputData.m_matchedTypes.Count <= 0)
		{
			return 0f;
		}
		if (gameInputData.m_isSetted)
		{
			return gameInputData.m_AxisValue;
		}
		SetGameInputData(ref gameInputData);
		return gameInputData.m_AxisValue;
	}

	public virtual ButtonState GetButtonState(InputType inputType, bool isAxisPositive = false)
	{
		InputState inputState = GetInputState(inputType);
		return (inputState != null) ? ((!isAxisPositive) ? inputState.m_ButtonState : inputState.m_ButtonState2) : ButtonState.None;
	}

	public virtual ButtonState GetButtonState(GameInput gameInput, bool isAxisPositive = false)
	{
		GameInputData gameInputData = GetGameInputData(gameInput);
		if (gameInputData == null || gameInputData.m_matchedTypes.Count <= 0)
		{
			return ButtonState.None;
		}
		if (!gameInputData.m_isSetted)
		{
			SetGameInputData(ref gameInputData);
		}
		return (!isAxisPositive) ? gameInputData.m_ButtonState : gameInputData.m_ButtonState2;
	}

	public virtual float GetInputedTime(InputType inputType)
	{
		return GetInputState(inputType)?.m_Time ?? 0f;
	}

	public virtual float GetInputedTime(GameInput gameInput)
	{
		GameInputData gameInputData = GetGameInputData(gameInput);
		if (gameInputData == null || gameInputData.m_matchedTypes.Count <= 0)
		{
			return 0f;
		}
		if (gameInputData.m_isSetted)
		{
			return gameInputData.m_Time;
		}
		SetGameInputData(ref gameInputData);
		return gameInputData.m_Time;
	}

	private void SetGameInputData(ref GameInputData gameInputData)
	{
		if (gameInputData.m_isSetted)
		{
			return;
		}
		gameInputData.m_AxisValue = 0f;
		gameInputData.m_ButtonState = ButtonState.None;
		gameInputData.m_ButtonState2 = ButtonState.None;
		gameInputData.m_Time = 0f;
		InputState inputState = null;
		int count = gameInputData.m_matchedTypes.Count;
		for (int i = 0; i < count; i++)
		{
			inputState = GetInputState(gameInputData.m_matchedTypes[i]);
			if (inputState != null)
			{
				if (Mathf.Abs(gameInputData.m_AxisValue) < Mathf.Abs(inputState.m_AxisValue))
				{
					gameInputData.m_AxisValue = inputState.m_AxisValue;
				}
				gameInputData.m_ButtonState |= inputState.m_ButtonState;
				gameInputData.m_ButtonState2 |= inputState.m_ButtonState2;
				if (gameInputData.m_Time < inputState.m_Time)
				{
					gameInputData.m_Time = inputState.m_Time;
				}
			}
		}
		gameInputData.m_isSetted = true;
	}

	public virtual void SetVibration(float fLargeMotor_Normalized, float fSmallMotor_Normalized)
	{
	}

	public virtual void SetLightbarColor(int iRed, int iGreen, int iBlue)
	{
	}

	public virtual void SetLightbarColor_ToDefault()
	{
	}

	public virtual string GetIconImageName(InputType inputType, bool isAxisIgnore = false)
	{
		if (s_curButtonIconType != ButtonIconType.Keyboard)
		{
			return inputType switch
			{
				InputType.LStickX => string.Empty, 
				InputType.LStickY => string.Empty, 
				InputType.RStickX => string.Empty, 
				InputType.RStickY => string.Empty, 
				InputType.DPadX => (!isAxisIgnore) ? "Directional_03_Left_Right" : "Directional_02_All", 
				InputType.DPadY => (!isAxisIgnore) ? "Directional_01_Up_Down" : "Directional_02_All", 
				InputType.Button0 => "Button_Square", 
				InputType.Button1 => "Button_X", 
				InputType.Button2 => "Button_O", 
				InputType.Button3 => "Button_Triangle", 
				InputType.Button4 => string.Empty, 
				InputType.Button5 => "Button_Options", 
				InputType.Button6 => string.Empty, 
				InputType.Button7 => "Button_Touchpad", 
				InputType.ButtonL1 => "Button_L1", 
				InputType.ButtonR1 => "Button_R1", 
				InputType.ButtonL2 => string.Empty, 
				InputType.ButtonR2 => string.Empty, 
				InputType.ButtonL3 => string.Empty, 
				InputType.ButtonR3 => string.Empty, 
				_ => string.Empty, 
			};
		}
		return inputType switch
		{
			InputType.Keyboard_Bound => (!isAxisIgnore) ? "Directional_03_Left_Right" : "Directional_02_All", 
			InputType.Keyboard_DirY => (!isAxisIgnore) ? "Directional_01_Up_Down" : "Directional_02_All", 
			InputType.Keyboard_PageUpDown => string.Empty, 
			InputType.Keyboard_Key0 => "Key_Enter", 
			InputType.Keyboard_Key1 => "Key_Backspace", 
			InputType.Keyboard_Key2 => "Key_A", 
			InputType.Keyboard_Key3 => "Key_S", 
			InputType.Keyboard_Key4 => "Key_[", 
			InputType.Keyboard_Key5 => "Key_]", 
			InputType.Keyboard_Key6 => "Key_Q", 
			InputType.Keyboard_Key7 => "Key_Esc", 
			InputType.Keyboard_KeySkip => "Key_Ctrl", 
			_ => string.Empty, 
		};
	}

	public virtual string[] GetIconImageNames(GameInput gameInput, bool isAxisIgnore = false)
	{
		if (gameInput < GameInput.LStickX || (int)gameInput >= m_gameInputDatas.Length)
		{
			return null;
		}
		GameInputData gameInputData = m_gameInputDatas[(int)gameInput];
		if (gameInputData == null || gameInputData.m_matchedTypes.Count <= 0)
		{
			return null;
		}
		List<string> list = new List<string>();
		string empty = string.Empty;
		int count = gameInputData.m_matchedTypes.Count;
		for (int i = 0; i < count; i++)
		{
			empty = GetIconImageName(gameInputData.m_matchedTypes[i], isAxisIgnore);
			if (!string.IsNullOrEmpty(empty) && !list.Contains(empty))
			{
				list.Add(empty);
			}
		}
		return (list.Count <= 0) ? null : list.ToArray();
	}

	public virtual void ReflashControllerType()
	{
	}
}
