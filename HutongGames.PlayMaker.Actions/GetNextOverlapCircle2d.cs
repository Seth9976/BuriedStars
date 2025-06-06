using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Physics2D)]
[Tooltip("Iterate through a list of all colliders that fall within a circular area.The colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
public class GetNextOverlapCircle2d : FsmStateAction
{
	[ActionSection("Setup")]
	[Tooltip("Center of the circle area. \nOr use From Position parameter.")]
	public FsmOwnerDefault fromGameObject;

	[Tooltip("CEnter of the circle area as a world position. \nOr use fromGameObject parameter. If both define, will add fromPosition to the fromGameObject position")]
	public FsmVector2 fromPosition;

	[Tooltip("The circle radius")]
	public FsmFloat radius;

	[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
	public FsmInt minDepth;

	[Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
	public FsmInt maxDepth;

	[ActionSection("Filter")]
	[UIHint(UIHint.Layer)]
	[Tooltip("Pick only from these layers.")]
	public FsmInt[] layerMask;

	[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
	public FsmBool invertMask;

	[ActionSection("Result")]
	[Tooltip("Store the number of colliders found for this overlap.")]
	[UIHint(UIHint.Variable)]
	public FsmInt collidersCount;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("Store the next collider in a GameObject variable.")]
	public FsmGameObject storeNextCollider;

	[Tooltip("Event to send to get the next collider.")]
	public FsmEvent loopEvent;

	[Tooltip("Event to send when there are no more colliders to iterate.")]
	public FsmEvent finishedEvent;

	private Collider2D[] colliders;

	private int colliderCount;

	private int nextColliderIndex;

	public override void Reset()
	{
		fromGameObject = null;
		fromPosition = new FsmVector2
		{
			UseVariable = true
		};
		radius = 10f;
		minDepth = new FsmInt
		{
			UseVariable = true
		};
		maxDepth = new FsmInt
		{
			UseVariable = true
		};
		layerMask = new FsmInt[0];
		invertMask = false;
		collidersCount = null;
		storeNextCollider = null;
		loopEvent = null;
		finishedEvent = null;
	}

	public override void OnEnter()
	{
		if (colliders == null)
		{
			colliders = GetOverlapCircleAll();
			colliderCount = colliders.Length;
			collidersCount.Value = colliderCount;
		}
		DoGetNextCollider();
		Finish();
	}

	private void DoGetNextCollider()
	{
		if (nextColliderIndex >= colliderCount)
		{
			nextColliderIndex = 0;
			colliders = null;
			base.Fsm.Event(finishedEvent);
			return;
		}
		storeNextCollider.Value = colliders[nextColliderIndex].gameObject;
		if (nextColliderIndex >= colliderCount)
		{
			nextColliderIndex = 0;
			base.Fsm.Event(finishedEvent);
			return;
		}
		nextColliderIndex++;
		if (loopEvent != null)
		{
			base.Fsm.Event(loopEvent);
		}
	}

	private Collider2D[] GetOverlapCircleAll()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(fromGameObject);
		Vector2 value = fromPosition.Value;
		if (ownerDefaultTarget != null)
		{
			value.x += ownerDefaultTarget.transform.position.x;
			value.y += ownerDefaultTarget.transform.position.y;
		}
		if (minDepth.IsNone && maxDepth.IsNone)
		{
			return Physics2D.OverlapCircleAll(value, radius.Value, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
		}
		float num = ((!minDepth.IsNone) ? ((float)minDepth.Value) : float.NegativeInfinity);
		float num2 = ((!maxDepth.IsNone) ? ((float)maxDepth.Value) : float.PositiveInfinity);
		return Physics2D.OverlapCircleAll(value, radius.Value, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value), num, num2);
	}
}
