using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Character)]
[Tooltip("Tests if a Character Controller on a Game Object was touching the ground during the last move.")]
public class ControllerIsGrounded : FsmStateAction
{
	[RequiredField]
	[CheckForComponent(typeof(CharacterController))]
	[Tooltip("The GameObject to check.")]
	public FsmOwnerDefault gameObject;

	[Tooltip("Event to send if touching the ground.")]
	public FsmEvent trueEvent;

	[Tooltip("Event to send if not touching the ground.")]
	public FsmEvent falseEvent;

	[Tooltip("Store the result in a bool variable.")]
	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	[Tooltip("Repeat every frame while the state is active.")]
	public bool everyFrame;

	private GameObject previousGo;

	private CharacterController controller;

	public override void Reset()
	{
		gameObject = null;
		trueEvent = null;
		falseEvent = null;
		storeResult = null;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		DoControllerIsGrounded();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoControllerIsGrounded();
	}

	private void DoControllerIsGrounded()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (!(ownerDefaultTarget == null))
		{
			if (ownerDefaultTarget != previousGo)
			{
				controller = ownerDefaultTarget.GetComponent<CharacterController>();
				previousGo = ownerDefaultTarget;
			}
			if (!(controller == null))
			{
				bool isGrounded = controller.isGrounded;
				storeResult.Value = isGrounded;
				base.Fsm.Event((!isGrounded) ? falseEvent : trueEvent);
			}
		}
	}
}
