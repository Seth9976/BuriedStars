using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.StateMachine)]
[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
[Tooltip("Set the value of an Integer Variable in another FSM.")]
public class SetFsmInt : FsmStateAction
{
	[RequiredField]
	[Tooltip("The GameObject that owns the FSM.")]
	public FsmOwnerDefault gameObject;

	[UIHint(UIHint.FsmName)]
	[Tooltip("Optional name of FSM on Game Object")]
	public FsmString fsmName;

	[RequiredField]
	[UIHint(UIHint.FsmInt)]
	[Tooltip("The name of the FSM variable.")]
	public FsmString variableName;

	[RequiredField]
	[Tooltip("Set the value of the variable.")]
	public FsmInt setValue;

	[Tooltip("Repeat every frame. Useful if the value is changing.")]
	public bool everyFrame;

	private GameObject goLastFrame;

	private string fsmNameLastFrame;

	private PlayMakerFSM fsm;

	public override void Reset()
	{
		gameObject = null;
		fsmName = string.Empty;
		setValue = null;
	}

	public override void OnEnter()
	{
		DoSetFsmInt();
		if (!everyFrame)
		{
			Finish();
		}
	}

	private void DoSetFsmInt()
	{
		if (setValue == null)
		{
			return;
		}
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (ownerDefaultTarget == null)
		{
			return;
		}
		if (ownerDefaultTarget != goLastFrame || fsmName.Value != fsmNameLastFrame)
		{
			goLastFrame = ownerDefaultTarget;
			fsmNameLastFrame = fsmName.Value;
			fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, fsmName.Value);
		}
		if (fsm == null)
		{
			LogWarning("Could not find FSM: " + fsmName.Value);
			return;
		}
		FsmInt fsmInt = fsm.FsmVariables.GetFsmInt(variableName.Value);
		if (fsmInt != null)
		{
			fsmInt.Value = setValue.Value;
		}
		else
		{
			LogWarning("Could not find variable: " + variableName.Value);
		}
	}

	public override void OnUpdate()
	{
		DoSetFsmInt();
	}
}
