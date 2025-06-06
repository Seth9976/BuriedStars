using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Physics2D)]
[Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis.")]
public class AddForce2d : ComponentAction<Rigidbody2D>
{
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	[Tooltip("The GameObject to apply the force to.")]
	public FsmOwnerDefault gameObject;

	[Tooltip("Option for applying the force")]
	public ForceMode2D forceMode;

	[UIHint(UIHint.Variable)]
	[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
	public FsmVector2 atPosition;

	[UIHint(UIHint.Variable)]
	[Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
	public FsmVector2 vector;

	[Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
	public FsmFloat x;

	[Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
	public FsmFloat y;

	[Tooltip("A Vector3 force to add. z is ignored")]
	public FsmVector3 vector3;

	[Tooltip("Repeat every frame while the state is active.")]
	public bool everyFrame;

	public override void Reset()
	{
		gameObject = null;
		atPosition = new FsmVector2
		{
			UseVariable = true
		};
		forceMode = ForceMode2D.Force;
		vector = null;
		vector3 = new FsmVector3
		{
			UseVariable = true
		};
		x = new FsmFloat
		{
			UseVariable = true
		};
		y = new FsmFloat
		{
			UseVariable = true
		};
		everyFrame = false;
	}

	public override void OnPreprocess()
	{
		base.Fsm.HandleFixedUpdate = true;
	}

	public override void OnEnter()
	{
		DoAddForce();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnFixedUpdate()
	{
		DoAddForce();
	}

	private void DoAddForce()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (UpdateCache(ownerDefaultTarget))
		{
			Vector2 force = ((!vector.IsNone) ? vector.Value : new Vector2(x.Value, y.Value));
			if (!vector3.IsNone)
			{
				force.x = vector3.Value.x;
				force.y = vector3.Value.y;
			}
			if (!x.IsNone)
			{
				force.x = x.Value;
			}
			if (!y.IsNone)
			{
				force.y = y.Value;
			}
			if (!atPosition.IsNone)
			{
				base.rigidbody2d.AddForceAtPosition(force, atPosition.Value, forceMode);
			}
			else
			{
				base.rigidbody2d.AddForce(force, forceMode);
			}
		}
	}
}
