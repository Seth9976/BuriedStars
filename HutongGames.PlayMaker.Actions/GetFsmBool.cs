using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.StateMachine)]
[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
[Tooltip("Get the value of a Bool Variable from another FSM.")]
public class GetFsmBool : FsmStateAction
{
	[RequiredField]
	public FsmOwnerDefault gameObject;

	[UIHint(UIHint.FsmName)]
	[Tooltip("Optional name of FSM on Game Object")]
	public FsmString fsmName;

	[RequiredField]
	[UIHint(UIHint.FsmBool)]
	public FsmString variableName;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmBool storeValue;

	public bool everyFrame;

	private GameObject goLastFrame;

	private PlayMakerFSM fsm;

	public override void Reset()
	{
		gameObject = null;
		fsmName = string.Empty;
		storeValue = null;
	}

	public override void OnEnter()
	{
		DoGetFsmBool();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoGetFsmBool();
	}

	private void DoGetFsmBool()
	{
		if (storeValue == null)
		{
			return;
		}
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (ownerDefaultTarget == null)
		{
			return;
		}
		if (ownerDefaultTarget != goLastFrame)
		{
			goLastFrame = ownerDefaultTarget;
			fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, fsmName.Value);
		}
		if (!(fsm == null))
		{
			FsmBool fsmBool = fsm.FsmVariables.GetFsmBool(variableName.Value);
			if (fsmBool != null)
			{
				storeValue.Value = fsmBool.Value;
			}
		}
	}
}
