using System;
using UnityEngine;

public class GamePadInput
{
	public enum StickDir
	{
		None = -1,
		Left,
		Right,
		Up,
		Down,
		Count
	}

	public enum VibrationType
	{
		Weak,
		Middle,
		Strong
	}

	private static PadInput s_PadInput = null;

	private const float c_MaxVibrationTime = 3f;

	private static float s_fVibrationTime = 0f;

	private static float s_fCurVibration_LargeMotor = 0f;

	private static float s_fCurVibration_SmallMotor = 0f;

	private static Color s_clrCurLightBar = Color.black;

	private static Vector2[] s_VibrationPowers = null;

	public static float curLargeMotorPower => s_fCurVibration_LargeMotor;

	public static float curSmallMotorPower => s_fCurVibration_SmallMotor;

	public static Color curLightBarColor => s_clrCurLightBar;

	public static int loggedInUserID
	{
		get
		{
			if (s_PadInput == null)
			{
				return -1;
			}
			return s_PadInput.loggedInUserID;
		}
	}

	public static PadInput_Steam.GamePadType CurrentGamePadType
	{
		get
		{
			if (s_PadInput == null)
			{
				return PadInput_Steam.GamePadType.None;
			}
			PadInput_Steam padInput_Steam = s_PadInput as PadInput_Steam;
			return padInput_Steam.CurGamePadType;
		}
	}

	public static void Init()
	{
		s_PadInput = new PadInput_Steam();
		if (s_PadInput != null)
		{
			s_PadInput.Init();
		}
		int length = Enum.GetValues(typeof(VibrationType)).Length;
		s_VibrationPowers = new Vector2[length];
		ref Vector2 reference = ref s_VibrationPowers[0];
		reference = new Vector2(20f / 51f, 0.11764706f);
		ref Vector2 reference2 = ref s_VibrationPowers[1];
		reference2 = new Vector2(0.7058824f, 20f / 51f);
		ref Vector2 reference3 = ref s_VibrationPowers[2];
		reference3 = new Vector2(1f, 0.7058824f);
	}

	public static void Destroy()
	{
		if (s_PadInput != null)
		{
			s_PadInput.Destroy();
		}
		s_PadInput = null;
	}

	public static void Update()
	{
		if (LoadingScreen.GetCurrentInstance != null || SavingScreen.GetCurrentInstance != null)
		{
			Reset();
		}
		else
		{
			if (s_PadInput == null)
			{
				return;
			}
			s_PadInput.Update();
			if (s_fVibrationTime > 0f && !GameGlobalUtil.IsAlmostSame(s_fVibrationTime, 0f))
			{
				s_fVibrationTime -= Time.deltaTime;
				if (s_fVibrationTime < 0f || GameGlobalUtil.IsAlmostSame(s_fVibrationTime, 0f))
				{
					SetVibration_Off();
					s_fVibrationTime = 0f;
				}
			}
		}
	}

	public static void Reset()
	{
		if (s_PadInput != null)
		{
			s_PadInput.Reset();
		}
	}

	public static float GetAxisValue(PadInput.InputType inputType)
	{
		if (s_PadInput == null)
		{
			return 0f;
		}
		return s_PadInput.GetAxisValue(inputType);
	}

	public static float GetAxisValue(PadInput.GameInput gameInput)
	{
		if (s_PadInput == null)
		{
			return 0f;
		}
		return s_PadInput.GetAxisValue(gameInput);
	}

	public static PadInput.ButtonState GetButtonState(PadInput.InputType inputType, bool isAxisPositive = false)
	{
		if (s_PadInput == null)
		{
			return PadInput.ButtonState.None;
		}
		return s_PadInput.GetButtonState(inputType, isAxisPositive);
	}

	public static PadInput.ButtonState GetButtonState(PadInput.GameInput gameInput, bool isAxisPositive = false)
	{
		if (s_PadInput == null)
		{
			return PadInput.ButtonState.None;
		}
		return s_PadInput.GetButtonState(gameInput, isAxisPositive);
	}

