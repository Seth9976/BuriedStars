using System;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Test if all the Bool variables of array are True.")]
public class ArrayAllTrue : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("Array to Check True/False")]
	[ArrayEditor(VariableType.Bool, "", 0, 0, 65536)]
	public FsmArray TargetArray;

	[Tooltip("The event to trigger if all value is true")]
	public FsmEvent IsAllTrue;

	[Tooltip("The event to trigger if any value is false")]
	public FsmEvent NotAllTrue;

	[Tooltip("Reset All value as false if Checked")]
	public FsmBool ResetAfterCheck = true;

	private int currentIndex;

	public override void Reset()
	{
		TargetArray = null;
		IsAllTrue = null;
		NotAllTrue = null;
		ResetAfterCheck = true;
	}

	public override void OnEnter()
	{
		currentIndex = 0;
		TestArray();
		Finish();
	}

	private void TestArray()
	{
		bool flag = true;
		while (currentIndex < TargetArray.Length)
		{
			object value = TargetArray.Values[currentIndex];
			if (!Convert.ToBoolean(value))
			{
				flag = false;
			}
			if (ResetAfterCheck.Value)
			{
				TargetArray.Set(currentIndex, false);
			}
			currentIndex++;
		}
		if (flag)
		{
			base.Fsm.Event(IsAllTrue);
		}
		else
		{
			base.Fsm.Event(NotAllTrue);
		}
		Finish();
	}
}
