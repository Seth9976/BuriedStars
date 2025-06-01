namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Copy values from array to each variables")]
public class SetVariablesFromArray : FsmStateAction
{
	[ActionSection("Lists")]
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("A String variable to store the converted value.")]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	public FsmArray Att_types;

	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	public FsmArray Att_descs;

	[ActionSection("VariableName")]
	public FsmString Att_type;

	[Tooltip("Repeat every frame. Useful if the Int variable is changing.")]
	public bool everyFrame;

	private int currentIndex;

	public override void OnEnter()
	{
		if (Att_types != null && Att_type != null)
		{
			DoSetStringValueFromArray();
		}
		if (!everyFrame)
		{
			Finish();
		}
	}

	private void DoSetStringValueFromArray()
	{
		for (currentIndex = 0; currentIndex < Att_types.Length; currentIndex++)
		{
			string text = currentIndex.ToString();
			string text2 = Att_type.Value + text;
			FsmString fsmString = base.Fsm.Variables.GetFsmString(text2);
			object obj = Att_types.Get(currentIndex);
			object obj2 = Att_descs.Get(currentIndex);
			if (obj == null)
			{
				obj = string.Empty;
			}
			if (obj2 == null)
			{
				obj2 = string.Empty;
			}
			string value = (string)obj + ": " + obj2;
			if (fsmString != null)
			{
				fsmString.Value = value;
			}
			else
			{
				LogWarning("Could not find variable: " + text2);
			}
		}
	}
}
