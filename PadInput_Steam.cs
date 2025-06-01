using System;
using GameData;
using Rewired;
using Rewired.ControllerExtensions;
using Steamworks;
using UnityEngine;

public class PadInput_Steam : PadInput
{
	public enum GamePadType
	{
		None,
		Xbox,
		PlayStation,
		Unknown
	}

	private const int c_idPlayerGamePad = 0;

	private const int c_idPlayerKeyMouse = 1;

	private Player m_playerGamePad;

	private Player m_playerKeyMouse;

	private GamePadType m_curGamePadType;

	private bool m_steamInputInitialized;

	private ControllerHandle_t[] m_steamInputHandles;

	public GamePadType CurGamePadType => m_curGamePadType;

	public PadInput_Steam()
	{
		m_padInputStates[0] = new InputState(InputType.LStickX, ProcType.Axis, "LStickX");
		m_padInputStates[1] = new InputState(InputType.LStickY, ProcType.Axis, "LStickY");
		m_padInputStates[2] = new InputState(InputType.RStickX, ProcType.Axis, "RStickX");
		m_padInputStates[3] = new InputState(InputType.RStickY, ProcType.Axis, "RStickY");
		m_padInputStates[4] = new InputState(InputType.DPadX, ProcType.Axis, "DPadX");
		m_padInputStates[5] = new InputState(InputType.DPadY, ProcType.Axis, "DPadY");
		m_padInputStates[6] = new InputState(InputType.Button0, ProcType.Button, "Button0");
		m_padInputStates[7] = new InputState(InputType.Button1, ProcType.Button, "Button1");
		m_padInputStates[8] = new InputState(InputType.Button2, ProcType.Button, "Button2");
		m_padInputStates[9] = new InputState(InputType.Button3, ProcType.Button, "Button3");
		m_padInputStates[11] = new InputState(InputType.Button5, ProcType.Button, "Button5");
		m_padInputStates[13] = new InputState(InputType.Button7, ProcType.Button, "Button7");
		m_padInputStates[14] = new InputState(InputType.ButtonL1, ProcType.Button, "ButtonL1");
		m_padInputStates[15] = new InputState(InputType.ButtonR1, ProcType.Button, "ButtonR1");
		m_padInputStates[18] = new InputState(InputType.ButtonL3, ProcType.Button, "ButtonL3");
		m_padInputStates[19] = new InputState(InputType.ButtonR3, ProcType.Button, "ButtonR3");
		m_padInputStates[16] = new InputState(InputType.ButtonL2, ProcType.Button, "ButtonL2");
		m_padInputStates[17] = new InputState(InputType.ButtonR2, ProcType.Button, "ButtonR2");
		m_padInputStates[20] = new InputState(InputType.LStickX, ProcType.Axis, "LStickX");
		m_padInputStates[21] = new InputState(InputType.LStickY, ProcType.Axis, "LStickY");
		m_padInputStates[22] = new InputState(InputType.RStickY, ProcType.Axis, "RStickY");
		m_padInputStates[23] = new InputState(InputType.Button2, ProcType.Button, "Button2");
		m_padInputStates[24] = new InputState(InputType.Button1, ProcType.Button, "Button1");
		m_padInputStates[25] = new InputState(InputType.Button3, ProcType.Button, "Button3");
		m_padInputStates[26] = new InputState(InputType.Button0, ProcType.Button, "Button0");
		m_padInputStates[27] = new InputState(InputType.ButtonL1, ProcType.Button, "ButtonL1");
		m_padInputStates[28] = new InputState(InputType.ButtonR1, ProcType.Button, "ButtonR1");
		m_padInputStates[29] = new InputState(InputType.Button7, ProcType.Button, "Button7");
		m_padInputStates[30] = new InputState(InputType.Button5, ProcType.Button, "Button5");
		m_padInputStates[31] = new InputState(InputType.Keyboard_KeySkip, ProcType.Button, "SkipButton");
		AddGameInput2InputType(GameInput.LStickX, InputType.LStickX);
		AddGameInput2InputType(GameInput.LStickX, InputType.DPadX, isOldClear: false);
		AddGameInput2InputType(GameInput.LStickY, InputType.LStickY);
		AddGameInput2InputType(GameInput.LStickY, InputType.DPadY, isOldClear: false);
		AddGameInput2InputType(GameInput.RStickX, InputType.RStickX);
		AddGameInput2InputType(GameInput.RStickY, InputType.RStickY);
		AddGameInput2InputType(GameInput.CircleButton, InputType.Button2);
		AddGameInput2InputType(GameInput.CrossButton, InputType.Button1);
		AddGameInput2InputType(GameInput.TriangleButton, InputType.Button3);
		AddGameInput2InputType(GameInput.SquareButton, InputType.Button0);
		AddGameInput2InputType(GameInput.L1Button, InputType.ButtonL1);
		AddGameInput2InputType(GameInput.R1Button, InputType.ButtonR1);
		AddGameInput2InputType(GameInput.OptionButton, InputType.Button5);
		AddGameInput2InputType(GameInput.TouchPadButton, InputType.Button7);
		AddGameInput2InputType(GameInput.LStickX, InputType.Keyboard_Bound, isOldClear: false);
		AddGameInput2InputType(GameInput.LStickY, InputType.Keyboard_DirY, isOldClear: false);
		AddGameInput2InputType(GameInput.RStickY, InputType.Keyboard_PageUpDown, isOldClear: false);
		AddGameInput2InputType(GameInput.CircleButton, InputType.Keyboard_Key0, isOldClear: false);
		AddGameInput2InputType(GameInput.CrossButton, InputType.Keyboard_Key1, isOldClear: false);
		AddGameInput2InputType(GameInput.TriangleButton, InputType.Keyboard_Key2, isOldClear: false);
		AddGameInput2InputType(GameInput.SquareButton, InputType.Keyboard_Key3, isOldClear: false);
		AddGameInput2InputType(GameInput.L1Button, InputType.Keyboard_Key4, isOldClear: false);
		AddGameInput2InputType(GameInput.R1Button, InputType.Keyboard_Key5, isOldClear: false);
		AddGameInput2InputType(GameInput.OptionButton, InputType.Keyboard_Key7, isOldClear: false);
		AddGameInput2InputType(GameInput.TouchPadButton, InputType.Keyboard_Key6, isOldClear: false);
		AddGameInput2InputType(GameInput.SkipButton, InputType.Keyboard_KeySkip);
		KeyCode[] array = (KeyCode[])Enum.GetValues(typeof(KeyCode));
		int num = Array.IndexOf(array, KeyCode.None);
		int num2 = Array.IndexOf(array, KeyCode.Mouse6);
		int num3 = num2 - num;
		if (num3 > 0)
		{
			m_keyboardInputCheckCodes = new KeyCode[num3];
			Array.Copy(array, num + 1, m_keyboardInputCheckCodes, 0, num3);
		}
	}

