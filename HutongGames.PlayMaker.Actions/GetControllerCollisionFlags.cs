using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Character)]
[Tooltip("Gets the Collision Flags from a Character Controller on a Game Object. Collision flags give you a broad overview of where the character collided with any other object.")]
public class GetControllerCollisionFlags : FsmStateAction
{
	[RequiredField]
	[CheckForComponent(typeof(CharacterController))]
	[Tooltip("The GameObject with a Character Controller component.")]
	public FsmOwnerDefault gameObject;

	[UIHint(UIHint.Variable)]
	[Tooltip("True if the Character Controller capsule is on the ground")]
	public FsmBool isGrounded;

	[UIHint(UIHint.Variable)]
	[Tooltip("True if no collisions in last move.")]
	public FsmBool none;

	[UIHint(UIHint.Variable)]
	[Tooltip("True if the Character Controller capsule was hit on the sides.")]
	public FsmBool sides;

	[UIHint(UIHint.Variable)]
	[Tooltip("True if the Character Controller capsule was hit from above.")]
	public FsmBool above;

	[UIHint(UIHint.Variable)]
	[Tooltip("True if the Character Controller capsule was hit from below.")]
	public FsmBool below;

	private GameObject previousGo;

	private CharacterController controller;

	public override void Reset()
	{
		gameObject = null;
		isGrounded = null;
		none = null;
		sides = null;
		above = null;
		below = null;
	}

	public override void OnUpdate()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (!(ownerDefaultTarget == null))
		{
			if (ownerDefaultTarget != previousGo)
			{
				controller = ownerDefaultTarget.GetComponent<CharacterController>();
				previousGo = ownerDefaultTarget;
			}
			if (controller != null)
			{
				isGrounded.Value = controller.isGrounded;
				FsmBool fsmBool = none;
				_ = controller.collisionFlags;
				fsmBool.Value = false;
				sides.Value = (controller.collisionFlags & CollisionFlags.Sides) != 0;
				above.Value = (controller.collisionFlags & CollisionFlags.Above) != 0;
				below.Value = (controller.collisionFlags & CollisionFlags.Below) != 0;
			}
		}
	}
}
