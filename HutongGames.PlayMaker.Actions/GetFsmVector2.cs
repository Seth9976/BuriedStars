using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.StateMachine)]
[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
[Tooltip("Get the value of a Vector2 Variable from another FSM.")]
public class GetFsmVector2 : FsmStateAction
{
	[RequiredField]
	public FsmOwnerDefault gameObject;

	[UIHint(UIHint.FsmName)]
	[Tooltip("Optional name of FSM on Game Object")]
	public FsmString fsmName;

	[RequiredField]
	[UIHint(UIHint.FsmVector2)]
	public FsmString variableName;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmVector2 storeValue;

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
		DoGetFsmVector2();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoGetFsmVector2();
	}

	private void DoGetFsmVector2()
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
			FsmVector2 fsmVector = fsm.FsmVariables.GetFsmVector2(variableName.Value);
			if (fsmVector != null)
			{
				storeValue.Value = fsmVector.Value;
			}
		}
	}
}
