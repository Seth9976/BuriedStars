using System;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Math)]
[Tooltip("Sets a Int variable to its absolute value.")]
public class IntAbs : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("The Float variable.")]
	public FsmInt IntVariable;

	[Tooltip("Repeat every frame. Useful if the variable is changing.")]
	public bool everyFrame;

	public override void Reset()
	{
		IntVariable = null;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		DoIntAbs();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoIntAbs();
	}

	private void DoIntAbs()
	{
		IntVariable.Value = Math.Abs(IntVariable.Value);
	}
}