	public override void Init()
	{
		ReInput.ControllerConnectedEvent += OnControllerConnected;
		ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
		m_playerGamePad = ReInput.players.GetPlayer(0);
		m_playerKeyMouse = ReInput.players.GetPlayer(1);
		m_steamInputInitialized = SteamController.Init();
		m_steamInputHandles = new ControllerHandle_t[16];
		CheckConnectedGamePad();
	}

	public override void Destroy()
	{
		ReInput.ControllerConnectedEvent -= OnControllerConnected;
		ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
	}

	private void OnControllerConnected(ControllerStatusChangedEventArgs args)
	{
		m_playerGamePad = ReInput.players.GetPlayer(0);
		m_playerKeyMouse = ReInput.players.GetPlayer(1);
		CheckConnectedGamePad();
	}

	private void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
	{
		m_playerGamePad = ReInput.players.GetPlayer(0);
		m_playerKeyMouse = ReInput.players.GetPlayer(1);
		CheckConnectedGamePad();
	}

	public override void Update()
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
		if (!ReInput.isReady)
		{
			return;
		}
		bool flag = false;
		if (m_playerGamePad != null)
		{
			int num2 = 20;
			for (int j = 0; j < num2; j++)
			{
				InputState inputState = m_padInputStates[j];
				if (inputState != null)
				{
					bool flag2 = false;
					switch (inputState.procType)
					{
					case ProcType.Axis:
						flag2 = CheckInputState_Axis(m_playerGamePad, inputState);
						break;
					case ProcType.Button:
						flag2 = CheckInputState_Button(m_playerGamePad, inputState);
						break;
					}
					if (!flag && flag2)
					{
						flag = true;
					}
				}
			}
		}
		bool flag3 = false;
		if (m_playerKeyMouse != null)
		{
			int num3 = m_padInputStates.Length;
			for (int k = 20; k < num3; k++)
			{
				InputState inputState2 = m_padInputStates[k];
				if (inputState2 != null)
				{
					bool flag4 = false;
					switch (inputState2.procType)
					{
					case ProcType.Axis:
						flag4 = CheckInputState_Axis(m_playerKeyMouse, inputState2);
						break;
					case ProcType.Button:
						flag4 = CheckInputState_Button(m_playerKeyMouse, inputState2);
						break;
					}
					if (!flag3 && flag4)
					{
						flag3 = true;
					}
				}
			}
		}
		if (!flag && !flag3 && PadInput.LastInputDeviceType != InputDeviceType.Keyboard)
		{
			KeyCode[] keyboardInputCheckCodes = m_keyboardInputCheckCodes;
			foreach (KeyCode key in keyboardInputCheckCodes)
			{
				if (Input.GetKeyDown(key))
				{
					flag3 = true;
					break;
				}
			}
		}
		CheckConnectedGamePad();
		CheckButtonIconChanging(flag, flag3);
	}

	protected bool CheckInputState_Axis(Player player, InputState inputState)
	{
		float axisValue = inputState.m_AxisValue;
		inputState.m_AxisValue = player.GetAxis(inputState.axisName);
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

	protected virtual bool CheckInputState_Button(Player player, InputState inputState)
	{
		bool button = player.GetButton(inputState.axisName);
		SetButtonInputState(ref inputState.m_ButtonState, ref inputState.m_Time, button);
		return button;
	}

	public override void SetVibration(float fLargeMotor_Normalized, float fSmallMotor_Normalized)
	{
		if (m_playerGamePad != null && ReInput.isReady)
		{
			m_playerGamePad.SetVibration(0, fLargeMotor_Normalized);
			m_playerGamePad.SetVibration(1, fSmallMotor_Normalized);
		}
	}

	private void CheckConnectedGamePad()
	{
		GamePadType gamePadType = GamePadType.None;
		if (m_playerGamePad != null && m_playerGamePad.controllers != null && m_playerGamePad.controllers.Joysticks != null && m_playerGamePad.controllers.Joysticks.Count > 0)
		{
			GameSwitch instance = GameSwitch.GetInstance();
			switch (instance.GetControllerType())
			{
			case GameSwitch.eControllerType.Auto:
				if (!Application.isEditor && m_steamInputInitialized)
				{
					ESteamInputType eSteamInputType = ESteamInputType.k_ESteamInputType_Unknown;
					int connectedControllers = SteamController.GetConnectedControllers(m_steamInputHandles);
					if (connectedControllers > 0)
					{
						ControllerHandle_t controllerForGamepadIndex = SteamController.GetControllerForGamepadIndex(0);
						eSteamInputType = SteamController.GetInputTypeForHandle(controllerForGamepadIndex);
						gamePadType = ((eSteamInputType != ESteamInputType.k_ESteamInputType_PS4Controller) ? GamePadType.Xbox : GamePadType.PlayStation);
					}
				}
				else
				{
					Joystick joystick = m_playerGamePad.controllers.Joysticks[0];
					if (joystick != null)
					{
						IDualShock4Extension extension = joystick.GetExtension<IDualShock4Extension>();
						gamePadType = ((extension == null) ? GamePadType.Xbox : GamePadType.PlayStation);
					}
				}
				break;
			case GameSwitch.eControllerType.SonyDualShock:
				gamePadType = GamePadType.PlayStation;
				break;
			case GameSwitch.eControllerType.XboxController:
				gamePadType = GamePadType.Xbox;
				break;
			}
		}
		if (m_curGamePadType != gamePadType)
		{
			m_curGamePadType = gamePadType;
			if (m_curGamePadType == GamePadType.None)
			{
				CheckButtonIconChanging(isGamePadInputed: false, isKeyboardInputed: true);
			}
			else if (PadInput.s_curButtonIconType != ButtonIconType.Keyboard)
			{
				CheckButtonIconChanging(isGamePadInputed: true, isKeyboardInputed: false);
			}
		}
	}

	private void CheckButtonIconChanging(bool isGamePadInputed, bool isKeyboardInputed)
	{
		ButtonIconType buttonIconType = PadInput.s_curButtonIconType;
		if (isGamePadInputed && m_curGamePadType != GamePadType.None)
		{
			buttonIconType = ((m_curGamePadType != GamePadType.PlayStation) ? ButtonIconType.XBox : ButtonIconType.PS4);
		}
		else if (isKeyboardInputed)
		{
			buttonIconType = ButtonIconType.Keyboard;
		}
		if (buttonIconType != PadInput.s_curButtonIconType)
		{
			PadInput.s_curButtonIconType = buttonIconType;
			GameSwitch.eUIButType eUIButType = GameSwitch.eUIButType.DEF;
			eUIButType = PadInput.s_curButtonIconType switch
			{
				ButtonIconType.PS4 => GameSwitch.eUIButType.PS4, 
				ButtonIconType.XBox => GameSwitch.eUIButType.XBOX, 
				ButtonIconType.Keyboard => GameSwitch.eUIButType.KEYMOUSE, 
				_ => GameSwitch.eUIButType.KEYMOUSE, 
			};
			GameSwitch.GetInstance().SetUIButType(eUIButType);
			CommonButtonGuide commonButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
			if (commonButtonGuide != null)
			{
				commonButtonGuide.ReflashContents();
			}
			PadIconHandler.ReflashIcons();
		}
	}

	public override void ReflashControllerType()
	{
		GamePadType curGamePadType = m_curGamePadType;
		CheckConnectedGamePad();
		if (curGamePadType != m_curGamePadType)
		{
			bool flag = PadInput.s_curButtonIconType != ButtonIconType.Keyboard;
			CheckButtonIconChanging(flag, !flag);
		}
	}
}
