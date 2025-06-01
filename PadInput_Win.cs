public class PadInput_Win : PadInput
{
	public PadInput_Win()
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
		m_padInputStates[10] = new InputState(InputType.Button4, ProcType.Button, "Button8");
		m_padInputStates[11] = new InputState(InputType.Button5, ProcType.Button, "Button9");
		m_padInputStates[12] = new InputState(InputType.Button6, ProcType.Button, "Button12");
		m_padInputStates[13] = new InputState(InputType.Button7, ProcType.Button, "Button13");
		m_padInputStates[14] = new InputState(InputType.ButtonL1, ProcType.Button, "Button4");
		m_padInputStates[15] = new InputState(InputType.ButtonR1, ProcType.Button, "Button5");
		m_padInputStates[16] = new InputState(InputType.ButtonL2, ProcType.Button, "Button6");
		m_padInputStates[17] = new InputState(InputType.ButtonR2, ProcType.Button, "Button7");
		m_padInputStates[18] = new InputState(InputType.ButtonL3, ProcType.Button, "Button10");
		m_padInputStates[19] = new InputState(InputType.ButtonR3, ProcType.Button, "Button11");
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
	}
}