	public static float GetInputedTime(PadInput.InputType inputType)
	{
		if (s_PadInput == null)
		{
			return 0f;
		}
		return s_PadInput.GetInputedTime(inputType);
	}

	public static float GetInputedTime(PadInput.GameInput gameInput)
	{
		if (s_PadInput == null)
		{
			return 0f;
		}
		return s_PadInput.GetInputedTime(gameInput);
	}

	public static StickDir GetLStickDownDir()
	{
		StickDir result = StickDir.None;
		if (IsButtonState_Down(PadInput.GameInput.LStickX))
		{
			result = StickDir.Left;
		}
		else if (IsButtonState_Down(PadInput.GameInput.LStickX, isAxisPositive: true))
		{
			result = StickDir.Right;
		}
		else if (IsButtonState_Down(PadInput.GameInput.LStickY, isAxisPositive: true))
		{
			result = StickDir.Down;
		}
		else if (IsButtonState_Down(PadInput.GameInput.LStickY))
		{
			result = StickDir.Up;
		}
		return result;
	}

	public static bool GetLStickMove(out float fAxisX, out float fAxisY)
	{
		fAxisX = GetAxisValue(PadInput.GameInput.LStickX);
		fAxisY = GetAxisValue(PadInput.GameInput.LStickY);
		return !GameGlobalUtil.IsAlmostSame(fAxisX, 0f) || !GameGlobalUtil.IsAlmostSame(fAxisY, 0f);
	}

	public static bool GetLStickMove(out Vector2 axis)
	{
		axis = default(Vector2);
		return GetLStickMove(out axis.x, out axis.y);
	}

	public static Vector2 GetLStickMove()
	{
		Vector2 result = default(Vector2);
		GetLStickMove(out result.x, out result.y);
		return result;
	}

	public static bool GetRStickMove(out float fAxisX, out float fAxisY)
	{
		fAxisX = GetAxisValue(PadInput.GameInput.RStickX);
		fAxisY = GetAxisValue(PadInput.GameInput.RStickY);
		return !GameGlobalUtil.IsAlmostSame(fAxisX, 0f) || !GameGlobalUtil.IsAlmostSame(fAxisY, 0f);
	}

	public static bool GetRStickMove(out Vector2 axis)
	{
		axis = default(Vector2);
		return GetRStickMove(out axis.x, out axis.y);
	}

	public static Vector2 GetRStickMove()
	{
		Vector2 result = default(Vector2);
		GetRStickMove(out result.x, out result.y);
		return result;
	}

	public static bool IsButtonState(PadInput.InputType inputType, PadInput.ButtonState btnState, bool isAxisPositive = false)
	{
		PadInput.ButtonState buttonState = GetButtonState(inputType, isAxisPositive);
		return (buttonState & btnState) == btnState;
	}

	public static bool IsButtonState(PadInput.GameInput gameInput, PadInput.ButtonState btnState, bool isAxisPositive = false)
	{
		PadInput.ButtonState buttonState = GetButtonState(gameInput, isAxisPositive);
		return (buttonState & btnState) == btnState;
	}

	public static bool IsButtonState_None(PadInput.InputType inputType, bool isAxisPositive = false)
	{
		PadInput.ButtonState buttonState = GetButtonState(inputType, isAxisPositive);
		return buttonState == PadInput.ButtonState.None;
	}

	public static bool IsButtonState_None(PadInput.GameInput gameInput, bool isAxisPositive = false)
	{
		PadInput.ButtonState buttonState = GetButtonState(gameInput, isAxisPositive);
		return buttonState == PadInput.ButtonState.None;
	}

	public static bool IsButtonState_Down(PadInput.InputType inputType, bool isAxisPositive = false)
	{
		return IsButtonState(inputType, PadInput.ButtonState.Down, isAxisPositive);
	}

