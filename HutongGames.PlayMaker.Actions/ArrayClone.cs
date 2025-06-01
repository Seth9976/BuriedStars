namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Clone Original Array to Taget, Two Array must be same type.")]
public class ArrayClone : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("Array to copy")]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	public FsmArray OriginalArray;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("array to place the copied values.")]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	public FsmArray TargetArray;

	public override void Reset()
	{
		OriginalArray = null;
		TargetArray = null;
	}

	public override void OnEnter()
	{
		if (OriginalArray == null || TargetArray == null)
		{
			Finish();
		}
		StartFsm();
		Finish();
	}

	private void StartFsm()
	{
		TargetArray.Resize(OriginalArray.Length);
		for (int i = 0; i < OriginalArray.Length; i++)
		{
			TargetArray.Set(i, OriginalArray.Values[i]);
		}
	}
}
