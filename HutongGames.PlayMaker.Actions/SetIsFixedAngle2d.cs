using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[Obsolete("This action is obsolete; use Constraints instead.")]
[ActionCategory(ActionCategory.Physics2D)]
[Tooltip("Controls whether the rigidbody 2D should be prevented from rotating")]
public class SetIsFixedAngle2d : ComponentAction<Rigidbody2D>
{
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	[Tooltip("The GameObject with the Rigidbody2D attached")]
	public FsmOwnerDefault gameObject;

	[RequiredField]
	[Tooltip("The flag value")]
	public FsmBool isFixedAngle;

	[Tooltip("Repeat every frame.")]
	public bool everyFrame;

	public override void Reset()
	{
		gameObject = null;
		isFixedAngle = false;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		DoSetIsFixedAngle();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoSetIsFixedAngle();
	}

	private void DoSetIsFixedAngle()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (UpdateCache(ownerDefaultTarget))
		{
			if (isFixedAngle.Value)
			{
				base.rigidbody2d.constraints = base.rigidbody2d.constraints | RigidbodyConstraints2D.FreezeRotation;
			}
			else
			{
				base.rigidbody2d.constraints = base.rigidbody2d.constraints & ~RigidbodyConstraints2D.FreezeRotation;
			}
		}
	}
}
