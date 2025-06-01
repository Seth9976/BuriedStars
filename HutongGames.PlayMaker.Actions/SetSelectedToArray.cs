namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Splits CSV string value and sets it to Array.")]
public class SetSelectedToArray : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	[Tooltip("String to split.")]
	public FsmString stringToSplit;

	[UIHint(UIHint.Variable)]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	[Tooltip("Store the split strings in a String Array.")]
	public FsmArray stringArray;

	[Tooltip("Reset string value when finished.")]
	public FsmBool ResetString = false;

	public override void Reset()
	{
		stringToSplit = null;
		stringArray = null;
		ResetString = false;
	}

	public override void OnEnter()
	{
		DoAddValue();
		Finish();
	}

	private void DoAddValue()
	{
		if (stringToSplit == null)
		{
			Finish();
		}
		char[] array = new char[1] { ',' };
		string value = stringToSplit.Value;
		value = value.TrimEnd(array);
		stringArray.Values = value.Split(array);
		if (ResetString.Value)
		{
			stringToSplit.Value = null;
		}
	}
}
