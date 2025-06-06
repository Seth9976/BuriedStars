using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[Tooltip("Create a dynamic transition between the current state and the destination state.Both state as to be on the same layer. note: You cannot change the current state on a synchronized layer, you need to change it on the referenced layer.")]
public class AnimatorCrossFade : FsmStateAction
{
	[RequiredField]
	[CheckForComponent(typeof(Animator))]
	[Tooltip("The target. An Animator component is required")]
	public FsmOwnerDefault gameObject;

	[Tooltip("The name of the state that will be played.")]
	public FsmString stateName;

	[Tooltip("The duration of the transition. Value is in source state normalized time.")]
	public FsmFloat transitionDuration;

	[Tooltip("Layer index containing the destination state. Leave to none to ignore")]
	public FsmInt layer;

	[Tooltip("Start time of the current destination state. Value is in source state normalized time, should be between 0 and 1.")]
	public FsmFloat normalizedTime;

	private Animator _animator;

	private int _paramID;

	public override void Reset()
	{
		gameObject = null;
		stateName = null;
		transitionDuration = 1f;
		layer = new FsmInt
		{
			UseVariable = true
		};
		normalizedTime = new FsmFloat
		{
			UseVariable = true
		};
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
		if (_animator != null)
		{
			int num = ((!layer.IsNone) ? layer.Value : (-1));
			float normalizedTimeOffset = ((!normalizedTime.IsNone) ? normalizedTime.Value : float.NegativeInfinity);
			_animator.CrossFade(stateName.Value, transitionDuration.Value, num, normalizedTimeOffset);
		}
		Finish();
	}
}
