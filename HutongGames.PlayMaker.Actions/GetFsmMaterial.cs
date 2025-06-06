using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.StateMachine)]
[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
[Tooltip("Get the value of a Material Variable from another FSM.")]
public class GetFsmMaterial : FsmStateAction
{
	[RequiredField]
	[Tooltip("The GameObject that owns the FSM.")]
	public FsmOwnerDefault gameObject;

	[UIHint(UIHint.FsmName)]
	[Tooltip("Optional name of FSM on Game Object")]
	public FsmString fsmName;

	[RequiredField]
	[UIHint(UIHint.FsmMaterial)]
	public FsmString variableName;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmMaterial storeValue;

	[Tooltip("Repeat every frame.")]
	public bool everyFrame;

	private GameObject goLastFrame;

	protected PlayMakerFSM fsm;

	public override void Reset()
	{
		gameObject = null;
		fsmName = string.Empty;
		variableName = string.Empty;
		storeValue = null;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		DoGetFsmVariable();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoGetFsmVariable();
	}

	private void DoGetFsmVariable()
	{
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
		if (!(fsm == null) && storeValue != null)
		{
			FsmMaterial fsmMaterial = fsm.FsmVariables.GetFsmMaterial(variableName.Value);
			if (fsmMaterial != null)
			{
				storeValue.Value = fsmMaterial.Value;
			}
		}
	}
}
