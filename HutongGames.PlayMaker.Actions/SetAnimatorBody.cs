using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[Tooltip("Sets the position and rotation of the body. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
public class SetAnimatorBody : FsmStateAction
{
	[RequiredField]
	[CheckForComponent(typeof(Animator))]
	[Tooltip("The target. An Animator component is required")]
	public FsmOwnerDefault gameObject;

	[Tooltip("The gameObject target of the ik goal")]
	public FsmGameObject target;

	[Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
	public FsmVector3 position;

	[Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
	public FsmQuaternion rotation;

	[Tooltip("Repeat every frame.")]
	public bool everyFrame;

	private Animator _animator;

	private Transform _transform;

	public override void Reset()
	{
		gameObject = null;
		target = null;
		position = new FsmVector3
		{
			UseVariable = true
		};
		rotation = new FsmQuaternion
		{
			UseVariable = true
		};
		everyFrame = false;
	}

	public override void OnPreprocess()
	{
		base.Fsm.HandleAnimatorIK = true;
	}

	public override void OnEnter()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (ownerDefaultTarget == null)
		{
			Finish();
			return;
		}
		_animator = ownerDefaultTarget.GetComponent<Animator>();
		if (_animator == null)
		{
			Finish();
			return;
		}
		GameObject value = target.Value;
		if (value != null)
		{
			_transform = value.transform;
		}
	}

	public override void DoAnimatorIK(int layerIndex)
	{
		DoSetBody();
		if (!everyFrame)
		{
			Finish();
		}
	}

	private void DoSetBody()
	{
		if (_animator == null)
		{
			return;
		}
		if (_transform != null)
		{
			if (position.IsNone)
			{
				_animator.bodyPosition = _transform.position;
			}
			else
			{
				_animator.bodyPosition = _transform.position + position.Value;
			}
			if (rotation.IsNone)
			{
				_animator.bodyRotation = _transform.rotation;
			}
			else
			{
				_animator.bodyRotation = _transform.rotation * rotation.Value;
			}
		}
		else
		{
			if (!position.IsNone)
			{
				_animator.bodyPosition = position.Value;
			}
			if (!rotation.IsNone)
			{
				_animator.bodyRotation = rotation.Value;
			}
		}
	}
}