	public static bool IsButtonState_Down(PadInput.GameInput gameInput, bool isAxisPositive = false)
	{
		return IsButtonState(gameInput, PadInput.ButtonState.Down, isAxisPositive);
	}

	public static bool IsButtonState_Up(PadInput.InputType inputType, bool isAxisPositive = false)
	{
		return IsButtonState(inputType, PadInput.ButtonState.Up, isAxisPositive);
	}

	public static bool IsButtonState_Up(PadInput.GameInput gameInput, bool isAxisPositive = false)
	{
		return IsButtonState(gameInput, PadInput.ButtonState.Up, isAxisPositive);
	}

	public static bool IsButtonState_Pushing(PadInput.InputType inputType, bool isAxisPositive = false)
	{
		return IsButtonState(inputType, PadInput.ButtonState.Pushing, isAxisPositive);
	}

	public static bool IsButtonState_Pushing(PadInput.GameInput gameInput, bool isAxisPositive = false)
	{
		return IsButtonState(gameInput, PadInput.ButtonState.Pushing, isAxisPositive);
	}

	public static bool IsLStickState_Left(PadInput.ButtonState btnState)
	{
		return IsButtonState(PadInput.GameInput.LStickX, btnState);
	}

	public static bool IsLStickState_Right(PadInput.ButtonState btnState)
	{
		return IsButtonState(PadInput.GameInput.LStickX, btnState, isAxisPositive: true);
	}

	public static bool IsLStickState_Up(PadInput.ButtonState btnState)
	{
		return IsButtonState(PadInput.GameInput.LStickY, btnState);
	}

	public static bool IsLStickState_Down(PadInput.ButtonState btnState)
	{
		return IsButtonState(PadInput.GameInput.LStickY, btnState, isAxisPositive: true);
	}

	public static bool IsRStickState_Left(PadInput.ButtonState btnState)
	{
		return IsButtonState(PadInput.GameInput.RStickX, btnState);
	}

	public static bool IsRStickState_Right(PadInput.ButtonState btnState)
	{
		return IsButtonState(PadInput.GameInput.RStickX, btnState, isAxisPositive: true);
	}

	public static bool IsRStickState_Up(PadInput.ButtonState btnState)
	{
		return IsButtonState(PadInput.GameInput.RStickY, btnState);
	}

	public static bool IsRStickState_Down(PadInput.ButtonState btnState)
	{
		return IsButtonState(PadInput.GameInput.RStickY, btnState, isAxisPositive: true);
	}

	public static void SetVibration_WithTime(float fTime, float fLargeMoter_Normalized, float fSmallMoter_Normalized)
	{
		s_fVibrationTime = fTime;
		if (s_fVibrationTime <= 0f || s_fVibrationTime > 3f)
		{
			s_fVibrationTime = 3f;
		}
		s_fCurVibration_LargeMotor = Mathf.Clamp01(fLargeMoter_Normalized);
		s_fCurVibration_SmallMotor = Mathf.Clamp01(fSmallMoter_Normalized);
		if (s_PadInput != null)
		{
			s_PadInput.SetVibration(s_fCurVibration_LargeMotor, s_fCurVibration_SmallMotor);
		}
	}

	public static void SetVibration_WithTime(float fTime, VibrationType vibType)
	{
		if (vibType >= VibrationType.Weak && (int)vibType < s_VibrationPowers.Length)
		{
			Vector2 vector = s_VibrationPowers[(int)vibType];
			SetVibration_WithTime(fTime, vector.x, vector.y);
		}
	}

	public static void SetVibration_On(float fLargeMoter_Normalized, float fSmallMoter_Normalized)
	{
		SetVibration_WithTime(0f, fLargeMoter_Normalized, fSmallMoter_Normalized);
	}

	public static void SetVibration_On(VibrationType vibType)
	{
		if (vibType >= VibrationType.Weak && (int)vibType < s_VibrationPowers.Length)
		{
			Vector2 vector = s_VibrationPowers[(int)vibType];
			SetVibration_WithTime(0f, vector.x, vector.y);
		}
	}

	public static void SetVibration_Off()
	{
		s_fCurVibration_LargeMotor = 0f;
		s_fCurVibration_SmallMotor = 0f;
		if (s_PadInput != null)
		{
			s_PadInput.SetVibration(0f, 0f);
		}
	}

	public static bool IsCurVabrating()
	{
		return (!(s_fCurVibration_LargeMotor < 0f) && !GameGlobalUtil.IsAlmostSame(s_fCurVibration_LargeMotor, 0f)) || (!(s_fCurVibration_SmallMotor < 0f) && !GameGlobalUtil.IsAlmostSame(s_fCurVibration_SmallMotor, 0f));
	}

	public static void SetLightbarColor(int iRed, int iGreen, int iBlue)
	{
		iRed = Mathf.Clamp(iRed, 0, 255);
		iGreen = Mathf.Clamp(iGreen, 0, 255);
		iBlue = Mathf.Clamp(iBlue, 0, 255);
		if (s_PadInput != null)
		{
			s_PadInput.SetLightbarColor(iRed, iGreen, iBlue);
		}
		s_clrCurLightBar = new Color((float)iRed / 255f, (float)iGreen / 255f, (float)iBlue / 255f);
	}

	public static void SetLightbarColor(Color color)
	{
		if (s_PadInput != null)
		{
			s_PadInput.SetLightbarColor(Mathf.RoundToInt(color.r * 255f), Mathf.RoundToInt(color.g * 255f), Mathf.RoundToInt(color.b * 255f));
		}
		s_clrCurLightBar = color;
	}

	public static void SetLigthbarColor_ToDefault()
	{
		if (s_PadInput != null)
		{
			s_PadInput.SetLightbarColor_ToDefault();
		}
	}

	public static string GetIconImageName(PadInput.InputType inputType, bool isAxisIgnore = false)
	{
		if (s_PadInput == null)
		{
			return null;
		}
		return s_PadInput.GetIconImageName(inputType, isAxisIgnore);
	}

	public static string[] GetIconImageNames(PadInput.GameInput gameInput, bool isAxisIgnore = false)
	{
		if (s_PadInput == null)
		{
			return null;
		}
		return s_PadInput.GetIconImageNames(gameInput, isAxisIgnore);
	}

	public static void SetCircleButtonSubmit(bool isSubmit)
	{
		if (s_PadInput != null)
		{
			if (isSubmit)
			{
				s_PadInput.AddGameInput2InputType(PadInput.GameInput.CircleButton, PadInput.InputType.Button2);
				s_PadInput.AddGameInput2InputType(PadInput.GameInput.CrossButton, PadInput.InputType.Button1);
			}
			else
			{
				s_PadInput.AddGameInput2InputType(PadInput.GameInput.CircleButton, PadInput.InputType.Button1);
				s_PadInput.AddGameInput2InputType(PadInput.GameInput.CrossButton, PadInput.InputType.Button2);
			}
			s_PadInput.AddGameInput2InputType(PadInput.GameInput.CircleButton, PadInput.InputType.Keyboard_Key0, isOldClear: false);
			s_PadInput.AddGameInput2InputType(PadInput.GameInput.CrossButton, PadInput.InputType.Keyboard_Key1, isOldClear: false);
		}
	}

	public static void ReflashControllerType()
	{
		if (s_PadInput != null)
		{
			s_PadInput.ReflashControllerType();
		}
	}

	public static float GetMouseWheelScrollDelta()
	{
		return Input.mouseScrollDelta.y;
	}

	public static bool TryMouseWheelScrollPower(out float wheelScrollPower)
	{
		wheelScrollPower = 0f;
		float mouseWheelScrollDelta = GetMouseWheelScrollDelta();
		if (GameGlobalUtil.IsAlmostSame(mouseWheelScrollDelta, 0f))
		{
			return false;
		}
		wheelScrollPower = mouseWheelScrollDelta * 2000f;
		return true;
	}
}
